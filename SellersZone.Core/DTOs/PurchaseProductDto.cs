using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.DTOs
{
    public class PurchaseProductDto : BaseEntityDto
    {
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public int? Quantity { get; set; }
        public decimal? PricePerItem { get; set; }
        public string? Image { get; set; }
        public string? SourceUrl { get; set; }
        public string? MadeIn { get; set; }
        public int? TargetCountry { get; set; }
        public int? Status { get; set; }
        public string? Note { get; set; }

        public string? AppUserId { get; set; }
    }
}
