using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Common.Interfaces.Validators.UserValidator
{
    public interface IUserNamesValidator : IBaseValidator
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
