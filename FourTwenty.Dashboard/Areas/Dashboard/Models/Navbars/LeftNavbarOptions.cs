﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FourTwenty.Dashboard.Areas.Dashboard.Models.Navbars
{
    public class LeftNavbarOptions
    {
        public IEnumerable<LeftNavbarItem> LeftNavbarItems { get; set; }
        public IEnumerable<LeftNavbarItem> ProfileDropdownItems { get; set; }
    }

}
