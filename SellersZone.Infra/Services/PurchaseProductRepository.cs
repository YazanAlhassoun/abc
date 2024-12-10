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
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace SellersZone.Infra.Services
{
    public class PurchaseProductRepository : IPurchaseProductRepository
    {
        private readonly StoreContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PurchaseProductRepository(StoreContext db, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public Response<PurchaseProductDto> GetPurchaseProducts(RequestParam param, ClaimsPrincipal userClaims)
        {
            try
            {
                var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                if (user == null) throw new Exception("There is no user !");

                IQueryable<PurchaseProduct> query = _db.PurchaseProducts.Where(p => !userClaims.IsAdmin() ?
                                                        (p.AppUserId == user.Id && param.Id != null ? p.Id == param.Id : false)
                                                        : userClaims.IsAdmin() ? param.Id != null ? p.Id == param.Id : false : false);

                int totalPurchase = query.Count();
                var purchasesDto = query.Skip((param.Page - 1) * param.ItemPerPage)
                                        .Take(param.ItemPerPage)
                                        .OrderBy(o => o.Id)
                                        .Select(p => new PurchaseProductDto
                                        {
                                            Name = p.Name,
                                            NameAr = p.NameAr,
                                            Image = p.Image,
                                            MadeIn = p.MadeIn,
                                            PricePerItem = p.PricePerItem,
                                            Quantity = p.Quantity,
                                            SourceUrl = p.SourceUrl,
                                            TargetCountry = p.TargetCountry,
                                            Status = p.Status,
                                            Note = p.Note
                                        }).ToList();


                var paginationMetaData = new PaginationMetaData();
                if (totalPurchase > param.ItemPerPage)
                {
                    var requestUrl = _httpContextAccessor?.HttpContext?.Request;
                    paginationMetaData = new PaginationMetaData(param.Page, totalPurchase, param.ItemPerPage, requestUrl);
                }

                return new Response<PurchaseProductDto>
                {
                    Data = purchasesDto,
                    Pagination = paginationMetaData,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response<PurchaseProductDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            }
        }

        public Response<PurchaseProductDto> AddPurchaseProducts(PurchaseProductDto dto, ClaimsPrincipal userClaims)
        {
            try
            {
                if (DtoValidator.AreAnyNullOrEmpty(dto.Name, dto.NameAr, dto.Image, dto.SourceUrl, dto.MadeIn, dto.Quantity, dto.PricePerItem, dto.TargetCountry))
                {
                    throw new Exception("Please fill all required fields");
                }

                var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                var purchase = new PurchaseProduct()
                {
                    Name = dto.Name,
                    NameAr = dto.NameAr,
                    Status = (int)PurchaseProductStatus.OnRequest,
                    AppUser = user,
                    Image = dto.Image,
                    MadeIn = dto.MadeIn,
                    PricePerItem = dto.PricePerItem,
                    Quantity = dto.Quantity,
                    SourceUrl = dto.SourceUrl,
                    TargetCountry = dto.TargetCountry,
                    Note = dto.Note,
                    CreatedAt = DateTime.Now
                };

                _db.PurchaseProducts.Add(purchase);
                _db.SaveChanges();

                return new Response<PurchaseProductDto>
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response<PurchaseProductDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            }
        }

        public Response<PurchaseProductDto> UpdatePurchaseProducts(PurchaseProductDto dto, ClaimsPrincipal userClaims)
        {
            try
            {
                if (DtoValidator.AreAnyNullOrEmpty(dto.Name, dto.NameAr, dto.Image, dto.SourceUrl, dto.MadeIn, dto.Quantity, dto.PricePerItem, dto.TargetCountry))
                {
                    throw new Exception("Please fill all required fields");
                }

                var existPurchase = _db.PurchaseProducts.FirstOrDefault(p => p.Id == dto.Id);
            
                if ( (existPurchase?.Status == (int)PurchaseProductStatus.Approved || existPurchase?.Status == (int)PurchaseProductStatus.Reject) && !userClaims.IsAdmin())
                {
                    throw new Exception("Can't change a data that approved or reject from admin");
                }

                if (userClaims.IsAdmin())
                {
                    existPurchase.Status = dto.Status;
                }

                existPurchase.Name = dto.Name;
                existPurchase.NameAr = dto.NameAr;
                existPurchase.Image = dto.Image;
                existPurchase.MadeIn = dto.MadeIn;
                existPurchase.PricePerItem = dto.PricePerItem;
                existPurchase.Quantity = dto.Quantity;
                existPurchase.SourceUrl = dto.SourceUrl;
                existPurchase.TargetCountry = dto.TargetCountry;
                existPurchase.Note = dto.Note;
                existPurchase.UpdatedAt = DateTime.Now;

                _db.SaveChanges();

                return new Response<PurchaseProductDto>
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response<PurchaseProductDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            }
        }
    }
}
