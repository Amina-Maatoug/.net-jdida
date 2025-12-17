using System.Net.Http.Json;
using pharmacieBlazor.Models;

namespace pharmacieBlazor.Services
{
    public class OrdonnanceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/Ordonnance";

        public OrdonnanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<OrdonnanceDto>> GetAllOrdonnancesAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<OrdonnanceDto>>(_baseUrl);
                return result ?? new List<OrdonnanceDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur récupération ordonnances : {ex.Message}");
                return new List<OrdonnanceDto>();
            }
        }

        // =========================
        // GET MES ORDONNANCES
        // =========================
        public async Task<List<OrdonnanceDto>> GetMesOrdonnancesAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<OrdonnanceDto>>($"{_baseUrl}/MesOrdonnances");
                return result ?? new List<OrdonnanceDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur récupération mes ordonnances : {ex.Message}");
                return new List<OrdonnanceDto>();
            }
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<OrdonnanceDto?> GetOrdonnanceByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<OrdonnanceDto>($"{_baseUrl}/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur récupération ordonnance : {ex.Message}");
                return null;
            }
        }

        // =========================
        // CREATE
        // =========================
        public async Task<bool> CreateOrdonnanceAsync(OrdonnanceDto ordonnance)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, ordonnance);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erreur API : {response.StatusCode} - {error}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur création ordonnance : {ex.Message}");
                return false;
            }
        }

        // =========================
        // UPDATE
        // =========================
        public async Task<bool> UpdateOrdonnanceAsync(OrdonnanceDto ordonnance)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(_baseUrl, ordonnance);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erreur API : {response.StatusCode} - {error}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur mise à jour ordonnance : {ex.Message}");
                return false;
            }
        }

        // =========================
        // DELETE
        // =========================
        public async Task<bool> DeleteOrdonnanceAsync(int id)
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
                Console.WriteLine($"Erreur suppression ordonnance : {ex.Message}");
                return false;
            }
        }
    }
}