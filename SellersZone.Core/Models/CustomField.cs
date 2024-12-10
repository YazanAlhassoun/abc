
using System.ComponentModel.DataAnnotations;

namespace SellersZone.Core.Models
{
    public class CustomField : BaseEntity
    {
        [StringLength(200)]
        public string? FieldName { get; set; }

        [StringLength(200)]
        public string? FieldNameAr { get; set; }

        [StringLength(200)]
        public string? FieldType { get; set; }

        [StringLength(200)]
        public string? FieldPlaceHolder { get; set; }

        [StringLength(200)]
        public string? FieldPlaceHolderAr { get; set; }

        [StringLength(200)]
        public string? FieldDefualtValue { get; set; }

        [StringLength(200)]
        public string? FieldDefualtValueAr { get; set; }

        public bool IsRequired { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }
        public int? PaymentMethodId { get; set; }

        public short? Ordering { get; set; }
    }
}
