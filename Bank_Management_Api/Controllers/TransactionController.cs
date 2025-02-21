using Azure.Core;
using Bank_Management_Api.Models.Transactions;
using Bank_Management_Api.Services;
using Bank_Management_Api.Services.Interfaces;
using Bank_Management_Data.Data;
using Bank_Management_Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Bank_Management_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly AppDbContext _context;

        public TransactionController(ITransactionService transactionService, AppDbContext context)
        {
            _transactionService = transactionService;
            _context = context;
        }
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] TransactionRequest request)
        {

            var userId = await GetUserId(request.AccountNumber);
            var response = await _transactionService.DepositAsync(request, userId);
            if(response == null)
                return BadRequest("Deposit is not possible!");
            return Ok(response);
        }
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] TransactionRequest request)
        {
            var userId = await GetUserId(request.AccountNumber);
            var response = await _transactionService.WithdrawAsync(request, userId);
            return Ok(response);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransactionRequest request)
        {
            var userId = await GetUserId(request.AccountNumber);
            var response = await _transactionService.TransferAsync(request, userId);
            return Ok(response);
        }

        [HttpGet("history/{accountNumber}")]
        public async Task<IActionResult> GetTransactionHistory(string accountNumber)
        {
            var userId = await GetUserId(accountNumber);
            var response = await _transactionService.GetTransactionHistoryAsync(accountNumber, userId);
            return Ok(response);
        }
        private async Task<string> GetUserId(string accountNumber) 
        {
            string? userId;
            if (User.IsInRole("Admin") || User.IsInRole("BankStaff"))
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(ac => ac.AccountNumber == accountNumber);
                userId = account?.UserId;
            }
            else if (User.IsInRole("Customer"))
            {
                userId = User.FindFirst(ClaimTypes.Actor)?.Value;
            }
            else
            {
                throw new InvalidOperationException();
            }
            
            return userId;
        } 
    }
}
