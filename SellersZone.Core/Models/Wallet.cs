using SellersZone.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Models
{
    public class Wallet : BaseEntity
    {
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ExpectedProfit { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Profit { get; set; }

        public List<AppUser>? AppUsers { get; set; }
        public List<WithdrawalRequest>? WithdrawalRequests { get; set; }
    }
}
