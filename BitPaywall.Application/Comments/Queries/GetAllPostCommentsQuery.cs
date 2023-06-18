using BitPaywall.Core.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Comments.Queries
{
    public class GetAllPostCommentsQuery  : IRequest<Result>
    {
        public int PostId { get; set; }
    }

    public class GetAllPostCommentsQueryHandler : IRequestHandler<GetAllPostCommentsQuery, Result>
    {
        public Task<Result> Handle(GetAllPostCommentsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
