using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;

namespace SellersZone.API.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _repo;

        public OrderController(IOrderRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public Response<OrderDto> GetUserOrders([FromQuery] RequestParam param)
           => _repo.GetOrder(param, User);

        [HttpPost]
        public Response<OrderDto> AddOreder(OrderDto orederDto)
           => _repo.AddOreder(orederDto, User);
        

    }
}
