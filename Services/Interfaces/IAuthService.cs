using restaurant_api.DTOs;

namespace restaurant_api.Services.Interfaces;

public interface IAuthService
{
    Task<string?> LoginAsync(LoginDto dto);
}
