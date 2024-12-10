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
    public interface IUserRepository
    {
        Response<UserDto> GetUsers(RequestParam param, ClaimsPrincipal userClaims);
        Response<UserDto> GetUser(RequestParam param, ClaimsPrincipal userClaims);
        Task<Response<UserDto>> AddUser(UserDto userDto, ClaimsPrincipal userClaims);
        Response<UserDto> UpdateUser(UserDto userDto, ClaimsPrincipal userClaims);
        Response<UserDto> DeleteUser(RequestParam param);
    }
}
