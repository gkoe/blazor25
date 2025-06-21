using Core.Contracts.Persistence;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomersController(IUnitOfWork unitOfWork, ILogger<CustomersController> logger) : ControllerBase
    {
       public IUnitOfWork UnitOfWork { get; } = unitOfWork;
        private readonly ILogger<CustomersController> _logger = logger;

        // GET: api/Customers
        /// <summary>
        /// Liefert alle Kunden sortiert nach Nachnamen und Vorname
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(List<Customer>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await UnitOfWork.Customers.GetAllAsync());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] CustomerApiPostDto postCustomer)
        {
            if (ModelState.IsValid)
            {
                Customer customer = new()
                {
                    CustomerNr = postCustomer.CustomerNr,
                    LastName = postCustomer.LastName,
                    FirstName = postCustomer.FirstName
                };
                UnitOfWork.Customers.Add(customer);
                try
                {
                    await UnitOfWork.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Fehler beim Anlegen des Customers");
                    return BadRequest(e.Message);
                }

                return Created($"/api/orders/{customer.Id}", new CustomerApiGetDto(customer.Id, customer.CustomerNr, customer.FirstName,customer.LastName));
            }
            else
            {
                return BadRequest("Illegaler API Aufruf");
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> IsFullNameUnique([FromQuery] string firstName, [FromQuery] string lastName)
        {
            bool isUnique = await UnitOfWork.Customers.IsFullNameUniqueAsync(firstName, lastName);
            return Ok(isUnique);
        }


    }
}
