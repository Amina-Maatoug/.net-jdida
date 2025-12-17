using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project.Models
{
    public class OrdonnanceMedicament
    {
        [Key]
        public int Id { get; set; }

        public int OrdonnanceId { get; set; }

        [ForeignKey("OrdonnanceId")]
        public Ordonnance Ordonnance { get; set; }

        public int MedicamentId { get; set; }

        [ForeignKey("MedicamentId")]
        public Medicament Medicament { get; set; }
    }
}