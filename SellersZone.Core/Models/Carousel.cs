using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Models
{
    public class Carousel : BaseEntity
    {
        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(250)]
        public string? ImageUrl { get; set; }

        public short Ordering { get; set; }

        public bool IsActive { get; set; }
    }
}
