using Grpc.Core;
using Lnrpc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Infrastructure.Helper
{
    public class LightningHelper
    {
        private readonly IConfiguration _config;
        private readonly string userMacaroonPath;
        private readonly string userSslCertificatePath;
        private readonly string userGRPCHost;
        private readonly string adminMacaroonPath;
        private readonly string adminSslCertificatePath;
        private readonly string adminGRPCHost;

        public LightningHelper(IConfiguration config)
        {
            _config = config;
            userMacaroonPath = _config["Lightning:UserMacaroonPath"];
            userSslCertificatePath = _config["Lightning:UserSslCertPath"];
            userGRPCHost = _config["Lightning:UserRpcHost"];
            adminMacaroonPath = _config["Lightning:AdminMacaroonPath"];
            adminSslCertificatePath = _config["Lightning:AdminSslCertPath"];
            adminGRPCHost = _config["Lightning:AdminRpcHost"];
        }

        public Lnrpc.Lightning.LightningClient GetUserClient()
        {
            var sslCreds = GetUserSslCredentials();
            var channel = new Grpc.Core.Channel(userGRPCHost, sslCreds);
            var client = new Lnrpc.Lightning.LightningClient(channel);
            return client;
        }

        public Lnrpc.Lightning.LightningClient GetAdminClient()
        {
            var sslCreds = GetAdminSslCredentials();
            var channel = new Grpc.Core.Channel(adminGRPCHost, sslCreds);
            return new Lnrpc.Lightning.LightningClient(channel);
        }

        public string GetUserMacaroon()
        {
            byte[] macaroonBytes = File.ReadAllBytes(userMacaroonPath);
            var macaroon = BitConverter.ToString(macaroonBytes).Replace("-", "");
            return macaroon;
        }

        public string GetAdminMacaroon()
        {
            byte[] macaroonBytes = File.ReadAllBytes(adminMacaroonPath);
            var macaroon = BitConverter.ToString(macaroonBytes).Replace("-", "");
            return macaroon;
        }

        public SslCredentials GetUserSslCredentials()
        {
            Environment.SetEnvironmentVariable("GRPC_SSL_CIPHER_SUITES", "HIGH+ECDSA");
            var cert = File.ReadAllText(userSslCertificatePath);
            var sslCreds = new SslCredentials(cert);
            return sslCreds;
        }

        public SslCredentials GetAdminSslCredentials()
        {
            Environment.SetEnvironmentVariable("GRPC_SSL_CIPHER_SUITES", "HIGH+ECDSA");
            var cert = File.ReadAllText(adminSslCertificatePath);
            var sslCreds = new SslCredentials(cert);
            return sslCreds;
        }

        public AddInvoiceResponse CreateUserInvoice(long satoshi, string memo)
        {
            try
            {
                var client = GetUserClient();
                var invoice = new Invoice();
                invoice.Memo = memo;
                invoice.Value = satoshi; // Value in satoshis
                var metadata = new Metadata() { new Metadata.Entry("macaroon", GetUserMacaroon()) };
                var invoiceResponse = client.AddInvoice(invoice, metadata);
                return invoiceResponse;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public AddInvoiceResponse CreateAdminInvoice(long satoshi, string memo)
        {
            try
            {
                var client = GetAdminClient();
                var invoice = new Invoice();
                invoice.Memo = memo;
                invoice.Value = satoshi; // Value in satoshis
                var metadata = new Metadata() { new Metadata.Entry("macaroon", GetAdminMacaroon()) };
                var invoiceResponse = client.AddInvoice(invoice, metadata);
                return invoiceResponse;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
