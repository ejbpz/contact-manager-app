using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace ContactsManager.Controllers
{
    [Controller]
    [AllowAnonymous]
    [Route("error")]
    public class ErrorController : Controller
    {
        [HttpGet("")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            return View(exceptionHandlerPathFeature?.Error.Message);
        }
    }
}
