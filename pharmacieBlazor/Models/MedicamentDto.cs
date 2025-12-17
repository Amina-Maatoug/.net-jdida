using System.ComponentModel.DataAnnotations;

namespace pharmacieBlazor.Models
{
    public class MedicamentDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 200 caractères")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le dosage est requis")]
        [StringLength(100, ErrorMessage = "Le dosage ne peut pas dépasser 100 caractères")]
        public string Dosage { get; set; } = string.Empty;

        [Required(ErrorMessage = "La quantité est requise")]
        [Range(1, 10000, ErrorMessage = "La quantité doit être entre 1 et 10000")]
        public int Quantite { get; set; } = 1;
    }
}