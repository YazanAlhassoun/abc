using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.DTOs
{
    public class ProductsOrderDto : BaseEntityDto
    {
        public OrderDto? Order { get; set; }

        public List<ProductDto>? Products { get; set; }

    }
}
