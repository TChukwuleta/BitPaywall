using BitPaywall.Core.Entities;
using BitPaywall.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<Result> CreateUserAsync(User user);
        Task<Result> UpdateUserAsync(User user);
        Task<Result> ChangePasswordAsync(string email, string oldPassword, string newPassword);
        Task<Result> ResetPassword(string email, string password);
        Task<Result> EmailVerification(string email, string otp);
        Task<Result> Login(string email, string password);
        Task<Result> GenerateOTP(string email); 
        Task<Result> ValidationOTP(string email, string otp);
        Task<(Result result, User user)> GetUserByEmail(string email);
        Task<(Result result, User user)> GetUserById(string userid);
        Task<(Result result, List<User> users)> GetAllUsers(int skip, int take);
    }
}
