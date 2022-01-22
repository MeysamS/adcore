using ADCore.Kafka.Enums;

namespace ADCore.Kafka.Settings
{
    public class FormatterOptions
    {
        public static readonly string DefaultDecimalFormatString = "F0";
        public static readonly string DefaultFloatFormatString = "F2";
        public static readonly string DefaultDoubleFormatString = "F4";

        public bool UseUnixTimestamp { get; set; } = true;
        public bool OutputLongAsString { get; set; } = true;
        public bool AcceptInputLongString { get; set; } = true;
        public NamingConvention NamingConvention { get; set; } = NamingConvention.CamelCase;
        public string DecimalFormatString { get; set; } = DefaultDecimalFormatString;
        public string FloatFormatString { get; set; } = DefaultFloatFormatString;
        public string DoubleFormatString { get; set; } = DefaultDoubleFormatString;
        public bool IsStringEnumEnabled { get; set; } = true;
    }
}
