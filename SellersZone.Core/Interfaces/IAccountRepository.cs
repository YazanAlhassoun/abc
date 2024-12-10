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
    public interface IAccountRepository
    {
        Response<UserDto> Login(LoginDto loginDto);
        Task<Response<UserDto>> Register(RegesterDto regesterDto);
        Task<Response<UserDto>> ReSendEmail(RegesterDto regesterDto);
        Task<Response<string>> ConfirmEmail(string userId, string token);
   
    }
}
