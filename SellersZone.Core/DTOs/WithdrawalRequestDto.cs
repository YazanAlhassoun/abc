using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.DTOs
{
    public class WithdrawalRequestDto : BaseEntityDto
    {
        public string? WalletType { get; set; }
        public int Status { get; set; }
        public DateTime RequestsDate { get; set; }
    }
}
