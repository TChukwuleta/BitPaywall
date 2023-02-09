using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Common.Interfaces
{
    public interface IBitcoinCoreClientService
    {
        Task<string> BitcoinRequestServer(string methodname);
        Task<string> BitcoinResponseServer(string methodname, string parameters);
        Task<string> BitcoinResponseServer(string methodname, List<string> parameters);
        Task<string> BitcoinResponseServer(string methodname, List<JToken>parameters);
    }
}
