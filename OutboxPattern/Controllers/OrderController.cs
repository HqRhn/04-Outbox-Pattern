using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace OutboxPattern.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly OrderRepository _repository;

        public OrderController(ILogger<OrderController> logger, OrderRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpPost, Route("AddNewOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<Guid>> AddNewOrder([FromBody] OrderRequest request)
        {

            using var transaction = _repository.Database.BeginTransaction();
            Order neworder = new Order()
            {
                OrderId = new Guid(),
                OrderDate = request.OrderDate,
                OrderReference = request.OrderReference
            };
            _repository.Orders.Add(neworder);
            await _repository.SaveChangesAsync();

            OrderEvent newOrderEvent = new OrderEvent()
            {
                OrderId = neworder.OrderId,
                OrderDate = neworder.OrderDate,
                OrderReference = neworder.OrderReference
            };
            OutboxMessage newOutboxMessage = new OutboxMessage()
            {
                EventName = "Order Created",
                MessageContent = JsonConvert.SerializeObject(newOrderEvent)
            };
            _repository.OutboxMessages.Add(newOutboxMessage);
            await _repository.SaveChangesAsync(); 
            
            transaction.Commit(); 
            return Ok(neworder.OrderId);
        }
    }
}