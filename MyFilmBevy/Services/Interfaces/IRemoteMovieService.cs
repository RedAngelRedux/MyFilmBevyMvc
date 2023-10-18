using MyFilmBevy.Enums;
using MyFilmBevy.Models.TMDB;

namespace MyFilmBevy.Services.Interfaces
{
    public interface IRemoteMovieService
    {
        Task<MovieDetail> GetMovieDetailAsync(int id);
        Task<MovieSearch> SearchMoviesAsync(MovieCategory category, int count);
        Task<ActorDetail> GetActorDetailAsync(int id);
    }
}
