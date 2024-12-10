using Microsoft.AspNetCore.Http;
using SellersZone.Core.Wrapper.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.DTOs
{
    public class CountryDto : BaseEntityDto
    {
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public string? Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public string? ImageUrl { get; set; }
        public short? Ordering { get; set; }
       public List<StateDto>? States { get; set; }

        public List<SectionDto>? Sections { get; set; }

    }
}
