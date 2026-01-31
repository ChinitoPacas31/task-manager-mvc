using TaskManager.API.DTOs;

namespace TaskManager.API.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    string GenerateJwtToken(string userId, string username, string role);
}
