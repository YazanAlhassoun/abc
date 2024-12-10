using SellersZone.Core.DTOs;
using SellersZone.Core.Wrapper;
using System.Security.Claims;

namespace SellersZone.Core.Interfaces
{
    public interface IWalletRepository
    {
        Response<WalletDto> GetWallet(RequestParam param, ClaimsPrincipal userClaims);

        void MoveExpectedProfitToProfit(int orderId, int sellerWalletId, string adminId, decimal sellerProfit, decimal adminProfit);
    }
}
