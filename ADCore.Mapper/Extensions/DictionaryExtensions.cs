using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;

namespace  ADCore.Mapper.Extensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<string, object> ConvertJsonTypeValueToRegularType(this Dictionary<string, object> data)
        {
            Dictionary<string, object> tempDic = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> item in data)
            {
                if (item.Value!=null&&item.Value.GetType().Name == "Int64")
                {
                    if ((long)item.Value <= int.MaxValue)
                    {
                        var cnvt = Convert.ToInt32(item.Value);
                        tempDic.Add(item.Key, cnvt);
                    }
                }

                //if (item.Value != null && item.Value.GetType().Name == "JObject")
                //{
                //    tempDic.Add(item.Key, item.Value.ToString());
                //}

            }

            foreach (KeyValuePair<string, object> item in tempDic)
                data[item.Key] = item.Value;
            return data;

        }



    }



}
