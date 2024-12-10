using SellersZone.Core.DTOs;
using SellersZone.Core.Wrapper;
using System.Security.Claims;

namespace SellersZone.Core.Interfaces
{
    public interface ISectionRepository
    {
        Task<Response<SectionDto>> GetSection(RequestParam param, ClaimsPrincipal userClaims);
        Task<Response<SectionDto>> AddSection(SectionDto section);
        Task<Response<SectionDto>> UpdateSection(SectionDto section);    
    }
}
