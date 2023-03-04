using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Core.Entities
{
    public class PostRating : AuditableEntity
    {
        public int PostId { get; set; }
        public int Rating { get; set; }
        public string UserId { get; set; }
    }
}
