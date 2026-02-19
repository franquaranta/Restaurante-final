using Microsoft.AspNetCore.Mvc;
using restaurant_api.DTOs;
using restaurant_api.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    // POST: /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);

        if (token == null)
        {
            return Unauthorized("Credenciales inválidas.");
        }

        return Ok(new { token });
    }
}
