using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyFilmBevy.Data;
using MyFilmBevy.Models.Database;
using MyFilmBevy.Models.Settings;
using MyFilmBevy.Services.Interfaces;

namespace MyFilmBevy.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly IRemoteMovieService _tmdbMovieService;
        private readonly IDataMappingService _tmdbMappingService;

        public MoviesController(IOptions<AppSettings> appSettings, ApplicationDbContext context, IImageService imageService, IRemoteMovieService remoteMovieService, IDataMappingService tmdbMappingService)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _imageService = imageService;
            _tmdbMovieService = remoteMovieService;
            _tmdbMappingService = tmdbMappingService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Import()
        {
            var movies = await _context.Movies.ToListAsync();
            return View(movies);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(int id)
        {
            try
            {
                // Step 1:  Abort Request if Requested Movie is Already Local
                if (_context.Movies.Any(m => m.MovieId == id))
                {
                    var localMovie = await _context.Movies.FirstOrDefaultAsync(m => m.MovieId == id);
                    if (localMovie != null)
                    {
                        // TO-DO:  Add Friently Notification Here
                        // TO-DO:  In the pop-up, give the user the option of viewing the details or importing a different movie
                        return RedirectToAction("Details", "Movies", new { id = localMovie.Id, local = true });
                    }
                    else
                    {
                        // Implement Smart Alert and Notify User Something Went Wrong Wtith their Request
                        // TO-DO:  Add Friendly Alert Here
                        return RedirectToAction("Movies", "Import");
                    }
                }

                // Step 2:  Get the raw data from the API
                var movieDetail = await _tmdbMovieService.GetMovieDetailAsync(id);
                if (movieDetail != null)
                {
                    var movie = await _tmdbMappingService.MapMovieDetailAsync(movieDetail);
                    if (movie != null)
                    {
                        _context.Add(movie);
                        await _context.SaveChangesAsync();

                        await AddToMovieCollection(movie.Id, _appSettings.MovieProSettings.DefaultCollection.Name);

                        return RedirectToAction("Import");
                    }
                }
                else
                {
                    // Implement Smart Alert and Notify User Something Went Wrong Wtith their Request
                    // TO-DO:  Add Friendly Alert Here
                    return RedirectToAction("Movies", "Import");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An Exception occured in MovieController.Import(movieId), {ex.Message}");
                // Notify User There was a problem processing their request and return to where they came from
                // TO-DO:  Add Friendly Alert Here
                return RedirectToAction("Movies", "Import");
            }

            return View();
        }

        public async Task<IActionResult> Library()
        {
            return View(await _context.Movies.ToListAsync());
        }

        private async Task AddToMovieCollection(int movieId, string collectionName)
        {
            var collection = await _context.Collection.FirstOrDefaultAsync(c => c.Name == collectionName);
            if(collection != null) await AddToMovieCollection(movieId, collection.Id);
        }

        private async Task AddToMovieCollection(int movieId, int  collectionId)
        {
            _context.Add(
                new MovieCollection()
                {
                    CollectionId = collectionId,
                    MovieId = movieId
                });
            await _context.SaveChangesAsync();
        }
    }
}
