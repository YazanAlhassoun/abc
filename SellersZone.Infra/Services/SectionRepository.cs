using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models;
using SellersZone.Core.Wrapper;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace SellersZone.Infra.Services
{
    public class SectionRepository : ISectionRepository
    {
        private readonly StoreContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SectionRepository(StoreContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<SectionDto>> GetSection(RequestParam param, ClaimsPrincipal userClaims)
        {

            var role = userClaims.FindFirst(ClaimTypes.Role);
            bool isAdmin = role != null && role.Value == "Admin";

            if (param.SectionId != null)
            {
                return await GetSectionById(param);
            }
            else
            {
                return await GetSections(param, isAdmin);
            }
        }

        // Info

        // Admin
        // get add sections list *
        // can filter sections based on country *
        // pagination sections *
        // Search sections *


        // Client
        // Client can see sections and product in every section base on country id. *
        // pagination sections
        // return 8 product for every section

        private async Task<Response<SectionDto>> GetSections(RequestParam param, bool isAdmin)
        {
            try
            {
                if (!isAdmin) // client
                {
                    if (param.CountryId == null || param.CountryId == 0)
                    {
                        throw new ArgumentNullException(string.Empty, "Country cannot be null");
                    }
                }

                IQueryable<Section> query = isAdmin ? _db.Sections :
                                   _db.Sections.Where(s => s.CountryId == param.CountryId && s.IsActive && s.ProductsSetion.Count() >= 1)
                                               .Include(ps => ps.ProductsSetion)
                                               .ThenInclude(p => p.Product);

                if (isAdmin && (param.CountryId != null && param.CountryId != 0)) // admin can filter sections based on country
                {
                    query = query.Where(s => s.CountryId == param.CountryId);
                }

                if (isAdmin && !string.IsNullOrWhiteSpace(param.Search)) // admin can search
                {
                    query = query.Where(s => s.Name!.Contains(param.Search) || s.NameAr!.Contains(param.Search));
                }

                int sectionsCount = query != null ? query.Count() : 0;

                var sectionsDto = await query.Skip((param.Page - 1) * param.ItemPerPage)
                                    .Take(param.ItemPerPage)
                                    .OrderBy(s => s.Ordering)
                                    .Select(s => new SectionDto
                                    {
                                        Id = s.Id,
                                        Name = s.Name,
                                        NameAr = s.NameAr,
                                        IsSlider = s.IsSlider,
                                        Ordering = s.Ordering,
                                        IsActive = s.IsActive,
                                        IsDefault = s.IsDefault,
                                        CountryId = s.CountryId,
                                        CountryNameAr = s.Country.NameAr,
                                        Products = isAdmin ? null : s.ProductsSetion
                                            .Where(product => product.Product.IsActive)
                                            .Take(8) // products per section
                                            .OrderByDescending(product => product.Product.Id)
                                            .Select(product => new ProductDto
                                            {
                                                Id = product.Product.Id,
                                                Name = product.Product.Name,
                                                NameAr = product.Product.NameAr,
                                                IsActive = product.Product.IsActive,
                                                Description = product.Product.Description,
                                                InStock = product.Product.InStock,
                                                MainImageUrl = product.Product.MainImageUrl,
                                                ProfitPrice = product.Product.ProfitPrice,
                                                SalePrice = product.Product.SalePrice,
                                                SKU = product.Product.SKU,
                                            }).ToList(),
                                    }).ToListAsync();


                var paginationMetaData = new PaginationMetaData();
                if (sectionsCount != 0)
                {
                    HttpRequest requestUrl = _httpContextAccessor.HttpContext.Request;
                    paginationMetaData = new PaginationMetaData(param.Page, sectionsCount, param.ItemPerPage, requestUrl);
                }

                return new Response<SectionDto>
                {
                    Data = sectionsDto,
                    Pagination = paginationMetaData,
                    IsSuccess = true

                };
            }
            catch (Exception ex)
            {
                var res = new Response<SectionDto>();
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

        // Info
        // Admin
        // display edit page or details 
        // when clicked on one section inside sections list
        // get the section info and all its products
        // paging on products inside section
        private async Task<Response<SectionDto>> GetSectionById(RequestParam param)
        {
            try
            {
                if (param.SectionId == null || param.SectionId == 0)
                {
                    throw new ArgumentNullException(string.Empty, "Section cannot be null");
                }

                IQueryable<Section> query = _db.Sections.Where(s => s.Id == param.SectionId)
                                          .Include(s => s.ProductsSetion)
                                          .ThenInclude(p => p.Product);
                if (query.Any())
                {
                    var sectionDto = await query.Select(s => new SectionDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        IsSlider = s.IsSlider,
                        Ordering = s.Ordering,
                        IsActive = s.IsActive,
                        IsDefault = s.IsDefault,
                        CountryId = s.CountryId,
                        Products = s.ProductsSetion.Select(product => new ProductDto
                        {
                            Id = product.Product.Id,
                            Name = product.Product.Name,
                            NameAr = product.Product.NameAr,
                            IsActive = product.Product.IsActive,
                            InStock = product.Product.InStock,
                            MainImageUrl = product.Product.MainImageUrl,
                            ProfitPrice = product.Product.ProfitPrice,
                            SalePrice = product.Product.SalePrice,
                            SKU = product.Product.SKU,
                        }).Skip((param.Page - 1) * param.ItemPerPage).Take(param.ItemPerPage).ToList()
                    }).FirstOrDefaultAsync();

                    int productsCount = query != null ? query.Select(x => x.ProductsSetion.Count()).FirstOrDefault() : 0;
                    HttpRequest requestUrl = _httpContextAccessor.HttpContext.Request;
                    var paginationMetaData = new PaginationMetaData(param.Page, productsCount, param.ItemPerPage, requestUrl);

                    return new Response<SectionDto>
                    {
                        Data = new List<SectionDto> { sectionDto },
                        Pagination = sectionDto != null ? paginationMetaData : null,
                        IsSuccess = true,
                        StatusCode = 200
                    };
                }
                else
                {
                    return new Response<SectionDto>
                    {
                        StatusCode = 404,
                        ErrorMessage = $"There is no data for section with id '{param.SectionId}'",
                        IsError = true
                    };
                }

            }
            catch (Exception ex)
            {
                var res = new Response<SectionDto>();
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

        // Info
        // Admin
        // add section or add section with its products
        public async Task<Response<SectionDto>> AddSection(SectionDto dto)
        {
            try
            {
                if (dto?.CountryId == null || dto.CountryId == 0 || string.IsNullOrEmpty(dto.NameAr))
                {
                    throw new ArgumentNullException(string.Empty, "Please check the entry field.");
                }
                else
                {
                    var section = new Section
                    {
                        Name = dto.Name,
                        NameAr = dto.NameAr,
                        CountryId = dto.CountryId,
                        IsActive = dto.IsActive,
                        IsSlider = dto.IsSlider,
                        IsDefault = dto.IsDefault,
                        Ordering = dto.Ordering,
                        CreatedAt = DateTime.Now,
                    };

                    _db.Sections.Add(section);
                    await _db.SaveChangesAsync();

                    if (dto.ProductsIds != null && dto.ProductsIds.Any())
                    {
                        // Add products to section
                        AddDataToProductsSection(section.Id, dto.ProductsIds);
                    }

                    var sectionDto = new SectionDto
                    {
                        Id = section.Id,
                        Name = section.Name,
                        NameAr = section.NameAr,
                        CountryId = section.CountryId,
                        IsActive = section.IsActive,
                        IsSlider = section.IsSlider,
                        IsDefault = section.IsDefault,
                        Ordering = section.Ordering,
                        CreationDate = section.CreatedAt,
                    };

                    return new Response<SectionDto>
                    {
                        Data = new List<SectionDto> { sectionDto },
                        IsSuccess = true,
                        StatusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                var res = new Response<SectionDto>();
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

        // Info
        // Admin
        // update section or section with its products
        public async Task<Response<SectionDto>> UpdateSection(SectionDto dto)
        {
            try
            {
                if (dto?.Id == null || dto.Id == 0 || dto?.CountryId == null || dto.CountryId == 0 || string.IsNullOrEmpty(dto.NameAr))
                {
                    throw new ArgumentNullException(string.Empty, "Please check the entry field.");
                }
                else
                {
                    var oldSection = _db.Sections.Where(s => s.Id == dto.Id).FirstOrDefault();

                    if (oldSection != null)
                    {
                        oldSection.Name = dto.Name;
                        oldSection.NameAr = dto.NameAr;
                        oldSection.IsActive = dto.IsActive;
                        oldSection.IsSlider = dto.IsSlider;
                        oldSection.IsDefault = dto.IsDefault;
                        oldSection.Ordering = dto.Ordering;
                        oldSection.CountryId = dto.CountryId;
                   
                        await _db.SaveChangesAsync();

                        if (dto.ProductsIds != null && dto.ProductsIds.Any())
                        {
                            // Remove existing entries
                            var existingProducts = _db.ProductsSetions.Where(p => p.SectionId == dto.Id).ToList();
                            _db.ProductsSetions.RemoveRange(existingProducts);
                            AddDataToProductsSection(dto.Id, dto.ProductsIds);
                        }

                        var sectionDto = new SectionDto
                        {
                            Id = oldSection.Id,
                            Name = oldSection.Name,
                            NameAr = oldSection.NameAr,
                            CountryId = oldSection.CountryId,
                            IsActive = oldSection.IsActive,
                            IsSlider = oldSection.IsSlider,
                            IsDefault = oldSection.IsDefault,
                            Ordering = oldSection.Ordering,
                        };

                        return new Response<SectionDto>
                        {
                            Data = new List<SectionDto> { sectionDto },
                            IsSuccess = true,
                            StatusCode = 200
                        };
                    }
                    else
                    {
                        return new Response<SectionDto>
                        {

                            IsError = true,
                            StatusCode = 404,
                            ErrorMessage = $"Section with id '{dto.Id}' not found."
                        };
                    };
                }

            }
            catch (Exception ex)
            {
                var res = new Response<SectionDto>();
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

        // Info
        // add data to ProductsSection table
        private void AddDataToProductsSection(int sectionId, List<int> productsIds)
        {
            List<int> parsedProductIds = productsIds.ToList();
            List<ProductsSetion> productSections = new List<ProductsSetion>();

            foreach (var productId in parsedProductIds)
            {
                var productSection = new ProductsSetion
                {
                    SectionId = sectionId,
                    ProductId = productId
                };
                productSections.Add(productSection);
            }

            _db.ProductsSetions.AddRange(productSections);
            _db.SaveChanges();
        }

    }
}
