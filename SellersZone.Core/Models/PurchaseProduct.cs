using SellersZone.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Models
{
    public class PurchaseProduct : BaseEntity
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? NameAr { get; set; }
        public int? Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PricePerItem { get; set; }

        [StringLength(1000)]
        public string? Image { get; set; }

        [StringLength(1000)]
        public string? SourceUrl { get; set; }

        [StringLength(100)]
        public string? MadeIn { get; set; }
        public int? TargetCountry { get; set; }
        public int? Status { get; set; }

        [StringLength(2000)]
        public string? Note { get; set; }

        public AppUser? AppUser { get; set; }
        public string? AppUserId { get; set; }
    }
}
