using SellersZone.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Models
{
    public class Cart : BaseEntity
    {
        public AppUser? AppUser { get; set; }
        public string? AppUserId { get; set; }

        public Product? Product { get; set; }
        public int? ProductId { get; set; }

        public int? Quantity { get; set; }
    }
}
