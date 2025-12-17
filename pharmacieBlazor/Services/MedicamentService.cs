using System.Net.Http.Json;
using pharmacieBlazor.Models;

namespace pharmacieBlazor.Services
{
    public class MedicamentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/Medicament";

        public MedicamentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<MedicamentDto>> GetAllMedicamentsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<MedicamentDto>>(_baseUrl);
                return result ?? new List<MedicamentDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur récupération médicaments : {ex.Message}");
                return new List<MedicamentDto>();
            }
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<MedicamentDto?> GetMedicamentByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<MedicamentDto>($"{_baseUrl}/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur récupération médicament : {ex.Message}");
                return null;
            }
        }

        // =========================
        // CREATE
        // =========================
        public async Task<bool> CreateMedicamentAsync(MedicamentDto medicament)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, medicament);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erreur API : {response.StatusCode} - {error}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur création médicament : {ex.Message}");
                return false;
            }
        }

        // =========================
        // UPDATE
        // =========================
        public async Task<bool> UpdateMedicamentAsync(MedicamentDto medicament)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(_baseUrl, medicament);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erreur API : {response.StatusCode} - {error}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur mise à jour médicament : {ex.Message}");
                return false;
            }
        }

        // =========================
        // DELETE
        // =========================
        public async Task<bool> DeleteMedicamentAsync(int id)
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
                Console.WriteLine($"Erreur suppression médicament : {ex.Message}");
                return false;
            }
        }
    }
}