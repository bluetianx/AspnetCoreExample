using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoUser.Models;
using Microsoft.AspNetCore.Identity;

namespace DemoUser.Infrastructure
{
    public class CustomPasswordValidator : PasswordValidator<AppUser>
    {
        public override async Task<IdentityResult> ValidateAsync(
            UserManager<AppUser> manager, AppUser user, string password)
        {
            IdentityResult result = await base.ValidateAsync(manager,
                user, password);
            List<IdentityError> errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

            if (password.Contains("china"))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordContainsillegalletter",
                    Description = "Password cannot contain china"
                });
            }

            return errors.Count == 0
                ? IdentityResult.Success
                : IdentityResult.Failed(errors.ToArray());
        }
    }
}

