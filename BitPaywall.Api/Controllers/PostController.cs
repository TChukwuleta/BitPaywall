using BitPaywall.Application.Posts.Queiries;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BitPaywall.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        private readonly IMediator _mediator;
        public PostController(IHttpContextAccessor contextAccessor, IMediator mediator)
        {
            _contextAccessor = contextAccessor;
            _mediator = mediator;
        }

        [HttpGet("getall/{skip}/{take}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAll(int skip, int take)
        {
            try
            {
                return await _mediator.Send(new GetAllPostsQuery { Skip = skip, Take = take });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }


        [HttpGet("getpostsummary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetPostSummary()
        {
            try
            {
                return await _mediator.Send(new GetPostsSummaryQuery());
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts summary retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getpopularposts/{take}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetPopularPosts(int take)
        {
            try
            {
                return await _mediator.Send(new GetPopularPostsQuery { Take = take });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyid/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetPostById(int id)
        {
            try
            {
                return await _mediator.Send(new GetPostByIdQuery { Id = id });
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
                return await _mediator.Send(new GetDraftPostByIdQuery { Id = id, UserId = userid });
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
                return await _mediator.Send(new GetAllUsersPostQuery { Skip = skip, Take = take, UserId = userid });
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
                return await _mediator.Send(new GetAllPublishedPostsQuery { Skip = skip, Take = take, UserId = userid });
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
                return await _mediator.Send(new GetAllPostDraftQuery { Skip = skip, Take = take, UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Draft posts retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
