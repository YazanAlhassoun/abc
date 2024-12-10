using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;

namespace SellersZone.API.Controllers
{
    [Route("api/carousel")]
    [ApiController]
    public class CarouselController : ControllerBase
    {
        private readonly ICarouselsRepository _repo;
        public CarouselController(ICarouselsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public Response<CarouselDto> GetCarousel([FromQuery] RequestParam param)
          => _repo.GetCarousel(param);

        [HttpPost]
        public Response<CarouselDto> AddCarousel(CarouselDto dto)
          => _repo.AddCarousel(dto);

        [HttpPut]
        public Response<CarouselDto> EditCarousel(CarouselDto dto)
          => _repo.EditCarousel(dto);

        [HttpDelete]
        public bool DeleteCarousel([FromBody] RequestParam param)
          => _repo.DeleteCarousel(param);
    }
}
