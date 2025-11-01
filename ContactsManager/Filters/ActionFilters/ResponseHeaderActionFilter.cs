using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactory : Attribute, IFilterFactory
    {
        public bool IsReusable => true;

        private string Key { get; set; } = "";
        private string Value { get; set; } = "";
        private int Order { get; set; }

        public ResponseHeaderFilterFactory(string key, string value, int order)
        {
            Key = key;
            Value = value;
            Order = order;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            ResponseHeaderActionFilter? filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            filter.Key = Key;
            filter.Value = Value;
            filter.Order = Order;

            return filter;
        }
    }

    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
        public int Order { get; set; }

        private readonly ILogger<ResponseHeaderActionFilter> _logger;

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{FilterMethod} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
            context.HttpContext.Response.Headers[Key] = Value;

            await next();

            _logger.LogInformation("{FilterName}.{FilterMethod} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
        }
    }
}
