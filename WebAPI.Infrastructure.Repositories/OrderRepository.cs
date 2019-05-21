using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebAPI.Infrastructure.Database;
using WebAPI.Infrastructure.DomainModel;
using WebAPI.Infrastructure.DomainModel.Pagination;
using WebAPI.Infrastructure.Interfaces;
using WebAPI.Infrastructure.ModelDomain.QueryParameter;
using WebAPI.Infrastructure.Repositories.Extensions;
using WebAPI.Infrastructure.ResourceModel;
using WebAPI.Infrastructure.ResourceModel.OrderResource;
using WebAPI.Infrastructure.Services;

namespace WebAPI.Infrastructure.Repositories
{
    public class OrderRepository:IOrderRepository
    {
        private readonly SolutionDbContext _dbContext;
        private readonly IPropertyMappingContainer _propertyMappingContainer;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(SolutionDbContext dbContext,IPropertyMappingContainer propertyMappingContainer,ILogger<OrderRepository> logger)
        {
            _dbContext = dbContext;
            _propertyMappingContainer = propertyMappingContainer;
            _logger = logger;
        }

        public async Task<PaginatedList<Order>> GetOrdersAsync(OrderQueryParameter orderQueryParameter)
        {
            var query = _dbContext.Orders.OrderBy(x=>x.Id).AsQueryable();
            
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

            _logger.LogInformation($"Order By => {orderQueryParameter.OrderBy}");

            query = query.ApplySort(orderQueryParameter.OrderBy,
                _propertyMappingContainer.Resolve<OrderResourceModel, Order>());
            
            var count = await query.CountAsync();

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

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            return await _dbContext.Orders.FindAsync(id);
        }

        public void DeleteOrderAsync(Order order)
        {
            _dbContext.Orders.Remove(order);
        }

        public void Update(Order order)
        {
            _dbContext.Entry(order).State = EntityState.Modified;
        }
    }
}