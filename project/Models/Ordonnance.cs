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

        [Required]
        public string PharmacienId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        // Many-to-many relationship with Medicament
        public ICollection<OrdonnanceMedicament> OrdonnanceMedicaments { get; set; } = new List<OrdonnanceMedicament>();
    }
}