using SellersZone.Core.Wrapper.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SellersZone.Core.Wrapper
{
    public class Response<T> : ResponseDetails
    {
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<T>? Data { get; set; }

        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PaginationMetaData? Pagination { get; set; }


    }
}
