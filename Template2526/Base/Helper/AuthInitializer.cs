
using Base.Entities;
using Base.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Helper
{
    public static class AuthInitializer
    {

        public static async Task InitalizeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if (userManager.Users.Any())
            {
                return;
            }
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = [MagicStrings.Role_Admin, MagicStrings.Role_User, MagicStrings.Role_Guest];

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            var admin = new ApplicationUser
            {
                UserName = "admin@htl.at",
                Email = "admin@htl.at",
                EmailConfirmed = true
            };
            var result = userManager.CreateAsync(admin, "Admin123*").GetAwaiter().GetResult();
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, MagicStrings.Role_Admin);
                await userManager.AddToRoleAsync(admin, MagicStrings.Role_User);
            }
            var user = new ApplicationUser
            {
                UserName = "user@htl.at",
                Email = "user@htl.at",
                EmailConfirmed = true
            };
            result =userManager.CreateAsync(user, "User123*").GetAwaiter().GetResult();
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, MagicStrings.Role_User);
            }

        }
    }
}
