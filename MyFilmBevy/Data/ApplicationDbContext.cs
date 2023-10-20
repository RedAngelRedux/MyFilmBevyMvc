using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFilmBevy.Models.Database;

namespace MyFilmBevy.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<MovieCrew> Crew { get; set; }

        public DbSet<MovieCast> Cast { get; set; }

        public DbSet<Collection> Collection { get; set; }

        public DbSet<MovieCollection> MovieCollection { get; set; }
    }
}