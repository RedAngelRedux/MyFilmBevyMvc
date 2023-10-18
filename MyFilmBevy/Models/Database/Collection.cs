using System.ComponentModel.DataAnnotations;

namespace MyFilmBevy.Models.Database
{
    public class Collection
    {
        // Keys
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }

        // Descriptive
        public string? Description { get; set; }

        // Navigational
        public virtual ICollection<MovieCollection> MovieCollections { get; set; } = new HashSet<MovieCollection>();

    }
}
