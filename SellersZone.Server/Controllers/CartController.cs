using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace SellersZone.API.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _repo;

        public CartController(ICartRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public Response<CartDto> GetCart()
          => _repo.GetCart(User);

        [HttpPost]
        public Response<CartDto> AddToCart([FromQuery] RequestParam param)
          => _repo.AddToCart(param, User);

        [HttpDelete]
        public Response<CartDto> RemoveFromCart([FromQuery] RequestParam param)
          => _repo.RemoveFromCart(param, User);
    }
}
