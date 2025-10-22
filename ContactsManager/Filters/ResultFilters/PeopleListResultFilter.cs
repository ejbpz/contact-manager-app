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

            await next();
        }
    }
}
