using SellersZone.Core.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace SellersZone.Core.Models
{
    public class Log : BaseEntity
    {
        [StringLength(500)]
        public string? LogMessage { get; set; }

        [StringLength(200)]
        public string? Action { get; set; }

        [StringLength(200)]
        public string? UserId { get; set; }

        public Order? Order { get; set; }
        public int? OrderId { get; set; }
    }
}
