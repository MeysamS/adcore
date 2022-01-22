using System;
using System.Diagnostics.CodeAnalysis;
using ADCore.Kafka.Hosting;
using ADCore.Kafka.Settings;
using Microsoft.Extensions.Configuration;

namespace ADCore.Kafka.Extensions
{
    public static partial class StartupExtensions
    {

        private static readonly StartupOptions StartupOptions = new StartupOptions();
        private static IConfiguration _configuration;

        public static void UseIn(
            this IConfiguration configuration,
            [NotNull] Action<StartupOptions> startupOptionsBuilder)
        {

            if (!AdHost.IsConfigured)
                throw new
                    InvalidOperationException("To use specified extensions, you have to initialize the AdHost object!");

            _configuration = configuration;

            if (startupOptionsBuilder == null) throw new ArgumentNullException(nameof(startupOptionsBuilder));
            startupOptionsBuilder.Invoke(StartupOptions);
            if (string.IsNullOrWhiteSpace(StartupOptions.ServiceName))
                throw new ArgumentNullException(nameof(StartupOptions.ServiceName));
        }
    }
}
