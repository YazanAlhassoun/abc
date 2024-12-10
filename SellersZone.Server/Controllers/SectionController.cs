using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;

namespace SellersZone.API.Controllers
{
    [Route("api/section")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly ISectionRepository _repo;
        public SectionController(ISectionRepository repo)
        {
            _repo = repo;
        }


        [HttpGet]
        public async Task<Response<SectionDto>> GetSections([FromQuery] RequestParam param)
            => await _repo.GetSection(param, User);


        [HttpPost]
        public async Task<Response<SectionDto>> AddSection(SectionDto sectionDto)
            => await _repo.AddSection(sectionDto);


        [HttpPut]
        public async Task<Response<SectionDto>> UpdateSection(SectionDto sectionDto)
            => await _repo.UpdateSection(sectionDto);

     
    }
}
