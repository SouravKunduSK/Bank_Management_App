using Bank_Management_Api.Models;

namespace Bank_Management_Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterUser(RegisterModel model, string creatorEmail);
        Task<AuthResponse> Login(LoginModel model);
        Task<AuthResponse> RefreshToken(string token);
        Task<bool> RevokeToken(string token);
        Task<AuthResponse> Logout(string refreshToken);
        Task<UserDetail> UserDetailAsync(string email);
    }
}
