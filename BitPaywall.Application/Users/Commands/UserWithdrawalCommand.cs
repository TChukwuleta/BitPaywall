using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Users.Commands
{
    public class UserWithdrawalCommand : IRequest<Result>
    {
        public string PaymentRequest { get; set; }
        public string UserId { get; set; }
    }

    public class UserWithdrawalCommandHandler : IRequestHandler<UserWithdrawalCommand, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        private readonly ILightningService _lightningService;
        public UserWithdrawalCommandHandler(IAppDbContext context, IAuthService authService, ILightningService lightningService)
        {
            _lightningService = lightningService;
            _context = context;
            _authService = authService;
        }
        public async Task<Result> Handle(UserWithdrawalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to withdraw funds. Invalid user details specified.");
                }
                var validateInvoice = await _lightningService.ValidateLightningAddress(request.PaymentRequest);
                if (!validateInvoice.success)
                {
                    return Result.Failure("Unable to withdraw funds. Invalid invoice");
                }
                var account = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                if (account == null)
                {
                    return Result.Failure("Invalid account details");
                }
                var nodeBalance = await _lightningService.GetWalletBalance();
                if (validateInvoice.amount > nodeBalance)
                {
                    return Result.Failure("Cannot process transaction. Please try again later");
                }
                if (validateInvoice.amount > account.Balance)
                {
                    return Result.Failure("Payment request amount is greater than current user account balance");
                }
                var sendFunds = await _lightningService.SendLightning(request.PaymentRequest);
                if (string.IsNullOrEmpty(sendFunds))
                {
                    return Result.Failure($"An error occured while processing payment. {sendFunds}");
                }
                account.Balance -= validateInvoice.amount;
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Transaction completed successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Withdrawal was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
