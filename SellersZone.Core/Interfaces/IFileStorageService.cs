using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFile(string containerName, IFormFile file, IHttpContextAccessor httpContextAccessor);

        Task DeleteFile(string fileRoute, string containerName);

        Task<string> EditFile(string containerName, IFormFile file, string fileRoute, IHttpContextAccessor httpContextAccessor); //fileRoute -> current picture

        string GetGuidFromFilePath(string filePath);
    }
}
