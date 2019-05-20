using System.Reflection;

namespace WebAPI.Infrastructure.Services
{
    public class TypeHelperService:ITypeHelperService
    {
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrEmpty(fields))
                return true;

            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                if(string.IsNullOrEmpty(propertyName))
                    continue;

                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                    return false;
            }

            return true;
        }
    }
}