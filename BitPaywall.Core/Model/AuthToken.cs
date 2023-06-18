using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BitPaywall.Core.Model
{
    public class AuthToken
    {
        [JsonIgnore]
        public string? AccessToken { get; set; }
        public string UserId { get; set; }    
    }
}
