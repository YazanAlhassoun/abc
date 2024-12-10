using SellersZone.Core.DTOs;
using SellersZone.Core.Wrapper;
using System.Security.Claims;

namespace SellersZone.Core.Interfaces
{
    public interface IProductRepository
    {
        Response<ProductDto> GetProduct(RequestParam param, ClaimsPrincipal userClaims);
        Response<ProductDto> AddProduct(ProductDto product);
        Response<ProductDto> UpdateProduct(ProductDto product);

    }
}
