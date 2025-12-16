
using global::project.DTO;
using global::project.Models;
using global::project.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;




namespace project.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MedicamentController : ControllerBase
    {
        private readonly IMedicamentRepository _repository;

        public MedicamentController(IMedicamentRepository repository)
        {
            _repository = repository;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAllMedicaments()
        {
            var medicaments = await _repository.GetAllAsync();

            var result = medicaments.Select(m => new MedicamentDTOs
            {
                Id = m.Id,
                Nom = m.Nom,
                Dosage = m.Dosage,
                OrdonnanceId = m.OrdonnanceId
            }).ToList();

            return Ok(result);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicamentById(int id)
        {
            var m = await _repository.GetByIdAsync(id);
            if (m == null) return NotFound("Médicament non trouvé");

            var dto = new MedicamentDTOs
            {
                Id = m.Id,
                Nom = m.Nom,
                Dosage = m.Dosage,
                OrdonnanceId = m.OrdonnanceId
            };

            return Ok(dto);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> AddMedicament([FromBody] MedicamentDTOs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var medicament = new Medicament
            {
                Nom = dto.Nom,
                Dosage = dto.Dosage,
                OrdonnanceId = dto.OrdonnanceId
            };

            await _repository.AddAsync(medicament);
            dto.Id = medicament.Id;

            return CreatedAtAction(nameof(GetMedicamentById), new { id = dto.Id }, dto);
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> UpdateMedicament([FromBody] MedicamentDTOs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var medicament = await _repository.GetByIdAsync(dto.Id);
            if (medicament == null) return NotFound("Médicament non trouvé");

            medicament.Nom = dto.Nom;
            medicament.Dosage = dto.Dosage;
            medicament.OrdonnanceId = dto.OrdonnanceId;

            await _repository.UpdateAsync(medicament);
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicament(int id)
        {
            var medicament = await _repository.GetByIdAsync(id);
            if (medicament == null) return NotFound("Médicament non trouvé");

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    } } 

