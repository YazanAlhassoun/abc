using SellersZone.Core.DTOs;
using SellersZone.Core.Wrapper;
using System.Security.Claims;

namespace SellersZone.Core.Interfaces
{
    public interface ISideEarningRepository
    {
        Response<SideEarningDto> GetSideEarning(RequestParam param);
        Response<SideEarningDto> AddSideEarning(SideEarningDto dto);
        Response<SideEarningDto> UpdateSideEarning(SideEarningDto dto, ClaimsPrincipal userClaims);
    }
}
