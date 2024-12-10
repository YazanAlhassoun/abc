
namespace SellersZone.Core.DTOs
{
    public class PaymentMethodDto : BaseEntityDto
    {
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public bool IsActive { get; set; }
        public short? Ordering { get; set; }
        public List<CustomFieldDto>? CustomFields { get; set; }
    }

}
