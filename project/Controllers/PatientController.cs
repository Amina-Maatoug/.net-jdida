using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.DTO;
using project.Models;
using project.Repositories;

namespace project.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController: ControllerBase
    {
        private readonly IPatientRepository repository;

        public PatientController(IPatientRepository repository)
        {
            this.repository = repository;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await repository.GetAllAsync();

            var result = patients.Select(p => new PatientDTOs
            {
                Id = p.Id,
                Nom = p.Nom,
                DateNaissance = p.DateNaissance,
                OrdonnanceIds = p.Ordonnances.Select(o => o.Id).ToList()
            });

            return Ok(result);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var p = await repository.GetByIdAsync(id);
            if (p == null) return NotFound("Patient not found");

            var dto = new PatientDTOs
            {
                Id = p.Id,
                Nom = p.Nom,
                DateNaissance = p.DateNaissance,
                OrdonnanceIds = p.Ordonnances.Select(o => o.Id).ToList()
            };

            return Ok(dto);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> AddPatient([FromBody] PatientDTOs dto)
        {
            var patient = new Patient
            {
                Nom = dto.Nom,
                DateNaissance = dto.DateNaissance
            };

            await repository.AddAsync(patient);
            // Mise à jour DTO avec l'ID généré
            dto.Id = patient.Id;

            return CreatedAtAction(nameof(GetPatientById), new { id = dto.Id }, dto);
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> UpdatePatient([FromBody] PatientDTOs dto)
        {
            var patient = await repository.GetByIdAsync(dto.Id);
            if (patient == null) return NotFound("Patient not found");

            patient.Nom = dto.Nom;
            patient.DateNaissance = dto.DateNaissance;

            await repository.UpdateAsync(patient);

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await repository.GetByIdAsync(id);
            if (patient == null) return NotFound("Patient not found");

            await repository.DeleteAsync(id);
            return NoContent();
        }
    }
}