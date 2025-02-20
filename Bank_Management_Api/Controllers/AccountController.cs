using Bank_Management_Api.Models.Account;
using Bank_Management_Api.Services.Interfaces;
using Bank_Management_Data.Data;
using Bank_Management_Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace Bank_Management_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly AppDbContext _context;

        public AccountController(IAccountService accountService, AppDbContext context)
        {
            _accountService = accountService;
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.Actor)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var account = await _accountService.CreateAccountAsync(request, userId);
            return Ok(account);
        }

        [HttpGet("details/{accountNumber}")]
        public async Task<IActionResult> GetAccountDetails(string accountNumber)
        {
            var userId = User.FindFirst(ClaimTypes.Actor)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            // Get user roles
            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            AccountResponse account;
            if (roles.Contains("BankStaff") || roles.Contains("Admin"))
            {
                account = await _accountService.GetAccountDetailsAsync(accountNumber, userId = null); // Get all accounts
            }
            else if (roles.Contains("Customer"))
            {
                account = await _accountService.GetAccountDetailsAsync(accountNumber, userId); // Get customer-specific accounts
            }
            else
            {
                return Forbid();
            }

            
            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAccounts()
        {
            
            var userId = User.FindFirst(ClaimTypes.Actor)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            // Get user roles
            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            List<AccountResponse> accounts;
            if (roles.Contains("BankStaff") || roles.Contains("Admin"))
            {
                accounts = await _accountService.GetAllAccountsAsync(userId = null); // Get all accounts
            }
            else if (roles.Contains("Customer"))
            {
                accounts = await _accountService.GetAllAccountsAsync(userId); // Get customer-specific accounts
            }
            else
            {
                return Forbid();
            }    
            return Ok(accounts);
        }
        [Authorize(Roles = "BankStaff, Admin")]
        [HttpPost("approve/{accountNumber}")]
        public async Task<IActionResult> ApproveAccount(string accountNumber)
        {
            var success = await _accountService.ApproveAccountAsync(accountNumber);
            //Send accountholder a notification
            var account = await _context.Accounts
                                .Include(u=>u.User)
                                .FirstOrDefaultAsync(ac=>ac.AccountNumber == accountNumber);
            if(account!=null)
            {
                var customerEmail = account.User?.Email;
                if (!string.IsNullOrEmpty(customerEmail))
                {
                    //Send email of account activation
                }
            }
            return success ? Ok("Acount Activated!") : BadRequest();
        }
        [Authorize(Roles = "BankStaff, Admin")]
        [HttpPost("freeze/{accountNumber}")]
        public async Task<IActionResult> FreezeAccount(string accountNumber)
        {
            var success = await _accountService.FreezeAccountAsync(accountNumber);
            //Send accountholder a notification
            var account = await _context.Accounts
                                .Include(u => u.User)
                                .FirstOrDefaultAsync(ac => ac.AccountNumber == accountNumber);
            if (account != null)
            {
                var customerEmail = account.User?.Email;
                if (!string.IsNullOrEmpty(customerEmail))
                {
                    //Send email of account freeze
                }
            }
            return success ? Ok() : BadRequest();
        }
        [Authorize(Roles = "BankStaff, Admin")]
        [HttpPost("close/{accountNumber}")]
        public async Task<IActionResult> CloseAccount(string accountNumber)
        {
            var success = await _accountService.CloseAccountAsync(accountNumber);
            //Send accountholder a notification
            var account = await _context.Accounts
                                .Include(u => u.User)
                                .FirstOrDefaultAsync(ac => ac.AccountNumber == accountNumber);
            if (account != null)
            {
                var customerEmail = account.User?.Email;
                if (!string.IsNullOrEmpty(customerEmail))
                {
                    //Send email of account closed
                }
            }
            return success ? Ok() : BadRequest();
        }
        [Authorize(Roles = "BankStaff, Admin")]
        [HttpDelete("{accountNumber}")]
        public async Task<IActionResult> DeleteAccount(string accountNumber)
        {
            var success = await _accountService.DeleteAccountAsync(accountNumber);
            //Send accountholder a notification
            var account = await _context.Accounts
                                .Include(u => u.User)
                                .FirstOrDefaultAsync(ac => ac.AccountNumber == accountNumber);
            if (account != null)
            {
                var customerEmail = account.User?.Email;
                if (!string.IsNullOrEmpty(customerEmail))
                {
                    //Send email of account delete
                }
            }
            return success ? Ok() : BadRequest();
        }
    }

}
