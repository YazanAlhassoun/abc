using Microsoft.AspNetCore.Http;
using SellersZone.Core.Wrapper.Errors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.DTOs
{
    public class ProductDto : BaseEntityDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        public string? Caption { get; set; }
        public string? CaptionAr { get; set; }

        public string? Audience { get; set; }
        public string? AudienceAr { get; set; }

        public decimal? CostPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ProfitPrice { get; set; }
        public decimal? HighestPrice { get; set; }
        public decimal? ServicePriceForAllSellers { get; set; }

        public int? InStock { get; set; }
        public int? ActualInStock { get; set; }

        public string? SKU { get; set; }
        public string? CountryName { get; set; }
        public string? SectionName { get; set; }
        public int? Ordering { get; set; }
        public bool? IsActive { get; set; }
        public int? PromotionId { get; set; }
        public string? BarCode { get; set; }

        public string? UserId { get; set; }
        public bool? IsForSellers { get; set; }

        public ICollection<SectionDto>? Sections { get; set; }
        public ICollection<CountryDto>? Countries { get; set; }

        public string? MainImageUrl { get; set; }
        public string? ZipFileUrl { get; set; }
        public ICollection<ProductImagesDto>? ImagesUrls { get; set; }

        public UserDto? User { get; set; }

        //public IFormFile? MainImage { get; set; }
        //public string? MainImageUrl { get; set; }

        //public IFormFile? ZipFileUrl { get; set; }
        //public string? ZipFileUrl { get; set; }

        //public ICollection<IFormFile>? Images { get; set; }
        //public ICollection<ProductImagesDto>? ImagesUrls { get; set; }
        //public ICollection<string>? ImagesUrlsAsString { get; set; }



    }
}
