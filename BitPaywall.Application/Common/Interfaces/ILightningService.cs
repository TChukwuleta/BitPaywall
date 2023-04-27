using BitPaywall.Application.Common.Model.Response;
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
        Task<string> CreateInvoice(long satoshis, string message);
        Task<long> GetChannelBalance();
        Task<long> GetWalletBalance();
        Task<string> SendLightning(string paymentRequest);
        Task<InvoiceSettlementResponse> ListenForSettledInvoice();
        Task<(bool success, long expiry, long amount)> ValidateLightningAddress(string paymentRequest);
    }
}
