using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;

namespace SellersZone.API.Controllers
{
    [Route("api/earning")]
    [ApiController]
    public class SideEarningController : ControllerBase
    {
        private readonly ISideEarningRepository _repo;
        public SideEarningController(ISideEarningRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public Response<SideEarningDto> GetSideEarning([FromQuery] RequestParam param)
           => _repo.GetSideEarning(param);

        [HttpPost]
        public Response<SideEarningDto> AddSideEarning(SideEarningDto dto)
          => _repo.AddSideEarning(dto);

        [HttpPut]
        public Response<SideEarningDto> UpdateSideEarning(SideEarningDto dto)
          => _repo.UpdateSideEarning(dto, User);
    }
}
