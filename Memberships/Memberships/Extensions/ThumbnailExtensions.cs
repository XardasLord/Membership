using Memberships.Comparers;
using Memberships.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Memberships.Extensions
{
    public static class ThumbnailExtensions
    {
        private static async Task<List<int>> GetSubscriptionIdsAsync(
            string userId = null, ApplicationDbContext db = null)
        {
            if (userId == null)
                return new List<int>();

            if (db == null)
                db = ApplicationDbContext.Create();

            try
            {
                return await db.UserSubscriptions
                .Where(us => us.UserId.Equals(userId))
                .Select(us => us.SubscriptionId)
                .ToListAsync();
            }
            catch { }

            return new List<int>();
        }

        public static async Task<IEnumerable<ThumbnailModel>> GetProductThumbnailsAsync(
            this List<ThumbnailModel> thumbnails, string userId = null, ApplicationDbContext db = null)
        {
            if (userId == null)
                return new List<ThumbnailModel>();

            if (db == null)
                db = ApplicationDbContext.Create();

            try
            {
                var subscriptionIds = await GetSubscriptionIdsAsync(userId, db);

                thumbnails = await db.SubscriptionProducts
                    .Where(sp => subscriptionIds.Contains(sp.SubscriptionId))
                    .Join(db.Products,
                        sp => sp.ProductId,
                        p => p.Id,
                        (sp, p) => new { sp, p })
                    .Join(db.ProductLinkTexts,
                        p2 => p2.p.ProductLinkTextId,
                        plt => plt.Id,
                        (p2, plt) => new { p2, plt })
                    .Join(db.ProductTypes,
                        p3 => p3.p2.p.ProductTypeId,
                        pt => pt.Id,
                        (p3, pt) => new ThumbnailModel
                        {
                            ProductId = p3.p2.p.Id,
                            SubscriptionId = p3.p2.sp.SubscriptionId,
                            Title = p3.p2.p.Title,
                            Description = p3.p2.p.Description,
                            ImageUrl = p3.p2.p.ImageUrl,
                            Link = "/ProductContent/Index/" + p3.p2.p.Id,
                            TagText = p3.plt.Title,
                            ContentTag = pt.Title
                        }).ToListAsync();
            }
            catch { }


            return thumbnails.Distinct(new ThumbnailEqualityComparer()).OrderBy(o => o.Title);
        }
    }
}