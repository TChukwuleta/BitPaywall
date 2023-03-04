using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Common.Model.Response
{
    public class PostRatingResponse
    {
        public int PostId { get; set; }
        public decimal PostAverageRating { get; set; }
    }
}
