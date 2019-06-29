using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;

namespace FourTwenty.Core.Binders
{
    public class InvariantDateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType && (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?)))
            {

                return new InvariantDateTimeModelBinder(context.Metadata.ModelType, context.Services.GetService(typeof(ILoggerFactory)) as ILoggerFactory);
            }

            return null;
        }
    }


    public class InvariantDateTimeModelBinder : IModelBinder
    {
        private readonly SimpleTypeModelBinder _baseBinder;

        public InvariantDateTimeModelBinder(Type modelType, ILoggerFactory loggerFactory)
        {
            _baseBinder = new SimpleTypeModelBinder(modelType, loggerFactory);
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);


                // Use invariant culture
                if (DateTime.TryParse(valueProviderResult.FirstValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    bindingContext.Result = ModelBindingResult.Success(date);
                    return Task.CompletedTask;
                }

                if (DateTime.TryParseExact(valueProviderResult.FirstValue, new[] { "dd.MM.yyyy","dd-MM-yyyy", "dd/MM/yyyy" },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime secondDate))
                {
                    bindingContext.Result = ModelBindingResult.Success(secondDate);
                    return Task.CompletedTask;
                }
            }

            // If we haven't handled it, then we'll let the base SimpleTypeModelBinder handle it
            return _baseBinder.BindModelAsync(bindingContext);
        }
    }
}
