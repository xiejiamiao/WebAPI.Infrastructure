using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Infrastructure.DomainModel;
using WebAPI.Infrastructure.Interfaces;

namespace WebAPI.Infrastructure.Gateway.Controllers
{
    [Route("api/orders")]
    public class OrderController:Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderRepository orderRepository,IUnitOfWork unitOfWork,ILogger<OrderController> logger)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderRepository.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Order order)
        {
            _orderRepository.AddOrderAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return Ok("Handle success");
        }
    }
}