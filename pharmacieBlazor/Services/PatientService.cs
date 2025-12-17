using System.Net.Http.Json;
using pharmacieBlazor.Models;

namespace pharmacieBlazor.Services
{
    public class PatientService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/Patient";

        public PatientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<PatientDto>> GetAllPatientsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<PatientDto>>(_baseUrl);
                return result ?? new List<PatientDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur récupération patients : {ex.Message}");
                return new List<PatientDto>();
            }
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<PatientDto?> GetPatientByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<PatientDto>($"{_baseUrl}/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur récupération patient : {ex.Message}");
                return null;
            }
        }

        // =========================
        // CREATE
        // =========================
        public async Task<bool> CreatePatientAsync(PatientDto patient)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, patient);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erreur API : {response.StatusCode} - {error}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur création patient : {ex.Message}");
                return false;
            }
        }

        // =========================
        // UPDATE
        // =========================
        public async Task<bool> UpdatePatientAsync(PatientDto patient)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(_baseUrl, patient);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erreur API : {response.StatusCode} - {error}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur mise à jour patient : {ex.Message}");
                return false;
            }
        }

        // =========================
        // DELETE
        // =========================
        public async Task<bool> DeletePatientAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erreur API : {response.StatusCode} - {error}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur suppression patient : {ex.Message}");
                return false;
            }
        }
    }
}