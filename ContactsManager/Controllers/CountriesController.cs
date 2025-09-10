using ContactsManager.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Controllers
{
    [Controller]
    [Route("countries")]
    public class CountriesController : Controller
    {
        private ICountriesService _countriesService;

        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        [HttpGet("upload-excel")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("upload-excel")]
        public async Task<IActionResult> UploadExcel([FromForm]IFormFile formFile)
        {
            if(formFile is null || formFile.Length == 0)
            {
                // Error message

                return View();
            }

            if(!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                // Error message

                return View();
            }

            int rowsAdded = await _countriesService.UploadCountriesFromExcelFile(formFile);
            // Info message

            return View();
        }
    }
}
