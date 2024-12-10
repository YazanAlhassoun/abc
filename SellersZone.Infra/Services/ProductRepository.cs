using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models;
using SellersZone.Core.Models.Identity;
using SellersZone.Core.Wrapper;
using SellersZone.Infra.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SellersZone.Infra.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string containerName = "Products";
        private readonly UserManager<AppUser> _userManager;

        public ProductRepository(StoreContext db, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public Response<ProductDto> GetProduct(RequestParam param, ClaimsPrincipal userClaims)
        {
            var role = userClaims.FindFirst(ClaimTypes.Role);
            bool isAdmin = role != null && role.Value == "Admin";

            if (param.ProductId != null)
            {
                return GetProductById(param, isAdmin);
            }
            else
            {
                return GetProducts(param, userClaims, isAdmin);
            }
        }

        // Info
        // Admin 
        // client press section direct then --> pass country UserId and Section UserId
        // or client in products list and press to another country --> get products with defualt section based on country
        // he can change the section --> pass country UserId and Section UserId

        // client should have country UserId
        // admin if there no country id then it will get products base on defualt country

        //countryId=1 & sectionId=3 & search=null
        //countryId=1 & search="abc" & section=null
        private Response<ProductDto> GetProducts(RequestParam param, ClaimsPrincipal userClaims, bool isAdmin)
        {
            try
            {
              

                if (!isAdmin && (param.CountryId == null || param.CountryId == 0))
                {
                    throw new ArgumentNullException(string.Empty, "Country cannot be null");
                }

                return isAdmin ? GetProductForAdmin(param) : GetProductsForClient(param, userClaims);
            }
            catch (Exception ex)
            {
                var res = new Response<ProductDto>();
                if (ex is ArgumentNullException argEx)
                {
                    res.ErrorMessage = argEx.Message;
                    res.StatusCode = 400;
                }
                else
                {
                    res.ErrorMessage = ex.Message;
                    res.StatusCode = 500;
                }
                res.IsError = true;
                return res;
            }
        }

        private Response<ProductDto> GetProductsForClient(RequestParam param, ClaimsPrincipal userClaims)
        {
            var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();

            IQueryable<ProductsSetion> query = _db.ProductsSetions
                                     .Where(ps => param.CountryId != null
                                         ? (ps.Section.CountryId == param.CountryId)
                                           && (param.SectionId != null ? ps.SectionId == param.SectionId : ps.Section.IsDefault)
                                           && ps.Product.IsActive
                                           && (ps.Product.IsForSeller == true || (ps.Product.IsForSeller == false && ps.Product.AppUserId == user.Id))
                                         : false)
                                     .Include(p => p.Product)
                                     .Include(p => p.Section);

            if (!string.IsNullOrWhiteSpace(param.Search))
            {
                query = query.Where(s => s.Product.Name!.Contains(param.Search) || s.Product.NameAr!.Contains(param.Search)
                                      || s.Product.Description!.Contains(param.Search) || s.Product.DescriptionAr!.Contains(param.Search)
                                      || s.Product.SKU.Contains(param.Search) || s.Product.SalePrice.ToString().Contains(param.Search));
            }

            int ProductsCount = query != null ? query.Count() : 0;

            var ProductsDto = query?.Skip((param.Page - 1) * param.ItemPerPage)
                               .Take(param.ItemPerPage)
                               .OrderBy(sp => sp.Product.Id)
                               .Select(sp => new ProductDto
                               {
                                   Id = sp.Product.Id,
                                   Name = sp.Product.Name,
                                   NameAr = sp.Product.NameAr,
                                   IsActive = sp.Product.IsActive,
                                   InStock = sp.Product.InStock,
                                   MainImageUrl = sp.Product.MainImageUrl,
                                   ProfitPrice = sp.Product.ProfitPrice,
                                   SalePrice = sp.Product.SalePrice,
                                   SKU = sp.Product.SKU,
                                   CountryName = sp.Section.Country.NameAr,
                                   SectionName = sp.Section.NameAr
                               }).ToList();

            var paginationMetaData = new PaginationMetaData();
            if (ProductsCount > param.ItemPerPage)
            {
                HttpRequest requestUrl = _httpContextAccessor.HttpContext.Request;
                paginationMetaData = new PaginationMetaData(param.Page, ProductsCount, param.ItemPerPage, requestUrl);
            }

            return new Response<ProductDto>
            {
                Data = ProductsDto,
                Pagination = paginationMetaData,
                IsSuccess = true,
                StatusCode = 200,
            };
        }

        public Response<ProductDto> GetProductForAdmin(RequestParam param)
        {
            var query = _db.Products.AsQueryable();

            // Apply filters based on SectionId and CountryId if provided
            if (param.SectionId != null || param.CountryId != null)
            {
                query = query.Where(p =>
                    (param.SectionId == null || p.ProductsSection.Any(ps => ps.SectionId == param.SectionId)) &&
                    (param.CountryId == null || p.ProductsSection.Any(ps => ps.Section.CountryId == param.CountryId))
                );
            }

            if (!string.IsNullOrWhiteSpace(param.Search))
            {
                query = query.Where(p => p.Name.ToUpper().Contains(param.Search.ToUpper()) || p.NameAr.ToUpper().Contains(param.Search.ToUpper())
                                      || p.Description.ToUpper().Contains(param.Search.ToUpper()) || p.DescriptionAr.ToUpper().Contains(param.Search.ToUpper())
                                      || p.SKU.ToUpper().Contains(param.Search.ToUpper()) || p.SalePrice.ToString().Contains(param.Search.ToUpper()));
            }

            int ProductsCount = query != null ? query.Count() : 0;

            var ProductsDto = query
                     .Skip((param.Page - 1) * param.ItemPerPage)
                     .Take(param.ItemPerPage)
                     .OrderBy(p => p.Id)
                     .Select(p => new ProductDto
                     {
                         Id = p.Id,
                         Name = p.Name,
                         NameAr = p.NameAr,
                         IsActive = p.IsActive,
                         InStock = p.InStock,
                         MainImageUrl = p.MainImageUrl,
                         ZipFileUrl = p.ZipFileUrl,
                         ProfitPrice = p.ProfitPrice,
                         SalePrice = p.SalePrice,
                         BarCode = p.BarCode,                      
                         SKU = p.SKU,
                         Countries = p.ProductsSection
                                              .GroupBy(ps => new
                                              {
                                                  ps.Section.Country.Id,
                                                  ps.Section.Country.NameAr,
                                                  ps.Section.Country.ImageUrl
                                              })
                                              .Select(g => new CountryDto
                                              {
                                                  Id = g.Key.Id,
                                                  Name = g.Key.NameAr,
                                                  ImageUrl = g.Key.ImageUrl,
                                                  Sections = g.Select(ps => new SectionDto
                                                  {
                                                      Id = ps.Section.Id,
                                                      Name = ps.Section.Name,
                                                      NameAr = ps.Section.NameAr,
                                                      CountryImage = ps.Section.Country.ImageUrl
                                                  }).ToList()
                                              }).ToList()
                     }).ToList();

            var paginationMetaData = new PaginationMetaData();
            if (ProductsCount != 0)
            {
                HttpRequest requestUrl = _httpContextAccessor.HttpContext.Request;
                paginationMetaData = new PaginationMetaData(param.Page, ProductsCount, param.ItemPerPage, requestUrl);
            }

            return new Response<ProductDto>
            {
                Data = ProductsDto,
                Pagination = paginationMetaData,
                IsSuccess = true,
                StatusCode = 200,
            };
        }

        // Info
        // Admin 
        // client 

        // get product by id --> param.ProductId
        private Response<ProductDto> GetProductById(RequestParam param, bool isAdmin)
        {
            try
            {
                if (param.ProductId != null)
                {
                    var query = _db.Products.Where(p => p.Id == param.ProductId)
                                .Include(x => x.ProductImage)
                                .Select(p => new ProductDto
                                {
                                    Id = p.Id,
                                    Name = p.Name,
                                    NameAr = p.NameAr,
                                    Description = p.Description,
                                    DescriptionAr = p.DescriptionAr,
                                    Audience = p.Audience,
                                    AudienceAr = p.AudienceAr,
                                    Caption = p.Caption,
                                    CaptionAr = p.CaptionAr,
                                    Ordering = p.Ordering,
                                    IsActive = p.IsActive,
                                    InStock = p.InStock,
                                    MainImageUrl = p.MainImageUrl,
                                    ProfitPrice = p.ProfitPrice,
                                    SalePrice = p.SalePrice,
                                    SKU = p.SKU,
                                    ZipFileUrl = p.ZipFileUrl,
                                    BarCode = p.BarCode,
                                    ServicePriceForAllSellers = isAdmin ? p.ServicePriceForAllSellers : 0,
                                    ActualInStock = isAdmin ? p.ActualInStock : 0,
                                    ImagesUrls = p.ProductImage != null ? p.ProductImage.Select(pi => new ProductImagesDto
                                    {
                                        Id = pi.Id,
                                        FileUrl = pi.FileUrl,
                                        CreationDate = pi.CreatedAt,
                                        ModificationDate = pi.UpdatedAt
                                    }).ToList() : null,
                                    UserId = isAdmin ? p.AppUserId : null
                                }).FirstOrDefault();

                    return new Response<ProductDto>
                    {
                        Data = new List<ProductDto> { query },
                        IsSuccess = true,
                        StatusCode = 200,
                    };
                }
                else
                {
                    throw new ArgumentNullException(string.Empty, "Product id cann't be null");
                }
            }
            catch (Exception ex)
            {
                var res = new Response<ProductDto>();
                if (ex is ArgumentNullException argEx)
                {
                    res.ErrorMessage = argEx.Message;
                    res.StatusCode = 400;
                }
                else
                {
                    res.ErrorMessage = ex.Message;
                    res.StatusCode = 500;
                }
                res.IsError = true;
                return res;
            }
        }

        public Response<ProductDto> AddProduct(ProductDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.NameAr) || string.IsNullOrWhiteSpace(dto.MainImageUrl))
                {
                    throw new Exception("Please fill all required fields.");
                }

                var product = new Product
                {
                    Name = dto.Name,
                    NameAr = dto.NameAr,
                    Description = dto.Description,
                    DescriptionAr = dto.DescriptionAr,
                    Caption = dto.Caption,
                    CaptionAr = dto.CaptionAr,
                    Audience = dto.Audience,
                    AudienceAr = dto.AudienceAr,
                    CostPrice = dto.CostPrice ?? 0,
                    SalePrice = dto.SalePrice ?? 0,
                    ProfitPrice = dto.ProfitPrice ?? 0,
                    HighestPrice = dto.HighestPrice ?? 0,
                    ServicePriceForAllSellers = dto.ServicePriceForAllSellers ?? 0,
                    SKU = GenerateSKU(),
                    Ordering = dto.Ordering.HasValue ? dto.Ordering : 1,
                    IsActive = dto.IsActive ?? false,
                    InStock = dto.InStock.HasValue ? dto.InStock : 0,
                    MainImageUrl = dto.MainImageUrl,
                    BarCode = dto.BarCode,
                    AppUserId = !string.IsNullOrEmpty(dto.UserId) ? dto.UserId : string.Empty,
                    ZipFileUrl = !string.IsNullOrEmpty(dto.ZipFileUrl) ? dto.ZipFileUrl : string.Empty,                 
                    ActualInStock = dto.ActualInStock,
                    ProductImage = new List<ProductImage>()
                };

                if (dto.ImagesUrls != null)
                {
                    foreach (var image in dto.ImagesUrls)
                    {
                        var productImage = new ProductImage
                        {
                            FileKey = image.Key,
                            FileUrl = image.FileUrl                    
                        };
                        product.ProductImage?.Add(productImage);
                    }
                }

                _db.Products.Add(product);
                _db.SaveChanges();

                return new Response<ProductDto>
                {
                    IsSuccess = true
                };

            }
            catch (Exception ex)
            {
                return new Response<ProductDto>
                {
                    IsError = true,
                    ErrorMessage = "An error occurred while processing the request." + "/" + ex.Message
                };
            }
        }

        public Response<ProductDto> UpdateProduct(ProductDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.NameAr) || string.IsNullOrEmpty(dto.MainImageUrl))
                {
                    throw new Exception("Please fill all required field.");
                }

                var oldProduct = _db.Products.Where(p => p.Id == dto.Id).Include(i => i.ProductImage).FirstOrDefault();

                if (oldProduct != null)
                {
                    oldProduct.Id = dto.Id;
                    oldProduct.Name = dto.Name;
                    oldProduct.NameAr = dto.NameAr;
                    oldProduct.CostPrice = dto.CostPrice ?? 0;
                    oldProduct.SalePrice = dto.SalePrice ?? 0;
                    oldProduct.ProfitPrice = dto.ProfitPrice ?? 0;
                    oldProduct.HighestPrice = dto.HighestPrice ?? 0;
                    oldProduct.ServicePriceForAllSellers = dto.ServicePriceForAllSellers;
                    oldProduct.Ordering = dto.Ordering ?? 1;
                    oldProduct.IsActive = dto.IsActive ?? false;
                    oldProduct.InStock = dto.InStock ?? 0;
                    oldProduct.SKU = oldProduct.SKU;
                    oldProduct.BarCode = oldProduct.BarCode;
                    oldProduct.Description = dto.Description;
                    oldProduct.DescriptionAr = dto.DescriptionAr;
                    oldProduct.Caption = dto.Caption;
                    oldProduct.CaptionAr = dto.CaptionAr;
                    oldProduct.Audience = dto.Audience;
                    oldProduct.AudienceAr = dto.AudienceAr;
                    oldProduct.MainImageUrl = dto.MainImageUrl;             
                    oldProduct.ActualInStock = dto.ActualInStock;
                    oldProduct.ZipFileUrl = !string.IsNullOrEmpty(dto.ZipFileUrl) ? dto.ZipFileUrl : string.Empty;
                    oldProduct.AppUserId = !string.IsNullOrEmpty(dto.UserId) ? dto.UserId : string.Empty;


                    // Delete old images
                    if (oldProduct.ProductImage != null)
                    {
                        _db.ProductImages.RemoveRange(oldProduct.ProductImage);
                        oldProduct.ProductImage.Clear();
                    }

                    // Add new images
                    if (dto.ImagesUrls != null)
                    {
                        foreach (var fileImage in dto.ImagesUrls)
                        {
                            var newProductImage = new ProductImage
                            {
                                FileKey = fileImage.Key,
                                FileUrl = fileImage.FileUrl
                            };
                            oldProduct?.ProductImage?.Add(newProductImage);
                        }
                    }
                }

                _db.Products.Update(oldProduct ?? new Product());
                _db.SaveChanges();

                return new Response<ProductDto>
                {
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                var res = new Response<ProductDto>();
                if (ex is ArgumentNullException argEx)
                {
                    res.ErrorMessage = argEx.Message;
                    res.StatusCode = 400;
                }
                else
                {
                    res.ErrorMessage = ex.Message;
                    res.StatusCode = 500;
                }
                res.IsError = true;
                return res;
            }
        }

        private string GenerateSKU()
        {
            return "SZ-" + Guid.NewGuid().ToString().Substring(0, 8);
        }


        #region old upload files
        //if (dto.MainImageUrl != null)
        //{
        //    var fileRoute = _fileStorageService.GetGuidFromFilePath(dto.MainImageUrl); // old image in server
        //    product.MainImageUrl = await _fileStorageService.EditFile(containerName, dto.MainImage, fileRoute, _httpContextAccessor);
        //}
        //else
        //{
        //    product.MainImageUrl = dto.MainImageUrl;
        //}

        //if (dto.ZipFileUrl != null)
        //{
        //    var fileRoute = _fileStorageService.GetGuidFromFilePath(dto.ZipFileUrl ?? ""); // old zipFile in server
        //    product.ZipFileUrl = await _fileStorageService.EditFile(containerName, dto.ZipFileUrl, fileRoute, _httpContextAccessor);
        //}
        //else
        //{
        //    product.ZipFileUrl = dto.ZipFileUrl;
        //}

        //if (dto.Images != null || dto.ImagesUrlsAsString != null)
        //{
        //    var productImage = new ProductImage();
        //    var oldImagesFromDb = await GetProductImages(dto.UserId);

        //    // old images
        //    if (dto.ImagesUrlsAsString != null)
        //    {

        //        foreach (var oldImage in oldImagesFromDb)
        //        {
        //            if (dto.ImagesUrlsAsString.Any(imageUrl => imageUrl == oldImage.FileUrl))
        //            {
        //                // keep old image at it's
        //                productImage = new ProductImage
        //                {
        //                    FileUrl = oldImage.FileUrl
        //                };
        //                product.ProductImage?.Add(productImage);
        //            }
        //            else
        //            {
        //                // delete image from server 
        //                var fileRoute = _fileStorageService.GetGuidFromFilePath(oldImage.FileUrl);
        //                await _fileStorageService.DeleteFile(fileRoute, containerName);
        //            }
        //        }
        //    }

        //    // add extra images
        //    if (dto.Images != null)
        //    {
        //        foreach (var image in dto.Images)
        //        {
        //            if (image is IFormFile)
        //            {
        //                productImage = new ProductImage
        //                {
        //                    FileUrl = await _fileStorageService.SaveFile(containerName, image, _httpContextAccessor)
        //                };
        //                product.ProductImage?.Add(productImage);
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    var oldImagesFromDb = await GetProductImages(dto.UserId);
        //    foreach (var oldImage in oldImagesFromDb)
        //    {
        //        // delete image from server in case delete all images
        //        var fileRoute = _fileStorageService.GetGuidFromFilePath(oldImage.FileUrl ?? "");
        //        await _fileStorageService.DeleteFile(fileRoute, containerName);
        //    }
        //}
        #endregion

        #region old GetProductForAdmin it return countries and section as separated
        //public Response<ProductDto> GetProductForAdmin(RequestParam param)
        //{
        //    var query = _db.Products.AsQueryable();

        //    // Apply filters based on SectionId and CountryId if provided
        //    if (param.SectionId != null || param.CountryId != null)
        //    {
        //        query = query.Where(p =>
        //            (param.SectionId == null || p.ProductsSection.Any(ps => ps.SectionId == param.SectionId)) &&
        //            (param.CountryId == null || p.ProductsSection.Any(ps => ps.Section.CountryId == param.CountryId))
        //        );
        //    }

        //    // Execute the query to retrieve the filtered products
        //    var productList = query.ToList();


        //    if (!string.IsNullOrWhiteSpace(param.Search))
        //    {
        //        query = query.Where(p => p.Name.ToUpper().Contains(param.Search.ToUpper()) || p.NameAr.ToUpper().Contains(param.Search.ToUpper())
        //                              || p.Description.ToUpper().Contains(param.Search.ToUpper()) || p.DescriptionAr.ToUpper().Contains(param.Search.ToUpper())
        //                              || p.SKU.ToUpper().Contains(param.Search.ToUpper()) || p.SalePrice.ToString().Contains(param.Search.ToUpper()));
        //    }

        //    int ProductsCount = query != null ? query.Count() : 0;

        //    var ProductsDto = query?.Skip((param.Page - 1) * param.ItemPerPage)
        //                       .Take(param.ItemPerPage)
        //                       .OrderBy(p => p.Id)
        //                       .Select(p => new ProductDto
        //                       {
        //                           Id = p.Id,
        //                           Name = p.Name,
        //                           NameAr = p.NameAr,
        //                           IsActive = p.IsActive,
        //                           InStock = p.InStock,
        //                           MainImageUrl = p.MainImageUrl,
        //                           ZipFileUrl = p.ZipFileUrl,
        //                           ProfitPrice = p.ProfitPrice,
        //                           SalePrice = p.SalePrice,
        //                           SKU = p.SKU,
        //                           Sections = p.ProductsSection != null ? p.ProductsSection.Select(ps => new SectionDto
        //                           {
        //                               Id = ps.Section.Id,
        //                               Name = ps.Section.Name,
        //                               CountryNameAr = ps.Section.Country.NameAr,
        //                               CountryImage = ps.Section.Country.ImageUrl
        //                           }).ToList() : null,
        //                           Countries = p.ProductsSection.Select(ps => new CountryDto
        //                           {
        //                               Id = ps.Section.Country.Id,
        //                               Name = ps.Section.Country.NameAr,
        //                               ImageUrl = ps.Section.Country.ImageUrl
        //                           }).GroupBy(c => c.Id).Select(g => g.First()).ToList()
        //                       }).ToList();

        //    var paginationMetaData = new PaginationMetaData();
        //    if (ProductsCount != 0)
        //    {
        //        HttpRequest requestUrl = _httpContextAccessor.HttpContext.Request;
        //        paginationMetaData = new PaginationMetaData(param.Page, ProductsCount, param.ItemPerPage, requestUrl);
        //    }

        //    return new Response<ProductDto>
        //    {
        //        Data = ProductsDto,
        //        Pagination = paginationMetaData,
        //        IsSuccess = true,
        //        StatusCode = 200,
        //    };
        //}
        #endregion
    }
}
