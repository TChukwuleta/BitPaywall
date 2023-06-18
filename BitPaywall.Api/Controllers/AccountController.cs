using BitPaywall.Application.Accounts.Queries;
using BitPaywall.Application.Users.Commands;
using BitPaywall.Core.Model;
using BitPaywall.Infrastructure.Utility;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BitPaywall.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : ApiController
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        private readonly IMediator _mediator;
        public AccountController(IHttpContextAccessor contextAccessor, IMediator mediator)
        {
            _contextAccessor = contextAccessor;
            _mediator = mediator;
            accessToken = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString()?.ExtractToken();
            if (accessToken == null)
            {
                throw new Exception("You are not authorized!");
            }
        }

        [HttpPost("topupaccount")]
        public async Task<ActionResult<Core.Model.Result>> TopUpAccount(TopUpCommand command)
        {
            try
            {
                accessToken.ValidateToken(command.UserId);
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to topup user account. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("withdrawsatoshis")]
        public async Task<ActionResult<Result>> WithdrawSatoshis(WithdrawSatoshiCommand command)
        {
            try
            {
                accessToken.ValidateToken(command.UserId);
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed make payment. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getallaccounts/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllAccounts(int skip, int take, string userid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                accessToken.CheckAccessLevels();
                return await _mediator.Send(new GetAllAccountsQuery { Skip = skip, Take = take, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Accounts retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyaccountnumber/{accountnumber}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetByAccountNumber(string accountnumber, string userid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                return await _mediator.Send(new GetAccountByAccountNumberQuery { 
                    AccountNumber = accountnumber, 
                    UserId = userid 
                });
            }
            catch (Exception ex)
            {
                return Result.Failure($"User account retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyaccountid/{id}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetByAccountId(int id, string userid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                return await _mediator.Send(new GetAccountByAccountIdQuery
                {
                    Id = id,
                    UserId = userid
                });
            }
            catch (Exception ex)
            {
                return Result.Failure($"User account retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyuserid/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetByUserId(string userid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                return await _mediator.Send(new GetUserAccountQuery { UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"User account retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}