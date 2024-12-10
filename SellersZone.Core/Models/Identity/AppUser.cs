using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SellersZone.Core.Models.Identity
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [StringLength(200)]
        public string? StoreName { get; set; }

        [StringLength(100)]
        public string? FromCountry { get; set; }

        public Wallet? Wallet { get; set; }
        public int? WalletId { get; set; }

        public Country? Country { get; set; }
        public int? CountryId { get; set; }

        public List<Cart>? Carts { get; set; }
        public List<WishList>? WishLists { get; set; }
        public List<Order>? Orders { get; set; }
        public List<Product>? Products { get; set; }
        public List<PurchaseProduct>? PurchaseProducts { get; set; }
        public List<UserPaymentMethod>? UserPaymentMethods { get; set; }
     
    }
}
