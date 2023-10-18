using MyFilmBevy.Models.TMDB;
using MyFilmBevy.Models.Database;

namespace MyFilmBevy.Models.View
{
    public class LandingPageVM
    {
        public List<Collection>? CustomCollectinos { get; set; }
        public MovieSearch? NowPlaying { get; set; }
        public MovieSearch? Popular { get; set; }
        public MovieSearch? TopRated { get; set; }
        public MovieSearch? Upcoming { get; set; }

    }
}
