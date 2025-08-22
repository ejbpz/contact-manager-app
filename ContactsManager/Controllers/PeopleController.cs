using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Controllers
{
    [Controller]
    [Route("/")]
    [Route("people")]
    public class PeopleController : Controller
    {
        private readonly IPeopleService _peopleService;
        private readonly ICountriesService _countriesService;

        public PeopleController(IPeopleService peopleService, ICountriesService countriesService)
        {
            _peopleService = peopleService;
            _countriesService = countriesService;
        }

        [HttpGet("")]
        public IActionResult Index(string searchBy, string? searchQuery, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrderOptions = SortOrderOptions.Ascending)
        {
            // Searching
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Name" },
                { nameof(PersonResponse.PersonEmail), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryName), "Country" },
                { nameof(PersonResponse.Address), "Address" },
            };

            List<PersonResponse> allPeople = _peopleService.GetFilteredPeople(searchBy, searchQuery);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchQuery = searchQuery;

            // Sorting
            allPeople = _peopleService.GetSortedPeople(allPeople, sortBy, sortOrderOptions);

            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrderOptions;

            return View(allPeople);
        }

        [HttpGet("new-person")]
        public IActionResult Create()
        {
            ViewBag.Genders = new Dictionary<string, string>()
            {
                { nameof(GenderOptions.Male), "Male" },
                { nameof(GenderOptions.Female), "Female" },
                { nameof(GenderOptions.Other), "Other" },
            };
            ViewBag.Countries = _countriesService.GetCountries();

            return View();
        }
    }
}
