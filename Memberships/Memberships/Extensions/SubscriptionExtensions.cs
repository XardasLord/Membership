using Memberships.Entities;
using Memberships.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Memberships.Extensions
{
    public static class SubscriptionExtensions
    {
        private static async Task<int> GetSubscriptionIdByRegistrationCode(this IDbSet<Subscription> subscription, string code)
        {
            if(code.IsNullOrEmpty() || subscription == null)
                return Int32.MinValue;

            try
            {
                return await subscription.Where(s => s.RegistrationCode.Equals(code)).Select(s => s.Id).FirstOrDefaultAsync();
            }
            catch { return Int32.MinValue; }
        }

        private static async Task Register(this IDbSet<UserSubscription> userSubscription, int subscriptionId, string userId)
        {
            if (userId.IsNullOrEmpty() || subscriptionId == Int32.MinValue || userSubscription == null)
                return;

            try
            {
                var exist = await Task.Run(() => userSubscription.CountAsync
                (s => s.SubscriptionId.Equals(subscriptionId) &&
                s.UserId.Equals(userId))) > 0;

                if (!exist)
                {
                    await Task.Run(() => userSubscription.Add(new UserSubscription
                    {
                        UserId = userId,
                        SubscriptionId = subscriptionId,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.MaxValue
                    }));
                }
            }
            catch { return; }
        }

        public static async Task<bool> RegisterUserSubscriptionCode(string userId, string code)
        {
            try
            {
                var db = ApplicationDbContext.Create();
                var id = await db.Subscriptions.GetSubscriptionIdByRegistrationCode(code);

                if (id <= 0)
                    return false;

                await db.UserSubscriptions.Register(id, userId);

                if (db.ChangeTracker.HasChanges())
                    await db.SaveChangesAsync();

                return true;
            }
            catch { return false; }
        }
    }
}