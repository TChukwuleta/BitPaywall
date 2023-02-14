using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Lightning.Commands
{
    public class PayInvoiceCommand : IRequest<Result>, IBaseValidator
    {
        public string PaymentRequest { get; set; }
        public string UserId { get; set; }
    }

    public class PayInvoiceCommandHandler : IRequestHandler<PayInvoiceCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly ILightningService _lightningService;
        public PayInvoiceCommandHandler(IAuthService authService, ILightningService lightningService)
        {
            _authService = authService;
            _lightningService = lightningService;
        }

        public async Task<Result> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unalble to pay lightning invoice. Invalid user details specified");
                }
                var payInvoice = await _lightningService.SendLightning(request.PaymentRequest, Core.Enums.UserType.User);
                if (!string.IsNullOrEmpty(payInvoice))
                {
                    return Result.Failure($"An error occured while paying invoice. {payInvoice}");
                }
                return Result.Success("Invoice payment was successful", payInvoice);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Invoice payment was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
