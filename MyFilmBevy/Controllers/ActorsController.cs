using Microsoft.AspNetCore.Mvc;
using MyFilmBevy.Services.Interfaces;
using NuGet.DependencyResolver;

namespace MyFilmBevy.Controllers
{
    public class ActorsController : Controller
    {
        private readonly IRemoteMovieService _tmdbMovieService;
        private readonly IDataMappingService _tmdbMappingService;

        public ActorsController(IDataMappingService tmdbMappingService, IRemoteMovieService tmdbMovieService)
        {
            _tmdbMappingService = tmdbMappingService;
            _tmdbMovieService = tmdbMovieService;
        }

        public async Task<IActionResult> Details(int id)
        {
            var actor = await _tmdbMovieService.GetActorDetailAsync(id);
            actor = _tmdbMappingService.MapActorDetail(actor);

            return View(actor);
        }
    }
}
