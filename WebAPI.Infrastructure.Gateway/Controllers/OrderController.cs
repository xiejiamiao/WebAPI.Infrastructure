using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebAPI.Infrastructure.DomainModel;
using WebAPI.Infrastructure.DomainModel.Pagination;
using WebAPI.Infrastructure.DomainModel.QueryParameter;
using WebAPI.Infrastructure.Interfaces;
using WebAPI.Infrastructure.ResourceModel;

namespace WebAPI.Infrastructure.Gateway.Controllers
{
    [Route("api/orders")]
    public class OrderController:Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderController> _logger;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public OrderController(IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            ILogger<OrderController> logger,
            IMapper mapper,
            IUrlHelper urlHelper)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        
        [HttpGet(Name = "GetOrders")]
        public async Task<IActionResult> Get(OrderQueryParameter parameter)
        {
            var paginatedList = await _orderRepository.GetOrdersAsync(parameter);
            var orderResourceModel = _mapper.Map<IEnumerable<Order>, IEnumerable<OrderResourceModel>>(paginatedList);
            var previousPageLink = paginatedList.HasPrevious ? CreateUrl(parameter, PaginatedUrlType.PreviousPage) : null;
            var nextPageLink = paginatedList.HasNext ? CreateUrl(parameter, PaginatedUrlType.NextPage) : null;
            var meta = new
            {
                paginatedList.PageIndex,
                paginatedList.PageSize,
                paginatedList.TotalItemCount,
                paginatedList.PageCount,
                previousPageLink,
                nextPageLink
            };
            Response.Headers.Add("X-Pagination",JsonConvert.SerializeObject(meta));
            return Ok(orderResourceModel);
        }

        [HttpPost]
        public async Task<IActionResult> Post(OrderResourceModel model)
        {
            var order = _mapper.Map<OrderResourceModel,Order>(model);
            _orderRepository.AddOrderAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return Ok("Handle success");
        }


        private string CreateUrl(OrderQueryParameter parameter,PaginatedUrlType paginatedUrlType)
        {
            switch (paginatedUrlType)
            {
                case PaginatedUrlType.PreviousPage:
                    parameter.PageIndex -= 1;
                    return _urlHelper.Link("GetOrders", parameter);
                case PaginatedUrlType.NextPage:
                    parameter.PageIndex += 1;
                    return _urlHelper.Link("GetOrders", parameter);
                default:
                    return _urlHelper.Link("GetOrders", parameter);
            }
        }
    }
}