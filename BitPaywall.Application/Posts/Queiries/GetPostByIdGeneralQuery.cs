using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Posts.Queiries
{
    public class GetPostByIdGeneralQuery : IRequest<Result>
    {
        public int PostId { get; set; }
    }

    public class GetPostByIdGeneralQueryHandler : IRequestHandler<GetPostByIdGeneralQuery, Result>
    {
        private readonly ILightningService _lightningService;
        private readonly IAppDbContext _context;

        public GetPostByIdGeneralQueryHandler(ILightningService lightningService, IAppDbContext context)
        {
            _lightningService = lightningService;
            _context = context;
        }

        public async Task<Result> Handle(GetPostByIdGeneralQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == request.PostId);
                if (post == null)
                {
                    return Result.Failure("Post retrieval was not successful. Invalid post specified");
                }
                var invoice = await _lightningService.CreateInvoice((long)post.Amount, $"{post.Id}/{post.UserId}");
                if (string.IsNullOrEmpty(invoice))
                {
                    return Result.Failure("An error occured while generating invoice");
                }
                return Result.Success("Invoice generated successfully", invoice);
            }
            catch (Exception ex)
            {
                return Result.Failure("Post retrieval was not successful", ex?.Message ?? ex?.InnerException.Message);
            }
        }
    }
}
