using AutoMapper;
using WebAPI.Infrastructure.DomainModel;
using WebAPI.Infrastructure.ResourceModel;
using WebAPI.Infrastructure.ResourceModel.OrderResource;

namespace WebAPI.Infrastructure.Gateway.Extensions
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Order,OrderResourceModel>();
            CreateMap<OrderResourceModel,Order>();

            CreateMap<Order, OrderAddResource>();
            CreateMap<OrderAddResource, Order>();
            
            CreateMap<Order, OrderUpdateResource>();
            CreateMap<OrderUpdateResource, Order>();
        }
    }
}