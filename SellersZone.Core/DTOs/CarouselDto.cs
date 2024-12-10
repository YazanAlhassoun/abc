using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.DTOs
{
    public class CarouselDto : BaseEntityDto
    {
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public short Ordering { get; set; }
        public bool IsActive { get; set; }
    }
}
