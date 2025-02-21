using Bank_Management_Api.Models.Account;
using Bank_Management_Api.Services.Interfaces;
using Bank_Management_Data.Data;
using Bank_Management_Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank_Management_Api.Services
{
    public class AccountTypeService : IAccountTypeService
    {
        
        private readonly AppDbContext _context;

        public AccountTypeService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<AccountTypeResponse> GetAccountTypeByNameAsync(string typeName)
        {
            var accountType = await _context.AccountTypes.FirstOrDefaultAsync(t=>t.TypeName==typeName);
            if (accountType == null)
                throw new Exception("Account Type not found!");
            var accountTypeDetail = new AccountTypeResponse
            {
                Id = accountType.Id,
                TypeName = accountType.TypeName,
                DailyMoneyTransactionLimit = accountType.DailyMoneyTransactionLimit,
                DailyTransactionNumberLimit = accountType.DailyTransactionNumberLimit,
                TransactionFee = accountType.TransactionFee
            };
            return accountTypeDetail;
        }

        public async Task<List<AccountTypeResponse>> GetAllAccountTypesAsync()
        {
            var accountTypes = await _context.AccountTypes
            .Select(at => new AccountTypeResponse
            {
                Id = at.Id,
                TypeName = at.TypeName,
                DailyMoneyTransactionLimit = at.DailyMoneyTransactionLimit,
                DailyTransactionNumberLimit = at.DailyTransactionNumberLimit,
                TransactionFee = at.TransactionFee
            }).ToListAsync();
            return accountTypes;
        }
        public async Task<AccountTypeResponse> AddAccountTypeAsync(AccountTypeRequest request)
        {
            var accountType = new AccountType
            {
                TypeName = request.TypeName,
                DailyMoneyTransactionLimit = request.DailyMoneyTransactionLimit,
                DailyTransactionNumberLimit = request.DailyTransactionNumberLimit,
                TransactionFee = request.TransactionFee
            };

            _context.AccountTypes.Add(accountType);
            await _context.SaveChangesAsync();
            var accountTypeDetail = new AccountTypeResponse
            {
                Id = accountType.Id,
                TypeName = accountType.TypeName,
                DailyMoneyTransactionLimit = accountType.DailyMoneyTransactionLimit,
                DailyTransactionNumberLimit = accountType.DailyTransactionNumberLimit,
                TransactionFee = accountType.TransactionFee
            };
            return accountTypeDetail;
        }

        public async Task<AccountTypeResponse> UpdateAccountTypeAsync(int id, AccountTypeRequest request)
        {
            var accountType = await _context.AccountTypes.FindAsync(id);
            if (accountType == null)
                throw new Exception("Account Type not found!");

            accountType.TypeName = request.TypeName;
            accountType.DailyMoneyTransactionLimit = request.DailyMoneyTransactionLimit;
            accountType.DailyTransactionNumberLimit = request.DailyTransactionNumberLimit;
            accountType.TransactionFee = request.TransactionFee;

            _context.AccountTypes.Update(accountType);
            await _context.SaveChangesAsync();
            var accountTypeDetail = new AccountTypeResponse
            {
                Id = accountType.Id,
                TypeName = accountType.TypeName,
                DailyMoneyTransactionLimit = accountType.DailyMoneyTransactionLimit,
                DailyTransactionNumberLimit = accountType.DailyTransactionNumberLimit,
                TransactionFee = accountType.TransactionFee
            };
            return accountTypeDetail;
        }

        public async Task<AccountTypeResponse> DeleteAccountTypeAsync(int id)
        {
            var accountType = await _context.AccountTypes.FindAsync(id);
            if (accountType == null)
                throw new Exception("Account Type not found!");

            _context.AccountTypes.Remove(accountType);
            await _context.SaveChangesAsync();
            var accountTypeDetail = new AccountTypeResponse
            {
                Id = accountType.Id,
                TypeName = accountType.TypeName,
                DailyMoneyTransactionLimit = accountType.DailyMoneyTransactionLimit,
                DailyTransactionNumberLimit = accountType.DailyTransactionNumberLimit,
                TransactionFee = accountType.TransactionFee
            };
            return accountTypeDetail;
        }
    }
}
