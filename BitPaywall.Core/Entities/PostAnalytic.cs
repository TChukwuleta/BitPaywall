using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Core.Entities
{
    public class PostAnalytic : AuditableEntity
    {
        public int PostId { get; set; }
        public int? ReadCount { get; set; }
        public decimal? AmountGenerated { get; set; }
    }
}
