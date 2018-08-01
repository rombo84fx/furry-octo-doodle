using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using GettingStartedWithIdentity.Infrastructure;

namespace GettingStartedWithIdentity.Controllers
{
    public class ClaimsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return View("_Error", new[] {"No claims available"});
            }
            else
            {
                return View(identity.Claims);
            }
        }

        [ClaimsAccess(Issuer = "RemoteClaims", ClaimType = ClaimTypes.PostalCode, Value = "DC 20500")]
        public string OtherAction()
        {
            return "This is the protected action";
        }
    }
}