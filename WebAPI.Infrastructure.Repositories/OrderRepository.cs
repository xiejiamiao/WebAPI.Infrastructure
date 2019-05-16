using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAPI.Infrastructure.Database;
using WebAPI.Infrastructure.DomainModel;
using WebAPI.Infrastructure.DomainModel.Pagination;
using WebAPI.Infrastructure.DomainModel.QueryParameter;
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

        public async Task<PaginatedList<Order>> GetOrdersAsync(OrderQueryParameter orderQueryParameter)
        {
            var query = _dbContext.Orders.AsQueryable();
            
            if (!string.IsNullOrEmpty(orderQueryParameter.OrderNo))
                query = query.Where(x=>x.OrderNo==orderQueryParameter.OrderNo);
            if (!string.IsNullOrEmpty(orderQueryParameter.ReciverName))
                query = query.Where(x => x.ReciverName == orderQueryParameter.ReciverName);
            if (!string.IsNullOrEmpty(orderQueryParameter.ReciverMobile))
                query = query.Where(x => x.ReciverMobile == orderQueryParameter.ReciverMobile);
            if (!string.IsNullOrEmpty(orderQueryParameter.ReciverProvince))
                query = query.Where(x => x.ReciverProvince == orderQueryParameter.ReciverProvince);
            if (!string.IsNullOrEmpty(orderQueryParameter.ReciverCity))
                query = query.Where(x => x.ReciverCity == orderQueryParameter.ReciverCity);
            if (!string.IsNullOrEmpty(orderQueryParameter.ReciverDistrict))
                query = query.Where(x => x.ReciverDistrict == orderQueryParameter.ReciverDistrict);

            var count = query.Count();

            query = query.Skip(orderQueryParameter.PageIndex * orderQueryParameter.PageSize)
                .Take(orderQueryParameter.PageSize);

            var data = await query.ToListAsync();
            var paginatedList = new PaginatedList<Order>(orderQueryParameter.PageIndex,orderQueryParameter.PageSize,count,data);
            return paginatedList;
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