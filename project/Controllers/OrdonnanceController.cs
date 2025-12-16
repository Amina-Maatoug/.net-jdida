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
        private readonly PharmacieDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdonnanceController(PharmacieDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET ALL - Récupérer toutes les ordonnances
        [HttpGet]
        public async Task<IActionResult> GetAllOrdonnances()
        {
            var ordonnances = await _context.Ordonnances
                .Include(o => o.Medicaments)
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
                    MedicamentIds = o.Medicaments.Select(m => m.Id).ToList()
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
                .Include(o => o.Medicaments)
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
                MedicamentIds = o.Medicaments.Select(m => m.Id).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdonnanceById(int id)
        {
            var o = await _context.Ordonnances
                .Include(ord => ord.Medicaments)
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
                MedicamentIds = o.Medicaments.Select(m => m.Id).ToList()
            };

            return Ok(dto);
        }

        // POST - Créer une ordonnance
        [HttpPost]
        public async Task<IActionResult> AddOrdonnance([FromBody] OrdonnanceDTOs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Récupérer le pharmacien connecté
            var username = User.Identity.Name;
            var pharmacien = await _userManager.FindByNameAsync(username);

            if (pharmacien == null)
                return Unauthorized("Pharmacien non trouvé");

            // Vérifier que le patient existe
            var patientExists = await _context.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists)
                return BadRequest("Le patient n'existe pas");

            var ordonnance = new Ordonnance
            {
                Date = dto.Date,
                PharmacienId = pharmacien.Id,  // Simple string - pas de relation!
                PatientId = dto.PatientId
            };

            await _context.Ordonnances.AddAsync(ordonnance);
            await _context.SaveChangesAsync();  // ← Devrait marcher maintenant!

            dto.Id = ordonnance.Id;
            dto.PharmacienId = pharmacien.Id;
            dto.PharmacienNom = pharmacien.Nom;

            return CreatedAtAction(nameof(GetOrdonnanceById), new { id = dto.Id }, dto);
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> UpdateOrdonnance([FromBody] OrdonnanceDTOs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ordonnance = await _context.Ordonnances.FindAsync(dto.Id);
            if (ordonnance == null) return NotFound("Ordonnance non trouvée");

            // Vérifier que c'est le pharmacien qui l'a créée
            var username = User.Identity.Name;
            var pharmacien = await _userManager.FindByNameAsync(username);

            if (ordonnance.PharmacienId != pharmacien.Id)
                return Forbid("Vous ne pouvez modifier que vos propres ordonnances");

            ordonnance.Date = dto.Date;
            ordonnance.PatientId = dto.PatientId;

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

            _context.Ordonnances.Remove(ordonnance);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}