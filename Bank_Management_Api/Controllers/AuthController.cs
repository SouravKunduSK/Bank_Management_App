using Bank_Management_Api.Models;
using Bank_Management_Api.Services;
using Bank_Management_Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

            return Ok(response);
        }
    }
}
