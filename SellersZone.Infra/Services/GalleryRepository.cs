using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models;
using SellersZone.Core.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SellersZone.Infra.Services
{
    public class GalleryRepository : IGalleryRepository
    {
        private readonly StoreContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GalleryRepository(StoreContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public Response<GalleryDto> GetGalleries(RequestParam param)
        {
            try
            {
                IQueryable<Gallery> query = _db.Galleries;

                if (param.Search != null)
                {
                    query.Where(g => g.FileName!.ToUpper().Contains(param.Search.ToUpper()) ||
                                     g.FileType!.ToUpper().Contains(param.Search.ToUpper()) ||
                                     g.FileUrl!.ToUpper().Contains(param.Search.ToUpper()) ||
                                     g.Key!.ToUpper().Contains(param.Search.ToUpper())
                    );
                }

                int galleryCount = query != null ? query.Count() : 0;

                var galleryDto = query?.Skip((param.Page - 1) * param.ItemPerPage)
                                   .Take(param.ItemPerPage)
                                   .OrderBy(g => g.Id)
                                   .Select(gallery => new GalleryDto
                                   {
                                       Id = gallery.Id,
                                       Key = gallery.Key,
                                       FileName = gallery.FileName,
                                       FileType = gallery.FileType,
                                       FileUrl = gallery.FileUrl,
                                       CreationDate = gallery.CreatedAt,
                                   }).ToList();
                var paginationMetaData = new PaginationMetaData();
                if (galleryCount != 0)
                {
                    HttpRequest requestUrl = _httpContextAccessor.HttpContext.Request;
                    paginationMetaData = new PaginationMetaData(param.Page, galleryCount, param.ItemPerPage, requestUrl);
                }

                return new Response<GalleryDto>
                {
                    Data = galleryDto ?? new List<GalleryDto>(),
                    Pagination = paginationMetaData,
                    IsSuccess = true,
                    StatusCode = 200,
                };
            }
            catch (Exception ex)
            {
                return new Response<GalleryDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true,
                    StatusCode = 500
                };
            }
        }

        public Response<GalleryDto> AddGallery(GalleryDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Key) || dto.FileUrl == null)
                {
                    throw new ArgumentNullException(string.Empty, "Please fill all required field.");
                }
                var gallery = new Gallery
                {
                    Key = dto.Key,
                    FileUrl = dto.FileUrl,
                    FileType = dto.FileType,
                    FileName = dto.FileName,
                    CreatedAt = DateTime.Now
                };

                _db.Galleries.Add(gallery);
                _db.SaveChanges();

                var gallerydto = new GalleryDto
                {
                    Id = gallery.Id,
                    Key = gallery.Key,
                    FileUrl = gallery.FileUrl,
                    FileType = gallery.FileType,
                    FileName = gallery.FileName
                };

                return new Response<GalleryDto>
                {
                    Data = new List<GalleryDto> { gallerydto },
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                var res = new Response<GalleryDto>();
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

        public bool DeleteGallery(RequestParam param)
        {
            if (param.Id == null)
            {
                throw new ArgumentNullException(string.Empty, "Gallery UserId can't be null");
            }

            var gallery = _db.Galleries.FirstOrDefault(g => g.Id == param.Id);

            if (gallery != null)
            {
                _db.Galleries.Remove(gallery);
                _db.SaveChanges();
                return true;
            }
            return false;           
        }

    }
}
