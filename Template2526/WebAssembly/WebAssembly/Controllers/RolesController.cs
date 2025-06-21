using Base.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Webassembly.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RolesController(UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpPost]
        //[Authorize(Roles = "Admin")] // Nur Admins dürfen das
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return NotFound("Benutzer nicht gefunden.");

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok($"Benutzer {user.Email} wurde der Rolle {model.Role} hinzugefügt.");
        }
    }

    public class AssignRoleModel
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}