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
    public class GetTransactionByIdQuery : IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }

    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;

        public GetTransactionByIdQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Transaction retrieval was not successful. Invalid user details");
                }
                var entity = await _context.Transactions.FirstOrDefaultAsync(c => c.Id == request.Id);
                if (entity == null)
                {
                    return Result.Failure("Transaction retrieval was not successsful. Invalid transaction id");
                }
                return Result.Success("Transaction retrieval was successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Transaction retrieval by id was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
