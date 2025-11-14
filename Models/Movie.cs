using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_gbward.Models;

public class Movie
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Url]
    public string? ImdbLink { get; set; }

    public string? Genre { get; set; }

    public string? Year { get; set; }

    // Poster stored as byte[]
    public byte[]? Poster { get; set; }

    // Navigation
    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}
