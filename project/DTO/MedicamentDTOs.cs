using System.ComponentModel.DataAnnotations;

namespace project.DTO
{
    public class MedicamentDTOs
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du médicament est requis")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le dosage du médicament est requis")]
        public string Dosage { get; set; }

        [Required(ErrorMessage = "La quantité est requise")]
        [Range(1, 10000, ErrorMessage = "La quantité doit être entre 1 et 10000")]
        public int Quantite { get; set; }
    }
}