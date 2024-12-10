using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SellersZone.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Infra.Helpers
{
    public static class UserManagerExtension
    {
        //to get individual user
        public static async Task<AppUser> FindByEmailFromClaims(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Email);
            return await userManager.Users.Include(u => u.Wallet).SingleOrDefaultAsync(x => x.Email == email);
        }

        //check if the user is an admin
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            return roleClaim != null && roleClaim.Value == "Admin";
        }

        public static bool IsSubAdmin(this ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            return roleClaim != null && roleClaim.Value == "SubAdmin";
        }
    }
}
