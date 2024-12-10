
namespace SellersZone.Core.Models
{
    public class PaymentMethod : BaseEntity
    {
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public bool ISActive { get; set; }
        public short? Ordering { get; set; }

        public List<UserPaymentMethod>? UserPaymentMethods { get; set; }
        public List<CustomField>? CustomFields { get; set; }
    }
}
