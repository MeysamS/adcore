using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ADCore.Kafka.Attributes;
using ADCore.Kafka.Extensions;

namespace ADCore.Kafka.Messaging.Publisher
{
    public static class PublishableMessageMapper<TPublishableMessage> where TPublishableMessage : IPublishableMessage
    {
        public static IEnumerable<PublishableMessageMap<TPublishableMessage>> Map => Mapper.Value;

        private static readonly Lazy<IEnumerable<PublishableMessageMap<TPublishableMessage>>> Mapper
            = new Lazy<IEnumerable<PublishableMessageMap<TPublishableMessage>>>(() => FindMapMethods().ToList());

        private static IEnumerable<PublishableMessageMap<TPublishableMessage>> FindMapMethods()
        {
            var type = typeof(TPublishableMessage);
            var interfaces = type.GetClosedInterfacesOf(typeof(IPublishableMessage<>));
            foreach (var @interface in interfaces)
            {
                var mapper = type.GetInterfaceMap(@interface)
                    .TargetMethods.Single();

                var messageParam = Expression.Parameter(type, "m");
                var mapperCall = Expression.Call(messageParam, mapper);
                var mapperExp = Expression.Lambda<Func<TPublishableMessage, Object>>(mapperCall, messageParam);
                var mapperFunc = mapperExp.Compile();

                var targetType = @interface.GetGenericArguments().Single();

                var subscribeAttr = targetType.GetCustomAttribute<SubscribableMessageAttribute>();
                if (subscribeAttr == null)
                    throw new InvalidOperationException(
                        $"Cannot publish a message of type {targetType}. It must has an attribute of type {typeof(SubscribableMessageAttribute)} with a not null or empty Topic property.");

                var topic = subscribeAttr.Topic;
                if (string.IsNullOrWhiteSpace(topic))
                    throw new InvalidOperationException(
                        $"Cannot publish a message of type {targetType}. It must has an attribute of type {typeof(SubscribableMessageAttribute)} with a not null or empty Topic property.");
                var targetParam = Expression.Parameter(typeof(object), "t");
                Expression<Func<object, string>> lambda;
                var topicConst = Expression.Constant(topic, typeof(string));
                if (string.IsNullOrWhiteSpace(subscribeAttr.PostFixProperty))
                {
                    lambda = Expression.Lambda<Func<object, string>>(topicConst, targetParam);
                }
                else
                {
                    var propInfo = targetType.GetProperty(subscribeAttr.PostFixProperty);
                    var castObj = Expression.TypeAs(targetParam, targetType);
                    var prop = Expression.Property(castObj, propInfo);
                    var sep = Expression.Constant(subscribeAttr.SeperatorChar);
                    var stringConcat = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object), typeof(object) });
                    var topicCall = Expression.Call(stringConcat,
                                                    topicConst,//Expression.Convert(topicConst, typeof(object)),
                                                    sep, //Expression.Convert(sep, typeof(object)),
                                                    Expression.Convert(prop, typeof(object)));
                    lambda = Expression.Lambda<Func<object, string>>(topicCall, targetParam);
                }
                var topicMethod = lambda.Compile();
                yield return new PublishableMessageMap<TPublishableMessage>
                {
                    MapperFunc = mapperFunc,
                    TargetType = targetType,
                    TopicFunc = topicMethod
                };
            }
        }
    }

}
