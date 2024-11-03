using System;
using System.IO;
using System.Threading.Tasks;
using FourTwenty.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace FourTwenty.Core.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="savePath"></param>
        /// <param name="fileName"></param>
        /// <returns>Filename of the saved file</returns>
        public async Task<string> SaveFile(IFormFile file, string savePath, string? fileName = null)
        {
            var uploadPath = Path.Combine(_environment.WebRootPath, savePath);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);
            var filePath = Path.Combine(uploadPath, $"{fileName ?? Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}");
            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Path.GetFileName(filePath);
        }


        public Task Delete(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var fullPath = Path.Combine(_environment.WebRootPath, path);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
            return Task.CompletedTask;
        }
    }
}
