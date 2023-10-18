namespace MyFilmBevy.Models.Database
{
    public class MovieCollection
    {
        // Keys
        public int Id { get; set; }

        public int CollectionId { get; set; }

        public int MovieId { get; set; }

        // Descriptive
        public int Order { get; set; }

        // Navigatinal
        public Collection? Collection { get; set; }

        public Movie? Movie { get; set; }
    }
}
