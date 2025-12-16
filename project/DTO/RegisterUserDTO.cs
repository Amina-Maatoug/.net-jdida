using System.ComponentModel.DataAnnotations;

namespace project.DTO
{
    // DTO pour l'inscription d'un nouveau pharmacien
    public class RegisterUserDTO
    {
        [Required(ErrorMessage = "Le nom est requis")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        public string Password { get; set; }
    }
}
