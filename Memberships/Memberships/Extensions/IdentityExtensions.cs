using Memberships.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Memberships.Extensions
{
    public static class IdentityExtensions
    {
        public static async Task<string> GetUserFirstNameAsync(this IIdentity identity)
        {
            var db = ApplicationDbContext.Create();
            var user = await db.Users.FirstOrDefaultAsync(u => u.UserName.Equals(identity.Name));

            return user != null ? user.FirstName : String.Empty;
        }

        public static async Task GetUsersAsync(this List<UserViewModel> users)
        {
            var db = ApplicationDbContext.Create();
            users.AddRange(await db.Users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName
            }).OrderBy(o => o.Email).ToListAsync());
        }
    }
}