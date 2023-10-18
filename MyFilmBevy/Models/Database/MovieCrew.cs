using System.ComponentModel.DataAnnotations;

namespace MyFilmBevy.Models.Database
{
    public class MovieCrew
    {
        // Keys
        public int Id { get; set; }

        public int MovieId { get; set; }

        // TMDB Ids
        public int CrewId { get; set; }

        // Descriptive
        public string? Department { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Job { get; set; }

        public string? ImageUrl { get; set; }

        // Navigational
        public Movie Movie { get; set; }

    }
}
