using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using WebAPI.Infrastructure.DomainModel;
using WebAPI.Infrastructure.DomainModel.Pagination;
using WebAPI.Infrastructure.ModelDomain.QueryParameter;

namespace WebAPI.Infrastructure.Interfaces
{
    public interface IOrderRepository
    {
        Task<PaginatedList<Order>> GetOrdersAsync(OrderQueryParameter orderQueryParameter);
        void AddOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(Guid id);
    }
}