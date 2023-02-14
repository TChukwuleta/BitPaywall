using BitPaywall.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Common.Interfaces
{
    public interface ILightningService
    {
        Task<string> CreateInvoice(long satoshis, string message, UserType userType);
        Task<long> GetChannelOutboundBalance(UserType userType);
        Task<long> GetChannelInboundBalance(UserType userType);
        Task<long> GetWalletBalance(UserType userType);
        Task<string> SendLightning(string paymentRequest, UserType userType);
    }
}
