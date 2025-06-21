using Core.Contracts;
using Core.Contracts.Persistence;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public record OrderItemCreateDto(int OrderId, int ProductId, int Amount);

    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderItemsController(IUnitOfWork unitOfWork) : ControllerBase
    {
        public IUnitOfWork UnitOfWork { get; } = unitOfWork;

        [HttpGet("GetByOrderId")]
        [ProducesResponseType(typeof(List<OrderItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            return Ok(await UnitOfWork.OrderItems.GetByOrderIdAsync(orderId));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(OrderItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            OrderItem? orderItem = await UnitOfWork.OrderItems.GetByIdAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            else
            {
                UnitOfWork.OrderItems.Delete(orderItem);
                await UnitOfWork.SaveChangesAsync();
                return Ok(orderItem);
            }
        }

        // GET: api/OrderItems/5
        [HttpGet("{id}", Name = "GetOrderItem")]
        [ProducesResponseType(typeof(OrderItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            OrderItem? orderItem = await UnitOfWork.OrderItems.GetByIdAsync(id);
            if (orderItem != null)
                return Ok(orderItem);
            else
                return NotFound();

        }

        // POST: api/Orders
        [HttpPost]
        [ProducesResponseType(typeof(OrderItem), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] OrderItemCreateDto orderItem)
        {
            if (orderItem != null && ModelState.IsValid)
            {
                OrderItem newOrderItem = new()
                {
                    OrderId = orderItem.OrderId,
                    Amount = orderItem.Amount,
                    ProductId = orderItem.ProductId,
                };
                UnitOfWork.OrderItems.Insert(newOrderItem);
                await UnitOfWork.SaveChangesAsync();
                return new CreatedAtRouteResult("GetOrderItem", new { id = newOrderItem.Id }, newOrderItem);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}

