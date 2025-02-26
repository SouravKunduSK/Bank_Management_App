using Bank_Management_Api.Models.Transactions;
using Bank_Management_Data.Models;
using Bank_Management_UI.Helpers;
using Bank_Management_UI.Models.Transaction;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;

namespace Bank_Management_UI.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IHttpClientFactory _httpClient;

        public TransactionController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet]
        public async Task <IActionResult> DepositMoney()
        {
            var userMail = User.FindFirstValue(ClaimTypes.Name);
            var client = _httpClient.CreateClient("ApiClient");
            AuthorizeToken.DoAuthorization(client, HttpContext);
            var userName = await UserName.FindUserName(client, HttpContext, userMail);
            var model = new ResponseVM
            {
                UserName = userName
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DepositMoney(ResponseVM model)
        {
            var client = _httpClient.CreateClient("ApiClient");
            if (model.Amount <= 0 && model.AccountNumber == null)
            {
                return View(model);
            }
            AuthorizeToken.DoAuthorization(client, HttpContext);
            var response = await client.PostAsJsonAsync("Transaction/deposit", model);
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var responseContent = await response.Content.ReadAsStringAsync();
                // Assuming the response is a JSON object with a DepositAmount field
                var responseModel = JsonConvert.DeserializeObject<ResponseVM>(responseContent);

                var rmodel = new ResponseVM
                {
                    AccountNumber = responseModel.AccountNumber,
                    UserName = responseModel.UserName,
                    Amount = responseModel.Amount,
                    PreviousBalance = responseModel.PreviousBalance,
                    CurrentBalance = responseModel.CurrentBalance,
                    TransactionId = responseModel.TransactionId
                };
                return View(rmodel);
            }
            ModelState.AddModelError("", "Deposit is failed!");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> WithDrawMoney()
        {
            var userMail = User.FindFirstValue(ClaimTypes.Name);
            var client = _httpClient.CreateClient("ApiClient");
            AuthorizeToken.DoAuthorization(client, HttpContext);
            var userName = await UserName.FindUserName(client, HttpContext, userMail);
            var model = new ResponseVM
            {
                UserName = userName
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WithDrawMoney(ResponseVM model)
        {
            var client = _httpClient.CreateClient("ApiClient");
            if (model.Amount <= 0 && model.AccountNumber == null)
            {
                return View(model);
            }
            AuthorizeToken.DoAuthorization(client, HttpContext);
            var response = await client.PostAsJsonAsync("Transaction/withdraw", model);
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var responseContent = await response.Content.ReadAsStringAsync();
                // Assuming the response is a JSON object with a DepositAmount field
                var responseModel = JsonConvert.DeserializeObject<ResponseVM>(responseContent);

                var rmodel = new ResponseVM
                {
                    AccountNumber = responseModel.AccountNumber,
                    UserName = responseModel.UserName,
                    Amount = responseModel.Amount,
                    PreviousBalance = responseModel.PreviousBalance,
                    CurrentBalance = responseModel.CurrentBalance,
                    TransactionId = responseModel.TransactionId
                };
                return View(rmodel);
            }
            ModelState.AddModelError("", "Withdraw is failed!");
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> TransferMoney()
        {
            var userMail = User.FindFirstValue(ClaimTypes.Name);
            var client = _httpClient.CreateClient("ApiClient");
            AuthorizeToken.DoAuthorization(client, HttpContext);
            var userName = await UserName.FindUserName(client, HttpContext, userMail);
            var model = new ResponseVM
            {
                UserName = userName
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferMoney(ResponseVM model)
        {
            var client = _httpClient.CreateClient("ApiClient");
            if (model.Amount <= 0 || model.AccountNumber == null || model.TargetAccountNumber == null )
            {
                return View(model);
            }
            AuthorizeToken.DoAuthorization(client, HttpContext);
            var response = await client.PostAsJsonAsync("Transaction/transfer", model);
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var responseContent = await response.Content.ReadAsStringAsync();
                // Assuming the response is a JSON object with a DepositAmount field
                var responseModel = JsonConvert.DeserializeObject<ResponseVM>(responseContent);

                var rmodel = new ResponseVM
                {
                    AccountNumber = responseModel.AccountNumber,
                    TargetAccountNumber = responseModel.TargetAccountNumber,
                    UserName = responseModel.UserName,
                    Amount = responseModel.Amount,
                    PreviousBalance = responseModel.PreviousBalance,
                    CurrentBalance = responseModel.CurrentBalance,
                    TransactionId = responseModel.TransactionId
                };
                return View(rmodel);
            }
            ModelState.AddModelError("", "Transfer is failed!");
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> TransactionHistory()
        {
            var userMail = User.FindFirstValue(ClaimTypes.Name);
            var client = _httpClient.CreateClient("ApiClient");
            AuthorizeToken.DoAuthorization(client, HttpContext);
            var response = await client.GetAsync("Account/all-accounts");
            if (response.IsSuccessStatusCode)
            {
                var accounts = await response.Content.ReadFromJsonAsync<List<AccountNumberVM>>();
                ViewBag.Accounts = accounts;
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransactionHistory(AccountNumberVM model)
        {
            if (string.IsNullOrEmpty(model.AccountNumber))
            {
                // Handle the case where no account is selected
                return View(model);
            }
            var client = _httpClient.CreateClient("ApiClient");
            AuthorizeToken.DoAuthorization(client, HttpContext);
            // Fetch transactions for the selected account
            var response = await client.GetAsync($"Transaction/history/{model.AccountNumber}");
            if (response.IsSuccessStatusCode)
            {
                var transactions = await response.Content.ReadFromJsonAsync<List<TransactionResponse>>();
                ViewBag.Transactions = transactions;  // Store transactions in ViewBag
            }
            else
            {
                ViewBag.Transactions = new List<TransactionResponse>();  // Empty list if no transactions
            }

            // Retrieve accounts again to show them in dropdown
            var accountsResponse = await client.GetAsync("Account/all-accounts");
            if (accountsResponse.IsSuccessStatusCode)
            {
                var accounts = await accountsResponse.Content.ReadFromJsonAsync<List<AccountNumberVM>>();
                ViewBag.Accounts = accounts;  // Pass accounts back to the view
            }

            return View();
        }
    }
}
