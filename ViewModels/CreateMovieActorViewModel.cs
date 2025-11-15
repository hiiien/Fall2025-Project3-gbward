using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_gbward.ViewModels
{
    public class CreateMovieActorViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Movie")]
        [Required(ErrorMessage = "Please select a movie.")]
        public int MovieId { get; set; }

        [Display(Name = "Actor")]
        [Required(ErrorMessage = "Please select an actor.")]
        public int ActorId { get; set; }

        [ValidateNever]
        public SelectList Movies { get; set; } = null!;

        [ValidateNever]
        public SelectList Actors { get; set; } = null!;
    }
}
