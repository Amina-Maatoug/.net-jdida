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
                .Include(o => o.OrdonnanceMedicaments)
                .Include(o => o.Patient)
                .ToListAsync();

            var result = new List<OrdonnanceDTOs>();

            foreach (var o in ordonnances)
            {
                var pharmacien = await _userManager.FindByIdAsync(o.PharmacienId);

                result.Add(new OrdonnanceDTOs
                {
                    Id = o.Id,
                    Date = o.Date,
                    PharmacienId = o.PharmacienId,
                    PharmacienNom = pharmacien != null
                        ? (!string.IsNullOrEmpty(pharmacien.Nom) ? pharmacien.Nom : pharmacien.UserName)
                        : "Inconnu",
                    PatientId = o.PatientId,
                    MedicamentIds = o.OrdonnanceMedicaments.Select(om => om.MedicamentId).ToList(),
                    MedicamentQuantites = o.OrdonnanceMedicaments.ToDictionary(om => om.MedicamentId, om => om.Quantite)
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
                .Include(o => o.OrdonnanceMedicaments)
                .Include(o => o.Patient)
                .Where(o => o.PharmacienId == pharmacien.Id)
                .ToListAsync();

            var pharmacienNom = !string.IsNullOrEmpty(pharmacien.Nom) ? pharmacien.Nom : pharmacien.UserName;

            var result = ordonnances.Select(o => new OrdonnanceDTOs
            {
                Id = o.Id,
                Date = o.Date,
                PharmacienId = o.PharmacienId,
                PharmacienNom = pharmacienNom,
                PatientId = o.PatientId,
                MedicamentIds = o.OrdonnanceMedicaments.Select(om => om.MedicamentId).ToList(),
                MedicamentQuantites = o.OrdonnanceMedicaments.ToDictionary(om => om.MedicamentId, om => om.Quantite)
            }).ToList();

            return Ok(result);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdonnanceById(int id)
        {
            var o = await _context.Ordonnances
                .Include(ord => ord.OrdonnanceMedicaments)
                .Include(ord => ord.Patient)
                .FirstOrDefaultAsync(ord => ord.Id == id);

            if (o == null) return NotFound("Ordonnance non trouvée");

            var pharmacien = await _userManager.FindByIdAsync(o.PharmacienId);

            var dto = new OrdonnanceDTOs
            {
                Id = o.Id,
                Date = o.Date,
                PharmacienId = o.PharmacienId,
                PharmacienNom = pharmacien != null
                    ? (!string.IsNullOrEmpty(pharmacien.Nom) ? pharmacien.Nom : pharmacien.UserName)
                    : "Inconnu",
                PatientId = o.PatientId,
                MedicamentIds = o.OrdonnanceMedicaments.Select(om => om.MedicamentId).ToList(),
                MedicamentQuantites = o.OrdonnanceMedicaments.ToDictionary(om => om.MedicamentId, om => om.Quantite)
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
            string pharmacienId;
            ApplicationUser pharmacien;

            if (!string.IsNullOrEmpty(dto.PharmacienId))
            {
                pharmacienId = dto.PharmacienId;
                pharmacien = await _userManager.FindByIdAsync(pharmacienId);
            }
            else
            {
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

            // Ajouter les médicaments avec leurs quantités et décrémenter le stock
            if (dto.MedicamentQuantites != null && dto.MedicamentQuantites.Any())
            {
                foreach (var item in dto.MedicamentQuantites)
                {
                    int medId = item.Key;
                    int quantite = item.Value;

                    // Vérifier si le médicament existe
                    var medicament = await _context.Medicaments.FindAsync(medId);
                    if (medicament == null)
                        return BadRequest($"Le médicament avec l'ID {medId} n'existe pas");

                    // Vérifier si le stock est suffisant
                    if (medicament.Quantite < quantite)
                        return BadRequest($"Stock insuffisant pour {medicament.Nom}. Disponible: {medicament.Quantite}, Demandé: {quantite}");

                    // Décrémenter le stock
                    medicament.Quantite -= quantite;

                    // Ajouter à la table de jonction
                    var ordonnanceMedicament = new OrdonnanceMedicament
                    {
                        OrdonnanceId = ordonnance.Id,
                        MedicamentId = medId,
                        Quantite = quantite
                    };
                    await _context.OrdonnanceMedicaments.AddAsync(ordonnanceMedicament);
                }

                await _context.SaveChangesAsync();
            }

            dto.Id = ordonnance.Id;
            dto.PharmacienId = pharmacienId;
            dto.PharmacienNom = !string.IsNullOrEmpty(pharmacien.Nom) ? pharmacien.Nom : pharmacien.UserName;

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

            var username = User.Identity.Name;
            var pharmacien = await _userManager.FindByNameAsync(username);

            if (ordonnance.PharmacienId != pharmacien.Id)
                return Forbid("Vous ne pouvez modifier que vos propres ordonnances");

            // Restore stock for old medicaments
            foreach (var oldItem in ordonnance.OrdonnanceMedicaments)
            {
                var medicament = await _context.Medicaments.FindAsync(oldItem.MedicamentId);
                if (medicament != null)
                {
                    medicament.Quantite += oldItem.Quantite; // Restore stock
                }
            }

            ordonnance.Date = dto.Date;
            ordonnance.PatientId = dto.PatientId;

            // Remove old associations
            _context.OrdonnanceMedicaments.RemoveRange(ordonnance.OrdonnanceMedicaments);

            // Add new associations and decrement stock
            if (dto.MedicamentQuantites != null && dto.MedicamentQuantites.Any())
            {
                foreach (var item in dto.MedicamentQuantites)
                {
                    int medId = item.Key;
                    int quantite = item.Value;

                    var medicament = await _context.Medicaments.FindAsync(medId);
                    if (medicament == null)
                        return BadRequest($"Le médicament avec l'ID {medId} n'existe pas");

                    if (medicament.Quantite < quantite)
                        return BadRequest($"Stock insuffisant pour {medicament.Nom}. Disponible: {medicament.Quantite}, Demandé: {quantite}");

                    medicament.Quantite -= quantite;

                    var ordonnanceMedicament = new OrdonnanceMedicament
                    {
                        OrdonnanceId = ordonnance.Id,
                        MedicamentId = medId,
                        Quantite = quantite
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
            var ordonnance = await _context.Ordonnances
                .Include(o => o.OrdonnanceMedicaments)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ordonnance == null) return NotFound("Ordonnance non trouvée");

            var username = User.Identity.Name;
            var pharmacien = await _userManager.FindByNameAsync(username);

            if (ordonnance.PharmacienId != pharmacien.Id)
                return Forbid("Vous ne pouvez supprimer que vos propres ordonnances");

            // Restore stock when deleting
            foreach (var item in ordonnance.OrdonnanceMedicaments)
            {
                var medicament = await _context.Medicaments.FindAsync(item.MedicamentId);
                if (medicament != null)
                {
                    medicament.Quantite += item.Quantite; // Restore stock
                }
            }

            _context.Ordonnances.Remove(ordonnance);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}