using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GettingStartedWithIdentity.Infrastructure;
using GettingStartedWithIdentity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace GettingStartedWithIdentity.Controllers
{
    public class HomeController : Controller
    {
        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
        private AppUser CurrentUser => UserManager.FindByName(HttpContext.User.Identity.Name);

        [Authorize]
        public ActionResult Index()
        {
            return View(GetData("Index"));
        }

        [Authorize(Roles = "Users")]
        public ActionResult OtherAction()
        {
            return View("Index", GetData("OtherAction"));
        }

        [Authorize]
        public ActionResult UserProps()
        {
            return View(CurrentUser);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UserProps(Cities city)
        {
            AppUser user = CurrentUser;
            user.City = city;
            user.SetCountryFromCity(city);
            await UserManager.UpdateAsync(user);
            return View(user);
        }

        private Dictionary<string, object> GetData(string actionName)
        {
            return new Dictionary<string, object>
            {
                {"Action", actionName},
                {"User", HttpContext.User.Identity.Name},
                {"Authenticated", HttpContext.User.Identity.IsAuthenticated},
                {"Auth Type", HttpContext.User.Identity.AuthenticationType},
                {"In Users Role", HttpContext.User.IsInRole("Users")}
            };
        }
    }
}