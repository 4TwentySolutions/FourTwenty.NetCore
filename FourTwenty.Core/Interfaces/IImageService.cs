using System.Threading.Tasks;
using ImageMagick;

namespace FourTwenty.Core.Interfaces
{
    public interface IImageService
    {
        bool CompressImage(string filePath, bool lossless = true);
        Task<string> CreateThumbnail(int width, int height, string filePath, string thumbnailSuffix = "-thumbnail");
        Task<string> ConvertToFormat(string filePath, MagickFormat format, string newFormatExtension);
    }
}
