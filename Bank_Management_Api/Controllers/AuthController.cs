using Bank_Management_Api.Models;
using Bank_Management_Api.Services;
using Bank_Management_Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Bank_Management_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        [HttpPost("create new user")]
        [Authorize(Roles = "Admin, BankStaff")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
         
            var creatorEmail = User.FindFirstValue(ClaimTypes.Email);
            var response = await _authService.RegisterUser(model, creatorEmail);

            if (response.Message.Contains("successfully"))
            {
                var password = model.Password;
                await _emailService.SendEmailAsync(model.Email, "Welcome to Our Bank!", 
                    "Congratulations! Your account has been created successfully in Demo Bank.<br>" +
                    "Login using:<br>"+
                    "<strong>Email: " + model.Email + "<br>"+
                    "Password: " + password +"</strong> <br> <br> <br>" +
                    "<small><strong>N:B: </strong>" +
                    " Please change your password after logging in, it's a weak password!</small>");
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var response = await _authService.Login(model);
            if (response.Token == null)
                return Unauthorized(response);
            var ipAddress = GetIpAddress();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var deviceType = GetDeviceType(userAgent);
            /*await _emailService.SendEmailAsync(model.Email, "New Login Alert",
        $"<p>New login detected on your account:</p>" +
        $"<ul>" +
        $"<li><strong>Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</li>" +
        $"<li><strong>Device:</strong> {deviceType}</li>" +
        $"<li><strong>IP Address:</strong> {ipAddress}</li>" +
        $"</ul>" +
        $"<p>If this wasn't you, secure your account immediately.</p>");*/
            return Ok(response);
        }
        [HttpGet("user-data")]
        public async Task<IActionResult> GetUserData(string email)
        {
            var response = await _authService.UserDetailAsync(email);
            if(response.FullName == "Not Found!")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            if (string.IsNullOrEmpty(model.Token))
                return BadRequest("Invalid token");

            var response = await _authService.RefreshToken(model.Token);

            if (response.Message != "Token refreshed successfully")
                return BadRequest(response.Message);

            return Ok(response);
        }

        [HttpPost("revoke-token")]
        [Authorize]  // Optional: Restrict to authenticated users
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenModel model)
        {
            if (string.IsNullOrEmpty(model.Token))
                return BadRequest("Token is required");

            var result = await _authService.RevokeToken(model.Token);

            if (!result)
                return BadRequest("Token revocation failed");

            return Ok("Token revoked successfully");
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var response = await _authService.Logout(refreshToken);

            if (response.Message == "Logout successful!")
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        // Models for request bodies
        public class RefreshTokenModel
        {
            public string Token { get; set; }
        }

        public class RevokeTokenModel
        {
            public string Token { get; set; }
        }
        private string GetIpAddress()
        {
            var ip = HttpContext.Connection.RemoteIpAddress;
            if (ip == null) return "Unknown IP";

            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                ip = ip.MapToIPv4();

            return ip.ToString();
        }

        private string GetDeviceType(string userAgent)
        {
            userAgent = userAgent.ToLower();
            if (userAgent.Contains("mobile")) return "Mobile";
            if (userAgent.Contains("tablet")) return "Tablet";
            if (userAgent.Contains("windows")) return "Windows PC";
            if (userAgent.Contains("mac")) return "Mac";
            if (userAgent.Contains("linux")) return "Linux PC";
            return "Unknown Device";
        }
    }
}
