using Microsoft.AspNetCore.Mvc;

namespace CineScope.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
