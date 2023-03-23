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

        [HttpPost("payforpost")]
        public async Task<ActionResult<Result>> PayForPost(EngagePostCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to pay for post. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
