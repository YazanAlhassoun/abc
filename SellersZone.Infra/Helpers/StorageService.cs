using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SellersZone.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Infra.Helpers
{
    public class StorageService : IFileStorageService
    {
        private readonly IHostingEnvironment env;


        public StorageService(IHostingEnvironment env)
        {
            this.env = env;
        }

        public async Task<string> SaveFile(string containerName, IFormFile file, IHttpContextAccessor httpContextAccessor)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, containerName); //containerName is a folder

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string route = Path.Combine(folder, fileName);
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var content = ms.ToArray();
                await File.WriteAllBytesAsync(route, content);
            }

            var url = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var routeForDb = Path.Combine(url, containerName, fileName).Replace("\\", "/");

            return routeForDb;
        }

        public async Task<string> EditFile(string containerName, IFormFile file, string fileRoute, IHttpContextAccessor httpContextAccessor)
        {
            await DeleteFile(fileRoute, containerName);
            return await SaveFile(containerName, file, httpContextAccessor);
        }

        public Task DeleteFile(string fileRoute, string containerName)
        {
            if (string.IsNullOrEmpty(fileRoute))
            {
                return Task.CompletedTask;
            }
            var fileName = Path.GetFileName(fileRoute);
            var fileDirectory = Path.Combine(env.WebRootPath, containerName, fileName);

            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }
            return Task.CompletedTask;
        }

        public string GetGuidFromFilePath(string filePath)
        {
            // Remove the wwwroot path from the file path
            var relativePath = filePath.Replace(env.WebRootPath, "").TrimStart('/').TrimStart('\\');

            // Split the relative path into segments
            var segments = relativePath.Split('/', '\\');

            // The first segment should be the container name, so we can skip it
            var fileName = segments.Last();

            return fileName;
        }
    }
}
