using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ContactsManager.Controllers;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;

namespace ContactsManager.Filters.ActionFilters
{
    public class PersonCreateAndEditActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesGetterService _countriesGetterService;

        public PersonCreateAndEditActionFilter(ICountriesGetterService countriesGetterService)
        {
            _countriesGetterService = countriesGetterService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // TODO: Before logic
            if(context.Controller is PeopleController peopleController)
            {
                if (!peopleController.ModelState.IsValid)
                {
                    CallingGenders(peopleController.ViewData);
                    await CallingCountries(peopleController.ViewData);

                    PersonAddRequest? personAddRequest = ((PersonAddRequest?)context.ActionArguments["personRequest"]) ?? null;
                    context.Result = peopleController.View();
                    return;
                } 
                else
                {
                    await next();
                }
            } 
            else
            {
                await next();
            }
        }

        private void CallingGenders(ViewDataDictionary viewData)
        {
            viewData["Genders"] = new Dictionary<string, string>()
            {
                { nameof(GenderOptions.Male), "Male" },
                { nameof(GenderOptions.Female), "Female" },
                { nameof(GenderOptions.Other), "Other" },
            };
        }

        private async Task CallingCountries(ViewDataDictionary viewData)
        {
            List<CountryResponse>? countries = await _countriesGetterService.GetCountries();
            viewData["Countries"] = countries.Select(c => new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() });
        }
    }
}
