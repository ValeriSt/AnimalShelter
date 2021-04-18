using AS.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AS.Data.Repos
{
    public class UserRepo
    {
        private readonly UserManager<ASUser> userManager;

        public UserRepo(UserManager<ASUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<ASUser> GetUserAsync(ClaimsPrincipal user)
        {
            return await userManager.GetUserAsync(user);
        }
        public async Task<bool> IsInAdminAsync(ASUser user)
        {
            return await userManager.IsInRoleAsync(user, "Admin");
        }
    }
}
