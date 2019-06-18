using System.Collections.Generic;
using System.Linq;
using FourTwenty.Dashboard.Interfaces.Navbars;

namespace FourTwenty.Dashboard.Areas.Dashboard.Models.Navbars
{

    public class BaseNavbarItem : INavbarItem
    {
        public string Area { get; set; }
        public string DisplayName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public Dictionary<string,string> Parameters { get; set; }
        public string ImageClass { get; set; }
        public bool IsSeparator { get; set; }
    }

    public class LeftNavbarItem : BaseNavbarItem
    {
        public string ActiveCss { get; set; }
        public string[] Roles { get; set; }

        public bool IsParent => Children?.Any() ?? false;

        public IList<LeftNavbarItem> Children { get; set; }
    }
}
