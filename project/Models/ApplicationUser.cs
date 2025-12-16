using Microsoft.AspNetCore.Identity;

namespace project.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Nom { get; set; }
    }
}
