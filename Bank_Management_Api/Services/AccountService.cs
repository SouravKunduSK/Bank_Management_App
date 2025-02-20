using Bank_Management_Api.Models.Account;
using Bank_Management_Api.Services.Interfaces;
using Bank_Management_Data.Data;
using Bank_Management_Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Bank_Management_Api.Helpers;

namespace Bank_Management_Api.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly ICurrencyService _currencyService;

        public AccountService(AppDbContext context, ICurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        public async Task<AccountResponse> CreateAccountAsync(AccountRequest request, string userId)
        {
            if (request == null || string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(request));

            // Check if currency is valid
           /* var currency = await _context.Currencies.FirstOrDefaultAsync(c => c.CurrencyId == request.CurrencyId);
            if (currency == null)
            {
                throw new Exception("Invalid currency.");
            }*/

            // Get exchange rate to base currency (e.g., USD)
            var baseCurrencyCode = "BDT"; // You can set this dynamically
            var exchangeRate = await _currencyService.GetExchangeRateAsync(request.CurrencyCode, baseCurrencyCode);
            var currency = await _context.Currencies.FirstOrDefaultAsync(c=>c.CurrencyCode == request.CurrencyCode);
            if(currency == null)
            {
                var newCurrency = new Currency
                {
                    CurrencyCode = request.CurrencyCode,
                    ExchangeRate = exchangeRate,
                    CurrencySymbol = string.Empty
                };
                await _context.Currencies.AddAsync(newCurrency);
                await _context.SaveChangesAsync();
                currency = newCurrency;
            }
            var account = new Account
            {
                AccountNumber = GenerateAccountNumberUsingGuid(), // Unique Account Number
                Type = request.Type,
                Balance = request.InitialDeposit * exchangeRate,
                UserId = userId,
                CurrencyId = currency.CurrencyId
            };

            var transaction = new Transaction
            {
                TransactionId = GenerateGUIDNumber.GenerateNumberUsingGuid(),
                AccountNumber = account.AccountNumber,
                Type = TransactionType.Deposit,
                Amount = account.Balance
            };


            await _context.Accounts.AddAsync(account);
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return new AccountResponse
            {
                AccountNumber = account.AccountNumber,
                Type = account.Type,
                Balance = account.Balance,
                Status = account.Status,
                CreatedAt = account.CreatedAt,
                CurrencyCode = currency.CurrencyCode,
                CurrencySymbol = currency.CurrencySymbol,
                CurrencyExchangeRate = exchangeRate
            };
        }

        public async Task<AccountResponse> GetAccountDetailsAsync(string accountNumber, string userId = null)
        {
            var query = _context.Accounts
                    .Include(a => a.Currency);

            Account account;
            if (userId == null)
            {
                // If userId is null, get the account without filtering by userId
                account = await query.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            }
            else
            {
                // If userId is provided, filter by account number and userId
                account = await query.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber && a.UserId == userId);
            }
            if (account == null)
                throw new Exception("Account not found.");

            return new AccountResponse
            {
                AccountNumber = account.AccountNumber,
                Type = account.Type,
                Balance = account.Balance,
                Status = account.Status,
                CreatedAt = account.CreatedAt,
                CurrencyCode = account.Currency.CurrencyCode,
                CurrencySymbol = account.Currency.CurrencySymbol,
                CurrencyExchangeRate = account.Currency.ExchangeRate
            };
        }

        public async Task<List<AccountResponse>> GetAllAccountsAsync(string userId=null)
        {
            IQueryable<Account> query =  _context.Accounts
                                .Include(a => a.Currency);
            if(!string.IsNullOrEmpty(userId))
            {
                query = query.Where(a => a.UserId == userId);
            }
            var accounts = await query.ToListAsync();

            return accounts.Select(account => new AccountResponse
            {
                AccountNumber = account.AccountNumber,
                Type = account.Type,
                Balance = account.Balance,
                Status = account.Status,
                CreatedAt = account.CreatedAt,
                CurrencyCode = account.Currency.CurrencyCode,
                CurrencySymbol = account.Currency.CurrencySymbol,
                CurrencyExchangeRate = account.Currency.ExchangeRate
            }).ToList();
        }

        public async Task<bool> ApproveAccountAsync(string accountNumber)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
                throw new Exception("Account not found.");
            if (account.Status == AccountStatus.Active)
                throw new Exception("Already Activated!");
            account.Status = AccountStatus.Active;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FreezeAccountAsync(string accountNumber)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
                throw new Exception("Account not found.");

            account.Status = AccountStatus.Frozen;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CloseAccountAsync(string accountNumber)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
                throw new Exception("Account not found.");

            account.Status = AccountStatus.Closed;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAccountAsync(string accountNumber)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
                throw new Exception("Account not found.");

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateAccountNumberUsingGuid()
        {
            string guidNumbers = new string(Guid.NewGuid().ToString("N").Where(char.IsDigit).ToArray());
            string part1 = guidNumbers.Substring(0, 5);
            string part2 = guidNumbers.Substring(5, 5);

            return $"{part1}-{part2}";
        }
    }

}
