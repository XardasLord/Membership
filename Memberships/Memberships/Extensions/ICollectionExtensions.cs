using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Memberships.Extensions
{
    public static class ICollectionExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this ICollection<T> items, int selectedValue)
        {
            return items.Select(x => new SelectListItem
            {
                Text = x.GetPropertyValue("Title"),
                Value = x.GetPropertyValue("Id"),
                Selected = x.GetPropertyValue("Id").Equals(selectedValue.ToString())
            });
        }
    }
}