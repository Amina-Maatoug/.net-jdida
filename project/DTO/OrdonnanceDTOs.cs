using System.ComponentModel.DataAnnotations;

namespace project.DTO
{
    public class OrdonnanceDTOs
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La date est requise")]
        public DateTime Date { get; set; }

        // Pas Required car sera rempli automatiquement par le système
        public string? PharmacienId { get; set; }  // ← CHANGÉ de int à string + nullable
        public string? PharmacienNom { get; set; }  // ← AJOUTÉ pour affichage

        [Required(ErrorMessage = "L'ID du patient est requis")]
        public int PatientId { get; set; }

        public List<int> MedicamentIds { get; set; } = new List<int>();
    }
}
