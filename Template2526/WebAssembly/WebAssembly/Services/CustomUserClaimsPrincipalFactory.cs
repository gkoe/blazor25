using Base.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace WebAssembly.Services
{
    //public class CustomUserClaimsPrincipalFactory
    //    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    //{
    //    public CustomUserClaimsPrincipalFactory(
    //        UserManager<ApplicationUser> userManager,
    //        RoleManager<IdentityRole> roleManager,
    //        IOptions<IdentityOptions> optionsAccessor)
    //        : base(userManager, roleManager, optionsAccessor)
    //    {
    //    }

    //    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    //    {
    //        var identity = await base.GenerateClaimsAsync(user);
    //        var roles = await UserManager.GetRolesAsync(user);
    //        //identity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Roles, role)));
    //        return identity;
    //    }
    //}

}
