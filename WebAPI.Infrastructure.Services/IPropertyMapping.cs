using System.Collections.Generic;

namespace WebAPI.Infrastructure.Services
{
    public interface IPropertyMapping
    {
        Dictionary<string,List<MappedProperty>> MappingDictionary { get; }
    }
}