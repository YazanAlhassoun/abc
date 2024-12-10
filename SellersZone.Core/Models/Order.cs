using SellersZone.Core.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellersZone.Core.Models
{
    public class Order : BaseEntity
    {

        public int Status { get; set; }
        public int? DeliveryReturnPayer { get; set; }
        public int ItemCount { get; set; }


        [StringLength(150)]
        public string? BuyerName { get; set; }

        [StringLength(50)]
        public string? BuyerPhone { get; set; }

        [StringLength(1000)]
        public string? Note { get; set; }

        [StringLength(50)]
        public string SKU { get; set; }

        [StringLength(1000)]
        public string? DeliveryToTxt { get; set; }

        [StringLength(200)]
        public string? StoreName { get; set; }

        [StringLength(500)]
        public string? ScheduledJobId { get; set; }

        [StringLength(1500)]
        public string? CanceledOrderDetails { get; set; }

        [StringLength(1500)]
        public string? ShippingBarCode { get; set; }

        [StringLength(500)]
        public string? ShippingUrl { get; set; }

        [StringLength(1000)]
        public string? DashboardNote { get; set; }


        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? CostShippingPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? CostReturnedPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SellerProfit { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? AdminProfit { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? shippingProfit { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CollectionAmount { get; set; }

        public DateTime? ProfitTransferDate { get; set; }

        public bool? JobCompleted { get; set; }
        public bool? isDelivered { get; set; }
        public bool? isReturned { get; set; }
        public bool? isCanceled { get; set; }

        public bool IsOrderLifecycleEnded { get; set; }


        public State State { get; set; }
        public int StateId { get; set; }

        public AppUser? AppUser { get; set; }
        public string? AppUserId { get; set; }

        public CanceledOrder? CanceledOrder { get; set; }
        public int? CanceledOrderId { get; set; }

        public List<ProductsOrder> ProductsOrder { get; set; }
        public List<Log>? Logs { get; set; }

    }
}
