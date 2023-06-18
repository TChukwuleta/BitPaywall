using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BitPaywall.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilityController : ControllerBase
    {
        [HttpGet("getpaymentmodetype")]
        public async Task<ActionResult<Result>> GetPaymentType()
        {
            try
            {
                return await Task.Run(() => Result.Success(
                 ((PaymentModeType[])Enum.GetValues(typeof(PaymentModeType))).Select(x => new { Value = (int)x, Name = x.ToString() }).ToList()
                 ));
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Get payment mode type enums failed" + ex?.Message ?? ex?.InnerException?.Message });
            }
        }

        [HttpGet("getpostcategory")]
        public async Task<ActionResult<Result>> GetPostCategory()
        {
            try
            {
                return await Task.Run(() => Result.Success(
                 ((PostCategory[])Enum.GetValues(typeof(PostCategory))).Select(x => new { Value = (int)x, Name = x.ToString() }).ToList()
                 ));
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Get post category enums failed" + ex?.Message ?? ex?.InnerException?.Message });
            }
        }

        [HttpGet("getpoststatustype")]
        public async Task<ActionResult<Result>> GetPostStatusType()
        {
            try
            {
                return await Task.Run(() => Result.Success(
                 ((PostStatusType[])Enum.GetValues(typeof(PostStatusType))).Select(x => new { Value = (int)x, Name = x.ToString() }).ToList()
                 ));
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Get post status type enums failed" + ex?.Message ?? ex?.InnerException?.Message });
            }
        }

        [HttpGet("gettransactionstatus")]
        public async Task<ActionResult<Result>> GetTransactionStatus()
        {
            try
            {
                return await Task.Run(() => Result.Success(
                 ((TransactionStatus[])Enum.GetValues(typeof(TransactionStatus))).Select(x => new { Value = (int)x, Name = x.ToString() }).ToList()
                 ));
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Get transaction status enums failed" + ex?.Message ?? ex?.InnerException?.Message });
            }
        }

        [HttpGet("gettransactiontype")]
        public async Task<ActionResult<Result>> GetTransactionType()
        {
            try
            {
                return await Task.Run(() => Result.Success(
                 ((TransactionType[])Enum.GetValues(typeof(TransactionType))).Select(x => new { Value = (int)x, Name = x.ToString() }).ToList()
                 ));
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Get transaction type enums failed" + ex?.Message ?? ex?.InnerException?.Message });
            }
        }

        [HttpGet("getstatus")]
        public async Task<ActionResult<Result>> GetStatus()
        {
            try
            {
                return await Task.Run(() => Result.Success(
                 ((Status[])Enum.GetValues(typeof(Status))).Select(x => new { Value = (int)x, Name = x.ToString() }).ToList()
                 ));
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Get status enums failed" + ex?.Message ?? ex?.InnerException?.Message });
            }
        }
    }
}
