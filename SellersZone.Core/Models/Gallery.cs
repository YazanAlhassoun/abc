using System.ComponentModel.DataAnnotations;

namespace SellersZone.Core.Models
{
    public class Gallery : BaseEntity
    {
        [StringLength(500)]
        public string? Key { get; set; }

        [StringLength(500)]
        public string? FileName { get; set; }

        [StringLength(500)]
        public string? FileType { get; set; }

        [StringLength(500)]
        public string? FileUrl { get; set; }
    }
}
