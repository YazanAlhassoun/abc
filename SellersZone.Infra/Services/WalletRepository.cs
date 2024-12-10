using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SellersZone.Core.DTOs;
using SellersZone.Core.Enums;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models;
using SellersZone.Core.Models.Identity;
using SellersZone.Core.Wrapper;
using SellersZone.Infra.Helpers;
using System.Security.Claims;

namespace SellersZone.Infra.Services
{
    public class WalletRepository : IWalletRepository
    {
        private readonly StoreContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public WalletRepository(StoreContext db, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // pass user id to get (User Wallet) for admin
        // pass token to get (User Wallet) for client
        // paging on WithdrawalRequests
        public Response<WalletDto> GetWallet(RequestParam param, ClaimsPrincipal userClaims)
        {
            try
            {
                bool isAdmin = userClaims.IsAdmin();
                IQueryable<Wallet>? query = null;

                if (param?.UserId == null && isAdmin)
                {
                    throw new Exception("User id can't be null");
                }

                string? userId = null;

                // Get user ID for admin or client
                if (isAdmin)
                {
                    userId = param?.UserId;
                }
                else
                {
                    var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                    if (user != null)
                    {
                        userId = user.Id;
                    }
                }

                if (userId != null)
                {
                    query = _db.Wallets.Where(w => w.AppUsers.Any(u => u.Id == userId))
                        .Include(wr => wr.WithdrawalRequests);
                }

                int totalWithdrawalRequestsCount = query!.SelectMany(u => u.WithdrawalRequests).Count();
                var walletDto = query.Select(w => new WalletDto
                {
                    Id = w.Id,
                    Total = w.Total,
                    ExpectedProfit = w.ExpectedProfit,
                    Profit = w.Profit,
                    WithdrawalRequests = w.WithdrawalRequests != null ? w.WithdrawalRequests
                                        .Skip((param.Page - 1) * param.ItemPerPage)
                                        .Take(param.ItemPerPage)
                                        .OrderByDescending(p => p.Id)
                                        .Select(o => new WithdrawalRequestDto()
                                        {
                                            Id = o.Id,
                                            RequestsDate = o.RequestsDate,
                                            Status = o.Status,
                                            WalletType = o.WalletType,
                                            CreationDate = o.CreatedAt
                                        }).ToList() : new List<WithdrawalRequestDto>(),
                }).FirstOrDefault();

                PaginationMetaData paginationMetaData = new PaginationMetaData();
                if (totalWithdrawalRequestsCount > param.ItemPerPage)
                {
                    HttpRequest requestUrl = _httpContextAccessor.HttpContext.Request;
                    paginationMetaData = new PaginationMetaData(param.Page, totalWithdrawalRequestsCount, param.ItemPerPage, requestUrl);
                }

                return new Response<WalletDto>
                {
                    Data = new List<WalletDto> { walletDto ?? new WalletDto() },
                    Pagination = paginationMetaData,
                    IsSuccess = true,
                    StatusCode = 200,
                };
            }
            catch (Exception ex)
            {
                return new Response<WalletDto>
                {
                    IsError = true,
                    StatusCode = 500,
                    ErrorMessage = "An error occurred while processing the request." + "/" + ex.Message
                };
            }
        }


        public void MoveExpectedProfitToProfit(int orderId, int sellerWalletId, string adminId, decimal sellerProfit, decimal adminProfit)
        {
            var order = _db.Orders.FirstOrDefault(x => x.Id == orderId);

            if (order != null && order.Status == (int)OrderStatus.Delivered)
            {
                var sellerWallet = _db.Wallets.FirstOrDefault(w => w.Id == sellerWalletId);
                var adminWallet = _db.Wallets.FirstOrDefault(w => w.AppUsers.Any(u => u.Id == adminId));

                if (sellerWallet != null && adminWallet != null)
                {
                    sellerWallet.ExpectedProfit -= sellerProfit;
                    sellerWallet.Profit += sellerProfit;
                    sellerWallet.Total += sellerProfit;

                    adminWallet.ExpectedProfit -= adminProfit;
                    adminWallet.Profit += adminProfit;
                    adminWallet.Total += adminProfit;

                    // Mark the job as completed
                    order.JobCompleted = true;
                    order.IsOrderLifecycleEnded = true;
                    _db.SaveChanges();
                }
            }
        }

    }
}
