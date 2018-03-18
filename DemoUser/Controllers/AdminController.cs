using System.Threading.Tasks;
using DemoUser.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DemoUser.Controllers
{
    public class AdminController : Controller
    {
        private  UserManager<AppUser> _userManager { get; set; }
        private IPasswordHasher<AppUser> _passwordHasher;
        private  IUserValidator<AppUser> _userValidator { get; set; }
        private  IPasswordValidator<AppUser> _passwordValidator { get; set; }

        public AdminController(UserManager<AppUser> userManager,IPasswordHasher<AppUser> passwordHasher,
            IUserValidator<AppUser> userValidator , IPasswordValidator<AppUser> passwordValidator)
        {
            _userManager = userManager;
            this._passwordHasher = passwordHasher;
            _userValidator = userValidator;
            _passwordValidator = passwordValidator;
        }
        // GET
        public IActionResult Index()
        {
            return View(_userManager.Users);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.Name,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                   // _passwordHasher.HashPassword();
                    return RedirectToAction($"Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                }
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id) {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user != null) {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded) {
                    return RedirectToAction("Index");
                } else {
                    AddErrorsFromResult(result);
                }
            } else {
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index", _userManager.Users);
        }
        public async Task<IActionResult> Edit(string id) {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user != null) {
                return View(user);
            } else {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email,
            string password) {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user != null) {
                user.Email = email;
                IdentityResult validEmail
                    = await _userValidator.ValidateAsync(_userManager, user);
                if (!validEmail.Succeeded) {
                    AddErrorsFromResult(validEmail);
                }
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password)) {
                    validPass = await _passwordValidator.ValidateAsync(_userManager,
                        user, password);
                    if (validPass.Succeeded) {
                        user.PasswordHash = _passwordHasher.HashPassword(user,
                            password);
                    } else {
                        AddErrorsFromResult(validPass);
                    }
                }
                if ((validEmail.Succeeded && validPass == null)
                    || (validEmail.Succeeded
                        && password != string.Empty && validPass.Succeeded)) { IdentityResult result = await _userManager.UpdateAsync(user); if (result.Succeeded) {
                        return RedirectToAction("Index");
                    } else {
                        AddErrorsFromResult(result);
                    }
                }
            } else {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(user);
        }
        private void AddErrorsFromResult(IdentityResult result) {
            foreach (IdentityError error in result.Errors) {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}