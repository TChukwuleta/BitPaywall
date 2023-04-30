using BitPaywall.Application.PostAnalytics.Queries;
using BitPaywall.Core.Model;
using BitPaywall.Infrastructure.Utility;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BitPaywall.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostAnalyticsController : ApiController
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        private readonly IMediator _mediator;
        public PostAnalyticsController(IHttpContextAccessor contextAccessor, IMediator mediator)
        {
            _mediator = mediator;
            _contextAccessor = contextAccessor;
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
                return await _mediator.Send(new GetPostAnalyticsByIdQuery { Id = id, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post analytics retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbypostid/{id}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetByPostId(int id, string userid)
        {
            try
            {
                return await _mediator.Send(new GetPostAnalyticsByPostQuery { UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post analytics retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }


        [HttpGet("getuserspostanalytics/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetUsersPostAnalytics(string userid)
        {
            try
            {
                return await _mediator.Send(new GetUsersPostAnalyticsQuery {UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post analytics retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getallpostanalytics/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllPostAnalytics(string userid)
        {
            try
            {
                return await _mediator.Send(new GetAllSystemPostAnalyticsQuery { UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post analytics retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
