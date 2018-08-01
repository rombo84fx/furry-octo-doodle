using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [Authorize(Roles = "Administrators")]
    public class RoleAdminController : Controller
    {
        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
        private AppRoleManager RoleManager => HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();

        public ActionResult Index()
        {
            return View(RoleManager.Roles);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result =
                    await RoleManager.CreateAsync(new AppRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                AddErrorsFromResult(result);
            }

            return View(name);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            AppRole role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                return View("_Error", result.Errors);
            }

            return View("_Error", new[] { "Role Not Found" });
        }

        public async Task<ActionResult> Edit(string id)
        {
            AppRole role = await RoleManager.FindByIdAsync(id);
            string[] memberIds = role.Users.Select(u => u.UserId).ToArray();
            IEnumerable<AppUser> members =
                UserManager.Users.Where(u => memberIds.Contains(u.Id));
            IEnumerable<AppUser> nonMembers = UserManager.Users.Except(members);
            return View(new RoleEditModel
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(RoleModificationModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result;
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    result = await UserManager.AddToRoleAsync(userId, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("_Error", result.Errors);
                    }
                }

                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    result = await UserManager.RemoveFromRoleAsync(userId, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("_Error", result.Errors);
                    }
                }

                return RedirectToAction("Index");
            }

            return View("_Error", new[] {"Role Not Found"});
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}