using SellersZone.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.DTOs
{
    public class SideEarningDto : BaseEntityDto
    {
        public decimal Amount { get; set; }
        public int? SideEarningReason { get; set; }
        public string? Note { get; set; }
        public string? AppUserId { get; set; }

        public SideEarningStatus Status { get; set; }
}
}
