using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ExceptionFilters
{
    public class HandleExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HandleExceptionFilter> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError("Exception filter {FilterName}.{MethodName}\n\t{ExceptionType}\n\t{ExceptionMessage}", nameof(HandleExceptionFilter), nameof(OnException), context.Exception.GetType().ToString(), context.Exception.Message);

            if(!_hostEnvironment.IsProduction())
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 500,
                    Content = context.Exception.Message,
                };
            }
        }
    }
}
