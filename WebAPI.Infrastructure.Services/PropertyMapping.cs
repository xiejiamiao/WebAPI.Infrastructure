using System.Collections.Generic;
using WebAPI.Infrastructure.DomainModel;

namespace WebAPI.Infrastructure.Services
{
    public abstract class PropertyMapping<TSource,TDestination>:IPropertyMapping where TDestination: BaseEntity
    {
        public Dictionary<string, List<MappedProperty>> MappingDictionary { get; }

        protected PropertyMapping(Dictionary<string, List<MappedProperty>> mappingDictionary)
        {
            MappingDictionary = mappingDictionary;
            mappingDictionary[nameof(BaseEntity.Id)] = new List<MappedProperty>
            {
                new MappedProperty() {Name = nameof(BaseEntity.Id), Revert = false}
            };
        }
    }
}