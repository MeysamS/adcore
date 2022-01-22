using System;
using ADCore.Kafka.Formatters;
using ADCore.Kafka.Formatters.JsonNet;

namespace ADCore.Kafka.Settings
{
    public class StartupOptions
    {
        public string ServiceName { get; set; }
        public bool EnableAuthentication { get; set; } = true;
        public bool EnableApiExplorer { get; set; } = true;
        public bool EnableDataAnnotations { get; set; } = true;
        public bool EnableFormatterMappings { get; set; } = true;
        public bool EnableCleaners { get; set; } = true;
        public IFormatterProvider FormatterProvider { get; private set; } = new JsonNetFormatterProvider();
        public TFormatter UseFormatter<TFormatter>(Action<FormatterOptions> optionsBuilder = null)
            where TFormatter : class, IFormatterProvider, new()
        {
            FormatterProvider = new TFormatter();
            FormatterProvider.BuildOptions(optionsBuilder);

            return FormatterProvider as TFormatter;
        }

    }
}
