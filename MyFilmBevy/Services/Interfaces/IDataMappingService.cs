using MyFilmBevy.Models.Database;
using MyFilmBevy.Models.TMDB;

namespace MyFilmBevy.Services.Interfaces
{
    public interface IDataMappingService
    {
        Task<Movie> MapMovieDetailAsync(MovieDetail movie);
        ActorDetail MapActorDetail(ActorDetail actor);
    }
}
