using System.ComponentModel.DataAnnotations;

namespace pharmacieBlazor.Models
{
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
