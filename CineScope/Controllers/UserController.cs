using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CineScope.Services;

namespace CineScope.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly TmdbService _tmdbService;
        public MoviesController(TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Trending");
        }
        public async Task<IActionResult> Trending()
        {
            var trendingMovies = await _tmdbService.GetTrendingMoviesAsync();
            if (trendingMovies == null)
            {
                ViewData["Error"] = "Unable to fetch trending movies. Please try again later.";
            }

            return View("Trending", trendingMovies);
        }
      
    }
}
