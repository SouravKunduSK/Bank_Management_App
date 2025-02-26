using Bank_Management_Data.Data;
using Bank_Management_Data.Models;
using Bank_Management_UI.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Bank_Management_UI.Helpers;
using Bank_Management_Api.Models.Account;
namespace Bank_Management_UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClient;


        public AccountController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var accessToken = HttpContext.User.Claims.FirstOrDefault(t=>t.Type == "Token").Value;
            var userMail = User.FindFirstValue(ClaimTypes.Name);
            var url = $"Auth/user-data?email={Uri.EscapeDataString(userMail)}";
            // Fetching account types for dropdown
            var client = _httpClient.CreateClient("ApiClient");

            // Add the Bearer token to the Authorization header
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            AuthorizeToken.DoAuthorization(client, HttpContext);
            var userResponse = await client.GetAsync(url);

                var userData = userResponse.Content.ReadAsStringAsync().Result;
                var user = JsonConvert.DeserializeObject<UserResponse>(userData);
                var model = new CreateAccountModel
                {
                    FullName = user.FullName,
                    Email = user.Email
                };
            var response = await client.GetAsync("AccountType/all-account-types");
            if (response.IsSuccessStatusCode)
            {
                var accountTypes = await response.Content.ReadFromJsonAsync<List<AccountType>>();
                ViewBag.AccountTypes = accountTypes;
            }
            // Currency dropdown options
            ViewBag.Currencies = new List<string> { "BDT", "USD", "EUR", "INR" };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccountModel model)
        {
            var client = _httpClient.CreateClient("ApiClient");
            if (!ModelState.IsValid)
            {
                // Fetching account types for dropdown
                 
                 var accountResponse = await client.GetAsync("AccountType/all-account-types");
                if (accountResponse.IsSuccessStatusCode)
                {
                    var accountTypes = await accountResponse.Content.ReadFromJsonAsync<List<AccountType>>();
                    ViewBag.AccountTypes = accountTypes;
                }
                // Currency dropdown options
                ViewBag.Currencies = new List<string> { "BDT", "USD", "EUR", "INR" };
                return View(model);
            }
            AuthorizeToken.DoAuthorization(client, HttpContext);
            var response = await client.PostAsJsonAsync("Account/create", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("AccountList");
            }

            ModelState.AddModelError("", "Failed to create account.");
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AccountList()
        {
            var client = _httpClient.CreateClient("ApiClient");
            AuthorizeToken.DoAuthorization(client, HttpContext);
            var response = await client.GetAsync("Account/all-accounts");
            if (response.IsSuccessStatusCode) 
            {
                var accounts = await response.Content.ReadFromJsonAsync<List<AccountResponse>>();
                return View(accounts);
            }

            return View(new List<AccountResponse>());
        }
    }
}
