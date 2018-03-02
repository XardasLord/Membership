using Memberships.Areas.Admin.Models;
using Memberships.Entities;
using Memberships.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Memberships.Areas.Admin.Extensions
{
    public static class ConvertionExtensions
    {
        #region Product
        public static async Task<IEnumerable<ProductModel>> ConvertAsync(this IEnumerable<Product> products, ApplicationDbContext db)
        {
            if (products.Count().Equals(0))
                return new List<ProductModel>();

            var texts = await db.ProductLinkTexts.ToListAsync();
            var types = await db.ProductTypes.ToListAsync();

            return products.Select(p => new ProductModel
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                ProductLinkTextId = p.ProductLinkTextId,
                ProductTypeId = p.ProductTypeId,
                ProductLinkTexts = texts,
                ProductTypes = types
            });
        }

        public static async Task<ProductModel> ConvertAsync(this Product product, ApplicationDbContext db)
        {
            var texts = await db.ProductLinkTexts.FirstOrDefaultAsync(p => p.Id.Equals(product.ProductLinkTextId));
            var types = await db.ProductTypes.FirstOrDefaultAsync(p => p.Id.Equals(product.ProductTypeId));

            var productModel = new ProductModel
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                ProductLinkTextId = product.ProductLinkTextId,
                ProductTypeId = product.ProductTypeId,
                ProductLinkTexts = new List<ProductLinkText>(),
                ProductTypes = new List<ProductType>()
            };

            productModel.ProductLinkTexts.Add(texts);
            productModel.ProductTypes.Add(types);

            return productModel;
        }
        #endregion

        #region Product Item
        public static async Task<IEnumerable<ProductItemModel>> ConvertAsync(this IQueryable<ProductItem> productItems, ApplicationDbContext db)
        {
            if (productItems.Count().Equals(0))
                return new List<ProductItemModel>();

            return await productItems.Select(p => new ProductItemModel
            {
                ItemId = p.ItemId,
                ProductId = p.ProductId,
                ItemTitle = db.Items.FirstOrDefault(x => x.Id.Equals(p.ItemId)).Title,
                ProductTitle = db.Products.FirstOrDefault(x => x.Id.Equals(p.ProductId)).Title
            }).ToListAsync();
        }

        public static async Task<ProductItemModel> ConvertAsync(this ProductItem productItem, ApplicationDbContext db, bool addListData = true)
        {
            var productItemModel = new ProductItemModel
            {
                ItemId = productItem.ItemId,
                ProductId = productItem.ProductId,
                Items = addListData ? await db.Items.ToListAsync() : null,
                Products = addListData ? await db.Products.ToListAsync() : null,
                ItemTitle = (await db.Items.FirstOrDefaultAsync(x => x.Id.Equals(productItem.ItemId))).Title,
                ProductTitle = (await db.Products.FirstOrDefaultAsync(x => x.Id.Equals(productItem.ProductId))).Title
            };

            return productItemModel;
        }

        public static async Task<bool> CanChangeAsync(this ProductItem productItem, ApplicationDbContext db)
        {
            var oldPI = await db.ProductItems.CountAsync(p =>
                p.ProductId.Equals(productItem.OldProductId) &&
                p.ItemId.Equals(productItem.OldItemId));

            var newPI = await db.ProductItems.CountAsync(p =>
                p.ProductId.Equals(productItem.ProductId) &&
                p.ItemId.Equals(productItem.ItemId));

            return oldPI.Equals(1) && newPI.Equals(0);
        }

        public static async Task ChangeAsync(this ProductItem productItem, ApplicationDbContext db)
        {
            var oldProductItem = await db.ProductItems.FirstOrDefaultAsync(p =>
                p.ProductId.Equals(productItem.OldProductId) &&
                p.ItemId.Equals(productItem.OldItemId));

            var newProductItem = await db.ProductItems.FirstOrDefaultAsync(p =>
                p.ProductId.Equals(productItem.ProductId) &&
                p.ItemId.Equals(productItem.ItemId));

            if (oldProductItem != null && newProductItem == null)
            {
                newProductItem = new ProductItem
                {
                    ItemId = productItem.ItemId,
                    ProductId = productItem.ProductId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.ProductItems.Remove(oldProductItem);
                        db.ProductItems.Add(newProductItem);

                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch { transaction.Dispose(); }
                }
            }
        }
        #endregion

        #region Subscription Product 
        public static async Task<IEnumerable<SubscriptionProductModel>> ConvertAsync(this IQueryable<SubscriptionProduct> subscriptionProduct, ApplicationDbContext db)
        {
            if (subscriptionProduct.Count().Equals(0))
                return new List<SubscriptionProductModel>();

            return await subscriptionProduct.Select(p => new SubscriptionProductModel
            {
                SubscriptionId = p.SubscriptionId,
                ProductId = p.ProductId,
                SubscriptionTitle = db.Subscriptions.FirstOrDefault(x => x.Id.Equals(p.SubscriptionId)).Title,
                ProductTitle = db.Products.FirstOrDefault(x => x.Id.Equals(p.ProductId)).Title
            }).ToListAsync();
        }

        public static async Task<SubscriptionProductModel> ConvertAsync(this SubscriptionProduct subscriptionProduct, ApplicationDbContext db, bool addListData = true)
        {
            var subscriptionProductModel = new SubscriptionProductModel
            {
                SubscriptionId = subscriptionProduct.SubscriptionId,
                ProductId = subscriptionProduct.ProductId,
                Subscriptions = addListData ? await db.Subscriptions.ToListAsync() : null,
                Products = addListData ? await db.Products.ToListAsync() : null,
                SubscriptionTitle = (await db.Subscriptions.FirstOrDefaultAsync(x => x.Id.Equals(subscriptionProduct.SubscriptionId))).Title,
                ProductTitle = (await db.Products.FirstOrDefaultAsync(x => x.Id.Equals(subscriptionProduct.ProductId))).Title
            };

            return subscriptionProductModel;
        }

        public static async Task<bool> CanChangeAsync(this SubscriptionProduct subscriptionProduct, ApplicationDbContext db)
        {
            var oldSP = await db.SubscriptionProducts.CountAsync(p =>
                p.ProductId.Equals(subscriptionProduct.OldProductId) &&
                p.SubscriptionId.Equals(subscriptionProduct.OldSubscriptionId));

            var newSP = await db.SubscriptionProducts.CountAsync(p =>
                p.ProductId.Equals(subscriptionProduct.ProductId) &&
                p.SubscriptionId.Equals(subscriptionProduct.SubscriptionId));

            return oldSP.Equals(1) && newSP.Equals(0);
        }

        public static async Task ChangeAsync(this SubscriptionProduct subscriptionProduct, ApplicationDbContext db)
        {
            var oldSP = await db.SubscriptionProducts.FirstOrDefaultAsync(p =>
                p.ProductId.Equals(subscriptionProduct.OldProductId) &&
                p.SubscriptionId.Equals(subscriptionProduct.OldSubscriptionId));

            var newSP = await db.SubscriptionProducts.FirstOrDefaultAsync(p =>
                p.ProductId.Equals(subscriptionProduct.ProductId) &&
                p.SubscriptionId.Equals(subscriptionProduct.SubscriptionId));

            if (oldSP != null && newSP == null)
            {
                newSP = new SubscriptionProduct
                {
                    SubscriptionId = subscriptionProduct.SubscriptionId,
                    ProductId = subscriptionProduct.ProductId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.SubscriptionProducts.Remove(oldSP);
                        db.SubscriptionProducts.Add(newSP);

                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch { transaction.Dispose(); }
                }
            }
        }
        #endregion
    }
}