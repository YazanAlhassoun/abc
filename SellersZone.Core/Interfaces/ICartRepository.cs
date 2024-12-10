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
    public interface ICartRepository
    {
        Response<CartDto> GetCart(ClaimsPrincipal userClaims);
        Response<CartDto> AddToCart(RequestParam param, ClaimsPrincipal userClaims);
        Response<CartDto> RemoveFromCart(RequestParam param, ClaimsPrincipal userClaims);
    }
}