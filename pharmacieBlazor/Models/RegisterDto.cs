using System.ComponentModel.DataAnnotations;

namespace pharmacieBlazor.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(100, MinimumLength = 2)]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        public string Password { get; set; } = string.Empty;
    }
}