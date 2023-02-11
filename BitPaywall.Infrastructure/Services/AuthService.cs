using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<SystemUser> _userManager;

        public AuthService(IAppDbContext context, IConfiguration config, UserManager<SystemUser> userManager)
        {
            _config = config;
            _context = context;
            _userManager = userManager;
        }
        public async Task<Result> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Result.Failure("User not found");
                }
                var confirmOldPassword = await _userManager.CheckPasswordAsync(user, oldPassword);
                if (!confirmOldPassword)
                {
                    return Result.Failure("Please kindly input your correct password");
                }
                var confirmNewPassword = await _userManager.CheckPasswordAsync(user, newPassword);
                if (!confirmNewPassword)
                {
                    return Result.Failure("Please enter a different password");
                }
                var changePassword = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (!changePassword.Succeeded)
                {
                    var errors = changePassword.Errors.Select(c => c.Description);
                    return Result.Failure(errors);
                }
                return Result.Success("Password was updated successfully");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(Result result, User user)> CreateUserAsync(User user)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    return (Result.Failure("User with this details already exist"), null);
                }
                var newUser = new SystemUser
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Status = Status.Deactivated,
                    UserName = user.Email,
                    NormalizedEmail = user.Email,
                    Verified = false
                };
                var result = await _userManager.CreateAsync(newUser, user.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(c => c.Description);
                    return (Result.Failure(errors), null);
                }
                newUser.UserId = newUser.Id;
                newUser.Email = user.Email;
                await _userManager.UpdateAsync(newUser);
                await _context.SaveChangesAsync(new CancellationToken());
                var userResponse = new User
                {
                    UserId = newUser.Id,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    Email = newUser.Email,
                    Status = newUser.Status
                };
                return (Result.Success("User creation was successful"), userResponse);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Result> EmailVerification(string email, string otp)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    return Result.Failure("Invalid user details specified");
                }
                var otpValidation = await ValidationOTP(email, otp);
                if (!otpValidation.Succeeded)
                {
                    return Result.Failure(otpValidation.Message);
                }
                user.EmailConfirmed = true;
                user.Email = email;
                user.Status = Status.Active;
                var updatedUser = await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync(new CancellationToken());
                if (!updatedUser.Succeeded)
                {
                    return Result.Failure("An error occured while verifying email. Please contact support");
                }
                return Result.Success("User verification was successful");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Result> GenerateOTP(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Result.Failure("Invalid user specified");
                }
                var otp = await _userManager.GenerateTwoFactorTokenAsync(user, email);
                if(otp == null)
                {
                    return Result.Failure("Unable to generate OTP. Kindly contact support");
                }
                return Result.Success("OTP generation was successful", otp);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<(Result result, List<User> users)> GetAllUsers(int skip, int take)
        {
            var users = new List<User>();
            try
            {
                var applicationsUsers = await _userManager.Users.ToListAsync();
                if (skip != 0 && take != 0)
                {
                    applicationsUsers = applicationsUsers.Skip(skip).Take(take).ToList();
                }
                foreach (var appUser in applicationsUsers)
                {
                    users.Add(new User
                    {
                        FirstName = appUser.FirstName,
                        LastName = appUser.LastName,
                        Email = appUser.Email,
                        UserId = appUser.UserId
                    });
                }
                return (Result.Success("Users retrieval was successful"), users);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<(Result result, User user)> GetUserByEmail(string email)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser == null)
                {
                    return (Result.Failure("Invalid user details specified"), null);
                }
                var user = new User
                {
                    FirstName = existingUser.FirstName,
                    LastName = existingUser.LastName,
                    Email = existingUser.Email,
                    UserId = existingUser.UserId,
                };
                return (Result.Success("User details retrieval was successful"), user);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<(Result result, User user)> GetUserById(string userid)
        {
            try
            {
                var existingUser = await _userManager.FindByIdAsync(userid);
                if (existingUser == null)
                {
                    return (Result.Failure("Invalid user details specified"), null);
                }
                var user = new User
                {
                    FirstName = existingUser.FirstName,
                    LastName = existingUser.LastName,
                    Email = existingUser.Email,
                    UserId = existingUser.UserId,
                };
                return (Result.Success("User details retrieval was successful"), user);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Result> Login(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Result.Failure("Invalid email or password specified");
                }
                var checkPassword = await _userManager.CheckPasswordAsync(user, password);
                if (!checkPassword)
                {
                    return Result.Failure("Invalid email or password specified");
                }
                var jwtToken = GenerateJwtToken(user.UserId, user.UserName);
                return Result.Success(jwtToken);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Result> ResetPassword(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    return Result.Failure("Invalid user details specified");
                }
                var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (resetPasswordToken == null)
                {
                    return Result.Failure("An error occured while trying to reset password. Please contact support");
                }
                var resetPassword = await _userManager.ResetPasswordAsync(user, resetPasswordToken, password);
                if (!resetPassword.Succeeded)
                {
                    return Result.Failure("Password reset was not successful. Please contact support");
                }
                return Result.Success("Password reset was successful");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Result> UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    return Result.Failure("Invalid user details specified");
                }
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Verified = true;
                existingUser.Status = user.Status;
                var update = await _userManager.UpdateAsync(existingUser);
                if (!update.Succeeded)
                {
                    return Result.Failure("An error occured while updating user details. Please contact support");
                }
                return Result.Success("User details updated successfully");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Result> ValidationOTP(string email, string otp)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Result.Failure("Invalid user details specified");
                }
                var validate = await _userManager.VerifyTwoFactorTokenAsync(user, email, otp);
                if (!validate)
                {
                    return Result.Failure("Error validating OTP");
                }
                return Result.Success("OTP validated successfully");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string GenerateJwtToken(string UserId, string email)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["TokenConstants:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim("userId", UserId),
                    new Claim(JwtRegisteredClaimNames.Sub, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
}
