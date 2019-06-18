using System.Collections.Generic;

namespace FourTwenty.Dashboard.Areas.Dashboard.Helpers.Tables
{
    public interface ITableFilterData
    {
        string PropertyName { get; set; }
    }

    public class CheckBoxTableFilterData : ITableFilterData
    {



        public bool IsSelectAllEnabled { get; set; } = true;
        public IEnumerable<CheckBoxListItem> Items { get; set; }

        public string PropertyName { get; set; }

        public class CheckBoxListItem
        {
            public string DisplayName { get; set; }
            public string Value { get; set; }
            public bool IsChecked { get; set; }
        }

    }
}
