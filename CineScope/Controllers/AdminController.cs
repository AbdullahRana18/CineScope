using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineScope.Services;

namespace CineScope.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly TmdbService _tmdbService;

        public AdminController(TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        public async Task<IActionResult> ManageMovies()
        {
            var data = await _tmdbService.GetTrendingMoviesAsync();

            // if data is null, pass an empty list to the view (prevents Model == null)
            var movies = data?.Results ?? new List<MovieDto>();

            // optionally, you can show a ViewBag message if null
            if (data == null)
            {
                ViewBag.TmdbError = "Unable to fetch movies from TMDb at this time.";
            }

            return View(movies);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
