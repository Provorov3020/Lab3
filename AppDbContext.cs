using DotNetCrudWebApi.Movies;
using Microsoft.EntityFrameworkCore;

namespace DotNetCrudWebApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<MovieModel> Movies { get; set; }
        public DbSet<DirectorModel> Directors { get; set; }
        public DbSet<ActorModel> Actors { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=./data/MoviesApp.db");
        }
    }
}
