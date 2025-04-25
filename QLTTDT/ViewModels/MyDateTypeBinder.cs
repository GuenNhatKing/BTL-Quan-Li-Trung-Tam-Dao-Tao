using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace QLTTDT.ViewModels
{
    public class MyDateTypeBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // bindingContext -> name, type, data provider, model state, result...
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if(valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            var value = valueProviderResult.FirstValue;
            if(string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }
            try
            {
                var result = new MyDateType(value);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (Exception)
            {
                
            }
            return Task.CompletedTask;
        }
    }
}
