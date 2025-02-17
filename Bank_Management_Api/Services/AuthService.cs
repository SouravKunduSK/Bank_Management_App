﻿using Bank_Management_Api.Models;
using Bank_Management_Api.Services.Interfaces;
using Bank_Management_Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Bank_Management_Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<AuthResponse> Login(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new AuthResponse { Message = "Invalid credentials!" };

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens = new List<RefreshToken> { refreshToken };
            await _userManager.UpdateAsync(user);

            return new AuthResponse { Token = token, RefreshToken = refreshToken.Token, Message = "Login Successful!" };
        }

        public Task<AuthResponse> RefreshToken(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResponse> RegisterUser(RegisterModel model, string creatorEmail)
        {
            var creator = await _userManager.FindByEmailAsync(creatorEmail);
            if (creator == null)
                return new AuthResponse { Message = "Invalid creator!" };

            var creatorRoles = await _userManager.GetRolesAsync(creator);

            if (model.Role == "Admin")
                return new AuthResponse { Message = "You cannot create an Admin account!" };

            if (model.Role == "Bank-Staff" && !creatorRoles.Contains("Admin"))
                return new AuthResponse { Message = "Only Admin can create Bank-Staff users!" };

            if (model.Role == "Customer" && !creatorRoles.Contains("Admin") && !creatorRoles.Contains("Bank-Staff"))
                return new AuthResponse { Message = "Only Admin or Bank-Staff can create Customers!" };

            var user = new AppUser 
            { 
                UserName = model.Email, 
                Email = model.Email, 
                FullName = model.FullName, 
                EmailConfirmed = true, 
                CreatedAt = DateTime.UtcNow 
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return new AuthResponse { Message = "Registration failed!" };

            await _userManager.AddToRoleAsync(user, model.Role);

            return new AuthResponse { Message = "User registered successfully!" };
        }

        public Task<bool> RevokeToken(string token)
        {
            throw new NotImplementedException();
        }

        private string GenerateJwtToken(AppUser user)
        {
            var role = _userManager.GetRolesAsync(user).Result;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role.FirstOrDefault())
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Issuer"], claims, expires: DateTime.UtcNow.AddMinutes(30), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedByIp = GetIpAddress(),
                ReplacedByToken = string.Empty
            };
        }

        private string GetIpAddress()
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress;

            if (ipAddress != null)
            {
                // Handle IPv4-mapped IPv6 addresses
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    ipAddress = ipAddress.MapToIPv4();
                }
                return ipAddress.ToString();
            }

            return "Unknown IP";
        }
    }
}
