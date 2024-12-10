using SellersZone.Core.Wrapper;
using SellersZone.Core.Wrapper.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.DTOs
{
    public class SectionDto : BaseEntityDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public bool IsSlider { get; set; }
        public short Ordering { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public int CountryId { get; set; }
        public string? CountryNameAr { get; set; }
        public string? CountryImage { get; set; }
        public List<int>? ProductsIds { get; set; }
        public List<ProductDto>? Products { get; set; }

    }

}
