using Microsoft.AspNetCore.Identity;
using SellersZone.Core.Models;
using SellersZone.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Infra.Helpers
{
    public class IdentityConfiguration
    {
        public static async Task SeedIdentityAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Check if Admin role exists or create it
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var adminRole = new IdentityRole("Admin");
                await roleManager.CreateAsync(adminRole);
            }

            // Check if SubAdmin role exists or create it
            if (!roleManager.RoleExistsAsync("SubAdmin").Result)
            {
                var adminRole = new IdentityRole("Admin");
                await roleManager.CreateAsync(adminRole);
            }

            // Check if role exists or create it
            if (!roleManager.RoleExistsAsync("Client").Result)
            {
                var adminRole = new IdentityRole("Client");
                await roleManager.CreateAsync(adminRole);
            }

            // Check if admin user exists or create it
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    FirstName = "Admin",
                    Email = "Admin@gmail.com",
                    UserName = "Admin@gmail.com",
                    PhoneNumber = "0798727713",
                    EmailConfirmed = true
                };

                var newAdmin = await userManager.CreateAsync(user, "Admin@123");
                if (newAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");

                    var wallet = new Wallet
                    {
                        Total = 0,
                        Profit = 0,
                        ExpectedProfit = 0,
                        CreatedAt = DateTime.UtcNow,
                        //AppUser = user
                    };

                    user.Wallet = wallet;
                    await userManager.UpdateAsync(user);
                }
            }

        }
    }
}
