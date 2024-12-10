using SellersZone.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Models
{
    public class UserPaymentMethod : BaseEntity
    {
        public AppUser? AppUser { get; set; }
        public string? AppUserId { get; set; }


        public PaymentMethod? PaymentMethod { get; set; }
        public int? PaymentMethodId { get; set; }

        public bool Approval { get; set; }
    }
}
