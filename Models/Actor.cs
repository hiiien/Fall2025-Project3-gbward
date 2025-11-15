using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_gbward.Models;

public class Actor
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;  // Added = string.Empty

    public string? Gender { get; set; }

    public int? Age { get; set; }

    [Url]
    public string? ImdbLink { get; set; }

    public byte[]? Photo { get; set; }

    // Changed from MovieActors to ActorMovies
    public ICollection<MovieActor> ActorMovies { get; set; } = new List<MovieActor>();
}
