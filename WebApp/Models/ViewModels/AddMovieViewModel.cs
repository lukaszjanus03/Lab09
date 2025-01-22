using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.ViewModels
{
    public class AddMovieViewModel
    {
        [Required(ErrorMessage = "The Aktor field is required.")]
        public int ActorId { get; set; }

        public string ActorName { get; set; }

        [Required(ErrorMessage = "Pole Film jest wymagane.")]
        public int SelectedMovieId { get; set; }

        [Required(ErrorMessage = "Pole Postać (rola) jest wymagane.")]
        public string CharacterName { get; set; }

        public List<SelectListItem> Movies { get; set; } = new();
    }
}