namespace SellersZone.Core.DTOs
{
    public class BaseEntityDto 
    {
        public int Id { get; set; }
        public DateTime? CreationDate
        {
            set
            {
                createdAt = value.ToFormattedString();
            }
        }

        public DateTime? ModificationDate
        {
            set
            {
                updatedAt = value.ToFormattedString();
            }
        }

        public string? createdAt { get; private set; }
        public string? updatedAt { get; private set; }
    }

    public static class DateTimeExtensions
    {
        public static string ToFormattedString(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToString("yyyy-MM-dd") : null;
        }
    }

}
