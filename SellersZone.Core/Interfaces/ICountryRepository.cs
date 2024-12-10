using SellersZone.Core.DTOs;
using SellersZone.Core.Wrapper;

namespace SellersZone.Core.Interfaces
{
    public interface ICountryRepository
    {
        Response<CountryDto> GetCountry(RequestParam param);
        Response<CountryDto> AddCountry(CountryDto section);
        Response<CountryDto> UpdateCountry(CountryDto section);
    }
}
