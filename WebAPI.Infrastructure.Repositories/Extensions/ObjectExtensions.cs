using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace WebAPI.Infrastructure.Repositories.Extensions
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ToDynamic<TSource>(this TSource source, string fields = null)
        {
            if(source==null)
                throw new ArgumentNullException(nameof(source));
            
            var expandoObject = new ExpandoObject();
            if (string.IsNullOrEmpty(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string,object>)expandoObject).Add(propertyInfo.Name,propertyValue);
                }
                return expandoObject;
            }

            var fieldAfterSplit = fields.Split(',').ToList();
            foreach (var field in fieldAfterSplit)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if(propertyInfo==null)
                    throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");
                var propertyValue = propertyInfo.GetValue(source);
                ((IDictionary<string,object>)expandoObject).Add(propertyInfo.Name,propertyValue);
            }

            return expandoObject;
        }
    }
}