using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;

namespace SellersZone.API.Controllers
{
    [Route("api/country")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _repo;
        public CountryController(ICountryRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public Response<CountryDto> GetCountries([FromQuery] RequestParam param)
            => _repo.GetCountry(param);

        [HttpPost]
        public Response<CountryDto> AddCountry(CountryDto countryDto)
            => _repo.AddCountry(countryDto);

        [HttpPut]
        public Response<CountryDto> UpdateCountry(CountryDto countryDto)
            => _repo.UpdateCountry(countryDto);

    }
}
