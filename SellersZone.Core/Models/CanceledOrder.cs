using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Models
{
    public class CanceledOrder: BaseEntity
    {
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public List<Order>? Orders { get; set; }
    }
}
