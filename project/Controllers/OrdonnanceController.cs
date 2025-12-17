using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.DTO;
using project.Models;

namespace project.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdonnanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdonnanceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET ALL - Récupérer toutes les ordonnances
        [HttpGet]
        public async Task<IActionResult> GetAllOrdonnances()
        {
            var ordonnances = await _context.Ordonnances
                .Include(o => o.OrdonnanceMedicaments) // Changed from Medicaments
                .Include(o => o.Patient)
                .ToListAsync();

            var result = new List<OrdonnanceDTOs>();

            // Pour chaque ordonnance, récupérer le nom du pharmacien
            foreach (var o in ordonnances)
            {
                var pharmacien = await _userManager.FindByIdAsync(o.PharmacienId);

                result.Add(new OrdonnanceDTOs
                {
                    Id = o.Id,
                    Date = o.Date,
                    PharmacienId = o.PharmacienId,
                    PharmacienNom = pharmacien?.Nom ?? "Inconnu",
                    PatientId = o.PatientId,
                    MedicamentIds = o.OrdonnanceMedicaments.Select(om => om.MedicamentId).ToList() // Changed
                });
            }

            return Ok(result);
        }

        // GET: MES ordonnances
        [HttpGet("MesOrdonnances")]
        public async Task<IActionResult> GetMesOrdonnances()
        {
            var username = User.Identity.Name;
            var pharmacien = await _userManager.FindByNameAsync(username);

            if (pharmacien == null)
                return Unauthorized("Pharmacien non trouvé");

            var ordonnances = await _context.Ordonnances
                .Include(o => o.OrdonnanceMedicaments) // Changed from Medicaments
                .Include(o => o.Patient)
                .Where(o => o.PharmacienId == pharmacien.Id)
                .ToListAsync();

            var result = ordonnances.Select(o => new OrdonnanceDTOs
            {
                Id = o.Id,
                Date = o.Date,
                PharmacienId = o.PharmacienId,
                PharmacienNom = pharmacien.Nom,
                PatientId = o.PatientId,
                MedicamentIds = o.OrdonnanceMedicaments.Select(om => om.MedicamentId).ToList() // Changed
            }).ToList();

            return Ok(result);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdonnanceById(int id)
        {
            var o = await _context.Ordonnances
                .Include(ord => ord.OrdonnanceMedicaments) // Changed from Medicaments
                .Include(ord => ord.Patient)
                .FirstOrDefaultAsync(ord => ord.Id == id);

            if (o == null) return NotFound("Ordonnance non trouvée");

            // Récupérer le pharmacien
            var pharmacien = await _userManager.FindByIdAsync(o.PharmacienId);

            var dto = new OrdonnanceDTOs
            {
                Id = o.Id,
                Date = o.Date,
                PharmacienId = o.PharmacienId,
                PharmacienNom = pharmacien?.Nom ?? "Inconnu",
                PatientId = o.PatientId,
                MedicamentIds = o.OrdonnanceMedicaments.Select(om => om.MedicamentId).ToList() // Changed
            };

            return Ok(dto);
        }

        // POST - Créer une ordonnance
        [HttpPost]
        public async Task<IActionResult> AddOrdonnance([FromBody] OrdonnanceDTOs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Récupérer le pharmacien connecté (ou utiliser l'ID fourni dans le DTO)
            string pharmacienId;
            ApplicationUser pharmacien;

            if (!string.IsNullOrEmpty(dto.PharmacienId))
            {
                // Si un PharmacienId est fourni dans le DTO, l'utiliser
                pharmacienId = dto.PharmacienId;
                pharmacien = await _userManager.FindByIdAsync(pharmacienId);
            }
            else
            {
                // Sinon, utiliser le pharmacien connecté
                var username = User.Identity.Name;
                pharmacien = await _userManager.FindByNameAsync(username);
                pharmacienId = pharmacien?.Id;
            }

            if (pharmacien == null)
                return Unauthorized("Pharmacien non trouvé");

            // Vérifier que le patient existe
            var patientExists = await _context.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists)
                return BadRequest("Le patient n'existe pas");

            var ordonnance = new Ordonnance
            {
                Date = dto.Date,
                PharmacienId = pharmacienId,
                PatientId = dto.PatientId
            };

            await _context.Ordonnances.AddAsync(ordonnance);
            await _context.SaveChangesAsync();

            // Ajouter les médicaments à la table de jonction
            if (dto.MedicamentIds != null && dto.MedicamentIds.Any())
            {
                foreach (var medId in dto.MedicamentIds)
                {
                    var ordonnanceMedicament = new OrdonnanceMedicament
                    {
                        OrdonnanceId = ordonnance.Id,
                        MedicamentId = medId
                    };
                    await _context.OrdonnanceMedicaments.AddAsync(ordonnanceMedicament);
                }
                await _context.SaveChangesAsync();
            }

            dto.Id = ordonnance.Id;
            dto.PharmacienId = pharmacienId;
            dto.PharmacienNom = pharmacien.Nom;

            return CreatedAtAction(nameof(GetOrdonnanceById), new { id = dto.Id }, dto);
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> UpdateOrdonnance([FromBody] OrdonnanceDTOs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ordonnance = await _context.Ordonnances
                .Include(o => o.OrdonnanceMedicaments)
                .FirstOrDefaultAsync(o => o.Id == dto.Id);

            if (ordonnance == null) return NotFound("Ordonnance non trouvée");

            // Vérifier que c'est le pharmacien qui l'a créée
            var username = User.Identity.Name;
            var pharmacien = await _userManager.FindByNameAsync(username);

            if (ordonnance.PharmacienId != pharmacien.Id)
                return Forbid("Vous ne pouvez modifier que vos propres ordonnances");

            ordonnance.Date = dto.Date;
            ordonnance.PatientId = dto.PatientId;

            // Mettre à jour les médicaments
            // Supprimer les anciennes associations
            _context.OrdonnanceMedicaments.RemoveRange(ordonnance.OrdonnanceMedicaments);

            // Ajouter les nouvelles associations
            if (dto.MedicamentIds != null && dto.MedicamentIds.Any())
            {
                foreach (var medId in dto.MedicamentIds)
                {
                    var ordonnanceMedicament = new OrdonnanceMedicament
                    {
                        OrdonnanceId = ordonnance.Id,
                        MedicamentId = medId
                    };
                    await _context.OrdonnanceMedicaments.AddAsync(ordonnanceMedicament);
                }
            }

            _context.Ordonnances.Update(ordonnance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdonnance(int id)
        {
            var ordonnance = await _context.Ordonnances.FindAsync(id);
            if (ordonnance == null) return NotFound("Ordonnance non trouvée");

            // Vérifier que c'est le pharmacien qui l'a créée
            var username = User.Identity.Name;
            var pharmacien = await _userManager.FindByNameAsync(username);

            if (ordonnance.PharmacienId != pharmacien.Id)
                return Forbid("Vous ne pouvez supprimer que vos propres ordonnances");

            // Les OrdonnanceMedicaments seront supprimés automatiquement grâce à OnDelete(DeleteBehavior.Cascade)
            _context.Ordonnances.Remove(ordonnance);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}