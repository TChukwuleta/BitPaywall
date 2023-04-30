using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Model.Response;
using BitPaywall.Infrastructure.Helper;
using Grpc.Core;
using Lnrpc;
using Microsoft.Extensions.Configuration;

namespace BitPaywall.Infrastructure.Services
{
    public class LightningService : ILightningService
    {
        private readonly IConfiguration _config;
        public LightningService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> CreateInvoice(long satoshis, string message)
        {
            string paymentRequest = default;
            var helper = new LightningHelper(_config);
            try
            {
                var adminInvoice = helper.CreateAdminInvoice(satoshis, message);
                paymentRequest = adminInvoice.PaymentRequest;
                return paymentRequest;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<long> GetChannelBalance()
        {
            var helper = new LightningHelper(_config);
            var channelBalanceRequest = new ChannelBalanceRequest();
            try
            {
                var adminClient = helper.GetAdminClient();
                var response = adminClient.ChannelBalance(channelBalanceRequest, new Metadata() { new Metadata.Entry("macaroon", helper.GetAdminMacaroon()) }).Balance;
                return response;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<(bool success, long expiry, long amount)> ValidateLightningAddress(string paymentRequest)
        {
            var helper = new LightningHelper(_config);
            try
            {
                var adminClient = helper.GetAdminClient();
                var request = new PayReqString
                {
                    PayReq = paymentRequest
                };
                var response = await adminClient.DecodePayReqAsync(request, new Metadata() { new Metadata.Entry("macaroon", helper.GetAdminMacaroon()) });
                if (response == null)
                {
                    return (false, 0, 0);
                }
                var paymentAddr = response.PaymentAddr;
                var hash = response.PaymentHash;
                return (true, response.Expiry, response.NumSatoshis);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<long> GetWalletBalance()
        {
            var helper = new LightningHelper(_config);
            var walletBalanceRequest = new WalletBalanceRequest();
            try
            {
                var adminClient = helper.GetAdminClient();
                var response = adminClient.WalletBalance(walletBalanceRequest, new Metadata() { new Metadata.Entry("macaroon", helper.GetAdminMacaroon()) }).TotalBalance;
                return response;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<InvoiceSettlementResponse> ListenForSettledInvoice()
        {
            var settledInvoiceResponse = new InvoiceSettlementResponse();
            try
            {
                var helper = new LightningHelper(_config);
                var txnReq = new InvoiceSubscription();
                var adminClient = helper.GetAdminClient();
                var settledAdminInvoioce = adminClient.SubscribeInvoices(txnReq, new Metadata() { new Metadata.Entry("macaroon", helper.GetAdminMacaroon()) });
                using (var call = settledAdminInvoioce)
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var invoice = call.ResponseStream.Current;
                        if (invoice.State == Invoice.Types.InvoiceState.Settled)
                        {
                            Console.WriteLine(invoice.ToString());
                            var split = invoice.Memo.Split('/');
                            settledInvoiceResponse.PaymentRequest = invoice.PaymentRequest;
                            settledInvoiceResponse.IsKeysend = invoice.IsKeysend;
                            settledInvoiceResponse.Value = invoice.Value;
                            settledInvoiceResponse.Expiry = invoice.Expiry;
                            settledInvoiceResponse.Settled = invoice.Settled;
                            settledInvoiceResponse.SettledDate = invoice.SettleDate;
                            settledInvoiceResponse.SettledIndex = (long)invoice.SettleIndex;
                            settledInvoiceResponse.Private = invoice.Private;
                            settledInvoiceResponse.AmountInSat = invoice.AmtPaidSat;
                            settledInvoiceResponse.Type = split[0];
                            settledInvoiceResponse.UserId = split[1];
                            return settledInvoiceResponse;
                        }
                    }
                }
                return settledInvoiceResponse;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<string> SendLightning(string paymentRequest)
        {
            var helper = new LightningHelper(_config);
            var sendRequest = new SendRequest();
            var paymentReq = new PayReqString();
            try
            {
                var adminClient = helper.GetAdminClient();
                paymentReq.PayReq = paymentRequest;
                var decodedAdminPaymentReq = adminClient.DecodePayReq(paymentReq, new Metadata() { new Metadata.Entry("macaroon", helper.GetAdminMacaroon()) });
                sendRequest.Amt = decodedAdminPaymentReq.NumSatoshis;
                sendRequest.PaymentRequest = paymentRequest;
                var adminResponse = adminClient.SendPaymentSync(sendRequest, new Metadata() { new Metadata.Entry("macaroon", helper.GetAdminMacaroon()) });
                var result = adminResponse.PaymentError;
                return result;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<long> DecodePaymentRequest(string paymentRequest)
        {
            var helper = new LightningHelper(_config);
            var paymentReq = new PayReqString();
            var walletBalance = await GetWalletBalance();
            try
            {
                var adminClient = helper.GetAdminClient();
                paymentReq.PayReq = paymentRequest;
                var decodedAdminPaymentReq = adminClient.DecodePayReq(paymentReq, new Metadata() { new Metadata.Entry("macaroon", helper.GetAdminMacaroon()) });
                if (walletBalance < decodedAdminPaymentReq.NumSatoshis)
                {
                    throw new ArgumentException("Unable to complete lightning payment. Insufficient node balance. Please contact support");
                }
                var result = decodedAdminPaymentReq.NumSatoshis;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
