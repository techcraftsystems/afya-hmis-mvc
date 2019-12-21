using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models;
using AfyaHMIS.Service;
using AfyaHMIS.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace AfyaHMIS.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService IService;

        [BindProperty]
        public LoginModel Input { get; set; }

        public AccountController(IUserService service)
        {
            IService = service;
        }

        public async Task<IActionResult> Login(LoginModel model, string ReturnUrl = "/")
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            model.ReturnUrl = ReturnUrl;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, CrytoUtilsExtensions Cryto)
        {
            if (ModelState.IsValid)
            {
                Users user = IService.GetUser(Input.User.Username); //AuthenticateUser(Input.Email, Input.Password);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    model.Message = "Invalid login attempt.";
                    return View(model);
                }

                if (!user.Enabled)
                {
                    ModelState.AddModelError(string.Empty, "Login account Disabled.");
                    model.Message = "Login account Disabled.";
                    return View(model);
                }

                if (!Cryto.Decrypt(user.Password).Equals(Input.User.Password))
                {
                    ModelState.AddModelError(string.Empty, "Login Failed. Invalid password.");
                    model.Message = "Login Failed. Invalid password.";
                    return View(model);
                }

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Sid, user.Uuid),
                    new Claim(ClaimTypes.UserData, user.Username),
                    new Claim(ClaimTypes.Actor, user.Id.ToString())
                };

                if (string.IsNullOrEmpty(model.Password))
                {
                    if (user.ToChange)
                    {
                        model.ToChange = 1;
                        return View(model);
                    }
                }
                else
                {
                    user.Password = Cryto.Encrypt(model.Password);
                    user.UpdatePassword();
                }

                user.UpdateLastAccess();

                foreach (var roles in user.GetRoles())
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.Role.Name));
                }

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(180),
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (!string.IsNullOrEmpty(Input.ReturnUrl.Trim()))
                    return LocalRedirect(Input.ReturnUrl.Trim());
                return LocalRedirect("/");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied(string ReturnUrl = "")
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }
    }
}
