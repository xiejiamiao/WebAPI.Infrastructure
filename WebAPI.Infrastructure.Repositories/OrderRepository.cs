using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAPI.Infrastructure.Database;
using WebAPI.Infrastructure.DomainModel;
using WebAPI.Infrastructure.Interfaces;

namespace WebAPI.Infrastructure.Repositories
{
    public class OrderRepository:IOrderRepository
    {
        private readonly SolutionDbContext _dbContext;

        public OrderRepository(SolutionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _dbContext.Orders.ToListAsync();
        }

        public void AddOrderAsync(Order order)
        {
            _dbContext.Orders.AddAsync(order);
        }

        public async Task<Order> GetOrderByIdAsync(string id)
        {
            return await _dbContext.Orders.FindAsync(id);
        }
    }
}