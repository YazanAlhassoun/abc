using SellersZone.Core.DTOs;
using SellersZone.Core.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Interfaces
{
    public interface IGalleryRepository
    {
        Response<GalleryDto> GetGalleries(RequestParam param);
        Response<GalleryDto> AddGallery(GalleryDto section);
        bool DeleteGallery(RequestParam param);
    }
}
