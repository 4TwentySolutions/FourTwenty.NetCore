using System.Threading.Tasks;

namespace FourTwenty.Core.Interfaces
{
    public interface IImageService
    {
        bool CompressImage(string filePath, bool lossless = true);
        Task<string> CreateThumbnail(int width, int height, string filePath, string thumbnailSuffix = "-thumbnail");
    }
}
