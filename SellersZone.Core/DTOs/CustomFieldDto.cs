
namespace SellersZone.Core.DTOs
{
    public class CustomFieldDto : BaseEntityDto
    {
        public string? FieldName { get; set; }
        public string? FieldNameAr { get; set; }
        public string? FieldType { get; set; }
        public string? FieldPlaceHolder { get; set; }
        public string? FieldPlaceHolderAr { get; set; }
        public string? FieldDefualtValue { get; set; }
        public string? FieldDefualtValueAr { get; set; }
        public bool IsRequired { get; set; }
        public short? Ordering { get; set; }
    }
}
