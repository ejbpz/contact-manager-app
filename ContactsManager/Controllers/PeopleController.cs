using Microsoft.AspNetCore.Mvc;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            ViewBag.NewUser = TempData["NewUser"];

            return View(allPeople);
        }

        [HttpGet("new-person")]
        public IActionResult Create(string? purpose)
        {
            CallingGenders();
            CallingCountries();

            if (purpose is null) purpose = "Add";
            ViewBag.Purpose = purpose;


            return View();
        }

        //<form action = "~/people/new-person" method="post">
        [HttpPost("new-person")]
        public IActionResult Create(PersonAddRequest? personAddRequest, string? purpose)
        {
            if(!ModelState.IsValid)
            {
                CallingGenders();
                CallingCountries();

                if (purpose is null) purpose = "Add";
                ViewBag.Purpose = purpose;

                if (personAddRequest is not null) ViewData["PersonRequest"] = personAddRequest;

                return View();
            }
            _peopleService.AddPerson(personAddRequest);
            TempData["NewUser"] = $"{personAddRequest?.PersonName ?? "New person"} has been succesfully added.";
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
            ViewData["Countries"] = _countriesService.GetCountries().Select(c => new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() });
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
