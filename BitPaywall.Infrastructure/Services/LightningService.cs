using Azure;
using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Enums;
using BitPaywall.Infrastructure.Helper;
using Grpc.Core;
using Lnrpc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Infrastructure.Services
{
    public class LightningService : ILightningService
    {
        private readonly IConfiguration _config;
        public LightningService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> CreateInvoice(long satoshis, string message, UserType userType)
        {
            string paymentRequest = default;
            var helper = new LightningHelper(_config);
            try
            {
                switch (userType)
                {
                    case UserType.User:
                        var userInvoice = helper.CreateUserInvoice(satoshis, message);
                        paymentRequest = userInvoice.PaymentRequest;
                        break;
                    case UserType.Admin:
                        var adminInvoice = helper.CreateAdminInvoice(satoshis, message);
                        paymentRequest = adminInvoice.PaymentRequest;
                        break;
                    default:
                        break;
                }
                return paymentRequest;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<long> GetChannelInboundBalance(UserType userType)
        {
            long response = default;
            try
            {
                switch (userType)
                {
                    case UserType.User:
                        var helper = new LightningHelper(_config);
                        var client = helper.GetUserClient();
                        var channelBalanceRequest = new ChannelBalanceRequest();
                        response = client.ChannelBalance(channelBalanceRequest, new Metadata() { new Metadata.Entry("macaroon", helper.GetUserMacaroon()) }).Balance;
                        break;
                    case UserType.Admin:
                        break;
                    default:
                        throw new ArgumentException("Invalid user type specified");
                }
                return response;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task<long> GetChannelOutboundBalance(UserType userType)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetWalletBalance(UserType userType)
        {
            throw new NotImplementedException();
        }

        public Task<string> SendLightning(string paymentRequest, UserType userType)
        {
            throw new NotImplementedException();
        }
    }
}
