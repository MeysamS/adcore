using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Authentication;
using Autofac;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ADCore.Kafka.Extensions {

    static partial class StartupExtensions {
        public static void ConfigureAd(this ContainerBuilder builder, params Assembly[] assemblies) {
            string[] disallowNamespaces = new[] { "Jil", "Hosting", "Security", "Utils", "Identity" };

            string[] disallowClasses = new[] {
                "ClaimsExtensions", "Credentials", "IdentityToken", "JwtIssuerOptions", "ADClaims"
            };

            var thisAssembly = typeof(StartupExtensions).Assembly;

            assemblies = assemblies == null
                ? new[] { thisAssembly }
                : assemblies.Union(new[] { thisAssembly }).ToArray();

            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes()
                                    .Where(t => !string.IsNullOrWhiteSpace(t.Namespace))
                                    .Where(t => t.Namespace?.StartsWith("AD") == true)
                                    .Where(t => t.IsClass)
                                    .Where(t => !t.IsAbstract)
                                    .Where(t => !t.IsGenericType)
                                    .Where(t => disallowClasses.All(c => t.Name != c))
                                    .Where(t => disallowNamespaces.All(n => t.Namespace?.Contains(n) == false));

                foreach (var type in types) {
                    var interfaces = type.GetInterfaces()
                                         .Where(t => (t.Namespace?.StartsWith("Manex")).GetValueOrDefault() || (t.Namespace?.StartsWith("Baman")).GetValueOrDefault())
                                         .Where(t => !t.IsGenericType)
                                         .Where(t => !t.Name.StartsWith("ICrudStore"))
                                         .Where(t => !t.Name.StartsWith("ICrudService"))
                                         .ToList();

                    if (interfaces.Count > 0) {
                        foreach (var i in interfaces)
                            builder.RegisterType(type).As(i).InstancePerLifetimeScope();

                        // builder.RegisterType(type).AsImplementedInterfaces().InstancePerLifetimeScope();
                    } else
                        builder.RegisterType(type).AsSelf().InstancePerLifetimeScope();
                }
            }
        }
    }

}