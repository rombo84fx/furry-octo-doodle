using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GettingStartedWithIdentity.Infrastructure;
using GettingStartedWithIdentity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace GettingStartedWithIdentity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IAuthenticationManager AuthManager => HttpContext.GetOwinContext().Authentication;
        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return View("_Error", new[] { "Access Denied" });
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await UserManager.FindAsync(details.Name, details.Password);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid name or password.");
                }
                else
                {
                    ClaimsIdentity identity =
                        await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    identity.AddClaims(LocationClaimsProvider.GetClaims(identity));
                    identity.AddClaims(ClaimsRoles.CreateRolesFromClaims(identity));
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, identity);
                    return RedirectToLocal(returnUrl);
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(details);
        }

        [Authorize]
        public ActionResult Logout()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GoogleLogin(string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleLoginCallback",
                    new { returnUrl })
            };
            HttpContext.GetOwinContext().Authentication.Challenge(properties, "Google");
            return new HttpUnauthorizedResult();
        }

        [AllowAnonymous]
        public async Task<ActionResult> GoogleLoginCallback(string returnUrl)
        {
            ExternalLoginInfo loginInfo = await AuthManager.GetExternalLoginInfoAsync();
            AppUser user = await UserManager.FindAsync(loginInfo.Login) ?? new AppUser
            {
                Email = loginInfo.Email,
                UserName = loginInfo.Email,
                City = Cities.London,
                Country = Countries.Uk
            };
            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return View("_Error", result.Errors);
            }

            result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
            if (!result.Succeeded)
            {
                return View("_Error", result.Errors);
            }

            ClaimsIdentity identity = await UserManager
                .CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddClaims(loginInfo.ExternalIdentity.Claims);
            AuthManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
            }, identity);
            return Redirect(returnUrl ?? "/");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}