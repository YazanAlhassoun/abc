using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models;
using SellersZone.Core.Models.Identity;
using SellersZone.Core.Wrapper;
using SellersZone.Core.Wrapper.Errors;
using SellersZone.Infra.Helpers;
using System.Security.Claims;
using System.Web;

namespace SellersZone.Infra.Services
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AccountRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IEmailService emailService)
        {
            _userManager = userManager; // check user pass --> is that in data base
            _signInManager = signInManager; // get user from data base
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public Response<UserDto> Login(LoginDto loginDto)
        {
            try
            {
           
                var user = _userManager.FindByEmailAsync(loginDto.Email).GetAwaiter().GetResult();
                if (user == null) throw new UnauthorizedException("Unauthorized");

                var userDto = new UserDto();
                if (!user.EmailConfirmed)
                {
                    userDto = new UserDto
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        StoreName = user.StoreName,
                        EmailConfirmed = user.EmailConfirmed,
                        FromCountry = user.FromCountry,
                        PhoneNumber = user.PhoneNumber,
                    };

                    return new Response<UserDto>
                    {
                        Data = new List<UserDto> { userDto },
                        IsSuccess = true,
                        StatusCode = 200
                    };
                }

                var result = _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false).GetAwaiter().GetResult();
                if (!result.Succeeded) throw new UnauthorizedException("Unauthorized");

                var role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
                var (token, expirationTime) = _tokenService.CreateToken(user, role);

                userDto = new UserDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Token = token,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    StoreName = user.StoreName,
                    FromCountry = user.FromCountry,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    ExpireAt = expirationTime
                };

                return new Response<UserDto>
                {
                    Data = new List<UserDto> { userDto },
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                var res = new Response<UserDto>();
                if (ex is UnauthorizedException || ex.InnerException is UnauthorizedException)
                {
                    res.ErrorMessage = ex.Message;
                    res.StatusCode = 401;
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

        public async Task<Response<UserDto>> Register(RegesterDto regesterDto)
        {
            try
            {
                if (string.IsNullOrEmpty(regesterDto.FirstName) || string.IsNullOrEmpty(regesterDto.LastName) || string.IsNullOrEmpty(regesterDto.StoreName) || string.IsNullOrEmpty(regesterDto.FromCountry) || string.IsNullOrEmpty(regesterDto.Email) || string.IsNullOrEmpty(regesterDto.PhoneNumber) || string.IsNullOrEmpty(regesterDto.Password))
                {
                    throw new ArgumentNullException(string.Empty, "Some fields is required");
                }

                var user = new AppUser
                {
                    FirstName = regesterDto.FirstName,
                    LastName = regesterDto.LastName,
                    StoreName = regesterDto.StoreName,
                    FromCountry = regesterDto.FromCountry,
                    UserName = regesterDto.Email,
                    Email = regesterDto.Email,
                    PhoneNumber = regesterDto.PhoneNumber
                };

                //check email exist
                var checkEmailExist = await _userManager.FindByEmailAsync(regesterDto.Email) != null;
                if (checkEmailExist)
                {
                    return new Response<UserDto>
                    {
                        ErrorMessage = "Email Exist",
                        StatusCode = 200,
                        IsError = true
                    };
                }

                var result = await _userManager.CreateAsync(user, regesterDto.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "CLIENT");
                    var role = await _userManager.GetRolesAsync(user);

                    var token = await GenerateEmailConfirmationToken(user);
                   //var encodedToken = HttpUtility.UrlEncode(token);

                    //send email conformation
                    await SendConfirmationEmail(regesterDto.Email, user.Id, token);

                    var wallet = new Wallet
                    {
                        AppUsers = new List<AppUser> { user },
                        Total = 0,
                        Profit = 0,
                        ExpectedProfit = 0,
                        CreatedAt = DateTime.UtcNow,
                    };

                    user.Wallet = wallet;
                    await _userManager.UpdateAsync(user);

                    var userDto = new UserDto
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        Token = token,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        StoreName = user.StoreName,
                        FromCountry = user.FromCountry,
                        EmailConfirmed = user.EmailConfirmed
                    };

                    return new Response<UserDto>
                    {
                        Data = new List<UserDto> { userDto },
                        IsSuccess = true,
                        StatusCode = 200
                    };

                }
                else
                {
                    var res = new Response<UserDto>
                    {
                        ErrorMessage = "Registration failed/ " + result.Errors.ToString(),
                        IsError = true,
                        StatusCode = 400
                    };
                    return res;
                }
            }
            catch (Exception ex)
            {
                var res = new Response<UserDto>();
                if (ex is ArgumentNullException argEx)
                {
                    res.StatusCode = 400;
                    res.ErrorMessage = ex.InnerException.Message;
                }
                else
                {
                    res.StatusCode = 500;
                    res.ErrorMessage = ex.InnerException.Message;
                };
                res.IsError = true;
                return res;
            }
        }

        public async Task<Response<UserDto>> ReSendEmail(RegesterDto regesterDto)
        {
            try
            {
                if (string.IsNullOrEmpty(regesterDto.Email) || string.IsNullOrEmpty(regesterDto.UserId))
                {
                    throw new ArgumentNullException(string.Empty, "Email or user id can't be null");
                }

                var user = await _userManager.FindByIdAsync(regesterDto.UserId);

                if (user == null)
                {
                    return new Response<UserDto>
                    {
                        ErrorMessage = "User not found",
                        IsError = true,
                        StatusCode = 404
                    };
                }

                if (user.Email != regesterDto.Email)
                {
                    return new Response<UserDto>
                    {
                        ErrorMessage = "Email does not match the user",
                        IsError = true,
                        StatusCode = 400
                    };
                }

                var token = await GenerateEmailConfirmationToken(user);

                // Ensure the token is properly URL encoded
                var encodedToken = HttpUtility.UrlEncode(token);

                await SendConfirmationEmail(regesterDto.Email, user.Id, encodedToken);

                return new Response<UserDto>
                {
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                var res = new Response<UserDto>
                {
                    IsError = true,
                    StatusCode = ex is ArgumentNullException ? 400 : 500,
                    ErrorMessage = ex.Message
                };
                return res;
            }
        }

        public async Task<string> GenerateEmailConfirmationToken(AppUser user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task SendConfirmationEmail(string email, string userId, string token)
        {
            var confirmationLink = $"https://sellers-zone.com/api/account/confirm-email?userId={userId}&token={token}";
           // var confirmationLink = $"https://localhost:44364/api/account/confirm-email?userId={userId}&token={token}";

            var subject = "Confirm Your Email";
            var body = emailBody(confirmationLink);

            await _emailService.SendEmailAsync(email, subject, body, true);
        }

        public async Task<Response<string>> ConfirmEmail(string userId, string token)
        {
            var res = new Response<string>();

            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    res.ErrorMessage = "Invalid userId or token";
                    res.IsError = true;
                    return res;
                }

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    res.ErrorMessage = "User not found";
                    res.IsError = true;
                    return res;
                }

                if (user.EmailConfirmed)
                {
                    res.ErrorMessage = "Email already confirmed";
                    res.IsError = true;
                    return res;
                }

                // Decode the token before confirmation
                var decodedToken = HttpUtility.UrlDecode(token);

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    res.Data = new List<string> { "Thank you for confirming your email" };
                    res.IsSuccess = true;
                }
                else
                {
                    res.ErrorMessage = "Email not confirmed: " + string.Join(", ", result.Errors.Select(e => e.Description));
                    res.IsError = true;
                }
            }
            catch (Exception ex)
            {
                res.ErrorMessage = "An error occurred while confirming your email: " + ex.Message;
                res.IsError = true;
            }

            return res;
        }

        public string emailBody(string confirmationLink)
        {
            var body = $@"
                <h1 style='text-align: center; color: #20d8ed;'><strong>Confirmation Email</strong></h1>
                <p style='text-align: center;'><strong>TripBuilderPro email confirmation<br />Thank you for your interest, we hope you find the program useful</strong></p>
                <p style='text-align: center;'>&nbsp;</p>
                <p><strong><img style='display: block; margin-left: auto; margin-right: auto;' src='https://res.cloudinary.com/belum-punya/image/upload/v1487666277/datadrivensystems/tripbuilder.png' width='543' height='330' /></strong></p>
                <p>&nbsp;</p>
                <p style='text-align: center;'>You are just one step away from your free trial.<br />Please click the link below to verify your email.</p>
                <p style='text-align: center;'>&nbsp;</p>
                <p style='text-align: center;'><a style='background: orange; color: #ffffff; padding: 10px 50px; border-radius: 3px;' href='{confirmationLink}'>confirm</a></p>
                <p style='text-align: center;'>&nbsp;</p>
                <p style='text-align: center; font-size: 10px;'><code>Trip Builder Pro is a registered business name of Trip Builder Pro England Limited.</code><br /><code>Registered in London as a private limited company, Company Number 4777441</code><br /><code>registered Office: Wilton Plaza, Wilton Place, London</code></p>
                <p>&nbsp;</p>";
            return body;
        }


        //////////////////////////////////////////////////////////
        ///

        public async Task<Response<string>> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var res = new Response<string>();

            try
            {
                if (string.IsNullOrEmpty(forgotPasswordDto.Email))
                {
                    throw new ArgumentNullException("Email is required");
                }

                var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
                if (user == null)
                {
                    // Do not reveal that the user does not exist
                    res.ErrorMessage = "If your email address exists in our database, you will receive a password recovery link at your email address in a few minutes.";
                    res.IsSuccess = true;
                    return res;
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token);

                await SendPasswordResetEmail(user.Email, user.Id, encodedToken);

                res.Data = new List<string> { "Password reset link has been sent to your email." };
                res.IsSuccess = true;
                res.StatusCode = 200;
            }
            catch (Exception ex)
            {
                res.ErrorMessage = ex.Message;
                res.IsError = true;
                res.StatusCode = 500;
            }

            return res;
        }

        private async Task SendPasswordResetEmail(string email, string userId, string token)
        {
            var resetLink = $"https://yourapp.com/reset-password?userId={userId}&token={token}";

            // Your email sending logic here
            // await _emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password by clicking <a href='{resetLink}'>here</a>.");

            await _emailService.SendEmailAsync(email, "Reset your password", $"Please reset your password by clicking <a href='{resetLink}'>here</a>.", true);
        }

        public async Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var res = new Response<string>();

            try
            {
                if (string.IsNullOrEmpty(resetPasswordDto.UserId) || string.IsNullOrEmpty(resetPasswordDto.Token) || string.IsNullOrEmpty(resetPasswordDto.NewPassword))
                {
                    throw new ArgumentNullException("UserId, token and new password are required");
                }

                var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId);
                if (user == null)
                {
                    res.ErrorMessage = "Invalid user.";
                    res.IsError = true;
                    res.StatusCode = 400;
                    return res;
                }

                var decodedToken = HttpUtility.UrlDecode(resetPasswordDto.Token);

                var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
                if (result.Succeeded)
                {
                    res.Data = new List<string> { "Password has been reset successfully." };
                    res.IsSuccess = true;
                    res.StatusCode = 200;
                }
                else
                {
                    res.ErrorMessage = "Password reset failed: " + string.Join(", ", result.Errors.Select(e => e.Description));
                    res.IsError = true;
                    res.StatusCode = 400;
                }
            }
            catch (Exception ex)
            {
                res.ErrorMessage = "An error occurred while resetting your password: " + ex.Message;
                res.IsError = true;
                res.StatusCode = 500;
            }

            return res;
        }

    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; }
    }

    public class ResetPasswordDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
