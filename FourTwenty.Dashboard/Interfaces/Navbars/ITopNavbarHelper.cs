using System.Collections.Generic;
using System.Threading.Tasks;

namespace FourTwenty.Dashboard.Interfaces.Navbars
{
    public interface ITopNavbarHelper
    {
        Task<IEnumerable<INavbarItem>> GetDropdownItems();
    }
}
