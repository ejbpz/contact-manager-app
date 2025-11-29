using Microsoft.AspNetCore.Mvc.Filters;
using ContactsManager.Controllers;
using ContactsManager.ServiceContracts.DTOs;

namespace ContactsManager.Filters.ActionFilters
{
    public class PeopleListActionFilter : IActionFilter
    {
        private readonly ILogger<PeopleListActionFilter> _logger;

        public PeopleListActionFilter(ILogger<PeopleListActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // TODO: Add before logic here.
            _logger.LogInformation("{FilterName}.{FilterMethod} method.", nameof(PeopleListActionFilter), nameof(OnActionExecuting));

            if(context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                if(!string.IsNullOrEmpty(searchBy) && !string.IsNullOrWhiteSpace(searchBy))
                {
                    List<string> searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.Address),
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.PersonEmail),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.CountryName),
                    };

                    if(!searchByOptions.Any(o => o.Equals(searchBy)))
                    {
                        _logger.LogInformation("searchBy actual value {0}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("searchBy updated value {0}", context.ActionArguments["searchBy"]);
                    }
                }
            }

            context.HttpContext.Items["arguments"] = context.ActionArguments;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // TODO: Add after logic here.
            _logger.LogInformation("{FilterName}.{FilterMethod} method.", nameof(PeopleListActionFilter), nameof(OnActionExecuted));

            PeopleController peopleController = (PeopleController)context.Controller;

            IDictionary<string, object?>? arguments = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

            if(arguments is not null)
            {
                if(arguments.ContainsKey("searchBy"))
                {
                    peopleController.ViewData["CurrentSearchBy"] = Convert.ToString(arguments["searchBy"]);
                }

                if (arguments.ContainsKey("searchQuery"))
                {
                    peopleController.ViewData["CurrentSearchQuery"] = Convert.ToString(arguments["searchQuery"]);
                }

                if (arguments.ContainsKey("sortBy"))
                {
                    peopleController.ViewData["CurrentSortBy"] = Convert.ToString(arguments["sortBy"]);
                }

                if (arguments.ContainsKey("sortOrderOptions"))
                {
                    peopleController.ViewData["CurrentSortOrder"] = Convert.ToString(arguments["sortOrderOptions"]);
                }
            }

            peopleController.ViewData["SearchFields"] = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Name" },
                { nameof(PersonResponse.PersonEmail), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryName), "Country" },
                { nameof(PersonResponse.Address), "Address" },
            };
        }
    }
}
