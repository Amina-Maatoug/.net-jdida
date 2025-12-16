using System.ComponentModel.DataAnnotations;

namespace project.DTO
{
    public class PatientDTOs
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "La date de naissance est requise")]
        public DateTime DateNaissance { get; set; }

        public List<int> OrdonnanceIds { get; set; } = new List<int>();
    }

}
