using System;
using System.Collections.Generic;
using System.Text;
using FourTwenty.Dashboard.Interfaces.Navbars;

namespace FourTwenty.Dashboard.Areas.Dashboard.Models.Navbars
{
    public class TopNavbarOptions
    {
        public TopNavbarOptions()
        {

        }

        public string Title { get; set; }
        public string TitleImageUrl { get; set; }

        public bool IsMessagesEnabled { get; set; }
        public bool IsNotificationsEnabled { get; set; }
        public bool IsRectMenuEnabled { get; set; }
        public bool IsRightSliderToogleEnabled { get; set; }
        public bool IsSearchEnabled { get; set; }
        public string LayoutName { get; set; }

        public List<INavbarItem> MenuItems { get; set; }

    }
}
