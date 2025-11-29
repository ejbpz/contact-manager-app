using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.AlwaysRunResultFilters
{
    public class PersonAlwaysRunResultFilter : IAlwaysRunResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Filters.OfType<SkipFilters>().Any()) return;
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
