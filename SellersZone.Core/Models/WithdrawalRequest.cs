using SellersZone.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Models
{
    public class WithdrawalRequest : BaseEntity
    {
        [StringLength(100)]
        public string? WalletType { get; set; }
        public int Status { get; set; }
        public DateTime RequestsDate { get; set; }

        public Wallet? Wallet { get; set; }
        public int WalletId { get; set; }

      
    }
}
