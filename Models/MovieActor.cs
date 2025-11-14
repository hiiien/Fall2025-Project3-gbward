using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_gbward.Models
{
    public class MovieActor
    {
        public int Id { get; set; }

        [Display(Name = "Movie")]
        [Required(ErrorMessage = "Please select a movie")]
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        [Display(Name = "Actor")]
        [Required(ErrorMessage = "Please select an actor")]
        public int ActorId { get; set; }
        public Actor Actor { get; set; } = null!;
    }
}
