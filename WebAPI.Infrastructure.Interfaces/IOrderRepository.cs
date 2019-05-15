using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using WebAPI.Infrastructure.DomainModel;

namespace WebAPI.Infrastructure.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        void AddOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(string id);
    }
}