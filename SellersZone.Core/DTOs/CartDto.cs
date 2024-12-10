using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.DTOs
{
    public class CartDto : BaseEntityDto
    {
        public int? ProductId { get; set; }
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ProfitPrice { get; set; }
        public string? MainImageUrl { get; set; }
        public short? InStock { get; set; }

        public int? ItemQuantity { get; set; }
    }
}
