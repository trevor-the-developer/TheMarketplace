using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace.Data.Entities
{
    /// <summary>
    /// Represents the details for a product and/or service.
    /// </summary>
    public class ProductDetail : BaseEntity
    {
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ICollection<Media>? Media { get; set; }
        public ICollection<Document>? Documents { get; set; }
        // navigation properties
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}