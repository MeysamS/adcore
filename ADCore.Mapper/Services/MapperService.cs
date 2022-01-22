using ADCore.Mapper.Extensions;
using ADCore.Mapper.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ADCore.Mapper.Services
{

    public class MapperService : IMapperService
    {
        private Action<object, List<object>> MapperAction;

        public bool TryMap<T>(Resource resource, out object target)
        {

            var obj = (T)Activator.CreateInstance(typeof(T), new object[] { });
            var dictObj = DeserializeJsonToDic(resource.Data, resource.Config.PropertySetsDic);
            try
            {
                target = MapWithExpressionTree(dictObj, obj);
                return true;
            }
            catch (Exception e)
            {
                try
                {
                    target = MapWithReflection(dictObj, obj);
                    //  target = ParseJSON<T>(resource.Data, resource.Config.PropertySetsDic);
                    return true;
                }
                catch (Exception ex)
                {
                    target = obj;
                    return false;
                }
            }

        }

        public object Map<T>(Resource resource)
        {

            T target = (T)Activator.CreateInstance(typeof(T), new object[] { });
            var dictObj = DeserializeJsonToDic(resource.Data, resource.Config.PropertySetsDic);

            var resualt = MapWithExpressionTree(dictObj, target);
            return resualt;
        }


        #region General Methodes
        private Dictionary<string, object> DeserializeJsonToDic(string json, Dictionary<string, string> configDic)
        {
            var jsonObj = JsonConvert.DeserializeObject(json);
            if (jsonObj.GetType().Name == "JObject")
            {
                var dictObj = GetRequiredPeropertyFromJObjectData((JObject)jsonObj, configDic);
                dictObj = dictObj.ConvertJsonTypeValueToRegularType();
                return dictObj;
            }
            if (jsonObj.GetType().Name == "JArray")
            {
                var dictObj = GetRequiredPeropertyFromJArrayData((JArray)jsonObj, configDic);
                dictObj = dictObj.ConvertJsonTypeValueToRegularType();
                return dictObj;
            }


            return null;
        }
        private Dictionary<string, object> GetRequiredPeropertyFromJObjectData(JObject data, Dictionary<string, string> dic)
        {
            JObject jo = new JObject();

            foreach (KeyValuePair<string, string> item in dic)
            {
                if (item.Key.Contains("#"))
                {
                    var valArr = item.Value.Split('#');
                    jo.Add($"{item.Key}#{valArr[1]}", data.SelectToken(valArr[0]));
                }
                else
                    jo.Add(item.Key, data.SelectToken(item.Value));
            }
            Dictionary<string, object> dictObj = jo.ToObject<Dictionary<string, object>>();


            return dictObj;
        }
        private Dictionary<string, object> GetRequiredPeropertyFromJArrayData(JArray data, Dictionary<string, string> dic)
        {
            JObject jo = new JObject();

            foreach (KeyValuePair<string, string> item in dic)
            {
                if (item.Key.Contains("#"))
                {
                    var valArr = item.Value.Split('#');
                    jo.Add($"{item.Key}#{valArr[1]}", data.SelectToken(valArr[0]));
                }
                else
                    jo.Add(item.Key, data.SelectToken(item.Value));
            }
            Dictionary<string, object> dictObj = jo.ToObject<Dictionary<string, object>>();


            return dictObj;
        }

        #endregion


        #region Reflection Methodes

        private object MapWithReflection(Dictionary<string, object> dic, object target)
        {
            foreach (KeyValuePair<string, object> item in dic)
            {
                if (!item.Key.Contains("#"))
                {
                    try
                    {
                        SetPropertyByReflection(item.Key, target, item.Value);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                else
                {

                    SetListPropertyByReflection(item.Key, target, item.Value);

                }
            }
            return target;
        }
        private void SetListPropertyByReflection(string key, object target, object value)
        {
            var KeyArr = key.Split('#');
            var nestedListModel = KeyArr[0];
            var listModel = FindAndUseList(target, nestedListModel);
            SetListPropertyOneByOne(key, listModel, target, value);
        }

        private PropertyInfo FindAndUseList(object target, string listPropertyName)
        {
            var listModel = GetProperty(target, listPropertyName);
            var thisList = listModel.GetValue(target, null);
            if (thisList == null)
            {
                thisList = Activator.CreateInstance(listModel.PropertyType);
                listModel.SetValue(target, thisList, null);
            }

            return listModel;
        }

        private void SetListPropertyOneByOne(string key, PropertyInfo listModel, object target, object value)
        {

            var KeyArr = key.Split('#');
            var ModelKey = KeyArr[1];
            var JKey = KeyArr[2];
            var data = value;


            var thisList = listModel.GetValue(target, null);

            var listElementType = GetCollectionElementType(thisList.GetType());
            var jArray = JArray.Parse(data.ToString());
            var cnt = -1;
            foreach (var JlistItem in jArray)
            {
                cnt++;
                var lst = (listModel.GetGetMethod().Invoke(target, null) as IList);
                if (lst.Count <= cnt)
                {
                    //add an Item ToList
                    var thisListItem = Activator.CreateInstance(listElementType);
                    lst.GetType().GetMethod("Add").Invoke(lst, new[] { thisListItem });
                }
                var itemInList = lst[cnt];
                var val = JlistItem.SelectToken(JKey);
                SetPropertyByReflection(ModelKey, itemInList, val);
            }

        }

        private void SetPropertyByReflection(string compoundProperty, object target, object value)
        {
            string[] bits = compoundProperty.Split('.');
            for (int i = 0; i < bits.Length - 1; i++)
            {
                var parent = target;
                PropertyInfo propertyToGet = target.GetType().GetProperty(bits[i]);
                target = propertyToGet.GetValue(target, null);
                if (target == null)
                {
                    target = Activator.CreateInstance(propertyToGet.PropertyType);
                    propertyToGet.SetValue(parent, target, null);
                }
            }
            PropertyInfo propertyToSet = target.GetType().GetProperty(bits.Last());
            var valType = propertyToSet.PropertyType;
            var val = ChangeType(value, valType);

            propertyToSet.SetValue(target, val, null);
        }

        private static object ChangeType(object value, Type castTo)
        {
            return Convert.ChangeType(value, castTo);
        }

        private static PropertyInfo GetProperty(object target, string PropName)
        {
            return target.GetType().GetProperty(PropName);
        }
        private static Type GetCollectionElementType(Type objType)
        {
            Type elementType;
            Type[] genericArgs = objType.GenericTypeArguments;
            if (genericArgs.Length > 0)
            {
                elementType = genericArgs[0];
            }
            else
            {
                elementType = objType.GetElementType();
            }

            return elementType;
        }


        #endregion


        #region ExpressionTree Methodes
        private object MapWithExpressionTree(Dictionary<string, object> dic, object target)
        {
            BuildMapperAction(dic, target);
            MapperAction(target, dic.Values.ToList());

            return target;
        }

        private void BuildMapperAction(Dictionary<string, object> dic, object target)
        {
            ParameterExpression targetInstanceExpression = Expression.Parameter(target.GetType());
            ParameterExpression valuesExpression = Expression.Parameter(typeof(List<object>));
            ParameterExpression value = Expression.Variable(typeof(object));
            ParameterExpression enumerator = Expression.Variable(typeof(IEnumerator));

            var expList = new List<Expression>();
            expList.Add(Expression.Assign(enumerator, Expression.TypeAs(Expression.Call(valuesExpression, "GetEnumerator", null), typeof(IEnumerator))));
            foreach (var dicItem in dic)
            {
                if (dicItem.Value == null) continue;
                Expression moveNextExp = Expression.Call(enumerator, "MoveNext", null);
                expList.Add(moveNextExp);
                Type type = dicItem.Value.GetType();
                expList.Add(Expression.Assign(value, Expression.PropertyOrField(enumerator, "Current")));
                Expression assignExp = GetPropAssigner(dicItem.Key, type, targetInstanceExpression, value);
                expList.Add(assignExp);
            }
            Expression block = Expression.Block
            (
                 new[] { value, enumerator },
                 expList
            );
            //compile epression tree and get init action 
            MapperAction = Expression.Lambda<Action<object, List<object>>>(block, targetInstanceExpression, valuesExpression).Compile();

        }

        private static Expression GetPropAssigner(string propName, Type type,
             ParameterExpression targetInstanceExp, ParameterExpression valueExp)
        {
            MemberExpression fieldExp = Expression.PropertyOrField(targetInstanceExp, propName);
            BinaryExpression assignExp = Expression.Assign(fieldExp,
                type.IsValueType ? Expression.Unbox(valueExp, type) : Expression.TypeAs(valueExp, type));

            return assignExp;
        }
        #endregion


 
        //private static bool IsCollection(object obj)
        //{
        //    bool isCollection = false;

        //    Type objType = obj.GetType();
        //    if (!typeof(string).IsAssignableFrom(objType) && typeof(IEnumerable).IsAssignableFrom(objType))
        //    {
        //        isCollection = true;
        //    }

        //    return isCollection;
        //}





    }



}
