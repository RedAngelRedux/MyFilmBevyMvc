using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFilmBevy.Enums;
using MyFilmBevy.Data;
using MyFilmBevy.Models;
using MyFilmBevy.Models.View;
using MyFilmBevy.Services.Interfaces;
using System.Diagnostics;

namespace MyFilmBevy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IRemoteMovieService _tmdbMovieService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IRemoteMovieService tmdbMovieService)
        {
            _logger = logger;
            _context = context;
            _tmdbMovieService = tmdbMovieService;
        }

        public async Task<IActionResult> Index()
        {
            const int count = 16;
            var data = new LandingPageVM()
            {
                CustomCollectinos = await _context.Collection
                                            .Include(c => c.MovieCollections)
                                            .ThenInclude(mc => mc.Movie)
                                            .ToListAsync(),
                NowPlaying = await _tmdbMovieService.SearchMoviesAsync(MovieCategory.NOW_PLAYING,count),
                Popular = await _tmdbMovieService.SearchMoviesAsync(MovieCategory.POPULAR, count),
                TopRated = await _tmdbMovieService.SearchMoviesAsync(MovieCategory.TOP_RATED, count),
                Upcoming = await _tmdbMovieService.SearchMoviesAsync(MovieCategory.UPCOMING, count)

            };


            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}