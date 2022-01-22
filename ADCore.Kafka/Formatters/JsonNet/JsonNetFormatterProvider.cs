using System;
using ADCore.Kafka.Enums;
using ADCore.Kafka.Settings;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ADCore.Kafka.Formatters.JsonNet
{
    public class JsonNetFormatterProvider : IFormatterProvider
    {

        private readonly FormatterOptions _options = new FormatterOptions();

        public void BuildOptions(Action<FormatterOptions> optionsBuilder)
        {
            optionsBuilder?.Invoke(_options);
        }

        public void Setup(IMvcCoreBuilder mvcBuilder)
        {
            mvcBuilder.Services.AddScoped<ISerializer, JsonNetSerializer>();

            var serializerSettings = BuildSerializerSettings(new JsonSerializerSettings(), _options);
            JsonConvert.DefaultSettings = () => serializerSettings;

            mvcBuilder.AddNewtonsoftJson(settings =>
            {
                BuildSerializerSettings(settings.SerializerSettings, _options);
            });

            mvcBuilder.Services.AddSingleton(serializerSettings);
        }

        private static JsonSerializerSettings BuildSerializerSettings(
            JsonSerializerSettings settings, FormatterOptions options)
        {

            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            settings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;

            if (options.IsStringEnumEnabled)
                settings.Converters.Insert(0, new StringEnumConverter());

            /*
            settings.Converters.Insert(0,
                                       new JsonNetNumberConverter {
                                           DecimalFormatString = options.DecimalFormatString,
                                           DoubleFormatString  = options.DoubleFormatString,
                                           FloatFormatString   = options.FloatFormatString,
                                       });
            */

            if (options.UseUnixTimestamp)
            {
                settings.Converters.Insert(0, new JsonNetUnixNullableDateTimeFormatter());
                settings.Converters.Insert(0, new JsonNetUnixNullableDateTimeOffsetFormatter());
                settings.Converters.Insert(0, new JsonNetUnixDateTimeFormatter());
                settings.Converters.Insert(0, new JsonNetUnixDateTimeOffsetFormatter());
            }

            if (options.OutputLongAsString || options.AcceptInputLongString)
                settings.Converters.Insert(0, new JsonNetLongStringConverter
                {
                    OutputLongAsString = options.OutputLongAsString,
                    AcceptInputLongString = options.AcceptInputLongString
                });

            settings.Converters.Insert(0, new JsonNetNumberConverter());

            var namingStrategy = options.NamingConvention switch
            {
                NamingConvention.SnakeCase => new SnakeCaseNamingStrategy(),
                NamingConvention.CamelCase => new CamelCaseNamingStrategy(),
                NamingConvention.AsIs => new DefaultNamingStrategy(),
                _ => (NamingStrategy)new DefaultNamingStrategy()
            };

            settings.ContractResolver = new DefaultContractResolver { NamingStrategy = namingStrategy, };

            settings.NullValueHandling = NullValueHandling.Ignore;

            return settings;
        }

    }
}
