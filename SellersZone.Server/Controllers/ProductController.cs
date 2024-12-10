using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;

namespace SellersZone.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IProductRepository _repo;
        public ProductController(IProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public Response<ProductDto> GetProduct([FromQuery] RequestParam param)
            => _repo.GetProduct(param, User);


        [HttpPost]
        public Response<ProductDto> AddProduct(ProductDto productDto)
           =>  _repo.AddProduct(productDto);


        [HttpPut]
        public Response<ProductDto> UpdateProduct(ProductDto productDto)
           =>  _repo.UpdateProduct(productDto);

    }
}
