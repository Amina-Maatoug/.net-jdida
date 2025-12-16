using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using project.Models;

namespace project.Data
{
    public class PharmacieDbContext : DbContext
    {
        public PharmacieDbContext(DbContextOptions<PharmacieDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Ordonnance> Ordonnances { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
    }
}









