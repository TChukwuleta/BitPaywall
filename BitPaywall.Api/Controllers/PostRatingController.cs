using BitPaywall.Application.PostAnalytics.Queries;
using BitPaywall.Application.PostRatings.Queries;
using BitPaywall.Core.Model;
using BitPaywall.Infrastructure.Utility;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BitPaywall.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostRatingController : ApiController
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        private readonly IMediator _mediator;
        public PostRatingController(IHttpContextAccessor contextAccessor, IMediator mediator)
        {
            _contextAccessor = contextAccessor;
            _mediator = mediator;
            accessToken = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString()?.ExtractToken();
            if (accessToken == null)
            {
                throw new Exception("You are not authorized!");
            }
        }

        [HttpGet("getbyid/{id}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetById(int id, string userid)
        {
            try
            {
                return await _mediator.Send(new GetPostRatingQuery { Id = id, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post rating retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getall/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAll(string userid)
        {
            try
            {
                return await _mediator.Send(new GetAllPostsRatingQuery { UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post rating retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
