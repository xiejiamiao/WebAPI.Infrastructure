using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace WebAPI.Infrastructure.Repositories.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ToDynamicIEnumerable<TSource>(this IEnumerable<TSource> source, string fields = null)
        {
            if (source==null)
                throw new ArgumentNullException(nameof(source));
            
            var expandoObjectList = new List<ExpandoObject>();
            var propertyInfoList = new List<PropertyInfo>();
            if (string.IsNullOrEmpty(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',').ToList();
                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();
                    if(string.IsNullOrEmpty(propertyName))
                        continue;
                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null)
                        throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");
                    propertyInfoList.Add(propertyInfo);
                }
            }

            foreach (var sourceObject in source)
            {
                var expandoObject = new ExpandoObject();
                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(sourceObject);
                    ((IDictionary<string,object>)expandoObject).Add(propertyInfo.Name,propertyValue);
                }
                expandoObjectList.Add(expandoObject);
            }

            return expandoObjectList;
        }

    }
}