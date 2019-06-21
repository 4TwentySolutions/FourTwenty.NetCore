using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FourTwenty.Dashboard.Areas.Dashboard.Helpers.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace FourTwenty.Dashboard.Areas.Dashboard.TagHelpers
{
    public class TableChildContext
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string UniqueName { get; set; }
        public string Area { get; set; }

    }
    public class TableHeaderTagHelper : TagHelper
    {
        public static Dictionary<string, string> UniqueNames { get; set; } = new Dictionary<string, string>();
        //public static string UniqueName { get; set; }
        private string UniqueName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
        public string ElementToUpdateId { get; set; }
        public string SuccessCallback { get; set; }

        public IDictionary<string, string> Parameters { get; set; }

        #region fields

        private readonly IActionContextAccessor _contextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private IUrlHelper UrlHelper =>
            this._urlHelperFactory.GetUrlHelper(this._contextAccessor.ActionContext);


        #endregion

        public TableHeaderTagHelper(IActionContextAccessor contextAccessor, IUrlHelperFactory urlHelperFactory)
        {
            _contextAccessor = contextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "thead";
            var key = $"{Action}_{Controller}_{Area}";
            if (!UniqueNames.ContainsKey(key))
                UniqueNames.Add(key, Guid.NewGuid().ToString("N"));

            UniqueName = UniqueNames[key];
            TableChildContext parentChildContext = new TableChildContext()
            {
                Action = Action,
                UniqueName = UniqueName,
                Controller = Controller,
                Area = Area,
            };
            context.Items.Add(typeof(TableChildContext), parentChildContext);


            var childs = await output.GetChildContentAsync();
            output.Content.SetHtmlContent(childs);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<script>");
            builder.AppendLine($"function sortBy{UniqueName}(elem ,field, controller, action)");

            var url = UrlHelper.Action(Action, Controller, new { area = Area });
            builder.AppendLine("{$.ajax({type :\"GET\", url:");
            builder.AppendLine($"\"{url}\",");
            builder.AppendLine("dataType : \"html\",");
            if (Parameters == null)
            {
                builder.AppendLine("data :{sidx: field,customFilter:getAllTableFilters($(elem).closest(\"thead\")},");
            }
            else
            {
                builder.AppendLine("data:{");
                builder.AppendLine("sidx: field");

                foreach (var parameter in Parameters)
                {

                    if (parameter.Key == "sidx")
                    {
                        continue;
                    }

                    builder.AppendLine(",");
                    if (parameter.Key == "sord")
                    {
                        builder.AppendLine(parameter.Value.Contains("asc") ? $"{parameter.Key}:\"desc\"" : $"{parameter.Key}:\"asc\"");
                    }
                    else
                    {
                        builder.AppendLine($"{parameter.Key}:{parameter.Value}");
                    }

                }

                builder.AppendLine($",customFilter:JSON.stringify(getAllTableFilters($(elem).closest(\"thead\")))");
                builder.AppendLine("},");
            }
            builder.AppendLine("success: function(answer){");
            if (!string.IsNullOrEmpty(ElementToUpdateId))
                builder.AppendLine($"$('#{ElementToUpdateId}').html(answer);");
            if (!string.IsNullOrEmpty(SuccessCallback))
                builder.AppendLine($"{SuccessCallback}();" + "}");
            else
                builder.Append('}');

            builder.AppendLine("});}");
            builder.AppendLine("function applyFilters" + UniqueName + "(rootElement) {");
            builder.AppendLine("var area = $(rootElement).closest(\"th\").data(\"area\");");
            builder.AppendLine("var controller = $(rootElement).closest(\"th\").data(\"controller\");");
            builder.AppendLine("var action = $(rootElement).closest(\"th\").data(\"action\");");
            builder.AppendLine("var jsObj = getAllTableFilters($(rootElement).closest(\"thead\"));");
            builder.AppendLine("$.ajax({");
            builder.AppendLine("type: \"GET\",");


            builder.AppendLine($"url: \"{url}\",");
            builder.AppendLine();
            builder.AppendLine("dataType: \"html\",");
            builder.AppendLine("data: {");
            builder.AppendLine("customFilter:JSON.stringify(jsObj)");
            if (Parameters == null)
            {
                builder.AppendLine("},");
            }
            else
            {
                builder.AppendLine(",");
                int i = 0;
                foreach (var parameter in Parameters)
                {
                    if (i != 0)
                        builder.AppendLine(",");
                    builder.AppendLine($"{parameter.Key}:{parameter.Value}");
                    i++;
                }
                builder.AppendLine("},");
            }
            builder.AppendLine("success: function(answer){");
            if (!string.IsNullOrEmpty(ElementToUpdateId))
                builder.AppendLine($"$('#{ElementToUpdateId}').html(answer);");
            if (!string.IsNullOrEmpty(SuccessCallback))
                builder.AppendLine($"{SuccessCallback}();" + "}");
            else
                builder.Append('}');
            builder.AppendLine("});}</script>");


            output.PostContent.SetHtmlContent(builder.ToString());
        }

    }


    [HtmlTargetElement("table-column", ParentTag = "table-header")]
    public class TableColumnTagHelper : TagHelper
    {
        public ITableFilterData FilterData { get; set; }
        public string DisplayName { get; set; }
        public string ColumnName { get; set; }
        public bool IsSortable { get; set; } = true;

        public bool IsColumnCheckbox { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "th";
            output.Content.SetContent(DisplayName);
            output.TagMode = TagMode.StartTagAndEndTag;
            var dataContext = context.Items[typeof(TableChildContext)] as TableChildContext;
            output.Attributes.Add("data-uniqename", dataContext?.UniqueName);
            if (!IsColumnCheckbox)
            {
                if (FilterData != null)
                {
                    ProcessFilter(output, dataContext);
                }
                else
                {
                    TableChildContext tableHeader = context.Items[typeof(TableChildContext)] as TableChildContext;
                    output.Content.SetHtmlContent(
                        IsSortable
                            ? $"<span class=\"filtarable\" onclick=\"sortBy{tableHeader?.UniqueName}(this,'{ColumnName}', '{tableHeader?.Area}','{tableHeader?.Controller}', '{tableHeader?.Action}')\">{DisplayName}</span>"
                            : $"<span>{DisplayName}</span>");
                }
            }
            else
            {
                output.Content.SetHtmlContent("<div class=\"\"><input type=\"checkbox\" class=\"\" onchange=\"checkAll(this)\"></div>");
            }

        }

        private void ProcessFilter(TagHelperOutput output, TableChildContext tableHeader = null)
        {
            if (FilterData is CheckBoxTableFilterData chekbox && (chekbox.Items?.Any() ?? false))
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("<div class=\"dropdown\">");
                builder.AppendLine($"<span class=\"filtarable\" onclick=\"sortBy{tableHeader?.UniqueName}(this,'{ColumnName}', '{tableHeader?.Area}', '{tableHeader?.Controller}', '{tableHeader?.Action}')\">{DisplayName}</span>");
                builder.AppendLine(
                    "<i class=\"fa fa-filter pull-right dropdown-toggle\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"></i>");
                builder.AppendLine(
                    $"<div class=\"dropdown-menu filter-dropdown multiselect dropdown-menu-right\" data-property=\"{chekbox.PropertyName}\" >");
                if (chekbox.IsSelectAllEnabled)
                {
                    builder.AppendLine("<div class=\"dropdown-item select-all\">");
                    builder.AppendLine("<label class=\"checkbox\">");
                    string isChecked = chekbox.Items.All(x => x.IsChecked) ? "checked=\"checked\"" : "";
                    builder.AppendLine($"<input type=\"checkbox\" class=\"first\" value=\"all\" {isChecked}> Select all");
                    builder.AppendLine("</label>");
                    builder.AppendLine("</div>");
                }

                foreach (CheckBoxTableFilterData.CheckBoxListItem item in chekbox.Items)
                {
                    builder.AppendLine("<div class=\"dropdown-item\">");
                    builder.AppendLine("<label class=\"checkbox\">");
                    string isChecked = item.IsChecked ? "checked=\"checked\"" : "";
                    builder.AppendLine($"<input type=\"checkbox\" value=\"{item.Value}\" {isChecked}> {item.DisplayName}");
                    builder.AppendLine("</label>");
                    builder.AppendLine("</div>");
                }

                output.Content.SetHtmlContent(builder.ToString());
            }
        }
    }
}
