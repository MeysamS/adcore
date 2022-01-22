using System.Globalization;
using System.Text.RegularExpressions;

namespace ADCore.Kafka.Formatters
{
    public class Normalizer
    {

        public static readonly Regex PhoneVerifier = new Regex(@"^9{1}[0-9]{9}$",
                                                               RegexOptions.Compiled | RegexOptions.Singleline);

        public static readonly Regex PhoneMatcher = new Regex(@"^(?<prefix>0098{1}|\+?98{1}|0+)?(?<phone>[0-9]+)$",
                                                              RegexOptions.Compiled | RegexOptions.Singleline);
        public static string CleanUpPhone(string value)
        {
            value = value?.Trim();
            if (string.IsNullOrWhiteSpace(value)) return value;
            value = DigitsToLatin(value);

            //var match = regex.Match(value);
            if (PhoneMatcher.IsMatch(value))
                value = PhoneMatcher.Replace(value, "$2");
            return value;
            /*
             // old method
            value = value.Replace(" ", "")
                         .Replace("\n", "")
                         .Replace("\r", "")
                         .Replace("\\s", "")
                         .Replace("\t", "");
            value = value.Trim().TrimStart('0', '+');
            if (value.StartsWith("98"))
                value = value.Remove(0, 2);
            value = value.Trim().TrimStart('0', '+');
            return value;
            */
        }

        public static string DigitsToLatin(string value)
        {
            var chars = new char[value.Length];

            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];

                /*  var diff =  0;
                  if (c >= '٠' && c <= '٩') {
                      diff = '٠' - '0';
                  }else if (c >= '۰' && c <= '۹') {
                      diff = '۰' - '0';
                  }

                  var cleanedChar = (char) (c - diff);
                  chars[i] = cleanedChar;*/

                if (char.IsNumber(c))
                    chars[i] = char.GetNumericValue(c).ToString(CultureInfo.InvariantCulture)[0];
                else
                    chars[i] = c;
            }

            return new string(chars);
        }

        public static string NormalizePersian(string value)
        {
            var model = value.Replace((char)1610, (char)1740)
                             .Replace((char)1603, (char)1705);
            return model;
        }

    }
}
