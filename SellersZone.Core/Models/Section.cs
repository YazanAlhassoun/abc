using System.ComponentModel.DataAnnotations;

namespace SellersZone.Core.Models
{
    public class Section : BaseEntity
    {

        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? NameAr { get; set; }
        public bool IsSlider { get; set; }
        public short Ordering { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public List<ProductsSetion> ProductsSetion { get; set; }
    }

}
