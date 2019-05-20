using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebAPI.Infrastructure.DomainModel;
using WebAPI.Infrastructure.DomainModel.Pagination;
using WebAPI.Infrastructure.Interfaces;
using WebAPI.Infrastructure.ModelDomain.QueryParameter;
using WebAPI.Infrastructure.Repositories.Extensions;
using WebAPI.Infrastructure.ResourceModel;
using WebAPI.Infrastructure.Services;

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
        private readonly IPropertyMappingContainer _propertyMappingContainer;
        private readonly ITypeHelperService _typeHelperService;

        public OrderController(IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            ILogger<OrderController> logger,
            IMapper mapper,
            IUrlHelper urlHelper,
            IPropertyMappingContainer propertyMappingContainer,
            ITypeHelperService typeHelperService)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _urlHelper = urlHelper;
            _propertyMappingContainer = propertyMappingContainer;
            _typeHelperService = typeHelperService;
        }

        
        [HttpGet(Name = "GetOrders")]
        public async Task<IActionResult> Get(OrderQueryParameter parameter)
        {
            if (!_propertyMappingContainer.ValidateMappingExsitFor<OrderResourceModel, Order>(parameter.OrderBy))
            {
                return BadRequest("Can't finds fields for sorting");
            }

            if (!_typeHelperService.TypeHasProperties<OrderResourceModel>(parameter.Fields))
            {
                return BadRequest("Fields not exist");
            }
            
            
            var paginatedList = await _orderRepository.GetOrdersAsync(parameter);
            var orderResourceModel = _mapper.Map<IEnumerable<Order>, IEnumerable<OrderResourceModel>>(paginatedList);
            
            var previousPageLink = paginatedList.HasPrevious ? CreateOrderUrl(parameter, PaginatedUrlType.PreviousPage) : null;
            var nextPageLink = paginatedList.HasNext ? CreateOrderUrl(parameter, PaginatedUrlType.NextPage) : null;
            var meta = new
            {
                paginatedList.PageIndex,
                paginatedList.PageSize,
                paginatedList.TotalItemCount,
                paginatedList.PageCount,
                previousPageLink,
                nextPageLink
            };
            
            Response.Headers.Add("X-Pagination",JsonConvert.SerializeObject(meta,new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            var shapeOrderResources = orderResourceModel.ToDynamicIEnumerable(parameter.Fields);
            _logger.LogInformation(JsonConvert.SerializeObject(shapeOrderResources));

            var shapedWithLinks = shapeOrderResources.Select(x =>
            {
                var dict = x as IDictionary<string, object>;
                var orderLinks = CreateLinkForOrder((Guid) dict["Id"], parameter.Fields);
                dict.Add("links", orderLinks);
                return dict;
            });
            var links = CreateLinkForOrders(parameter, paginatedList.HasPrevious, paginatedList.HasNext);
            var result = new
            {
                value = shapedWithLinks,
                links
            };
            
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(OrderResourceModel model)
        {
            var order = _mapper.Map<OrderResourceModel,Order>(model);
            _orderRepository.AddOrderAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return Ok("Handle success");
        }

        [HttpGet("{id}",Name = "GetOrder")]
        public async Task<IActionResult> Get(Guid id, string fields = null)
        {
            if (!_typeHelperService.TypeHasProperties<OrderResourceModel>(fields))
            {
                return BadRequest("Fields not exist");
            }
            var order = await _orderRepository.GetOrderByIdAsync(id);
            var resource = _mapper.Map<Order, OrderResourceModel>(order);
            var shapeOrderResource = resource.ToDynamic(fields);
            var links = CreateLinkForOrder(id, fields);
            var result = (IDictionary<string, object>) shapeOrderResource;
            result.Add("link", links);
            return Ok(result);
        }

        private string CreateOrderUrl(OrderQueryParameter parameter,PaginatedUrlType paginatedUrlType)
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

        private IEnumerable<LinkResourceModel> CreateLinkForOrder(Guid id, string fields = null)
        {
            var links = new List<LinkResourceModel>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkResourceModel(_urlHelper.Link("GetOrder", new {id}), "self", "GET"));
            }
            else
            {
                links.Add(new LinkResourceModel(_urlHelper.Link("GetOrder", new {id, fields}), "self", "GET"));
            }

            links.Add(new LinkResourceModel(_urlHelper.Link("DeleteOrder", new {id}), "delete_order", "DELETE"));
            return links;
        }

        private IEnumerable<LinkResourceModel> CreateLinkForOrders(OrderQueryParameter orderQueryParameter, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkResourceModel>
            {
                new LinkResourceModel(CreateOrderUrl(orderQueryParameter,PaginatedUrlType.CurrentPage),"self","GET")
            };
            if(hasPrevious)
                links.Add(new LinkResourceModel(CreateOrderUrl(orderQueryParameter,PaginatedUrlType.PreviousPage),"previous_page","GET"));
            if(hasNext)
                links.Add(new LinkResourceModel(CreateOrderUrl(orderQueryParameter,PaginatedUrlType.NextPage),"next_page","GET"));
            return links;
        }
    }
}