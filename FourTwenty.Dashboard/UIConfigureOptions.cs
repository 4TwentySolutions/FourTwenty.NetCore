using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace FourTwenty.Dashboard
{
    public class UiConfigureOptions : IPostConfigureOptions<StaticFileOptions>
    {
        private readonly IHostingEnvironment _environment;
        public UiConfigureOptions(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public void PostConfigure(string name, StaticFileOptions options)
        {
            // Basic initialization in case the options weren't initialized by any other component
            options.ContentTypeProvider ??= new FileExtensionContentTypeProvider();

            if (options.FileProvider == null && _environment.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }

            options.FileProvider ??= _environment.WebRootFileProvider;


            // When deploying use the files that are embedded in the assembly.
            options.FileProvider = new CompositeFileProvider(options.FileProvider,
                new ManifestEmbeddedFileProvider(GetType().Assembly, "wwwroot"));


            _environment.WebRootFileProvider = options.FileProvider; // required to make asp-append-version work as it uses the WebRootFileProvider. https://github.com/aspnet/Mvc/issues/7459
        }
    }
    public class ViewConfigureOptions : IPostConfigureOptions<RazorViewEngineOptions>
    {
        private readonly IHostingEnvironment _environment;

        public ViewConfigureOptions(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public void PostConfigure(string name, RazorViewEngineOptions options)
        {
            
        }
    }
}
