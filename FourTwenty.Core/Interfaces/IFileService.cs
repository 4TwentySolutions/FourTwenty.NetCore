using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FourTwenty.Core.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFile(IFormFile file, string savePath, string? fileName = null);
        Task Delete(string path);
    }
}
