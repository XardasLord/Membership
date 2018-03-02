using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Memberships.Areas.Admin.Models
{
    public class EditButtonModel
    {
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public int SubscriptionId { get; set; }
        public string Link
        {
            get
            {
                var link = new StringBuilder("?");
                if (ItemId > 0)
                    link.Append($"itemId={ItemId}&");
                if (ProductId > 0)
                    link.Append($"productId={ProductId}&");
                if (SubscriptionId > 0)
                    link.Append($"subscriptionId={SubscriptionId}&");

                return link.ToString().Substring(0, link.Length - 1);
            }
        }
    }
}