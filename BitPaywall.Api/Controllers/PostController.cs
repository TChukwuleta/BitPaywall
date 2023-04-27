using BitPaywall.Application.Posts.Commands;
using BitPaywall.Application.Posts.Queiries;
using BitPaywall.Core.Model;
using BitPaywall.Infrastructure.Utility;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BitPaywall.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : ApiController
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        private readonly IMediator _mediator;
        public PostController(IHttpContextAccessor contextAccessor, IMediator mediator)
        {
            _contextAccessor = contextAccessor;
            _mediator = mediator;
            accessToken = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString()?.ExtractToken();
            if (accessToken == null)
            {
                throw new Exception("You are not authorized!");
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<Result>> CreatePost(CreatePostCommand command)
        {
            try
            {
                command.AccessToken = accessToken.RawData;
                accessToken.ValidateToken(command.UserId);
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to create new post. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("update")]
        public async Task<ActionResult<Result>> UpdatePost(UpdatePostCommand command)
        {
            try
            {
                command.AccessToken = accessToken.RawData;
                accessToken.ValidateToken(command.UserId);
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update user post. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("publishpost")]
        public async Task<ActionResult<Result>> PublishPost(PublishPostCommand command)
        {
            try
            {
                command.AccessToken = accessToken.RawData;
                accessToken.ValidateToken(command.UserId);
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to publish post. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("payforpost")]
        public async Task<ActionResult<Result>> PayForPost(PayForPostCommand command)
        {
            try
            {
                command.AccessToken = accessToken.RawData;
                accessToken.ValidateToken(command.UserId);
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to pay for post. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getall/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAll(int skip, int take, string userid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                return await _mediator.Send(new GetAllPostsQuery { Skip = skip, Take = take, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Transactions retrieval by user failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyid/{id}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetPostById(int id, string userid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                return await _mediator.Send(new GetPostByIdQuery { Id = id, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getdraftpostbyid/{id}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetDraftPostById(int id, string userid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                return await _mediator.Send(new GetDraftPostByIdQuery { Id = id, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getuserspost/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllUsersPost(int skip, int take, string userid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                return await _mediator.Send(new GetAllUsersPostQuery { Skip = skip, Take = take, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval by user failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getallpublishedposts/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllPublished(int skip, int take, string userid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                return await _mediator.Send(new GetAllPublishedPostsQuery { Skip = skip, Take = take, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval by user failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getalldraftposts/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllDraftPost(int skip, int take, string userid)
        {
            try
            {
                accessToken.ValidateToken(userid);
                return await _mediator.Send(new GetAllPostDraftQuery { Skip = skip, Take = take, UserId = userid, AccessToken = accessToken.RawData });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Draft posts retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
