using Microsoft.AspNetCore.Identity;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models;
using SellersZone.Core.Models.Identity;
using SellersZone.Core.Wrapper;
using SellersZone.Infra.Helpers;
using System.Security.Claims;

namespace SellersZone.Infra.Services
{
    public class WishListRepository : IWishListRepository
    {
        private readonly StoreContext _db;
        private readonly UserManager<AppUser> _userManager;

        public WishListRepository(StoreContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public Response<CartDto> GetWishList(ClaimsPrincipal userClaims)
        {
            try
            {
                var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                if (user == null) throw new Exception("There is no user !");

                IQueryable<WishList> query = _db.WishLists;
                var cartDto = query.Where(c => c.AppUserId == user.Id).Select(cart => new CartDto
                {
                    Id = cart.Id,
                    Name = cart.Product != null ? cart.Product.Name : string.Empty,
                    NameAr = cart.Product != null ? cart.Product.NameAr : string.Empty,
                    MainImageUrl = cart.Product != null ? cart.Product.MainImageUrl : string.Empty,
                    SalePrice = cart.Product != null ? cart.Product.SalePrice : 0,
                    ProfitPrice = cart.Product != null ? cart.Product.ProfitPrice : 0,
                    InStock = (short)(cart.Product != null ? cart.Product.InStock : 0),
                }).ToList();

                return new Response<CartDto>()
                {
                    Data = cartDto,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response<CartDto>()
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            };

        }

        public Response<CartDto> AddToWishList(RequestParam param, ClaimsPrincipal userClaims)
        {
            try
            {
                if (param.ProductId == null) throw new Exception("Product Id can't be null");

                var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                if (user == null) throw new Exception("There is no user !");

                var product = _db.Products.SingleOrDefault(p => p.Id == param.ProductId);
                var wishList = new WishList()
                {
                    AppUserId = user.Id,
                    ProductId = (int)param.ProductId,
                };

                _db.WishLists.Add(wishList);
                _db.SaveChanges();

                var cartDto = new CartDto
                {
                    Name = wishList.Product != null ? wishList.Product.Name : string.Empty,
                    NameAr = wishList.Product != null ? wishList.Product.NameAr : string.Empty,
                    MainImageUrl = wishList.Product != null ? wishList.Product.MainImageUrl : string.Empty,
                    SalePrice = wishList.Product != null ? wishList.Product.SalePrice : 0,
                    ProfitPrice = wishList.Product != null ? wishList.Product.ProfitPrice : 0,
                    InStock = (short)(wishList.Product != null ? wishList.Product.InStock : 0),
                    CreationDate = wishList.CreatedAt
                };

                return new Response<CartDto>
                {
                    Data = new List<CartDto> { cartDto },
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response<CartDto>
                {
                    IsError = true,
                    ErrorMessage = ex.Message
                };
            }
        }

        public Response<CartDto> RemoveFromWishList(RequestParam param, ClaimsPrincipal userClaims)
        {
            try
            {
                if (param.ProductId == null)
                {
                    throw new Exception("Product Id can't be null");
                }

                var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                if (user != null)
                {
                    var wishListItem = _db.WishLists.FirstOrDefault(c => c.AppUserId == user.Id && c.ProductId == param.ProductId);

                    if (wishListItem != null)
                    {
                        _db.WishLists.Remove(wishListItem);
                        _db.SaveChanges();
                    }
                }

                return GetWishList(userClaims);
            }
            catch (Exception ex)
            {
                return new Response<CartDto>()
                {
                    ErrorMessage = ex.Message,
                    StatusCode = 500,
                    IsError = true
                };
            };
        }

    }
}
