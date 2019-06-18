using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FourTwenty.Dashboard.Areas.Dashboard.Models.Navbars;

namespace FourTwenty.Dashboard.Interfaces.Navbars
{
    public interface ISidebarHelper
    {
        Task<IEnumerable<INavbarItem>> ProfileItemsPerUser(string userName);
        Task<IEnumerable<INavbarItem>> ItemsPerUser(string controller, string action, string userName);
    }
}
