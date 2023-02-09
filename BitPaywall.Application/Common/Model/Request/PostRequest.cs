using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Common.Model.Request
{
    public class PostRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Story { get; set; }
        public string Image { get; set; }
        public decimal Amount { get; set; }
    }
}
