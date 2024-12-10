using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SellersZone.Core.DTOs
{
    public class OrderDto : BaseEntityDto
    {
        public string? BuyerName { get; set; }
        public string? BuyerPhone { get; set; }
        public string? StoreName { get; set; }
        public string? Note { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DashboardNote { get; set; }
        public decimal CollectionAmount { get; set; }
        public int Status { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? DeliveryReturnPayer { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? DeliveryReturnPayerAmount { get; set; }
        public decimal SubTotal { get; set; }
        public int ItemCount { get; set; }
        public int DeliveryId { get; set; }
        public string? DeliveryTo { get; set; }
        public string? DeliveryToDetails { get; set; }
        public decimal? DeliveryAmount { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? CostShippingPrice { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? CostReturnedPrice { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? DeliveryFeeBalance { get; set; }
        public string? ShippingBarCode { get; set; }
        public string? ShippingUrl { get; set; }
        public decimal? Total { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? ItemToRemove { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ProductDto>? Products { get; set; }
    }
}
