
using System.Threading.Tasks;
using DemoUser.Models;
using Microsoft.AspNetCore.Identity;

namespace DemoUser.Infrastructure
{
    public class CustomUserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager,
            AppUser user) {
            if (user.Email.ToLower().EndsWith("test@163.com")) {
                return Task.FromResult(IdentityResult.Success);
            } else {
                return Task.FromResult(IdentityResult.Failed(new IdentityError {
                    Code = "EmailError",
                    Description = "仅仅可以包含test@163.com后缀"
                }));
            } }

    }
}