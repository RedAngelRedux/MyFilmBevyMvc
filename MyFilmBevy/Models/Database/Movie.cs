using MyFilmBevy.Enums;
//using MyFilmFiles.Models.TMDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MyFilmBevy.Models.Database
{
    public class Movie
    {
        // Keys
        public int Id { get; set; }
        public int MovieId { get; set; }

        [Required]
        public string Title { get; set; }
        public string? TagLine { get; set; }
        public string? Overview { get; set; }
        public int RunTime { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }
        public MovieRating Rating { get; set; }
        public float VoteAverage { get; set; }
        [Display(Name = "Poster")]
        public byte[]? PosterData { get; set; }
        public string? PosterType { get; set; }
        [Display(Name = "Backdrop")]
        public byte[]? BackdroprData { get; set; }
        public string? BackdropType { get; set; }
        public string? TrailerUrl { get; set; }

        // Support
        [NotMapped]
        [Display(Name = "Poster Image")]
        public IFormFile? PosterFile { get; set; }
        [NotMapped]
        [Display(Name = "Backdrop Image")]
        public IFormFile? BackdroprFile { get; set; }

        //// Navigational
        //public ICollection<MovieCollection> Collections { get; set; } = new HashSet<MovieCollection>();
        public virtual ICollection<MovieCast> Cast { get; set; } = new HashSet<MovieCast>();
        public virtual ICollection<MovieCrew> Crew { get; set; } = new HashSet<MovieCrew>();


    }
}
