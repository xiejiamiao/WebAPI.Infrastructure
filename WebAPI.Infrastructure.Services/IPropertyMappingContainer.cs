using WebAPI.Infrastructure.DomainModel;

namespace WebAPI.Infrastructure.Services
{
    public interface IPropertyMappingContainer
    {
        void Register<T>() where T : IPropertyMapping, new();
        IPropertyMapping Resolve<TSource, TDestination>() where TDestination : BaseEntity;
        bool ValidateMappingExsitFor<TSource, TDestination>(string fields) where TDestination : BaseEntity;
    }
}