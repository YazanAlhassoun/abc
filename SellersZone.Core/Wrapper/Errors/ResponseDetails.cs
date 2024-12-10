using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Wrapper.Errors
{
    public class ResponseDetails
    {
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsError { get; set; }

    }
}
