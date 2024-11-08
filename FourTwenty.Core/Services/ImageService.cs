﻿using System.IO;
using System.Threading.Tasks;
using FourTwenty.Core.Interfaces;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;

namespace FourTwenty.Core.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment? _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public bool CompressImage(string filePath, bool lossless = true)
        {
            var fullPath = GetBasePath(filePath);
            var file = new FileInfo(fullPath);
            var optimizer = new ImageOptimizer();
            bool result;
            if (lossless)
            {
                result = optimizer.LosslessCompress(file);
                file.Refresh();
                return result;

            }
            result = optimizer.Compress(file);
            return result;
        }

        public Task<string> CreateThumbnail(uint width, uint height, string filePath, string thumbnailSuffix = "-thumbnail")
        {
            filePath = GetBasePath(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var finalPath = filePath.Replace(fileName, $"{fileName}{thumbnailSuffix}");

            using (MagickImage image = new MagickImage(filePath))
            {
                width = image.Width < width ? image.Width : width;
                height = image.Height < height ? image.Height : height;

                image.Thumbnail(width, height);
                image.Write(finalPath);
            }
            return Task.FromResult(Path.GetFileName(finalPath));

        }



        public Task<string?> ConvertToFormat(string filePath, MagickFormat format, string newFormatExtension)
        {
            filePath = GetBasePath(filePath);
            // Read first frame of gif image
            using (MagickImage image = new MagickImage(filePath))
            {
                image.Format = format;
                var pathExtension = Path.GetExtension(filePath);
                if (!string.IsNullOrEmpty(pathExtension))
                {
                    var finalPath = filePath.Replace(pathExtension, newFormatExtension);
                    image.Write(finalPath);
                    return Task.FromResult<string?>(finalPath);
                }
            }

            return Task.FromResult<string?>(null);
        }

        protected virtual string GetBasePath(string fileName)
        {
            if (_environment == null)
                return fileName;
            return Path.Combine(_environment.WebRootPath, fileName);
        }

    }
}
