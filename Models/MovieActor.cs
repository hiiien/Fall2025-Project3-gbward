namespace Fall2025_Project3_gbward.Models;

public class MovieActor
{
    public int Id { get; set; } // Optional, but useful for scaffolding

    public int MovieId { get; set; }
    public Movie Movie { get; set; }

    public int ActorId { get; set; }
    public Actor Actor { get; set; }
}
