using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellersZone.Core.Models
{
    public class State : BaseEntity
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? NameAr { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? DescriptionAr { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }  
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ReturnedPrice { get; set; }

        public short? Ordering { get; set; }
        public bool IsActive { get; set; }

        public Country? Country { get; set; }
        public int? CountryId { get; set; }
    }
}
