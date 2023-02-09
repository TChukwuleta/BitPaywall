using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Core.Enums
{
    public enum TransactionStatus
    {
        Initiated = 1,
        Processing = 2,
        Success = 3,
        Failed = 4,
        Reversed = 5,
        Cancelled = 6
    }
}
