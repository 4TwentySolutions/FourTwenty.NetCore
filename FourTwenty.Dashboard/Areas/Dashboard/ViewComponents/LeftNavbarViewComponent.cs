using System.Linq;
using System.Threading.Tasks;
using FourTwenty.Dashboard.Areas.Dashboard.Models.Navbars;
using FourTwenty.Dashboard.Interfaces.Navbars;
using Microsoft.AspNetCore.Mvc;

namespace FourTwenty.Dashboard.Areas.Dashboard.ViewComponents
{
    public class LeftNavbarViewComponent : ViewComponent
    {

        private readonly ISidebarHelper _navbarHelper;
        public LeftNavbarViewComponent(ISidebarHelper leftNavbarHelper)
        {
            _navbarHelper = leftNavbarHelper;
        }


        public async Task<IViewComponentResult> InvokeAsync(string viewName = null)
        {
            var view = !string.IsNullOrEmpty(viewName) ? viewName : "_LeftNavbar";

            string controller = ViewContext.RouteData.Values["Controller"]?.ToString();
            string action = ViewContext.RouteData.Values["Action"]?.ToString();

            var options = new LeftNavbarOptions
            {
                LeftNavbarItems = (await _navbarHelper.ItemsPerUser(controller, action, User.Identity.Name ?? string.Empty)).Cast<LeftNavbarItem>(),
                ProfileDropdownItems = (await _navbarHelper.ProfileItemsPerUser(User.Identity.Name ?? string.Empty)).Cast<LeftNavbarItem>()
            };
            return View(view, options);
        }

    }
}
