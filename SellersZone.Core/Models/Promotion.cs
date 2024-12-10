using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Models
{
    public class Promotion : BaseEntity
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? NameAr { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
