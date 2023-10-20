using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [AllowAnonymous]
        public async Task<IActionResult> Library()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Temp/Create
        public IActionResult Create()
        {
            // Display list of all existing collectins
            ViewData["CollectionId"] = new SelectList(_context.Collection, "Id", "Name");

            return View();
        }

        // POST: Temp/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieId,Title,TagLine,Overview,RunTime,ReleaseDate,Rating,VoteAverage,PosterData,PosterType,BackdroprData,BackdropType,TrailerUrl")] Movie movie, int collectionId)
        {
            if (ModelState.IsValid)
            {
                if(movie.PosterFile is not null)
                {
                    movie.PosterType = movie.PosterFile.ContentType;
                    movie.PosterData = await _imageService.EncodeImageAsync(movie.PosterFile);
                }

                if(movie.BackdroprFile is not null)
                {
                    movie.BackdropType = movie.BackdroprFile.ContentType;
                    movie.BackdroprData = await _imageService.EncodeImageAsync(movie.BackdroprFile);
                }
                _context.Add(movie);
                await _context.SaveChangesAsync();

                await AddToMovieCollection(movie.Id, collectionId);

                return RedirectToAction("Index", "MovieCollections");
            }
            return View(movie);
        }

        // GET: Temp/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Temp/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,Title,TagLine,Overview,RunTime,ReleaseDate,Rating,VoteAverage,PosterData,PosterType,BackdroprData,BackdropType,TrailerUrl")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (movie.PosterFile is not null)
                    {
                        movie.PosterType = movie.PosterFile.ContentType;
                        movie.PosterData = await _imageService.EncodeImageAsync(movie.PosterFile);
                    }

                    if (movie.BackdroprFile is not null)
                    {
                        movie.BackdropType = movie.BackdroprFile.ContentType;
                        movie.BackdroprData = await _imageService.EncodeImageAsync(movie.BackdroprFile);
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details","Movies",new { id = movie.Id, local = true});
            }
            return View(movie);
        }

        // GET: Temp/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Temp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movies == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Movies'  is null.");
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Library","Movies");
        }

        private bool MovieExists(int id)
        {
            return (_context.Movies?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Details(int? id, bool local = false)
        {
            if (id is null) return NotFound();

            Movie? movie = new();

            if(local)
            {
                movie = await _context.Movies
                                .Include(m => m.Cast)
                                .Include(m => m.Crew)
                                .FirstOrDefaultAsync(m => m.Id == id);

            }
            else
            {
                var movieDetail = await _tmdbMovieService.GetMovieDetailAsync((int)id);
                movie = await _tmdbMappingService.MapMovieDetailAsync(movieDetail);
            }

            if(movie is null) return NotFound();

            ViewData["local"] = local;
            return View(movie);
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
