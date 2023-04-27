using BitPaywall.Application.Users.Commands;
using BitPaywall.Application.Users.Queries;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BitPaywall.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
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
