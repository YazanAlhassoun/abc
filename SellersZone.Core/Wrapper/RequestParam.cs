using SellersZone.Core.Enums;
using SellersZone.Core.Wrapper.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SellersZone.Core.Wrapper
{
    public class RequestParam : ResponseDetails
    {

        public int Page { get; set; } = 1;
        public int ItemPerPage { get; set; } = 30;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Id { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? UserId { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Search { get; set; } = null;


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CountryId { get; set; } = null;


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ProductId { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? SectionId { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CategoryId { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Email { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Quantity { get; set; } = 1;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public OrderStatus? Status { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SKU { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? FromDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? ToDate { get; set; }

    }
}
