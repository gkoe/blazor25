using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAssembly.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("status")]
        [Authorize]
        public IActionResult GetAuthStatus()
        {
            if (User.Identity == null)
            {
                return Ok("Identity is null");
            }
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }
    }

}
