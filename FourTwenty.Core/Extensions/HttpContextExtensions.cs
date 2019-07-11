using System;
using Microsoft.AspNetCore.Http;

namespace FourTwenty.Core.Extensions
{
    public static class HttpContextExtensions
    {
        public static (bool, string) CanonicalUrl(this HttpContext context, int localhostSslPort)
        {
            var host = context.Request.Host.Host;
            var port = host == "localhost"
                ? localhostSslPort
                : context.Request.Host.Port ?? 0;
            var portString = port == 80 || port == 443 || port == 0
                ? string.Empty
                : $":{port}";
            var pathBase = context.Request.PathBase.HasValue
                ? context.Request.PathBase.Value
                : string.Empty;

            var isCanonical = true;//!context.Request.IsHttps;

            var path = pathBase + context.Request.Path.Value;
            var pathLower = path.ToLower();
            isCanonical = path == pathLower;

            if (pathLower != "/" && pathLower.EndsWith("/"))
            {
                isCanonical = false;
                pathLower = pathLower.TrimEnd('/');
            }

            var query = context.Request.QueryString.HasValue
                ? $"{context.Request.QueryString.Value}"
                : string.Empty;

            const string httpsFormat = "{0}://{1}{2}{3}{4}";
            return (isCanonical, string.Format(httpsFormat, context.Request.Scheme, host, portString, pathLower, query));
        }

        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// 
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(this HttpContext request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.Request?.Headers != null)
                return request.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }
    }
}
