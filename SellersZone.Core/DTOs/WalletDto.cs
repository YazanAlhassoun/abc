using SellersZone.Core.Models.Identity;
using SellersZone.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.DTOs
{
    public class WalletDto : BaseEntityDto
    {
        public decimal Total { get; set; }
        public decimal ExpectedProfit { get; set; }
        public decimal Profit { get; set; }

        public List<WithdrawalRequestDto>? WithdrawalRequests { get; set; }
    }
}
