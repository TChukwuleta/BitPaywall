using Azure.Core;
using BitPaywall.Application.Posts.Commands;
using BitPaywall.Application.Posts.Queiries;
using BitPaywall.Application.Users.Commands;
using BitPaywall.Application.Users.Queries;
using BitPaywall.Core.Model;
using BitPaywall.Infrastructure.Utility;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BitPaywall.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ApiController
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        private readonly IMediator _mediator;

        public UserController(IMediator mediator, IHttpContextAccessor contextAccessor)
        {
            _mediator = mediator;
            _contextAccessor = contextAccessor;
            accessToken = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString()?.ExtractToken();
            if (accessToken == null)
            {
                throw new Exception("You are not authorized!");
            }
        }

        [HttpPost("createpost")]
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

        [HttpPost("updatepost")]
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

        [HttpPost("create")]
        public async Task<ActionResult<Result>> CreateUser(CreateUserCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"User creation was not successful. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<Result>> Login(UserLoginCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"User login was not successful. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("withdrawal")]
        public async Task<ActionResult<Result>> WIthDrawal(UserWithdrawalCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"User withdrawal was not successful. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getusersbyid/{userId}")]
        public async Task<ActionResult<Result>> GetUserByRoleId(string userId)
        {
            try
            {
                return await _mediator.Send(new GetUserByIdQuery
                {
                    UserId = userId
                });
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User retrieval was not successful" + ex?.Message ?? ex?.InnerException?.Message });
            }
        }

        [HttpGet("getpostbyid/{id}/{userId}")]
        public async Task<ActionResult<Result>> GetPostById(int id, string userId)
        {
            try
            {
                return await _mediator.Send(new GetUserPostByIdQuery
                {
                    Id = id,
                    UserId = userId
                });
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Post retrieval was not successful" + ex?.Message ?? ex?.InnerException?.Message });
            }
        }


        [HttpGet("getall/{skip}/{take}/{email}")]
        public async Task<ActionResult<Result>> GetAllUsers(int skip, int take, string email)
        {
            try
            {
                return await _mediator.Send(new GetAllUsersQuery
                {
                    Email = email,
                    Skip = skip,
                    Take = take
                });
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Users retrieval was not successful" + ex?.Message ?? ex?.InnerException?.Message });
            }
        }
    }
}
