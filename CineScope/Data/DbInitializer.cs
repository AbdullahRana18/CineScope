using Microsoft.AspNetCore.Identity;
namespace CineScope.Data
{
    public class DbInitializer
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            //Roles
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            var adminEmail = "admin@cinescope.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
