using SellersZone.Core.Models;

namespace SellersZone.Core.DTOs
{
    public class UserDto : BaseEntityDto
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? StoreName { get; set; }
        public string? FromCountry { get; set; }
        public int? CountryId { get; set; }
        public bool EmailConfirmed { get; set; }
        public long? ExpireAt { get; set; }
        public string? Password { get; set; }

        public bool LockoutEnabled { get; set; }

        public List<SellersProduct>? SellersProducts { get; set; }

    }

    public class SellersProduct
    {
        public int? ProductId { get; set; }
        public bool? IsForSeller { get; set; }
        public decimal? ServicePriceForAllSellers { get; set; }
    }
}
