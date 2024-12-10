using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models;
using SellersZone.Core.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Infra.Services
{
    public class CarouselsRepository : ICarouselsRepository
    {
        private readonly StoreContext _db;
        public CarouselsRepository(StoreContext db)
        {
            _db = db;
        }

        public Response<CarouselDto> GetCarousel(RequestParam param)
        {
            try
            {
                IQueryable<Carousel> query = _db.Carousels;

                if (param.Id.HasValue)
                {
                    query = query.Where(x => x.Id == param.Id.Value);
                }

                var carouselDto = query.Select(carousel => new CarouselDto
                {
                    Id = carousel.Id,
                    Description = carousel.Description,
                    ImageUrl = carousel.ImageUrl,
                    Ordering = carousel.Ordering,
                    IsActive = carousel.IsActive,
                    CreationDate = carousel.CreatedAt,
                    ModificationDate = carousel.UpdatedAt
                }).ToList();

                return new Response<CarouselDto>
                {
                    Data = carouselDto,
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new Response<CarouselDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true,
                    StatusCode = 500
                };
            }
        }

        public Response<CarouselDto> AddCarousel(CarouselDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.ImageUrl))
                {
                    throw new Exception("Please fill required fields.");
                }
                else
                {
                    var carousel = new Carousel
                    {
                        Description = dto.Description,
                        ImageUrl = dto.ImageUrl,
                        IsActive = dto.IsActive,                    
                        Ordering = dto.Ordering,
                        CreatedAt = DateTime.Now,
                    };

                    _db.Carousels.Add(carousel);
                    _db.SaveChanges();

                    var carouselDto = new CarouselDto
                    {
                        Id = carousel.Id,
                        ImageUrl = carousel.ImageUrl,
                        IsActive = carousel.IsActive,
                        Ordering = carousel.Ordering,
                        CreationDate = carousel.CreatedAt
                    };

                    return new Response<CarouselDto>
                    {
                        Data = new List<CarouselDto> { carouselDto },
                        IsSuccess = true,
                        StatusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response<CarouselDto>
                {
                    ErrorMessage = ex.Message,
                    StatusCode = 500,
                    IsError = true
                };
            }
        }

        public Response<CarouselDto> EditCarousel(CarouselDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.ImageUrl))
                {
                    throw new Exception("Please fill required fields.");
                }
                else
                {
                    var oldCarousel = _db.Carousels.Where(s => s.Id == dto.Id).FirstOrDefault();

                    if (oldCarousel != null)
                    {
                        oldCarousel.Description = dto.Description;
                        oldCarousel.ImageUrl = dto.ImageUrl;
                        oldCarousel.IsActive = dto.IsActive;
                        oldCarousel.Ordering = dto.Ordering;
                        oldCarousel.UpdatedAt = DateTime.Now;
                        _db.SaveChanges();

                        var carouselDto = new CarouselDto
                        {
                            Id = oldCarousel.Id,
                            ImageUrl = oldCarousel.ImageUrl,
                            IsActive = oldCarousel.IsActive,
                            Ordering = oldCarousel.Ordering,
                            CreationDate = oldCarousel.CreatedAt,
                            ModificationDate = oldCarousel.UpdatedAt
                        };

                        return new Response<CarouselDto>
                        {
                            Data = new List<CarouselDto> { carouselDto },
                            IsSuccess = true,
                            StatusCode = 200
                        };
                    }
                    else
                    {
                        return new Response<CarouselDto>
                        {
                            StatusCode = 404,
                            ErrorMessage = $"Carousel with id '{dto.Id}' not found.",
                            IsError = true
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                return new Response<CarouselDto>
                {
                    ErrorMessage = ex.Message,
                    StatusCode = 500,
                    IsError = true
                };
            }
        }

        public bool DeleteCarousel(RequestParam param)
        {
            if (param.Id == null)
            {
                throw new Exception("Carousel Id can't be null");
            }

            var carousels = _db.Carousels.FirstOrDefault(g => g.Id == param.Id);
            if (carousels != null)
            {
                _db.Carousels.Remove(carousels);
                _db.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
