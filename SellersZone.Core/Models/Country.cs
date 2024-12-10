using SellersZone.Core.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace SellersZone.Core.Models
{
    public class Country : BaseEntity
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? NameAr { get; set; }

        [StringLength(100)]
        public string? Code { get; set; }

        public bool IsActive { get; set; }

        public bool IsDefault { get; set; }

        [StringLength(250)]
        public string? ImageUrl { get; set; }

        public short? Ordering { get; set; }

        public List<Section>? Sections { get; set; }
        public List<State>? States { get; set; }

        public List<AppUser>? AppUsers { get; set; }

    }
}
