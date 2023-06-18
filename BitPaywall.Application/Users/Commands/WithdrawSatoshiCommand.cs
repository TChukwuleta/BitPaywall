using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Transactions.Commands;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BitPaywall.Application.Users.Commands
{
    public class WithdrawSatoshiCommand : IRequest<Result>
    {
        public string PaymentRequest { get; set; }
        public string UserId { get; set; }
    }

    public class WithdrawSatoshiCommandHandler : IRequestHandler<WithdrawSatoshiCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly ILightningService _lightningService;
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;
        public WithdrawSatoshiCommandHandler(IAuthService authService, ILightningService lightningService, IAppDbContext context, IConfiguration config)
        {
            _authService = authService;
            _config = config;
            _lightningService = lightningService;
            _context = context;
        }

        public async Task<Result> Handle(WithdrawSatoshiCommand request, CancellationToken cancellationToken)
        {
            var reference = $"BitPaywall_{DateTime.Now.Ticks}";
            var minimumWithdrawalAmount = _config["MinimumWIthdrawal"];
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to withdraw satoshis. Invalid user details");
                }

                var account = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                if (account == null)
                {
                    return Result.Failure("Unable to withdraw satoshis. Invalid user account details");
                }
                var decodedInvoiceAmount = await _lightningService.DecodePaymentRequest(request.PaymentRequest);
                if (decodedInvoiceAmount <= 0)
                {
                    return Result.Failure("An error occured while trying to decode payment request. Please try again later");
                }
                if (decodedInvoiceAmount <= int.Parse(minimumWithdrawalAmount))
                {
                    return Result.Failure($"Cannot withdraw less than the minimum withdrawal amount: {minimumWithdrawalAmount} sats");
                }
                if (account.Balance < decodedInvoiceAmount)
                {
                    return Result.Failure("Unable to withdraw satoshis. Insufficient funds in the account");
                }
                var payInvoiceResponse = await _lightningService.SendLightning(request.PaymentRequest);
                if (!string.IsNullOrEmpty(payInvoiceResponse))
                {
                    return Result.Failure($"An error occured while trying to pay invoice. {payInvoiceResponse}");
                }
                var transactionRequest = new CreateTransactionCommand
                {
                    Description = "Money withdrawal successfully",
                    DebitAccount = account.AccountNumber,
                    CreditAccount = "",
                    Amount = decodedInvoiceAmount,
                    TransactionType = TransactionType.Debit,
                    UserId = account.UserId
                };

                var fundingTransaction = await new CreateTransactionCommandHandler(_authService, _context).Handle(transactionRequest, cancellationToken);
                if (!fundingTransaction.Succeeded)
                {
                    return Result.Failure("An error while creating a transaction. Kindly contact support");
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Invoice has been paid successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Invoice payment was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
