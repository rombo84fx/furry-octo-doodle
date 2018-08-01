using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;

namespace GettingStartedWithIdentity.Infrastructure
{
    public class CustomPasswordValidator : PasswordValidator
    {
        public override async Task<IdentityResult> ValidateAsync(string password)
        {
            IdentityResult result = await base.ValidateAsync(password);
            if (!password.Contains("12345")) return result;
            var errors = result.Errors.ToList();
            errors.Add("Passwords cannot contain numeric sequences");
            return new IdentityResult(errors);
        }
    }
}