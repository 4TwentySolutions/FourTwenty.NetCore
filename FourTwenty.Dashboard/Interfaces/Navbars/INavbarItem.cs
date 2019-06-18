using System;
using System.Collections.Generic;
using System.Text;

namespace FourTwenty.Dashboard.Interfaces.Navbars
{
    public interface INavbarItem
    {
        string Area { get; set; }
        string DisplayName { get; set; }
        string Controller { get; set; }
        string Action { get; set; }
        Dictionary<string, string> Parameters { get; set; }
        string ImageClass { get; set; }
        bool IsSeparator { get; set; }
    }
}
