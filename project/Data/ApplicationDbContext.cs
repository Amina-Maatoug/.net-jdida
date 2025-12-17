using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Ordonnance> Ordonnances { get; set; }
        public DbSet<OrdonnanceMedicament> OrdonnanceMedicaments { get; set; }
        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship
            modelBuilder.Entity<OrdonnanceMedicament>()
                .HasOne(om => om.Ordonnance)
                .WithMany(o => o.OrdonnanceMedicaments)
                .HasForeignKey(om => om.OrdonnanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrdonnanceMedicament>()
                .HasOne(om => om.Medicament)
                .WithMany(m => m.OrdonnanceMedicaments)
                .HasForeignKey(om => om.MedicamentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}