using Memberships.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Memberships.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetUserFirstName(this IIdentity identity)
        {
            var db = ApplicationDbContext.Create();
            var user = db.Users.FirstOrDefault(u => u.UserName.Equals(identity.Name));

            return user != null ? user.FirstName : String.Empty;
        }

        public static void GetUsers(this List<UserViewModel> users)
        {
            var db = ApplicationDbContext.Create();
            users.AddRange(db.Users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName
            }).OrderBy(o => o.Email).ToList());
        }
    }
}