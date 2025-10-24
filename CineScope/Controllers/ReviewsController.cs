using Microsoft.AspNetCore.Mvc;

namespace CineScope.Controllers
{
    public class ReviewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
