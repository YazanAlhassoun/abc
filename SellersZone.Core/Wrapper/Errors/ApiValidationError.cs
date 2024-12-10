using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Wrapper.Errors
{
    public class ApiValidationError : ApiStatusError
    {
        public ApiValidationError() : base(400)
        {
        }

        public IEnumerable<string>? ValidationErrors { get; set; }
    }
}
