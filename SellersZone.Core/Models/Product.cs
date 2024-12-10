using SellersZone.Core.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellersZone.Core.Models
{
    public class Product : BaseEntity
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? NameAr { get; set; }

        [StringLength(10000)]
        public string? Description { get; set; }

        [StringLength(10000)]
        public string? DescriptionAr { get; set; }

        [StringLength(10000)]
        public string? Caption { get; set; }

        [StringLength(10000)]
        public string? CaptionAr { get; set; }

        [StringLength(10000)]
        public string? Audience { get; set; }

        [StringLength(10000)]
        public string? AudienceAr { get; set; }

        [StringLength(250)]
        public string? MainImageUrl { get; set; }

        [StringLength(250)]
        public string? ZipFileUrl { get; set; }

        [StringLength(20)]
        public string SKU { get; set; }

        [StringLength(100)]
        public string? BarCode { get; set; }


        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SalePrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ProfitPrice { get; set; } 
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal HighestPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ServicePriceForAllSellers { get; set; }


        public int? ActualInStock { get; set; }
        public int? InStock { get; set; }
        public int? DamageStock { get; set; }
        public int? Ordering { get; set; }
        public bool IsActive { get; set; }
        public bool? IsForSeller { get; set; }

        public AppUser? AppUser { get; set; }
        public string? AppUserId { get; set; }

        public List<ProductImage>? ProductImage { get; set; } 
        public List<ProductsSetion>? ProductsSection { get; set; }
        public List<Cart>? Carts { get; set; }
        public List<WishList>? WishLists { get; set; }
        public List<ProductsOrder>? ProductsOrder { get; set; }
    }
}
