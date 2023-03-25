using BitPaywall.Application.Posts.Commands;
using BitPaywall.Application.Posts.Queiries;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BitPaywall.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMediator _mediator;
        public ExternalController(IHttpContextAccessor contextAccessor, IMediator mediator)
        {
            _contextAccessor = contextAccessor;
            _mediator = mediator;
        }

        [HttpPost("listenforpayment")]
        public async Task<ActionResult<Result>> PayForPost(ListenForExternalPaymentCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to finalize payment. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyid/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetPostById(int id)
        {
            try
            {
                return await _mediator.Send(new GetPostByIdGeneralQuery { PostId = id });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
