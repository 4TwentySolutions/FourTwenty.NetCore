using System.Collections.Generic;
using System.Threading.Tasks;

namespace FourTwenty.Dashboard.Interfaces.Navbars
{
    public interface ISidebarHelper
    {
        Task<IEnumerable<INavbarItem>> ProfileItemsPerUser(string userName);
        Task<IEnumerable<INavbarItem>> ItemsPerUser(string controller, string action, string userName);
    }
}
