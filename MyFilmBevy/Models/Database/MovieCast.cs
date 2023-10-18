using System.ComponentModel.DataAnnotations;

namespace MyFilmBevy.Models.Database
{
    public class MovieCast
    {
        public int Id { get; set; }
        public int MovieId { get; set; }

        public int CastId { get; set; }
        public string? Department { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Character { get; set; }
        public string? ImageUrl { get; set; }

        public Movie Movie { get; set; }
    }
}
