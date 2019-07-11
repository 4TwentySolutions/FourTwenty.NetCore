using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FourTwenty.Core.Middleware
{
    public class RobotsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _baseUrl;
        private readonly string[] _lines;



        public RobotsMiddleware(RequestDelegate next, string[] lines, string baseUrl = null)
        {
            _next = next;
            _baseUrl = baseUrl;
            _lines = lines;
        }


        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Equals("/robots.txt", StringComparison.OrdinalIgnoreCase))
            {
                var stream = context.Response.Body;
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/plain";

                StringBuilder stringBuilder = new StringBuilder();
                if(_lines!=null)
                    foreach (string line in _lines)
                    {
                        stringBuilder.AppendLine(line);
                    }

                stringBuilder.Append("sitemap: ");

                stringBuilder.AppendLine(!string.IsNullOrEmpty(_baseUrl)
                    ? $"{_baseUrl}/sitemap.xml"
                    : $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}/sitemap.xml");


                using (var memoryStream = new MemoryStream())
                {
                    var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
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
        public static IApplicationBuilder UseRobots(
            this IApplicationBuilder builder, string baseUrl)
        {
            return builder.UseMiddleware<RobotsMiddleware>(baseUrl);
        }

        public static IApplicationBuilder UseRobots(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RobotsMiddleware>();
        }
    }
}
