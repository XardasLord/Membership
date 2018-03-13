using Memberships.Models;
using System.Collections.Generic;

namespace Memberships.Comparers
{
    public class ProductSectionEqualityComparer : IEqualityComparer<ProductSection>
    {
        public bool Equals(ProductSection section1, ProductSection section2)
        {
            return section1.Id.Equals(section2);
        }

        public int GetHashCode(ProductSection section)
        {
            return (section.Id).GetHashCode();
        }
    }
}