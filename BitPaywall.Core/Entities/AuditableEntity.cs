using BitPaywall.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Core.Entities
{
    public class AuditableEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public Status Status { get; set; }
        public string StatusDesc { get { return Status.ToString(); } }
    }
}
