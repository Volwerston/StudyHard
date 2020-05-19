using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudyHard.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly Settings _settings;

        public LoginController(Settings settings)
        {
            _settings = settings;
        }

        public IActionResult Index()
        {
            return View(_settings);
        }

        public IActionResult Logout()
        {
            return View();
        }
    }
}