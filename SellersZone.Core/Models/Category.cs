using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Models
{
    public class Category : BaseEntity
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? NameAr { get; set; }

        [StringLength(250)]
        public string? ImageUrl { get; set; }
        public short Ordering { get; set; }
        public bool IsActive { get; set; }
    }
}
