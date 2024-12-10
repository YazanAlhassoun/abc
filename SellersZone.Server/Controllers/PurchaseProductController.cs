using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;

namespace SellersZone.API.Controllers
{
    [Route("api/purchase")]
    [ApiController]
    public class PurchaseProductController : ControllerBase
    {
        private readonly IPurchaseProductRepository _repo;
        public PurchaseProductController(IPurchaseProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public Response<PurchaseProductDto> GetPurchaseProducts([FromQuery] RequestParam param)
           => _repo.GetPurchaseProducts(param, User);

        [HttpPost]
        public Response<PurchaseProductDto> AddPurchaseProducts(PurchaseProductDto dto)
          => _repo.AddPurchaseProducts(dto, User);

        [HttpPut]
        public Response<PurchaseProductDto> UpdatePurchaseProducts(PurchaseProductDto dto)
          => _repo.UpdatePurchaseProducts(dto, User);
    }
}
