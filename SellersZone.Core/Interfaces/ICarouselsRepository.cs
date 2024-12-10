using SellersZone.Core.DTOs;
using SellersZone.Core.Models;
using SellersZone.Core.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Interfaces
{
    public interface ICarouselsRepository
    {
        Response<CarouselDto> GetCarousel(RequestParam param);
        Response<CarouselDto> AddCarousel(CarouselDto carouselDto);
        Response<CarouselDto> EditCarousel(CarouselDto carouselDto);
        bool DeleteCarousel(RequestParam param);
    }
}
