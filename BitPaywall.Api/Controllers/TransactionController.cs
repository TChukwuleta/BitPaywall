using BitPaywall.Application.Transactions.Commands;
using BitPaywall.Application.Transactions.Queries;
using BitPaywall.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BitPaywall.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ApiController
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        private readonly string accessToken;
        public TransactionController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            accessToken = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("You are not authorized!");
            }
        }

        [HttpPost("createtransaction")]
        public async Task<ActionResult<Result>> CreateTransaction(CreateTransactionCommand command)
        {
            try
            {
                return await Mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to create transaction. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("gettransactionsbyid/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllTransactionsByUser(int skip, int take, string userid)
        {
            try
            {
                return await Mediator.Send(new GetAllTransactionsQuery { Skip = skip, Take = take, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Transactions retrieval by user failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyid/{id}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetTransactionById(int id, string userid)
        {
            try
            {
                return await Mediator.Send(new GetTransactionByIdQuery { Id = id, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Transaction retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbytxnid/{txnref}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetTransactionByTxnId(string txnref, string userid)
        {
            try
            {
                return await Mediator.Send(new GetTransactionsByTxnIdQuery { TxnRef = txnref, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Transaction retrieval by reference failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getallcredit/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllDebitTransactionsByUser(int skip, int take, string userid)
        {
            try
            {
                return await Mediator.Send(new GetCreditTransactionsByUserIdQuery { UserId = userid, Skip = skip, Take = take });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Credit transactions retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getcredittransaction/{skip}/{take}/{accountnumber}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetCreditTransactionsByAccountNumber(int skip, int take, string accountnumber, string userid)
        {
            try
            {
                return await Mediator.Send(new GetCreditTransactionByAccountNumberQuery { AccountNumber = accountnumber, UserId = userid, Skip = skip, Take = take });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Credit transactions retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getalldebit/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllCreditTransactionsByUser(int skip, int take, string userid)
        {
            try
            {
                return await Mediator.Send(new GetDebitTransactionByUserIdQuery { UserId = userid, Skip = skip, Take = take });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Debit transactions retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getdebittransaction/{skip}/{take}/{accountnumber}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetDebitTransactionsByAccountNumber(int skip, int take, string accountnumber, string userid)
        {
            try
            {
                return await Mediator.Send(new GetDebitTransactionByAccountNumberQuery { AccountNumber = accountnumber, UserId = userid, Skip = skip, Take = take });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Debit transactions retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
