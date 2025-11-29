using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ContactsManager.Core.DTOs;
using ContactsManager.Core.Enums;
using ContactsManager.Core.Domain.IdentityEntities;

namespace ContactsManager.UI.Controllers
{
    [Controller]
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        public AccountController(UserManager<ApplicationUser> userManager,
                                 RoleManager<ApplicationRole> roleManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet("register")]
        [Authorize("NotAuthenticated")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        [Authorize("NotAuthenticated")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values
                    .SelectMany(e => e.Errors)
                    .Select(e => e.ErrorMessage);

                return View(registerRequest);
            }

            ApplicationUser applicationUser = new ApplicationUser()
            {
                Email = registerRequest.Email,
                UserName = registerRequest.Email,
                PersonName = registerRequest.PersonName,
                PhoneNumber = registerRequest.PhoneNumber,
            };

            IdentityResult identityResult = await _userManager.CreateAsync(applicationUser, registerRequest.Password.Trim());

            if(!identityResult.Succeeded)
            {
                foreach(IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return View(registerRequest);
            }

            if(registerRequest.UserType is UserTypeOptions.Admin)
            {
                if(await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                {
                    ApplicationRole applicationRole = new ApplicationRole()
                    {
                        Name = UserTypeOptions.Admin.ToString(),
                    };

                    await _roleManager.CreateAsync(applicationRole);
                }

                await _userManager.AddToRoleAsync(applicationUser, UserTypeOptions.Admin.ToString());
            } else
            {
                if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                {
                    ApplicationRole applicationRole = new ApplicationRole()
                    {
                        Name = UserTypeOptions.User.ToString(),
                    };

                    await _roleManager.CreateAsync(applicationRole);
                }

                await _userManager.AddToRoleAsync(applicationUser, UserTypeOptions.User.ToString());
            }

            await _signInManager.SignInAsync(applicationUser, false);
            return RedirectToAction("Index", "People");
        }

        [HttpGet("login")]
        [Authorize("NotAuthenticated")]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost("login")]
        [Authorize("NotAuthenticated")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values
                    .SelectMany(e => e.Errors)
                    .Select(e => e.ErrorMessage);
                return View(loginRequest);
            }

            var signInResult = await _signInManager.PasswordSignInAsync(loginRequest?.Email ?? "", loginRequest?.Password ?? "", false, false);

            if(!signInResult.Succeeded)
            {
                ModelState.AddModelError("Login", "Email or password is incorrect");
                return View(loginRequest);
            }

            ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(loginRequest?.Email ?? "");

            if(applicationUser is not null)
            {
                if(await _userManager.IsInRoleAsync(applicationUser, UserTypeOptions.Admin.ToString()))
                {
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
            }

            return RedirectToAction("Index", "People");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "People");
        }


        [AllowAnonymous]
        public async Task<IActionResult> IsEmailRegistered(string email)
        {
            ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(email);

            if(applicationUser is null)
                return Json(true);

            return Json(false);
        }
    }
}
