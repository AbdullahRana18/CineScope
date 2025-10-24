using Microsoft.AspNetCore.Mvc;

namespace CineScope.Controllers
{
    public class MoviesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
