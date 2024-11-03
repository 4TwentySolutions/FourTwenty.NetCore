using System;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace FourTwenty.Core.Middleware
{
    public class CanonicalUrlMiddleware
    {

        private readonly int _localhostSslPort;
        private readonly string[]? _ignorePaths;
        private readonly string[]? _skipExtensions;
        private readonly RequestDelegate _next;

        public CanonicalUrlMiddleware(RequestDelegate next, int localhostSslPort, string[]? ignore = null, string[]? skipExtensions = null)
        {
            _next = next;

            _localhostSslPort = localhostSslPort;
            _ignorePaths = ignore;
            _skipExtensions = skipExtensions;
        }

        public async Task Invoke(HttpContext context)
        {
            var displayUrl = context.Request.Path.HasValue ? context.Request.Path.Value : context.Request.PathBase.HasValue ? context.Request.PathBase.Value : null;
            if (!context.IsAjaxRequest() && !(displayUrl != null && _skipExtensions != null && _skipExtensions.Any(x => displayUrl.EndsWith(x))))
            {
                if (_ignorePaths != null && _ignorePaths.Any())
                {
                    if (_ignorePaths.Any(x => context.Request.Path.StartsWithSegments(x)))
                    {
                        await _next(context);
                        return;
                    }
                }

                var (isCanonical, url) = context.CanonicalUrl(_localhostSslPort);
                if (isCanonical == false)
                {
                    context.Response.Redirect(new Uri(url).AbsoluteUri, true);
                }


            }

            await _next(context);
        }
    }


}
