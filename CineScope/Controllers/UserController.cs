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
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ViewData["Error"] = "Please enter a movie name.";
                return View("Trending", null);
            }
            var searchResults = await _tmdbService.SearchMoviesAsync(query);
            if (searchResults == null || searchResults.Results == null ||!searchResults.Results.Any())
            {
                ViewData["Error"] = $"No results found for '{query}'.";
                return View("Trending", null);
            }
            ViewData["Title"] = $"Search Results for '{query}'";
            return View("Trending", searchResults);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _tmdbService.GetMovieDetailsAsync(id);
            if(movie ==null)
            {
                return NotFound();
            }
            return View(movie);
        }
      
    }
}
