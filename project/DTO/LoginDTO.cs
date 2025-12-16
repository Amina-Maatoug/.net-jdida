using System.ComponentModel.DataAnnotations;

namespace project.DTO
{
    // DTO pour la connexion
    public class LoginDTO
    {
        [Required(ErrorMessage = "Username est requis")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        public string Password { get; set; }
    }
}
