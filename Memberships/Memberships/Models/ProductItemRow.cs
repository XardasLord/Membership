using System;

namespace Memberships.Models
{
    public class ProductItemRow
    {
        public int ItemId { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDownload { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}