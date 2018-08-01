using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using GettingStartedWithIdentity.Models;
using Microsoft.AspNet.Identity;

namespace GettingStartedWithIdentity.Infrastructure
{
    public class CustomUserValidator : UserValidator<AppUser>
    {
        public CustomUserValidator(AppUserManager manager) : base(manager) { }

        public override async Task<IdentityResult> ValidateAsync(AppUser user)
        {
            IdentityResult result = await base.ValidateAsync(user);
            if (user.Email.ToLower().EndsWith("@example.com")) return result;
            var errors = result.Errors.ToList();
            errors.Add("Only example.com email addresses are allowed");
            return new IdentityResult(errors);
        }
    }
}