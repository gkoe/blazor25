using Core.Contracts;
using Core.Contracts.Persistence;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController(IUnitOfWork unitOfWork, ILogger<OrderItemsController> logger) : ControllerBase
    {
        public IUnitOfWork UnitOfWork { get; } = unitOfWork;
        public ILogger<OrderItemsController> Logger { get; } = logger;

        // GET: api/Products
        [ProducesResponseType(typeof(List<Product>),StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await UnitOfWork.Products.GetAllOrderedByNameAsync());
        }
    }
}
