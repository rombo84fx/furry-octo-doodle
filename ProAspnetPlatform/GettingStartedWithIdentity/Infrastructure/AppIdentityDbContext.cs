using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GettingStartedWithIdentity.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace GettingStartedWithIdentity.Infrastructure
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext() : base("IdentityDb") { }

        static AppIdentityDbContext()
        {
            Database.SetInitializer(new IdentityDbInit());
        }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }

    public class IdentityDbInit : NullDatabaseInitializer<AppIdentityDbContext>
    {
    }
}