using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using pharmacieBlazor;
using pharmacieBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient points to HTTPS API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7017/") // API HTTPS
});

// Services
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<MedicamentService>();
builder.Services.AddScoped<OrdonnanceService>();

// Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());

await builder.Build().RunAsync();
