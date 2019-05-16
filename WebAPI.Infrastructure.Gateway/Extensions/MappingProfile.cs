using AutoMapper;
using WebAPI.Infrastructure.DomainModel;
using WebAPI.Infrastructure.ResourceModel;

namespace WebAPI.Infrastructure.Gateway.Extensions
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Order,OrderResourceModel>();
            CreateMap<OrderResourceModel,Order>();
        }
    }
}