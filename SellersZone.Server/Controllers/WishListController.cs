using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;

namespace SellersZone.API.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishListRepository _repo;

        public WishListController(IWishListRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public Response<CartDto> GetCart()
        => _repo.GetWishList(User);

        [HttpPost]
        public Response<CartDto> AddToCart([FromQuery] RequestParam param)
          => _repo.AddToWishList(param, User);

        [HttpDelete]
        public Response<CartDto> RemoveFromCart([FromQuery] RequestParam param)
          => _repo.RemoveFromWishList(param, User);
    }
}
