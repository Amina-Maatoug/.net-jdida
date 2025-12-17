using System.Net.Http.Json;
using pharmacieBlazor.Models;

namespace pharmacieBlazor.Services
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:5100/api/Account";

        public AccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // =========================
        // REGISTER
        // =========================
        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto userDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Register", userDto);

                if (response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    return (true, message);
                }

                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erreur inscription : {response.StatusCode} - {error}");
                return (false, error);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur inscription : {ex.Message}");
                return (false, ex.Message);
            }
        }

        // =========================
        // LOGIN
        // =========================
        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erreur connexion : {response.StatusCode} - {error}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur connexion : {ex.Message}");
                return null;
            }
        }

        // =========================
        // GET MON PROFIL
        // =========================
        public async Task<UserProfileDto?> GetMonProfilAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserProfileDto>($"{_baseUrl}/MonProfil");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur récupération profil : {ex.Message}");
                return null;
            }
        }
    }
}