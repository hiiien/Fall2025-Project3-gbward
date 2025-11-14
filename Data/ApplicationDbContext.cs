using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_gbward.Models;

namespace Fall2025_Project3_gbward.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<MovieActor> MovieActors { get; set; }
}
