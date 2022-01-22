 using System.Collections.Generic;
using System.Linq;
using System.Text;
 
namespace ADCore.ApiReader.Extensions
{
    public static class StringExtentions
    {
        public static string AddQueryStringsToEndOfUrl(this string url, Dictionary<string, string> queryStringDictinary)
        {
            if (queryStringDictinary == null || queryStringDictinary.Count == 0)
                return url;

            url += "?";
            url += queryStringDictinary.Aggregate(new StringBuilder(),
                      (sb, qs) => sb.AppendFormat("{0}{1}={2}",
                          sb.Length > 0 ? "&" : "", qs.Key, qs.Value),
                      sb => sb.ToString());
            return url;
             
        }

    }
}
