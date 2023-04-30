using BitPaywall.Application.EngagedPosts.Commands;
using BitPaywall.Application.EngagedPosts.Queries;
using BitPaywall.Core.Model;
using BitPaywall.Infrastructure.Utility;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BitPaywall.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EngagedPostController : ApiController
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        private readonly IMediator _mediator;
        public EngagedPostController(IHttpContextAccessor contextAccessor, IMediator mediator)
        {
            _contextAccessor = contextAccessor;
            _mediator = mediator;
            accessToken = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString()?.ExtractToken();
            if (accessToken == null)
            {
                throw new Exception("You are not authorized!");
            }
        }


        [HttpPost("activate")]
        public async Task<ActionResult<Result>> Activate(ActivateEngagedPostCommand command)
        {
            try
            {
                command.AccessToken = accessToken.RawData;
                accessToken.ValidateToken(command.UserId);
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update post status. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("deactivate")]
        public async Task<ActionResult<Result>> Deactivate(DeactivateEngagedPostCommand command)
        {
            try
            {
                command.AccessToken = accessToken.RawData;
                accessToken.ValidateToken(command.UserId);
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update post status. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("archive")]
        public async Task<ActionResult<Result>> Archive(ArchiveEngagedPostCommand command)
        {
            try
            {
                command.AccessToken = accessToken.RawData;
                accessToken.ValidateToken(command.UserId);
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update post status. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyid/{id}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetById(int id, string userid)
        {
            try
            {
                return await _mediator.Send(new GetEngagedPostByIdQuery { Id = id, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getall/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAll(int skip, int take, string userid)
        {
            try
            {
                return await _mediator.Send(new GetAllEngagedPostQuery { Skip = skip, Take = take, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getallactive/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllActive(int skip, int take, string userid)
        {
            try
            {
                return await _mediator.Send(new GetActiveEngagedPostQuery { Skip = skip, Take = take, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval by user failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getallarchived/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllArchived(int skip, int take, string userid)
        {
            try
            {
                return await _mediator.Send(new GetArchivedEngagedPostQuery { Skip = skip, Take = take, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getalldeactivated/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllDeactivated(int skip, int take, string userid)
        {
            try
            {
                return await _mediator.Send(new GetDeactivatedEngagedPostQuery { Skip = skip, Take = take, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}