using System;
using ADCore.Kafka.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace ADCore.Kafka.Formatters
{
    public interface IFormatterProvider
    {
        void BuildOptions(Action<FormatterOptions> options);
        void Setup(IMvcCoreBuilder mvcBuilder);
    }
}
