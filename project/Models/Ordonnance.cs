using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project.Models
{
    public class Ordonnance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Simple string - PAS de clé étrangère ni navigation property!
        [Required]
        public string PharmacienId { get; set; }  // Juste l'ID, pas de relation


        [Required]
        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        public List<Medicament> Medicaments { get; set; } = new List<Medicament>();
    }
}
