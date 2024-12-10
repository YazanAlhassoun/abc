using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SellersZone.Core.DTOs;
using SellersZone.Core.Enums;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models;
using SellersZone.Core.Models.Identity;
using SellersZone.Core.Wrapper;
using SellersZone.Core.Wrapper.Errors;
using SellersZone.Infra.Helpers;
using System.Security.Claims;

namespace SellersZone.Infra.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly StoreContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(StoreContext db, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public Response<UserDto> GetUsers(RequestParam param, ClaimsPrincipal userClaims)
        {
            try
            {
                if (!userClaims.IsAdmin())
                {
                    throw new UnauthorizedException("You don't have permission for this process");
                }

                IQueryable<AppUser> query = _db.Users.Where(u =>
                    (string.IsNullOrEmpty(param.Search) ||
                    (u.UserName.ToUpper().Contains(param.Search.ToUpper().Trim())) ||
                    (u.FirstName!.ToUpper().Contains(param.Search.ToUpper().Trim())) ||
                    (u.LastName!.ToUpper().Contains(param.Search.ToUpper().Trim())) ||
                    (u.StoreName!.ToUpper().Contains(param.Search.ToUpper().Trim())) ||
                    (u.PhoneNumber.ToUpper().Contains(param.Search.ToUpper().Trim())) ||
                    (u.FromCountry!.ToUpper().Contains(param.Search.ToUpper().Trim())) ||
                    (u.Email.ToUpper().Contains(param.Search.ToUpper().Trim())))
                );


                int userCount = query != null ? query.Count() : 0;

                var userDto = query?.Skip((param.Page - 1) * param.ItemPerPage)
                           .Take(param.ItemPerPage)
                           .OrderByDescending(p => p.Id)
                           .Select(u => new UserDto
                           {
                               UserId = u.Id,
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               UserName = u.UserName,
                               StoreName = u.StoreName,
                               Email = u.Email,
                               FromCountry = u.FromCountry,
                               PhoneNumber = u.PhoneNumber,
                               EmailConfirmed = u.EmailConfirmed
                           }).ToList();

                var paginationMetaData = new PaginationMetaData();
                if (userCount != 0)
                {
                    HttpRequest requestUrl = _httpContextAccessor.HttpContext.Request;
                    paginationMetaData = new PaginationMetaData(param.Page, userCount, param.ItemPerPage, requestUrl);
                }

                return new Response<UserDto>
                {
                    Data = userDto,
                    Pagination = paginationMetaData,
                    IsSuccess = true,
                    StatusCode = 200,
                };

            }
            catch (UnauthorizedException ex)
            {
                var res = new Response<UserDto>
                {
                    IsError = true,
                    ErrorMessage = ex.Message
                };
                return res;
            }

        }

        public Response<UserDto> GetUser(RequestParam param, ClaimsPrincipal userClaims)
        {
            try
            {
                bool isAdmin = userClaims.IsAdmin();
                IQueryable<AppUser>? query = null;

                // check admin if he forget pass UserId
                if (param?.UserId == null && isAdmin)
                {
                    throw new ArgumentNullException(string.Empty, "User id can't be null");
                }

                // get user for admin
                if (isAdmin)
                {
                    query = _db.Users.Where(u => u.Id == param.UserId);
                }

                // Get user for client
                else if (!isAdmin)
                {
                    var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                    if (user != null)
                    {
                        query = _db.Users.Where(u => u.Email == user.Email);
                    }
                    else
                    {
                        // Log claims for debugging purposes
                        var claims = string.Join(", ", userClaims.Claims.Select(c => $"{c.Type}: {c.Value}"));
                        var errorMessage = $"User ID and email claims are missing. Available claims: {claims}";

                        throw new UnauthorizedAccessException(errorMessage);
                    }
                }

                var userDto = query?.Select(u => new UserDto
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName,
                    StoreName = u.StoreName,
                    Email = u.Email,
                    FromCountry = u.FromCountry,
                    PhoneNumber = u.PhoneNumber,
                    EmailConfirmed = u.EmailConfirmed,
                }).FirstOrDefault();

                return new Response<UserDto>
                {
                    Data = new List<UserDto> { userDto ?? new UserDto() },
                    IsSuccess = true,
                    StatusCode = 200,
                };

            }
            catch (Exception ex)
            {
                var res = new Response<UserDto>();
                if (ex is ArgumentNullException argEx)
                {
                    res.ErrorMessage = argEx.Message;
                    res.StatusCode = 400;
                }
                else
                {
                    res.ErrorMessage = ex.Message;
                    res.StatusCode = 500;
                }
                res.IsError = true;
                return res;
            }
        }

        public Response<UserDto> DeleteUser(RequestParam param)
        {
            try
            {
                // check admin if he forget pass UserId
                if (param?.UserId == null)
                {
                    throw new ArgumentNullException(string.Empty, "User id can't be null");
                }

                var user = _db.Users.FirstOrDefault(c => c.Id == param.UserId);
                if (user != null)
                {
                    _db.Users.Remove(user);
                    _db.SaveChanges();
                }

                return new Response<UserDto>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                };

            }
            catch (Exception ex)
            {
                var res = new Response<UserDto>();
                if (ex is ArgumentNullException argEx)
                {
                    res.ErrorMessage = argEx.Message;
                    res.StatusCode = 400;
                }
                else
                {
                    res.ErrorMessage = ex.Message;
                    res.StatusCode = 500;
                }
                res.IsError = true;
                return res;
            }
        }

        public async Task<Response<UserDto>> AddUser(UserDto userDto, ClaimsPrincipal userClaims)
        {
            // this method for create sub admin
            try
            {
                if (!userClaims.IsAdmin()) throw new Exception("Not Authorized");
                if (string.IsNullOrEmpty(userDto.FirstName) || string.IsNullOrEmpty(userDto.LastName) || string.IsNullOrEmpty(userDto.StoreName) || string.IsNullOrEmpty(userDto.FromCountry) || string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.PhoneNumber) || string.IsNullOrEmpty(userDto.Password))
                {
                    throw new Exception("Please fill all required fields.");
                }

                var user = new AppUser
                {
                    FirstName = userDto.FirstName,
                    Email = userDto.Email,
                    UserName = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    EmailConfirmed = true,
                    CountryId = userDto.CountryId
                };

                var newAdmin = await _userManager.CreateAsync(user, userDto.Password);

                if (newAdmin.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "SubAdmin");

                    var wallet = await GetOrCreateWalletByCountryId(userDto.CountryId);
                    user.Wallet = wallet;

                    await _userManager.UpdateAsync(user);
                    _db.SaveChanges();
                }

                return new Response<UserDto> { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new Response<UserDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            }
        }

        public Response<UserDto> UpdateUser(UserDto userDto, ClaimsPrincipal userClaims)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == userDto.UserId);
                if (user == null) throw new Exception("User not found.");

                if (userClaims.IsAdmin() || userClaims.IsSubAdmin())
                {
                    user.StoreName = userDto.StoreName;
                    user.CountryId = userDto.CountryId;
                    user.FirstName = userDto.FirstName;
                    user.LastName = userDto.LastName;
                    user.Email = userDto.Email;
                    user.EmailConfirmed = userDto.EmailConfirmed;
                    user.LockoutEnabled = userDto.LockoutEnabled;

                    if (userDto.SellersProducts?.Any() == true)
                    {
                        var productIds = userDto.SellersProducts.Select(sp => sp.ProductId).ToList();
                        var products = _db.Products.Where(p => productIds.Contains(p.Id)).ToList();

                        foreach (var sellersProduct in userDto.SellersProducts)
                        {
                            var product = products.FirstOrDefault(p => p.Id == sellersProduct.ProductId);
                            if (product != null)
                            {
                                product.AppUserId = userDto.UserId;
                                product.IsForSeller = sellersProduct.IsForSeller;
                                product.ServicePriceForAllSellers = sellersProduct.ServicePriceForAllSellers;
                            }
                        }
                    }

                    var updateResult = _userManager.UpdateAsync(user).GetAwaiter().GetResult();
                    if (!updateResult.Succeeded)
                    {
                        return new Response<UserDto>
                        {
                            IsSuccess = false,
                            ErrorMessage = updateResult.Errors.FirstOrDefault()?.Description
                        };
                    }

                    _db.SaveChanges();

                    return new Response<UserDto>
                    {
                        IsSuccess = true
                    };
                }

                return new Response<UserDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Unauthorized."
                };

            }
            catch (Exception ex)
            {
                return new Response<UserDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            }
        }

        private async Task<Wallet> GetOrCreateWalletByCountryId(int? countryId)
        {
            var wallet = await (from w in _db.Wallets
                                join u in _db.Users on w.Id equals u.WalletId
                                join ur in _db.UserRoles on u.Id equals ur.UserId
                                join r in _db.Roles on ur.RoleId equals r.Id
                                where u.CountryId == countryId && r.Name == "SubAdmin"
                                select w).FirstOrDefaultAsync();

            if (wallet == null)
            {
                wallet = new Wallet
                {
                    Total = 0,
                    Profit = 0,
                    ExpectedProfit = 0,
                    CreatedAt = DateTime.UtcNow,
                    AppUsers = new List<AppUser>()
                };

                _db.Wallets.Add(wallet);
                await _db.SaveChangesAsync();
            }

            return wallet;
        }

  
    }
}
