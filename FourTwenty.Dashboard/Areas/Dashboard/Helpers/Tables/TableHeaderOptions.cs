using System;
using System.Collections.Generic;

namespace FourTwenty.Dashboard.Areas.Dashboard.Helpers.Tables
{
    public class TableHeaderOptions
    {
        private string _uniqeName;

        public TableHeaderOptions(string controller, string action, JsTableOptions jsOptions, params string[] columns)
        {
            Controller = controller;
            Action = action;
            JsOptions = jsOptions;
            foreach (var column in columns)
            {
                Columns.Add(new TableHeaderItem(column));
            }
        }

        public TableHeaderOptions(string controller, string action, JsTableOptions jsOptions, params TableHeaderItem[] columns)
        {
            Controller = controller;
            Action = action;
            JsOptions = jsOptions;
            Columns.AddRange(columns);
        }

        public JsTableOptions JsOptions { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public List<TableHeaderItem> Columns { get; set; } = new List<TableHeaderItem>();

        public string UniqueName => !string.IsNullOrEmpty(_uniqeName) ? _uniqeName : _uniqeName = Guid.NewGuid().ToString("N");
    }


    public class JsTableOptions
    {
        public JsTableOptions()
        {

        }

        public JsTableOptions(string callback, string id, Dictionary<string, string> param)
        {
            SuccessCallback = callback;
            ReplaceElementId = id;
            Parameters = param;
        }

        public string SuccessCallback { get; set; }
        public string ReplaceElementId { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }

    public class TableHeaderItem
    {
        public TableHeaderItem()
        {

        }

        public TableHeaderItem(string name, bool isSortable = true, ITableFilterData filterData = null)
        {
            ColumnName = name;
            DisplayName = name;
            IsSortable = isSortable;
            FilterData = filterData;
        }

        public TableHeaderItem(string columnsName, string displayName, bool isSortable = true, ITableFilterData filterData = null)
        {
            ColumnName = columnsName;
            DisplayName = displayName;
            IsSortable = isSortable;
            FilterData = filterData;
        }

        public string ColumnName { get; set; }
        public string DisplayName { get; set; }
        public bool IsSortable { get; set; }

        public ITableFilterData FilterData { get; set; }


    }
}
