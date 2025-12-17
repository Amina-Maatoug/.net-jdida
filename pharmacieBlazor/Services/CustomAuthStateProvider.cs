using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace pharmacieBlazor.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public CustomAuthStateProvider(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await GetTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            await SetTokenAsync(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await RemoveTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = null;

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private async Task<string?> GetTokenAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            }
            catch
            {
                return null;
            }
        }

        private async Task SetTokenAsync(string token)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        }

        private async Task RemoveTokenAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles);

                if (roles != null)
                {
                    if (roles.ToString()!.Trim().StartsWith("["))
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);
                        if (parsedRoles != null)
                        {
                            foreach (var parsedRole in parsedRoles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                            }
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
                    }

                    keyValuePairs.Remove(ClaimTypes.Role);
                }

                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}