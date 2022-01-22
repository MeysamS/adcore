using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ADCore.Kafka.Attributes;
using ADCore.Kafka.Jobs;
using ADCore.Kafka.Messaging.Handler;
using Microsoft.Extensions.DependencyInjection;

namespace ADCore.Kafka.Extensions
{
    public static class MessagingExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0) assemblies = new[] { Assembly.GetEntryAssembly(), };
            var allTypes = assemblies.SelectMany(a => a.GetTypes()).ToList();

            var messageTypes = allTypes
                              .Select(t => (type: t, attr: t.GetCustomAttribute<SubscribableMessageHandlerAttribute>()))
                              .Where(t => t.attr != null)
                              .ToList();

            foreach (var (handlerType, attr) in messageTypes)
            {

                var iHandlerTypes = handlerType.GetClosedInterfacesOf(typeof(IMessageHandler<>));

                foreach (var iHandlerType in iHandlerTypes)
                {
                    var cacheItem = new JobTypeCache
                    {
                        HandlerType = handlerType,
                        IHandlerType = iHandlerType,
                        HandlerAttribute = attr,
                    };

                    var messageType = iHandlerType.GetGenericArguments().Single();
                    cacheItem.MessageType = messageType;
                    cacheItem.SubscribableMessageAttribute = messageType.GetCustomAttribute<SubscribableMessageAttribute>();

                    // register iHandler<Message>
                    // per-scope
                    services.AddScoped(iHandlerType, handlerType);

                    // register iManager<handler, message>
                    // per-scope
                    var iManagerType = typeof(IMessageHandlerManager<,>).MakeGenericType(handlerType, messageType);
                    var managerType = typeof(MessageHandlerManager<,>).MakeGenericType(handlerType, messageType);
                    services.AddScoped(iManagerType, managerType);
                    cacheItem.IManagerType = iManagerType;
                    cacheItem.ManagerType = managerType;

                    // register iUnderlying<handler, message>
                    // singleton-per-message
                    var iUnderlyingType = typeof(IUnderlyingMessageConsumer<,>).MakeGenericType(handlerType, messageType);
                    var underlyingType = typeof(UnderlyingMessageConsumer<,>).MakeGenericType(handlerType, messageType);
                    services.AddSingleton(iUnderlyingType, underlyingType);
                    cacheItem.IUnderlyingType = iUnderlyingType;
                    cacheItem.UnderlyingType = underlyingType;

                    // register MessageConsumerJob
                    // transient
                    var jobType = typeof(MessageConsumerJob<,>).MakeGenericType(handlerType, messageType);
                    services.AddTransient(jobType);
                    cacheItem.JobType = jobType;

                    // cache types
                    JobTypeCache.Add(cacheItem);
                }
            }
            return services;
        }
    }
    public class JobTypeCache
    {
        public Type MessageType { get; set; }
        public SubscribableMessageAttribute SubscribableMessageAttribute { get; set; }
        public Type HandlerType { get; set; }
        public Type IHandlerType { get; set; }
        public SubscribableMessageHandlerAttribute HandlerAttribute { get; set; }
        public Type ManagerType { get; set; }
        public Type IManagerType { get; set; }
        public Type IUnderlyingType { get; set; }
        public Type UnderlyingType { get; set; }
        public Type JobType { get; set; }

        private static List<JobTypeCache> _cache = new List<JobTypeCache>();

        public static void Add(JobTypeCache item)
        {
            _cache.Add(item);
        }

        public static IEnumerable<JobTypeCache> IterateAndClear()
        {
            foreach (var item in _cache)
                yield return item;
            _cache.Clear();
            _cache = null;
        }
    }
}
