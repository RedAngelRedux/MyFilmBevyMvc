using Microsoft.Extensions.Options;
using MyFilmBevy.Enums;
using MyFilmBevy.Models.Database;
using MyFilmBevy.Models.Settings;
using MyFilmBevy.Models.TMDB;
using MyFilmBevy.Services.Interfaces;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using System.Diagnostics;

namespace MyFilmBevy.Services
{
    public class TMDBMappingService : IDataMappingService
    {
        private AppSettings _appSettings;
        private readonly IImageService _imageService;

        public TMDBMappingService(IOptions<AppSettings> appSettings, IImageService imageService)
        {
            _appSettings = appSettings.Value;
            _imageService = imageService;
        }

        public ActorDetail MapActorDetail(ActorDetail actor)
        {
            actor.profile_path = BuildCastImage(actor.profile_path);

            if (string.IsNullOrEmpty(actor.biography)) actor.biography = "Not Available";

            if (string.IsNullOrEmpty(actor.place_of_birth)) actor.place_of_birth = "Not Available";

            if (string.IsNullOrEmpty(actor.birthday)) actor.birthday = "Not Available";
            
            actor.birthday = (string.IsNullOrEmpty(actor.birthday))
                ? "Not Available"
                : DateTime.Parse(actor.birthday).ToString("MMM dd, yyyy");

            return actor;
        }

        public async Task<Movie> MapMovieDetailAsync(MovieDetail movie)
        {
            Movie? newMovie = null;

            try
            {
                newMovie = new Movie();
                newMovie.MovieId = movie.id;
                newMovie.Title = movie.title;
                newMovie.TagLine = movie.tagline;
                newMovie.Overview = movie.overview;
                newMovie.RunTime = movie.runtime;
                newMovie.BackdroprData = await EncodeBackdropImageAsync(movie.backdrop_path);
                newMovie.BackdropType = BuildImageType(movie.backdrop_path);
                newMovie.PosterData = await EncodePosterImageAsync(movie.poster_path);
                newMovie.PosterType = BuildImageType(movie.poster_path);
                newMovie.Rating = GetRating(movie.release_dates);
                newMovie.ReleaseDate = DateTime.Parse(movie.release_date);
                newMovie.TrailerUrl = BuildTrailerPath(movie.videos);
                newMovie.VoteAverage = movie.vote_average;

                //{
                //    MovieId = movie.id,
                //    Title = movie.title,
                //    TagLine = movie.tagline,
                //    Overview = movie.overview,
                //    RunTime = movie.runtime,
                //    BackdroprData = await EncodeBackdropImageAsync(movie.backdrop_path),
                //    BackdropType = BuildImageType(movie.backdrop_path),
                //    PosterData = await EncodePosterImageAsync(movie.poster_path),
                //    PosterType = BuildImageType(movie.poster_path),
                //    Rating = GetRating(movie.release_dates),
                //    ReleaseDate = DateTime.Parse(movie.release_date),
                //    TrailerUrl = BuildTrailerPath(movie.videos),
                //    VoteAverage = movie.vote_average,
                //};

                var castMembers = movie.credits.cast
                    .OrderByDescending(c => c.popularity)
                    .GroupBy(c => c.cast_id)
                    .Select(g => g.FirstOrDefault())
                    .Take(25)
                    .ToList();

                castMembers.ForEach(member =>
                {
                    newMovie.Cast.Add(new MovieCast()
                    {
                        CastId = member.id,
                        Department = member.known_for_department,
                        Name = member.name,
                        Character = member.character,
                        ImageUrl = BuildCastImage(member.profile_path)
                    });
                });

                var crewMembers = movie.credits.crew
                    .OrderByDescending(c => c.popularity)
                    .GroupBy(c => c.id)
                    .Select(g => g.First())
                    .Take(25)
                    .ToList();

                crewMembers.ForEach(member =>
                {
                    newMovie.Crew.Add(new MovieCrew() 
                    {
                        CrewId = member.id,
                        Department = member.department,
                        Name = member.name,
                        Job = member.job,
                        ImageUrl = BuildCastImage(member.profile_path)
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in MapMovieDetailAsync: {ex.Message}");
            }

            return newMovie;
        }

        private string BuildCastImage(string profile_path)
        {
            if (string.IsNullOrEmpty(profile_path)) return _appSettings.MovieProSettings.DefaultCastImage;
            return $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultPosterSize}/{profile_path}";
        }

        private MovieRating GetRating(Release_Dates release_dates)
        {
            var movieRating = MovieRating.NR;
            var certification = release_dates.results.FirstOrDefault(r => r.iso_3166_1 == "US");
            if (certification is not null)
            {
                var apiRating = certification.release_dates.FirstOrDefault(c => c.certification != "")?.certification.Replace("-", "");
                if (!string.IsNullOrEmpty(apiRating))
                {
                    movieRating = (MovieRating)Enum.Parse(typeof(MovieRating), apiRating, true);
                }
            }
            return movieRating;
        }

        private async Task<byte[]> EncodePosterImageAsync(string poster_path)
        {
            var posterPath = $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultPosterSize}/{poster_path}";
            return await _imageService.EncodeImageUrlAsync(posterPath);
        }

        private string BuildTrailerPath(Videos videos)
        {
            var videoKey = videos.results.FirstOrDefault(r => r.type.ToLower().Trim() == "trailer" && r.key != "")?.key;
            return string.IsNullOrEmpty(videoKey) ? videoKey : $"{_appSettings.TMDBSettings.BaseYouTubePath}{videoKey}";
        }

        private async Task<byte[]> EncodeBackdropImageAsync(string backdrop_path)
        {
            var backdropPath = $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultBackdropSize}/{backdrop_path}";
            return await _imageService.EncodeImageUrlAsync(backdropPath);
        }

        private string BuildImageType(string backdrop_path)
        {
            if (string.IsNullOrEmpty(backdrop_path)) return backdrop_path;
            return $"image/{Path.GetExtension(backdrop_path).TrimStart('.')}";
        }

    }
}
