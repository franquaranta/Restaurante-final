using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using restaurant_api.DTOs;
using restaurant_api.Models;
using restaurant_api.Repositories.Interfaces;
using restaurant_api.Services.Interfaces;

namespace restaurant_api.Services.Implementations;

public class AuthService(IRestauranteRepository restauranteRepository, IConfiguration config) : IAuthService
{
    private readonly IRestauranteRepository _restauranteRepository = restauranteRepository;
    private readonly IConfiguration _config = config;

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        if (dto.Email == null) return null;

        var restaurante = await _restauranteRepository.GetByEmailAsync(dto.Email);

        if (restaurante == null || !BCrypt.Net.BCrypt.Verify(dto.Password, restaurante.PasswordHash))
        {
            return null;
        }

        return GenerateJwtToken(restaurante);
    }

    private string GenerateJwtToken(Restaurante restaurante)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, restaurante.Id.ToString()),
            new Claim(ClaimTypes.Email, restaurante.Email!),
            new Claim(ClaimTypes.Name, restaurante.Nombre!)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
