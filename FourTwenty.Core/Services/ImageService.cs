using System.IO;
using System.Threading.Tasks;
using FourTwenty.Core.Interfaces;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;

namespace FourTwenty.Core.Services
{
    public class ImageService : IImageService
    {
        #region fields

        private readonly IHostingEnvironment _environment;

        #endregion

        public ImageService(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        public bool CompressImage(string filePath, bool lossless = true)
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);
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
        
        public Task<string> CreateThumbnail(int width, int height, string filePath, string thumbnailSuffix = "-thumbnail")
        {
            filePath = Path.Combine(_environment.WebRootPath, filePath);
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

    }
}
