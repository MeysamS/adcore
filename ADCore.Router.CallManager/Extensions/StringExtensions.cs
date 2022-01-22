using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADCore.Router.CallManager.Util
{
    public static class StringExtensions
    {
        public static string GetBaseHostNameFromLink(this string url)
        {
            var host = new System.Uri(url).Host;
            var hostArr = host.Split('.');
            if (hostArr.Length < 2) return null;
            var BaseHost = $"{hostArr[hostArr.Length - 2]}.{hostArr[hostArr.Length - 1]}";
            return BaseHost;
        }

   
    }
}
 