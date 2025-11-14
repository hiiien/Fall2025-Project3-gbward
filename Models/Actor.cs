using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_gbward.Models;

public class Actor
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Gender { get; set; }

    public int? Age { get; set; }

    [Url]
    public string? ImdbLink { get; set; }

    // Actor photo stored as byte[]
    public byte[]? Photo { get; set; }

    // Navigation
    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}

