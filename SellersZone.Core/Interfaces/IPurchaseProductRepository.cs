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
    public interface IPurchaseProductRepository
    {
        Response<PurchaseProductDto> GetPurchaseProducts(RequestParam param, ClaimsPrincipal userClaims);
        Response<PurchaseProductDto> AddPurchaseProducts(PurchaseProductDto dto, ClaimsPrincipal userClaims);
        Response<PurchaseProductDto> UpdatePurchaseProducts(PurchaseProductDto dto, ClaimsPrincipal userClaims);
    }
}
