using Microsoft.AspNetCore.Mvc;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Wrapper;

namespace SellersZone.API.Controllers
{
    [Route("api/wallet")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletRepository _repo;
        public WalletController(IWalletRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("walletInfo")]
        public Response<WalletDto> GetWallet([FromQuery] RequestParam param)
            => _repo.GetWallet(param, User);
    }
}
