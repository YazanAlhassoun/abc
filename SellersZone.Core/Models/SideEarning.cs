using SellersZone.Core.Enums;
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
    public class SideEarning : BaseEntity
    {
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public int? SideEarningReason { get; set; }

        [StringLength(1000)]
        public string? Note { get; set; }

        public int? Status { get; set; }

        public AppUser? AppUser { get; set; }
        public string? AppUserId { get; set; }

    }
}
