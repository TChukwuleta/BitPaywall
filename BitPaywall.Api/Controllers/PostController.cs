using BitPaywall.Application.Posts.Commands;
using BitPaywall.Application.Posts.Queiries;
using BitPaywall.Application.Transactions.Queries;
using BitPaywall.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BitPaywall.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ApiController
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        private readonly string accessToken;
        public PostController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            accessToken = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("You are not authorized!");
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<Result>> CreatePost(CreatePostCommand command)
        {
            try
            {
                return await Mediator.Send(command);
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
                return await Mediator.Send(command);
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
                return await Mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to publish post. Error: {ex?.Message ?? ex?.InnerException?.Message}");
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

        [HttpGet("getall/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAll(int skip, int take, string userid)
        {
            try
            {
                return await Mediator.Send(new GetAllPostsQuery { Skip = skip, Take = take, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Transactions retrieval by user failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getuserspost/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllUsersPost(int skip, int take, string userid)
        {
            try
            {
                return await Mediator.Send(new GetAllUsersPostQuery { Skip = skip, Take = take, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Transactions retrieval by user failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getallpublishedpost/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllPublished(int skip, int take, string userid)
        {
            try
            {
                return await Mediator.Send(new GetAllPublishedPostsQuery { Skip = skip, Take = take, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Transactions retrieval by user failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getallpostdraft/{skip}/{take}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllDraftPost(int skip, int take, string userid)
        {
            try
            {
                return await Mediator.Send(new GetAllPostDraftQuery { Skip = skip, Take = take, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Draft posts retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyid/{id}/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetPostById(int id, string userid)
        {
            try
            {
                return await Mediator.Send(new GetPostByIdQuery { Id = id, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
