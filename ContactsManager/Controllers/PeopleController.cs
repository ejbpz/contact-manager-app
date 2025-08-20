using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Controllers
{
    [Controller]
    [Route("/")]
    [Route("people")]
    public class PeopleController : Controller
    {
        private readonly IPeopleService _peopleService;

        public PeopleController(IPeopleService peopleService)
        {
            _peopleService = peopleService;
        }

        [HttpGet("")]
        public IActionResult Index(string searchBy, string? searchQuery)
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

            List<PersonResponse> allPeople = _peopleService.GetFilteredPeople(searchBy, searchQuery);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchQuery = searchQuery;
            return View(allPeople);
        }
    }
}
