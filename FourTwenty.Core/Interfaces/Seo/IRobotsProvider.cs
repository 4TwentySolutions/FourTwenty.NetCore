using System.Threading.Tasks;

namespace FourTwenty.Core.Interfaces.Seo
{
    public interface IRobotsProvider
    {
        Task<string> GetRobotsContent();
    }
}
