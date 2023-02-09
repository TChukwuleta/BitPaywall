using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Common.Interfaces.Validators.UserValidator
{
    public interface IUserLoginValidator
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
