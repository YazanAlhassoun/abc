using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace SellersZone.Core.Models
{
    public class ProductsSetion
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }


        public Section Section { get; set; }
        public int SectionId { get; set; }

    }
}
