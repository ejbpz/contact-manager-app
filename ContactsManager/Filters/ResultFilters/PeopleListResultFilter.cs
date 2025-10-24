using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ResultFilters
{
    public class PeopleListResultFilter : IAsyncResultFilter
    {
        private readonly ILogger<PeopleListResultFilter> _logger;

        public PeopleListResultFilter(ILogger<PeopleListResultFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            // Before logic
            _logger.LogInformation("{FilterName}.{MethodName} - before", nameof(PeopleListResultFilter), nameof(OnResultExecutionAsync));

            context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("F");

            await next();

            // After logic
            _logger.LogInformation("{FilterName}.{MethodName} - after", nameof(PeopleListResultFilter), nameof(OnResultExecutionAsync));
        }
    }
}
