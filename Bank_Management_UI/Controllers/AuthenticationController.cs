using Bank_Management_UI.Models.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace Bank_Management_UI.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IHttpClientFactory httpClient, IConfiguration configuration)
        { 
            _httpClient = httpClient;
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult Login() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var client = _httpClient.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("Auth/login", model);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<AuthResponse>(content);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim("Token", loginResponse.Token),
                    new Claim("RefreshToken", loginResponse.RefreshToken),
                    new Claim(ClaimTypes.Role, loginResponse.UserRole)

                };
                // You can store claims in a ClaimsIdentity and set them to the User (to make the user authenticated)
                var identity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(identity);

                // Assuming you're using ASP.NET Core Identity, you can sign in the user
                await HttpContext.SignInAsync(principal);
                return RedirectToAction("Index","Home");
            }
            return View(model);
        }
    }
}
