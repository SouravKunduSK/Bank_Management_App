using Azure.Core;
using Bank_Management_Data.Data;
using Bank_Management_Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank_Management_Api.Helpers
{
    public class CheckLimits
    {
        private readonly AppDbContext _context;

        public CheckLimits(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IfLimitExceeds(Account account)
        {
            var accountType = await _context.AccountTypes
                        .Where(a => a.Id == account.AccountTypeId)
                        .FirstOrDefaultAsync();
            var transactionNumberCount = await _context.Transactions
                        .Where(t => t.AccountNumber == account.AccountNumber
                                    && t.TransactionDate.Date == DateTime.Now.Date)
                                    .CountAsync();
            var transactionMoney = await _context.Transactions
                        .Where(t => t.AccountNumber == account.AccountNumber
                                    && t.TransactionDate.Date == DateTime.Now.Date)
                                    .SumAsync(t=>t.Amount);
            if(transactionMoney >= accountType.DailyMoneyTransactionLimit 
               || transactionNumberCount >= accountType.DailyMoneyTransactionLimit)
            {
                return true;
            }
            return false;
        }
    }
}
