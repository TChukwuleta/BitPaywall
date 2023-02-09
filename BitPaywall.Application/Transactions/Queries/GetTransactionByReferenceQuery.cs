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
    public class GetTransactionByReferenceQuery : IRequest<Result>, IBaseValidator
    {
        public string Reference { get; set; }
        public string UserId { get; set; }
    }

    public class GetTransactionByReferenceQueryHandler : IRequestHandler<GetTransactionByReferenceQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;

        public GetTransactionByReferenceQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetTransactionByReferenceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Transaction retrieval by reference was not successful. Invalid user details specified");
                }
                var entity = await _context.Transactions.FirstOrDefaultAsync(c => c.TransactionReference == request.Reference);
                if (entity == null)
                {
                    return Result.Failure("Transaction retrieval by reference was not successful. Invalid transaction reference specified");
                }
                return Result.Success("Transaction retrieval was successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Transaction retrieval by reference was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}


