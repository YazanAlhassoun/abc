using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Models
{
    public class ProductImage : BaseEntity
    {

        [StringLength(250)]
        public string? FileKey { get; set; }

        [StringLength(250)]
        public string? FileUrl { get; set; }

        public Product? Product { get; set; }
        public int? ProductId { get; set; }
    }
}
