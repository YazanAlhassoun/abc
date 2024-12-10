using SellersZone.Core.DTOs;
using SellersZone.Core.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Interfaces
{
    public interface IWishListRepository
    {
        Response<CartDto> GetWishList(ClaimsPrincipal userClaims);
        Response<CartDto> AddToWishList(RequestParam param, ClaimsPrincipal userClaims);
        Response<CartDto> RemoveFromWishList(RequestParam param, ClaimsPrincipal userClaims);
    }
}
