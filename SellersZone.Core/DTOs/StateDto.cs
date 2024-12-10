using SellersZone.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.DTOs
{
    public class StateDto : BaseEntityDto
    {
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal Price { get; set; }
        public short? Ordering { get; set; }
        public bool IsActive { get; set; }

        public int? CountryId { get; set; }
        public string? CountryNameAr { get; set; }
    }
}
