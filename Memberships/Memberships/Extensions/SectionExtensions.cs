using Memberships.Comparers;
using Memberships.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Memberships.Extensions
{
    public static class SectionExtensions
    {
        public static async Task<ProductSectionModel> GetProductSectionsAsync(int productId, string userId)
        {
            var db = ApplicationDbContext.Create();

            var sections = await db.Products
                .Where(p => p.Id.Equals(productId))
                .Join(db.ProductItems,
                    p => p.Id,
                    pi => pi.ProductId,
                    (p, pi) => new { p, pi })
                .Join(db.Items,
                    pi => pi.pi.ItemId,
                    i => i.Id,
                    (pi, i) => new { pi, i })
                .Join(db.Sections,
                    i => i.i.SectionId,
                    s => s.Id,
                    (i, s) => new ProductSection
                    {
                        Id = s.Id,
                        ItemTypeId = i.i.ItemTypeId,
                        Title = s.Title
                    })
                .OrderBy(s => s.Title)
                .ToListAsync();

            var result = sections.Distinct(new ProductSectionEqualityComparer()).ToList();

            var model = new ProductSectionModel
            {
                Sections = result,
                Title = await db.Products.Where(p => p.Id.Equals(productId)).Select(p => p.Title).FirstOrDefaultAsync()
            };


            return model;
        }
    }
}