using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public class Medicament
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Dosage { get; set; }

        [Required]
        public int Quantite { get; set; }

        // Many-to-many relationship with Ordonnance
        public ICollection<OrdonnanceMedicament> OrdonnanceMedicaments { get; set; } = new List<OrdonnanceMedicament>();
    }
}