using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Transactions.Queries
{
    public class GetTransactionsByTxnIdQuery : IRequest<Result>, IBaseValidator
    {
        public string TxnRef { get; set; }
        public string UserId { get; set; }
    }

    public class GetTransactionsByTxnIdQueryHandler : IRequestHandler<GetTransactionsByTxnIdQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public GetTransactionsByTxnIdQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetTransactionsByTxnIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve transactions by transaction reference. Invalid user details specified");
                }
                var transaction = await _context.Transactions.FirstOrDefaultAsync(c => c.TransactionReference == request.TxnRef && c.UserId == request.UserId);
                if (transaction == null)
                {
                    return Result.Failure("Invalid transaction reference specified");
                }
                return Result.Success("Retrieving transaction by transaction reference was successful", transaction);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Transaction retrieval by reference number was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
