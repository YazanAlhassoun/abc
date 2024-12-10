using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SellersZone.Core.DTOs;
using SellersZone.Core.Enums;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models;
using SellersZone.Core.Models.Identity;
using SellersZone.Core.Wrapper;
using SellersZone.Infra.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Infra.Services
{
    public class SideEarningRepository : ISideEarningRepository
    {
        private readonly StoreContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SideEarningRepository(StoreContext db, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public Response<SideEarningDto> GetSideEarning(RequestParam param)
        {
            try
            {
                IQueryable<SideEarning> query = _db.SideEarnings.Where(se =>
                                                               (!param.Id.HasValue || se.Id == param.Id) &&
                                                               (string.IsNullOrWhiteSpace(param.Search) || se.Note.Contains(param.Search)));

                int totalSideEarning = query.Count();
                var sideEarningDto = query.Skip((param.Page - 1) * param.ItemPerPage)
                                       .Take(param.ItemPerPage)
                                       .OrderBy(o => o.Id)
                                       .Select(p => new SideEarningDto
                                       {
                                           Amount = p.Amount,
                                           AppUserId = p.AppUserId,
                                           SideEarningReason = p.SideEarningReason,
                                           Note = p.Note,
                                           CreationDate = p.CreatedAt,
                                           ModificationDate = p.UpdatedAt
                                       }).ToList();

                var paginationMetaData = new PaginationMetaData();
                if (totalSideEarning > param.ItemPerPage)
                {
                    var requestUrl = _httpContextAccessor?.HttpContext?.Request;
                    paginationMetaData = new PaginationMetaData(param.Page, totalSideEarning, param.ItemPerPage, requestUrl);
                }

                return new Response<SideEarningDto>
                {
                    Data = sideEarningDto,
                    Pagination = paginationMetaData,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response<SideEarningDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            }
        }

        public Response<SideEarningDto> AddSideEarning(SideEarningDto dto)
        {
            try
            {
                if (DtoValidator.AreAnyNullOrEmpty(dto.Amount, dto.Note, dto.SideEarningReason))
                {
                    throw new Exception("Please fill all required fields");
                }

                var sideEarning = new SideEarning()
                {
                    Amount = dto.Amount,
                    AppUserId = dto.AppUserId,
                    SideEarningReason = dto.SideEarningReason,
                    Note = dto.Note,
                    Status = (int)SideEarningStatus.OnPending,
                    CreatedAt = DateTime.UtcNow,
                };

                _db.SideEarnings.Add(sideEarning);
                _db.SaveChanges();

                return new Response<SideEarningDto>
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response<SideEarningDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            }
        }

        public Response<SideEarningDto> UpdateSideEarning(SideEarningDto dto, ClaimsPrincipal userClaims)
        {
            try
            {
                // Validate input and authorization
                if (DtoValidator.AreAnyNullOrEmpty(dto.Amount, dto.Note, dto.SideEarningReason, dto.Id))
                {
                    throw new Exception("Please fill all required fields");
                }
                if (!userClaims.IsAdmin())
                {
                    throw new Exception("Unauthorized");
                }

                var currentSideEarning = _db.SideEarnings.FirstOrDefault(se => se.Id == dto.Id);
                if (currentSideEarning == null)
                {
                    throw new Exception("Side earning not found");
                }

                var currentUser = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                if (currentUser == null)
                {
                    throw new Exception("User not found");
                }

                // Handle transfer status
                if (dto.Status == SideEarningStatus.Transfer)
                {
                    if (currentUser.Wallet != null)
                    {
                        currentUser.Wallet.Total += dto.Amount;
                        _db.SaveChanges();
                    }
                }

                // Handle rollback status
                if (dto.Status == SideEarningStatus.Rollack)
                {
                    if (currentUser.Wallet != null)
                    {
                        currentUser.Wallet.Total -= currentSideEarning.Amount;
                        _db.SaveChanges();
                    }
                }

                return new Response<SideEarningDto>
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response<SideEarningDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            }
        }

    }
}
