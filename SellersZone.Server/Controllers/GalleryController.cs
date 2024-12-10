using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;

namespace SellersZone.API.Controllers
{
    [Route("api/gallery")]
    [ApiController]
    public class GalleryController : ControllerBase
    {
        private readonly IGalleryRepository _repo;
        public GalleryController(IGalleryRepository repo)
        {
            _repo = repo;
        }


        [HttpGet]
        public Response<GalleryDto> GetGalleries([FromQuery] RequestParam param)
          => _repo.GetGalleries(param);

        [HttpPost]
        public Response<GalleryDto> AddGallery(GalleryDto gallerydto)
           => _repo.AddGallery(gallerydto);


        [HttpDelete]
        public bool DeleteGallery([FromBody] RequestParam param)
          => _repo.DeleteGallery(param);
        
    }
}
