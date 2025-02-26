using Bank_Management_Api.Helpers;
using Bank_Management_Api.Models.Transactions;
using Bank_Management_Api.Services.Interfaces;
using Bank_Management_Data.Data;
using Bank_Management_Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using Bank_Management_Api.Helpers;

namespace Bank_Management_Api.Services
{
    public class TransactionService:ITransactionService
    {
        private readonly AppDbContext _context;
        private readonly CheckLimits _limits;

        public TransactionService(AppDbContext context, CheckLimits limits)
        {
            _context = context;
            _limits = limits;
        }

        public async Task<TransactionResponse> DepositAsync(TransactionRequest request, string userId)
        {

            if (request.Amount <= 0)
                throw new Exception("Amount must be greater than zero.");

            var account = await _context.Accounts
                .Include(u=>u.User)
                .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber && a.UserId == userId);

            if (account == null)
            {
                throw new Exception("Account not found!");
            }
            
            if (await _limits.IfLimitExceeds(account))
            {
                throw new Exception("Limit excceds for today!");
            }
            if (account.Status != AccountStatus.Active)
            {
                throw new Exception($"Account status {account.Status}");
            }

            var previousBalance = account.Balance;
            account.Balance += request.Amount;

            var transaction = new FundTransaction
            {
                TransactionId = GenerateGUIDNumber.GenerateNumberUsingGuid(),
                AccountNumber = request.AccountNumber,
                Type = TransactionType.Deposit,
                Amount = request.Amount,
                Account = account
            };

            // Begin a database transaction
            using (var transactionDb = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Add the transaction to the context
                    _context.Transactions.Add(transaction);

                    // Update the account balance in the context
                    _context.Accounts.Update(account);

                    // Save changes (this will save both the account update and the transaction)
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transactionDb.CommitAsync();

                    // Return the response data
                    var responseData = new TransactionResponse
                    {
                        TransactionId = transaction.TransactionId,
                        AccountNumber = account.AccountNumber,
                        Type = transaction.Type,
                        Amount = transaction.Amount,
                        TransactionDate = transaction.TransactionDate,
                        PreviousBalance = previousBalance,
                        CurrentBalance = account.Balance,
                        UserName = account.User.FullName
                    };

                    return responseData;
                }
                catch (Exception ex)
                {
                    // Rollback in case of an error
                    await transactionDb.RollbackAsync();
                    throw new Exception("Transaction failed: " + ex.Message);
                }
            }

        }

        public async Task<List<TransactionResponse>> GetTransactionHistoryAsync(string accountNumber, string userId)
        {
            var transactions = await _context.Transactions
           .Where(t => t.Account.AccountNumber == accountNumber && t.Account.UserId == userId)
           .Select(t => new TransactionResponse
           {
               TransactionId = t.TransactionId,
               AccountNumber = t.AccountNumber,
               TargetAccountNumber = t.TargetAccountNumber,
               Type = t.Type,
               Amount = t.Amount,
               TransactionDate = t.TransactionDate
           }).ToListAsync();

            return transactions;
        }

        public async Task<TransactionResponse> TransferAsync(TransactionRequest request, string userId)
        {
            if (request.Amount <= 0)
                throw new Exception("Amount must be greater than zero.");

            if (request.AccountNumber == request.TargetAccountNumber)
                throw new Exception("Cannot transfer to the same account!");

            var sourceAccount = await _context.Accounts
                .Include(u=>u.User)
                .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber && a.UserId == userId);

            var targetAccount = await _context.Accounts
                .Include(u=>u.User)
                .FirstOrDefaultAsync(a => a.AccountNumber == request.TargetAccountNumber);

            if (sourceAccount == null || targetAccount == null)
                throw new Exception("Account not found!");

            if (sourceAccount.Balance < request.Amount)
                throw new Exception("Insufficient funds!");
            if (await _limits.IfLimitExceeds(sourceAccount) || await _limits.IfLimitExceeds(targetAccount))
            {
                throw new Exception("Limit excceds for today!");
            }

            // Begin a database transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var prevBalance = sourceAccount.Balance;
                    // Perform the transfer
                    sourceAccount.Balance -= request.Amount;
                    targetAccount.Balance += request.Amount;

                    var transactionEntity = new FundTransaction
                    {
                        TransactionId = GenerateGUIDNumber.GenerateNumberUsingGuid(),
                        AccountNumber = request.AccountNumber,
                        TargetAccountNumber = request.TargetAccountNumber,
                        Type = TransactionType.Transfer,
                        Amount = request.Amount,
                        Account = sourceAccount
                    };
                    _context.Transactions.Add(transactionEntity);
                    _context.Accounts.Update(sourceAccount);
                    _context.Accounts.Update(targetAccount);
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return new TransactionResponse
                    {
                        TransactionId = transactionEntity.TransactionId,
                        AccountNumber = transactionEntity.AccountNumber,
                        TargetAccountNumber = transactionEntity.TargetAccountNumber,
                        Type = transactionEntity.Type,
                        Amount = transactionEntity.Amount,
                        TransactionDate = transactionEntity.TransactionDate,
                        UserName = sourceAccount.User.UserName,
                        CurrentBalance = sourceAccount.Balance,
                        PreviousBalance = prevBalance
                    };
                }
                catch (Exception)
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync();
                    throw; // Rethrow the exception
                }
            }
        }


        public async Task<TransactionResponse> WithdrawAsync(TransactionRequest request, string userId)
        {
            if (request.Amount <= 0)
                throw new Exception("Amount must be greater than zero.");

            var account = await _context.Accounts
                .Include(u=>u.User)
                .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber && a.UserId == userId);

            if (account == null)
                throw new Exception("Account not found!");

            if (account.Balance < request.Amount)
                throw new Exception("Insufficient funds!");
            if (await _limits.IfLimitExceeds(account))
            {
                throw new Exception("Limit excceds for today!");
            }
            if(account.Status != AccountStatus.Active)
            {
                throw new Exception($"Account status {account.Status}");
            }
            var prevBalance = account.Balance;
            account.Balance -= request.Amount;

            var transaction = new FundTransaction
            {
                TransactionId = GenerateGUIDNumber.GenerateNumberUsingGuid(),
                AccountNumber = request.AccountNumber,
                Type = TransactionType.Withdrawal,
                Amount = request.Amount,
                Account = account
            };

            // Begin a database transaction
            using (var transactionDb = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Add the transaction to the context
                    _context.Transactions.Add(transaction);

                    // Update the account balance in the context
                    _context.Accounts.Update(account);

                    // Save changes (this will save both the account update and the transaction)
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transactionDb.CommitAsync();

                    // Return the response data
                    var responseData = new TransactionResponse
                    {
                        TransactionId = transaction.TransactionId,
                        AccountNumber = account.AccountNumber,
                        Type = transaction.Type,
                        Amount = transaction.Amount,
                        TransactionDate = transaction.TransactionDate,
                        PreviousBalance = prevBalance,
                        CurrentBalance = account.Balance,
                        UserName = account.User.FullName
                    };

                    return responseData;
                }
                catch (Exception ex)
                {
                    // Rollback in case of an error
                    await transactionDb.RollbackAsync();
                    throw new Exception("Transaction failed: " + ex.Message);
                }
            }
        }
    }
}
