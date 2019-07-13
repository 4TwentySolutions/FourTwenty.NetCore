using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Threading.Tasks;

namespace FourTwenty.Core.Extensions
{
    public static class ControllerExtensions
    {
        public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false, HtmlHelperOptions options = null)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                if (viewEngine != null)
                {
                    ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

                    if (viewResult.Success == false)
                    {
                        return $"A view with the name {viewName} could not be found";
                    }

                    ViewContext viewContext = new ViewContext(
                        controller.ControllerContext,
                        viewResult.View,
                        controller.ViewData,
                        controller.TempData,
                        writer,
                        options ?? new HtmlHelperOptions());

                    await viewResult.View.RenderAsync(viewContext);
                }

                return writer.GetStringBuilder().ToString();
            }
        }

        public static async Task<string> RenderPartialViewAsync<TModel>(this Controller controller, string viewName, TModel model, HtmlHelperOptions options = null) => await RenderViewAsync(controller, viewName, model, true, options);
    }
}
