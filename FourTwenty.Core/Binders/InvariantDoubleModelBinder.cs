using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;

namespace FourTwenty.Core.Binders
{
    public class InvariantDoubleModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType && (context.Metadata.ModelType == typeof(double) || context.Metadata.ModelType == typeof(double?)))
            {
                return new InvariantDoubleModelBinder(context.Metadata.ModelType, context.Services.GetService(typeof(ILoggerFactory)) as ILoggerFactory);
            }

            return null;
        }
    }


    public class InvariantDoubleModelBinder : IModelBinder
    {
        private readonly SimpleTypeModelBinder _baseBinder;


        public InvariantDoubleModelBinder(Type modelType, ILoggerFactory loggerFactory)
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

                var valueAsString = valueProviderResult.FirstValue;
                string wantedSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                string alternateSeparator = (wantedSeparator == "," ? "." : ",");
                if (valueAsString.IndexOf(wantedSeparator, StringComparison.Ordinal) == -1
                    && valueAsString.IndexOf(alternateSeparator, StringComparison.Ordinal) != -1)
                {
                    valueAsString =
                        valueAsString.Replace(alternateSeparator, wantedSeparator);
                }
                // Use invariant culture
                if (double.TryParse(valueAsString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture, out double result))
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                    return Task.CompletedTask;
                }
            }

            // If we haven't handled it, then we'll let the base SimpleTypeModelBinder handle it
            return _baseBinder.BindModelAsync(bindingContext);
        }
    }
}
