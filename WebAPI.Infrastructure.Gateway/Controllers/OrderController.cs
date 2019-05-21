using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebAPI.Infrastructure.DomainModel;
using WebAPI.Infrastructure.DomainModel.Pagination;
using WebAPI.Infrastructure.Gateway.Helpers;
using WebAPI.Infrastructure.Interfaces;
using WebAPI.Infrastructure.ModelDomain.QueryParameter;
using WebAPI.Infrastructure.Repositories.Extensions;
using WebAPI.Infrastructure.ResourceModel;
using WebAPI.Infrastructure.ResourceModel.OrderResource;
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
        [RequestHeaderMatchingMediaType("Accept", new[] {"application/vnd.coName.hateoas+json"})]
        public async Task<IActionResult> GetHateaos(OrderQueryParameter parameter)
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
                var orderLinks = CreateLinksForOrder((Guid) dict["Id"], parameter.Fields);
                dict.Add("links", orderLinks);
                return dict;
            });
            var links = CreateLinksForOrders(parameter, paginatedList.HasPrevious, paginatedList.HasNext);
            var result = new
            {
                value = shapedWithLinks,
                links
            };
            
            return Ok(result);
        }


        [HttpGet(Name = "GetOrders")]
        [RequestHeaderMatchingMediaType("Accept", new[] {"application/json"})]
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
            return Ok(shapeOrderResources);
        }

        [HttpPost(Name = "CreateOrder")]
        [RequestHeaderMatchingMediaType("Content-Type", new[] { "application/vnd.coName.order.create+json" })]
        [RequestHeaderMatchingMediaType("Accept", new[] { "application/vnd.coName.hateoas+json" })]
        public async Task<IActionResult> Post([FromBody] OrderAddResource model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new MyUnprocessableEntityObjectResult(ModelState);
            }
            
            var order = _mapper.Map<OrderAddResource,Order>(model);
            _orderRepository.AddOrderAsync(order);
            if(await _unitOfWork.SaveChangesAsync()<=0)
                throw new Exception("Save Failed!");

            var orderResource =_mapper.Map<Order,OrderResourceModel>(order);
            var links = CreateLinksForOrder(order.Id);
            var linkedOrderResoure = orderResource.ToDynamic() as IDictionary<string, object>;
            linkedOrderResoure.Add("links",links);
            return CreatedAtRoute("GetOrder", new {id = linkedOrderResoure["Id"]}, linkedOrderResoure);
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
            var links = CreateLinksForOrder(id, fields);
            var result = (IDictionary<string, object>) shapeOrderResource;
            result.Add("link", links);
            return Ok(result);
        }

        [HttpDelete("{id}",Name = "DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();
            
            _orderRepository.DeleteOrderAsync(order);
            if(await _unitOfWork.SaveChangesAsync()<=0)
                throw new Exception($"Deleting order {id} failed when saving");
            
            return NoContent();
        }

        [HttpPut("{id}",Name = "UpdateOrder")]
        [RequestHeaderMatchingMediaType("Content-Type", new[] { "application/vnd.coName.order.update+json" })]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] OrderUpdateResource orderResource)
        {
            if (orderResource == null)
                return BadRequest();
            if(!ModelState.IsValid)
                return new MyUnprocessableEntityObjectResult(ModelState);

            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            _mapper.Map(orderResource, order);
            if(await _unitOfWork.SaveChangesAsync()<=0)
                throw new Exception($"Update order {id} failed when saving");
                
            return NoContent();
        }

        [HttpPatch("{id}",Name = "PartiallyUpdateOrder")]
        public async Task<IActionResult> PartiallyUpdate(Guid id, [FromBody] JsonPatchDocument<OrderUpdateResource> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            var orderToPatch = _mapper.Map<OrderUpdateResource>(order);
            patchDoc.ApplyTo(orderToPatch, ModelState);
            TryValidateModel(orderToPatch);
            if(!ModelState.IsValid)
                return new MyUnprocessableEntityObjectResult(ModelState);

            _mapper.Map(orderToPatch, order);
            _orderRepository.Update(order);
            
            if(await _unitOfWork.SaveChangesAsync() <= 0)
                throw new Exception($"Update order {id} failed when saving");

            return NoContent();
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

        private IEnumerable<LinkResourceModel> CreateLinksForOrder(Guid id, string fields = null)
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

        private IEnumerable<LinkResourceModel> CreateLinksForOrders(OrderQueryParameter orderQueryParameter, bool hasPrevious, bool hasNext)
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