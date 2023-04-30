using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Users.Commands
{
    public class TopUpCommand : IRequest<Result>
    {
        public decimal Amount { get; set; }
        public string UserId { get; set; }
    }

    public class TopUpCommandHandler : IRequestHandler<TopUpCommand, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        private readonly ILightningService _lightningService;
        public TopUpCommandHandler(IAppDbContext context, IAuthService authService, ILightningService lightningService)
        {
            _context = context;
            _authService = authService;
            _lightningService = lightningService;
        }

        public async Task<Result> Handle(TopUpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to bid for art. Invalid user details specified.");
                }
                var userAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                if (userAccount == null)
                {
                    return Result.Failure("User account isn't available");
                }
                var invoice = await _lightningService.CreateInvoice((long)request.Amount, $"{(int)PaymentType.Funding}|{userAccount.Id}/{request.UserId}");
                if (string.IsNullOrEmpty(invoice))
                {
                    return Result.Failure("Unable to generate invoice at the moment. Please try again later");
                }
                return Result.Success("Invoice generated successfully", invoice);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Invoice generation was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
