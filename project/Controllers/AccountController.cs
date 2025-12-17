using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using project.DTO;
using project.Models;
using project.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace project.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class AccountController : ControllerBase
        {
            // Injection des dépendances EXACTEMENT comme le professeur
            private readonly UserManager<ApplicationUser> userManager;
            private readonly IConfiguration configuration;

            public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
            {
                this.userManager = userManager;
                this.configuration = configuration;
            }
            [HttpPost("Register")]
            [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserDTO userDto)
            {
                // 1. Vérifier la validité du modèle
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 2. Vérifier si l'utilisateur existe déjà
                var userExists = await userManager.FindByNameAsync(userDto.Nom);
                if (userExists != null)
                    return BadRequest("User already exists!");

                // 3. Créer un nouvel ApplicationUser
                ApplicationUser user = new ApplicationUser()
                {
                    Email = userDto.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = userDto.Nom
                };

                // 4. Créer l'utilisateur avec UserManager (qui hash automatiquement le mot de passe)
                var result = await userManager.CreateAsync(user, userDto.Password);

                if (!result.Succeeded)
                    return BadRequest(result.Errors.FirstOrDefault()?.Description);

                return Ok("User created successfully!");
            }

        
        [HttpPost("Login")]
            public async Task<IActionResult> Login(LoginDTO loginUser)
            {
                // 1. Vérifier la validité du modèle
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 2. Trouver l'utilisateur par son email
                var user = await userManager.FindByNameAsync(loginUser.Username);

                // 3. Vérifier si l'utilisateur existe ET si le mot de passe est correct
                if (user != null && await userManager.CheckPasswordAsync(user, loginUser.Password))
                {
                    // 4. Récupérer les rôles de l'utilisateur
                    var userRoles = await userManager.GetRolesAsync(user);

                    // 5. Créer les claims (revendications)
                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                    // 6. Ajouter les rôles aux claims
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    // 7. Créer la clé de signature
                    var authSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:Secret"])
                    );

                    // 8. Créer le token JWT
                    var token = new JwtSecurityToken(
                        issuer: configuration["JWT:ValidIssuer"],
                        audience: configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                    // 9. Retourner le token
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }

                // Si l'authentification échoue
                return Unauthorized();
            }

        // see his info
        [Authorize]
        [HttpGet("MonProfil")]
        public async Task<IActionResult> GetMonProfil()
        {
            var username = User.Identity.Name;
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                Id = user.Id,
                Nom = user.Nom,
                Email = user.Email,
                UserName = user.UserName
            });
        }
    }
  }

