using System.Threading.Tasks;
using FourTwenty.Core.Seo;

namespace FourTwenty.Core.Interfaces.Seo
{
    public interface ISitemapProvider
    {
        SitemapBuilder GetSitemapString();
        Task<SitemapBuilder> GetSitemapAsync();
    }
}
