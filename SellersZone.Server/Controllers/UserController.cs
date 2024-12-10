using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;

namespace SellersZone.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;
        public UserController(IUserRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("usersInfo")]
        public Response<UserDto> GetUsers([FromQuery] RequestParam param)
          => _repo.GetUsers(param, User);

        [HttpGet("userInfo")]
        public Response<UserDto> GetUser([FromQuery] RequestParam param)
          => _repo.GetUser(param, User);

        [HttpGet("addUser")]
        public async Task<Response<UserDto>> AddUser(UserDto userDto)
          => await _repo.AddUser(userDto, User);

        [HttpDelete("delete")]
        public Response<UserDto> DeleteUser([FromQuery] RequestParam param)
        => _repo.DeleteUser(param);
    }
}
