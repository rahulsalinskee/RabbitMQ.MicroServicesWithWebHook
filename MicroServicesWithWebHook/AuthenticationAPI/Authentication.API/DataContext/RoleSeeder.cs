using Microsoft.AspNetCore.Identity;

namespace Authentication.API.DataContext
{
    public static class RoleSeeder
    {
        public static async Task SeedRoleAsyncExtension(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            /* Define the roles the application requires */
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                /* Check if the role already exists in the AspNetRoles table */
                var doesRoleExist = await roleManager.RoleExistsAsync(roleName: role);

                if (!doesRoleExist)
                {
                    /* Create the role if it does not exist */
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
