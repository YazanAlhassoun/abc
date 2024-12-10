using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Models
{
    public class AboutUs : BaseEntity
    {
        [StringLength(1000)]
        public string? Title { get; set; }

        [StringLength(10000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
