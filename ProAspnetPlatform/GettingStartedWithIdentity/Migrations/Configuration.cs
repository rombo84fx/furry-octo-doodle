using GettingStartedWithIdentity.Infrastructure;
using GettingStartedWithIdentity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GettingStartedWithIdentity.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<GettingStartedWithIdentity.Infrastructure.AppIdentityDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "GettingStartedWithIdentity.Infrastructure.AppIdentityDbContext";
        }

        protected override void Seed(GettingStartedWithIdentity.Infrastructure.AppIdentityDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        private void PerformInitialSetup(AppIdentityDbContext context)
        {
            AppUserManager userManager = new AppUserManager(new UserStore<AppUser>(context));
            AppRoleManager roleManager = new AppRoleManager(new RoleStore<AppRole>(context));
            string adminRole = "Administrators";
            string userRole = "Users";
            string employeeRole = "Employees";
            string userName = "Admin";
            string password = "Secret";
            string email = "admin@example.com";

            if (!roleManager.RoleExists(adminRole))
            {
                roleManager.Create(new AppRole(adminRole));
            }

            if (!roleManager.RoleExists(userRole))
            {
                roleManager.Create(new AppRole(userRole));
            }

            if (!roleManager.RoleExists(employeeRole))
            {
                roleManager.Create(new AppRole(employeeRole));
            }

            AppUser user = userManager.FindByName(userName);
            if (user == null)
            {
                userManager.Create(new AppUser
                {
                    UserName = userName,
                    Email = email
                }, password);
                user = userManager.FindByName(userName);
            }

            if (!userManager.IsInRole(user.Id, adminRole))
            {
                userManager.AddToRole(user.Id, adminRole);
            }

            foreach (AppUser appUser in userManager.Users)
            {
                if (appUser.Country == Countries.None)
                {
                    appUser.SetCountryFromCity(appUser.City);                    
                }
            }
        }
    }
}
