using System.ComponentModel.DataAnnotations;

namespace pharmacieBlazor.Models
{
    public class OrdonnanceDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La date est requise")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Le patient est requis")]
        public int PatientId { get; set; }

        public string? PharmacienId { get; set; }
        public string? PharmacienNom { get; set; }

        public List<int> MedicamentIds { get; set; } = new();

        // NEW: Dictionary to store quantity for each medicament
        public Dictionary<int, int> MedicamentQuantites { get; set; } = new();
    }
}