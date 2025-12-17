using System.ComponentModel.DataAnnotations;

namespace pharmacieBlazor.Models
{
    public class PatientDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(100, MinimumLength = 2)]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de naissance est requise")]
        public DateTime DateNaissance { get; set; }

        public List<int> OrdonnanceIds { get; set; } = new();
    }
}
