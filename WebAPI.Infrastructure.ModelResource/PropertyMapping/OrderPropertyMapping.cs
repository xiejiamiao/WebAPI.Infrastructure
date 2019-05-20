using System;
using System.Collections.Generic;
using WebAPI.Infrastructure.DomainModel;
using WebAPI.Infrastructure.Services;

namespace WebAPI.Infrastructure.ResourceModel.PropertyMapping
{
    public class OrderPropertyMapping:PropertyMapping<OrderResourceModel,Order>
    {
        public OrderPropertyMapping() : base(new Dictionary<string, List<MappedProperty>>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(OrderResourceModel.OrderNo)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(OrderResourceModel.OrderNo),Revert = false}
            },
            [nameof(OrderResourceModel.ReciverName)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(OrderResourceModel.ReciverName),Revert = false}
            },
            [nameof(OrderResourceModel.ReciverMobile)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(OrderResourceModel.ReciverMobile),Revert = false}
            },
            [nameof(OrderResourceModel.ReciverProvince)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(OrderResourceModel.ReciverProvince),Revert = false}
            },
            [nameof(OrderResourceModel.ReciverCity)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(OrderResourceModel.ReciverCity),Revert = false}
            },
            [nameof(OrderResourceModel.ReciverDistrict)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(OrderResourceModel.ReciverDistrict),Revert = false}
            },
            [nameof(OrderResourceModel.ReciverDetailAddress)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(OrderResourceModel.ReciverDetailAddress),Revert = false}
            },
            [nameof(OrderResourceModel.TotalAmount)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(OrderResourceModel.TotalAmount),Revert = false}
            },
            [nameof(OrderResourceModel.ShippingAmount)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(OrderResourceModel.ShippingAmount),Revert = false}
            }
        })
        {
        }
    }
}