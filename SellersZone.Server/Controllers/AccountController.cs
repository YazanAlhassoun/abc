using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;

namespace SellersZone.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _repo;
        public AccountController(IAccountRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("login")]
        public Response<UserDto> Login(LoginDto loginDto)
          => _repo.Login(loginDto);


        [HttpPost("register")]
        public async Task<Response<UserDto>> Register(RegesterDto regesterDto)
          => await _repo.Register(regesterDto);


        [HttpPost("resend-email")]
        public async Task<Response<UserDto>> ReSendEmail(RegesterDto dto)
          => await _repo.ReSendEmail(dto);


        [HttpGet("confirm-email")]
        public async Task<Response<string>> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
          => await _repo.ConfirmEmail(userId, token);

    }
}
