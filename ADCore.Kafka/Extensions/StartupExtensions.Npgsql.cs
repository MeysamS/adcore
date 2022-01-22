using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace ADCore.Kafka.Extensions {

    static partial class StartupExtensions {

        public static IServiceCollection AddNpgsqlPool<TContext>(
            this IServiceCollection                services, string connectionStringOrName, 
            string?                                migrationAssemblyName = null,
            Action<NpgsqlDbContextOptionsBuilder>? optionsBuilder        = null)
            where TContext : DbContext {
            services.AddDbContext<TContext>(options => {
                var connectionString = _configuration.GetConnectionString(connectionStringOrName)
                                       ?? connectionStringOrName;

                if (string.IsNullOrWhiteSpace(migrationAssemblyName))
                    options.UseNpgsql(connectionString, optionsBuilder);
                else
                    options.UseNpgsql(connectionString, b => {
                        b.MigrationsAssembly(migrationAssemblyName);
                        optionsBuilder?.Invoke(b);
                    });
            });

            return services;
        }

    }

}