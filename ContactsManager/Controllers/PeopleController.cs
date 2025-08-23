using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
            CreateColumns();

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
            CallingGenders();
            CallingCountries();

            return View();
        }

        //<form action = "~/people/new-person" method="post">
        [HttpPost("new-person")]
        public IActionResult Create(PersonAddRequest? personAddRequest)
        {
            if(!ModelState.IsValid)
            {
                CallingGenders();
                CallingCountries();

                ViewBag.Errors = ModelState.Values.SelectMany(p => p.Errors).Select(e => e.ErrorMessage).ToList();

                return View();
            }
            _peopleService.AddPerson(personAddRequest);
            return RedirectToAction("Index", "People");
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

        private void CallingCountries()
        {
            ViewData["Countries"] = _countriesService.GetCountries();
        }
    
        private void CreateColumns()
        {
            ViewBag.SearchFields = new Dictionary<string, string>()
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
