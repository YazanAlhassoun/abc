using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Models
{
    public class ProductsOrder: BaseEntity
    {
        public Order Order { get; set; }
        public int OrderId { get; set; }

        public Product Product { get; set; }
        public int ProductId { get; set; }

        public string OrderSku { get; set; }
        public int Quantity { get; set; }
    }
}
