using System.Linq;
using System.Threading.Tasks;
using FourTwenty.Dashboard.Areas.Dashboard.Helpers;
using FourTwenty.Dashboard.Areas.Dashboard.Models.Navbars;
using FourTwenty.Dashboard.Interfaces.Navbars;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FourTwenty.Dashboard.Areas.Dashboard.ViewComponents
{
    public class TopNavbarViewComponent : ViewComponent
    {
        private readonly ITopNavbarHelper _navbarHelper;

        public TopNavbarViewComponent(ITopNavbarHelper topNavbarHelper)
        {
            _navbarHelper = topNavbarHelper;
        }

        public async Task<IViewComponentResult> InvokeAsync(TopNavbarOptions options)
        {
            if (options == null)
                options = new TopNavbarOptions();
            var layout = options.LayoutName ?? "_TopNavbar";
            options.MenuItems = (await _navbarHelper.GetDropdownItems())?.ToList();
            return View(layout, options);
        }
    }
}
