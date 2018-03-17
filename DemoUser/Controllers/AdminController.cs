using System.Threading.Tasks;
using DemoUser.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace DemoUser.Controllers
{
    public class AdminController : Controller
    {
        private  UserManager<AppUser> _userManager { get; set; }
        private IPasswordHasher<AppUser> _passwordHasher;

        public AdminController(UserManager<AppUser> userManager,IPasswordHasher<AppUser> passwordHasher)
        {
            _userManager = userManager;
            this._passwordHasher = passwordHasher;
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
                    _passwordHasher.HashPassword();
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
    }
}