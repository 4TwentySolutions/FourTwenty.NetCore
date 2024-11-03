using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FourTwenty.Core.Interfaces.Seo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FourTwenty.Core.Middleware
{
    public class RobotsMiddleware
    {
        private readonly RequestDelegate _next;


        public RobotsMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value?.Equals("/robots.txt", StringComparison.OrdinalIgnoreCase) == true)
            {
                var stream = context.Response.Body;
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/plain";
                var provider = context.RequestServices.GetService<IRobotsProvider>();
                string content = string.Empty;
                if (provider != null)
                    content = await provider.GetRobotsContent();
                using (var memoryStream = new MemoryStream())
                {
                    var bytes = Encoding.UTF8.GetBytes(content);
                    memoryStream.Write(bytes, 0, bytes.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(stream, bytes.Length);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }

    public static class RobotsMiddlewareExtensions
    {
        public static IApplicationBuilder UseRobots(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RobotsMiddleware>();
        }
    }
}
