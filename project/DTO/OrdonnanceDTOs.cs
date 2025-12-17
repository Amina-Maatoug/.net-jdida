using System.ComponentModel.DataAnnotations;

namespace project.DTO
{
    public class OrdonnanceDTOs
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La date est requise")]
        public DateTime Date { get; set; }

        public string? PharmacienId { get; set; }
        public string? PharmacienNom { get; set; }

        [Required(ErrorMessage = "L'ID du patient est requis")]
        public int PatientId { get; set; }

        // Changed: Now we need medicament ID and quantity pairs
        public List<int> MedicamentIds { get; set; } = new List<int>();

        // NEW: Dictionary to store quantity for each medicament
        public Dictionary<int, int> MedicamentQuantites { get; set; } = new Dictionary<int, int>();
    }
}