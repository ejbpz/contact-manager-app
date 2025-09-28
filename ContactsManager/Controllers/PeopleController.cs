using Microsoft.AspNetCore.Mvc;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;

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
        public async Task<IActionResult> Index(string searchBy, string? searchQuery, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrderOptions = SortOrderOptions.Ascending)
        {
            // Searching
            CreateColumns();

            List<PersonResponse> allPeople = await _peopleService.GetFilteredPeople(searchBy, searchQuery);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchQuery = searchQuery;

            // Sorting
            allPeople = _peopleService.GetSortedPeople(allPeople, sortBy, sortOrderOptions);

            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrderOptions;
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
        public async Task<IActionResult> Create(PersonAddRequest? personAddRequest)
        {
            if(!ModelState.IsValid)
            {
                CallingGenders();
                await CallingCountries();

                return View(personAddRequest);
            }
            await _peopleService.AddPerson(personAddRequest);
            TempData["NewUser"] = $"{personAddRequest?.PersonName ?? "New person"} has been succesfully added.";
            return RedirectToAction("Index", "People");
        }

        [HttpGet("edit-person/{personId}")]
        public async Task<IActionResult> Edit(Guid? personId)
        {
            CallingGenders();
            await CallingCountries();

            PersonResponse? personResponse = await _peopleService.GetPersonByPersonId(personId);

            if (personResponse is null) return RedirectToAction("Index", "People");

            return View(personResponse.ToPersonUpdateRequest());
        }

        [HttpPost("edit-person/{personId}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            if (!ModelState.IsValid)
            {
                CallingGenders();
                await CallingCountries();

                return View(personUpdateRequest);
            }
            
            await _peopleService.UpdatePerson(personUpdateRequest);
            TempData["NewUser"] = $"{personUpdateRequest?.PersonName ?? "Person"} has been succesfully updated.";
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
