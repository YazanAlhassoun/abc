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
    public class CartRepository : ICartRepository
    {
        private readonly StoreContext _db;
        private readonly UserManager<AppUser> _userManager;

        public CartRepository(StoreContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public Response<CartDto> GetCart(ClaimsPrincipal userClaims)
        {
            try
            {
                var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                if (user == null) throw new Exception("There is no user !");

                IQueryable<Cart> query = _db.Carts;
                var cartDto = query.Where(c => c.AppUserId == user.Id).Select(cart => new CartDto
                {
                    Id = cart.Id,
                    ProductId = cart.Product != null ? cart.Product.Id : 0,
                    Name = cart.Product != null ? cart.Product.Name : string.Empty,
                    NameAr = cart.Product != null ? cart.Product.NameAr : string.Empty,
                    MainImageUrl = cart.Product != null ? cart.Product.MainImageUrl : string.Empty,
                    SalePrice = cart.Product != null ? cart.Product.SalePrice : 0,
                    ProfitPrice = cart.Product != null ? cart.Product.ProfitPrice : 0,
                    InStock = (short)(cart.Product != null ? cart.Product.InStock : 0),
                    ItemQuantity = cart.Quantity, // for one item 
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

        public Response<CartDto> AddToCart(RequestParam param, ClaimsPrincipal userClaims)
        {
            try
            {
                if (param.ProductId == null) throw new Exception("Product Id can't be null");

                var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                if (user == null) throw new Exception("There is no user!");

                var product = _db.Products.FirstOrDefault(p => p.Id == param.ProductId);
                if (product == null) throw new Exception("Product not found!");

                // Check if the product already exists in the user's cart
                var existingCartItem = _db.Carts.FirstOrDefault(c => c.AppUserId == user.Id && c.ProductId == param.ProductId);

                if (existingCartItem != null)
                {
                    // Update the quantity
                    existingCartItem.Quantity = param.Quantity;
                    _db.Carts.Update(existingCartItem);
                }
                else
                {
                    // Add a new item to the cart
                    var cart = new Cart
                    {
                        AppUserId = user.Id,
                        ProductId = param.ProductId,
                        Quantity = param.Quantity,
                        CreatedAt = DateTime.Now // Assuming you have a CreatedAt property
                    };

                    _db.Carts.Add(cart);
                }

                _db.SaveChanges();

                // Fetch the cart item to return the updated information
                var cartDto = new CartDto
                {
                    Name = product.Name,
                    NameAr = product.NameAr,
                    MainImageUrl = product.MainImageUrl,
                    SalePrice = product.SalePrice,
                    ProfitPrice = product.ProfitPrice,
                    InStock = (short)product.InStock,
                    ItemQuantity = existingCartItem != null ? existingCartItem.Quantity : param.Quantity, // Return the updated or new quantity
                    CreationDate = DateTime.Now // Adjust according to your logic
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

        public Response<CartDto> RemoveFromCart(RequestParam param, ClaimsPrincipal userClaims)
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
                    var cartItem = _db.Carts.FirstOrDefault(c => c.AppUserId == user.Id && c.ProductId == param.ProductId);

                    if (cartItem != null)
                    {
                        _db.Carts.Remove(cartItem);
                        _db.SaveChanges();
                    }
                }

                return GetCart(userClaims);
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
