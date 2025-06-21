using Core.Contracts.Persistence;
using Core.DataTransferObjects;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Web.Controllers;
using static Base.ExtensionMethods.IncludeHelper;

namespace WebAssembly.Controllers
{
    public record OrderCreateDto(string OrderNr, int CustomerId, OrderType OrderType);
    public record OrderUpdateDto(int Id, byte[]? RowVersion, string OrderNr, int CustomerId, OrderType OrderType);

    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController(IUnitOfWork unitOfWork, ILogger<OrderItemsController> logger) : ControllerBase
    {

        public IUnitOfWork UnitOfWork { get; } = unitOfWork;
        public ILogger<OrderItemsController> Logger { get; } = logger;

        [HttpGet()]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            var result = await UnitOfWork.Orders
                .GetAsync(includeProperties: Include<Order>(o => o.Customer,
                       o => o.OrderItems)).ToListAsync();
            return Ok(result);
        }


        /// <summary>
        /// Liefert eine Liste aller Bestellungen als DTO zurück
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPage(
            [FromQuery] string? nameFilter,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            Expression<Func<Order, bool>>? filter =
                o => string.IsNullOrEmpty(nameFilter) || o.Customer.LastName.Contains(nameFilter);

            static IOrderedQueryable<Order> orderBy(IQueryable<Order> q) => q.OrderByDescending(o => o.Date);

            Expression<Func<Order, OrdersApiGetDto>> selector =
                o => new OrdersApiGetDto(
                    o.Id,
                    o.OrderNr,
                    o.Customer.LastName + " " + o.Customer.FirstName,
                    o.OrderItems.Sum(oi => oi.Product.Price * oi.Amount)
                );

            var paged = await UnitOfWork.Orders.GetProjectedPageAsync(
                page,
                pageSize,
                disableTracking: true,
                filter: filter,
                orderBy: orderBy,
                selector: selector,
                includeProperties: Include<Order>(o => o.Customer,
                                           o => o.OrderItems)
            );

            return Ok(paged);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Count([FromQuery] string? nameFilter)
        {
            Expression<Func<Order, bool>> filter = o => string.IsNullOrEmpty(nameFilter) || o.Customer.LastName.Contains(nameFilter);
            var count = await UnitOfWork.Orders.CountAsync(filter);
            return count;
        }

        // GET: api/Orders/5
        /// <summary>
        /// Liefert eine Bestellung 
        /// </summary>
        /// <param name="id">Gewünschte Id der Bestellung</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}",Name = "GetOrder")]
        public async Task<IActionResult> Get(int id)
        {
            Order? order = await UnitOfWork.Orders.GetByIdAsync(id);
            if (order != null)
                return Ok(order);
            else
                return NotFound();

        }

        // POST: api/Orders
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderCreateDto order)
        {
            if (order != null && ModelState.IsValid)
            {
                Order newOrder = new()
                {
                    CustomerId = order.CustomerId,
                    Date = DateTime.Now,
                    OrderNr = order.OrderNr,
                    OrderType = order.OrderType
                };

                await UnitOfWork.Orders.AddAsync(newOrder);
                await UnitOfWork.SaveChangesAsync();
                return new CreatedAtRouteResult("GetOrder", new { id = newOrder.Id }, newOrder);
            }
            else
            {
                return BadRequest();
            }
        }

        // PUT: api/Orders/5
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] OrderUpdateDto order)
        {
            if (id > 0 && id == order.Id && ModelState.IsValid)
            {
                //because the date should not be changed, we have to get the original order and set the other field values
                Order? updateOrder = await UnitOfWork.Orders.GetByIdAsync(id);
                if (updateOrder == null)
                    return NotFound();
                
                updateOrder.CustomerId = order.CustomerId;
                updateOrder.OrderNr = order.OrderNr;
                updateOrder.OrderType = order.OrderType;
                updateOrder.RowVersion = order.RowVersion ?? [];

                //UnitOfWork.Orders.Update(updateOrder);
                try
                {
                    await UnitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await UnitOfWork.Orders.GetByIdAsync(id) == null)
                        return NotFound();
                    else
                        return Conflict();
                }           
                return Ok(updateOrder);
            }
            else
            {
                return BadRequest();
            }
        }

        // DELETE: api/ApiWithActions/5
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            if (!await UnitOfWork.Orders.DeleteAsync(id))
            {
                return NotFound();
            }
            else
            {
                await UnitOfWork.SaveChangesAsync();
                return Ok();
            }
        }

        [ProducesResponseType(typeof(SalesStatisticDto), StatusCodes.Status200OK)]
        [HttpGet("SalesStatistic")]
        public async Task<IActionResult> GetSalesStatistic()
        {
            return Ok(await UnitOfWork.Orders.GetSalesStatisticAsync());
        }
    }
}
