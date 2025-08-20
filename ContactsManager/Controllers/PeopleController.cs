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
        public IActionResult Index()
        {
            List<PersonResponse> allPeople = _peopleService.GetPeople();
            return View(allPeople);
        }
    }
}
