using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.DTOs
{
    public class ProductImagesDto : BaseEntityDto
    {
        public string? Key { get; set; }
        public string? FileUrl { get; set; }

       // public string? FileUrl { get; set; }
    }
}
