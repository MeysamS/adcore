using System;
using System.Collections.Generic;
using System.Linq;

namespace ADCore.Kafka.Extensions
{
    public static class ReflectionsExtensions
    {
        public static bool IsDefaultValue(this object value)
        {
            if (value == null) return true;
            var type = value.GetType();
            if (!type.IsValueType) return false;
            return value.Equals(Activator.CreateInstance(type));
        }
        public static void OverrideBy(this object source, object other)
        {
            if (source == null) throw new InvalidOperationException();
            if (other == null) return;
            var sourceType = source.GetType();

            var sourceProps = sourceType.GetProperties()
                .Where(t => t.CanRead)
                .Where(t => t.CanWrite)
                .ToList();

            var otherType = other.GetType();

            var otherProps = otherType.GetProperties()
                .Where(t => t.CanRead)
                .ToList();

            foreach (var sp in sourceProps)
            {
                var sv = sp.GetValue(source);
                if (!sv.IsDefaultValue()) continue;
                var op = otherProps.FirstOrDefault(t => t.Name == sp.Name);
                if (op == null) continue;
                var ov = op.GetValue(other);
                sp.SetValue(source, ov);
            }
        }

        public static IEnumerable<Type> AsClosedTypeOf(this IEnumerable<Type> source, Type openGenericType)
        {
            return source
                  .Where(t => t.IsGenericType)
                  .Where(t => t.GetGenericTypeDefinition() == openGenericType)
                  .ToList();
        }

        public static IEnumerable<Type> GetClosedInterfacesOf(this Type source, Type openGenericType)
        {
            return source
                  .GetInterfaces()
                  .AsClosedTypeOf(openGenericType)
                  .ToList();
        }
    }   
}
