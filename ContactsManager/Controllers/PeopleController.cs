using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.Filters.ActionFilters;
using ContactsManager.Filters.ResultFilters;
using ContactsManager.ServiceContracts.Enums;
using ContactsManager.Filters.ResourceFilters;
using ContactsManager.Filters.ExceptionFilters;
using ContactsManager.Filters.AuthorizationFilters;
using ContactsManager.Filters.AlwaysRunResultFilters;

namespace ContactsManager.Controllers
{
    [Controller]
    [Route("/")]
    [Route("people")]
    [TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonAlwaysRunResultFilter))]
    [ResponseHeaderFilterFactory("my-controller-key", "my-controller-value", 3)]
    public class PeopleController : Controller
    {
        private readonly IPeopleService _peopleService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PeopleController> _logger;

        public PeopleController(IPeopleService peopleService, 
                                ICountriesService countriesService,
                                ILogger<PeopleController> logger)
        {
            _peopleService = peopleService;
            _countriesService = countriesService;
            _logger = logger;
        }

        [HttpGet("")]
        [ResponseHeaderFilterFactory("my-method-key", "my-method-value", 1)]
        [ServiceFilter(typeof(PeopleListActionFilter), Order = 4)]
        [TypeFilter(typeof(PeopleListResultFilter))]
        public async Task<IActionResult> Index(string searchBy, string? searchQuery, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrderOptions = SortOrderOptions.Ascending)
        {
            _logger.LogInformation("Index action method of PeopleController.");
            _logger.LogDebug($"searchBy: {searchBy}");
            _logger.LogDebug($"searchQuery: {searchQuery}");
            _logger.LogDebug($"sortBy: {sortBy}");
            _logger.LogDebug($"sortOrderOptions: {sortOrderOptions}");

            // Searching
            List<PersonResponse>? allPeople = await _peopleService.GetFilteredPeople(searchBy, searchQuery);

            // Sorting
            allPeople = _peopleService.GetSortedPeople(allPeople ?? new List<PersonResponse>(), sortBy, sortOrderOptions);

            ViewBag.NewUser = TempData["NewUser"];
            ViewBag.ErrorDelete = TempData["ErrorDelete"];

            return View(allPeople);
        }

        [HttpGet("new-person")]
        public async Task<IActionResult> Create()
        {
            CallingGenders();
            await CallingCountries();

            return View();
        }

        //<form action = "~/people/new-person" method="post">
        [HttpPost("new-person")]
        [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter), Arguments = new object[]
            { false })]
        public async Task<IActionResult> Create(PersonAddRequest? personRequest)
        {
            await _peopleService.AddPerson(personRequest);
            TempData["NewUser"] = $"{personRequest?.PersonName ?? "New person"} has been succesfully added.";
            return RedirectToAction("Index", "People");
        }

        [HttpGet("edit-person/{personId}")]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid? personId)
        {
            CallingGenders();
            await CallingCountries();

            PersonResponse? personResponse = await _peopleService.GetPersonByPersonId(personId);

            if (personResponse is null) return RedirectToAction("Index", "People");

            return View(personResponse.ToPersonUpdateRequest());
        }

        [HttpPost("edit-person/{personId}")]
        [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            await _peopleService.UpdatePerson(personRequest);
            TempData["NewUser"] = $"{personRequest?.PersonName ?? "Person"} has been succesfully updated.";
            return RedirectToAction("Index", "People");
        }

        [HttpGet("delete-view/{personId}")]
        public async Task<IActionResult> DeleteView(Guid? personId)
        {
            PersonResponse? personResponse = await _peopleService.GetPersonByPersonId(personId);

            if (personResponse is null) return RedirectToAction("Index", "People");

            return PartialView("_Delete", personResponse);
        }

        [HttpPost("delete-person/{personId}")]
        public async Task<IActionResult> DeleteUser(Guid? personId)
        {
            PersonResponse? personResponse = await _peopleService.GetPersonByPersonId(personId);
            bool wasDeleted = await _peopleService.DeletePerson(personResponse?.PersonId);

            if(wasDeleted)
            {
                TempData["NewUser"] = $"{personResponse?.PersonName ?? "User"} was successfully deleted.";
            } 
            else
            {
                TempData["ErrorDelete"] = "Ocurrs an error while deleting.";
            }
        
            return RedirectToAction("Index", "People");
        }

        [HttpGet("people-pdf")]
        public async Task<IActionResult> PeoplePDF()
        {
            // Get People
            List<PersonResponse>? people = await _peopleService.GetPeople();

            // Return rotativa view
            return new ViewAsPdf("PeoplePDF", people, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins()
                {
                    Top = 20,
                    Bottom = 20,
                    Left = 20,
                    Right = 20,
                },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
            };
        }

        [HttpGet("people-csv")]
        public async Task<IActionResult> PeopleCSV()
        {
            MemoryStream memoryStream = await _peopleService.GetPeopleCSV();
            return File(memoryStream, "application/octet-stream", "people.csv");
        }

        [HttpGet("people-excel")]
        public async Task<IActionResult> PeopleExcel()
        {
            MemoryStream memoryStream = await _peopleService.GetPeopleExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "people.xlsx");
        }

        private void CallingGenders()
        {
            ViewData["Genders"] = new Dictionary<string, string>()
            {
                { nameof(GenderOptions.Male), "Male" },
                { nameof(GenderOptions.Female), "Female" },
                { nameof(GenderOptions.Other), "Other" },
            };
        }

        private async Task CallingCountries()
        {
            List<CountryResponse>? countries = await _countriesService.GetCountries();
            ViewData["Countries"] = countries.Select(c => new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() });
        }
    }
}
