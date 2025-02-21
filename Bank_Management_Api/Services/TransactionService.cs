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
                .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber && a.UserId == userId);

            if (account == null)
            {
                throw new Exception("Account not found!");
            }
            
            if (await _limits.IfLimitExceeds(account))
            {
                throw new Exception("Limit excceds for today!");
            }
            
            account.Balance += request.Amount;
            var transaction = new FundTransaction
            {
                TransactionId = GenerateGUIDNumber.GenerateNumberUsingGuid(),
                AccountNumber = request.AccountNumber,
                Type = TransactionType.Deposit,
                Amount = request.Amount
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var responseData = new TransactionResponse
            {
                TransactionId = transaction.TransactionId,
                AccountNumber = account.AccountNumber,
                Type = transaction.Type,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate
            };
            return responseData;

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
                .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber && a.UserId == userId);

            var targetAccount = await _context.Accounts
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
                        TransactionDate = transactionEntity.TransactionDate
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
                .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber && a.UserId == userId);

            if (account == null)
                throw new Exception("Account not found!");

            if (account.Balance < request.Amount)
                throw new Exception("Insufficient funds!");
            if (await _limits.IfLimitExceeds(account))
            {
                throw new Exception("Limit excceds for today!");
            }
            account.Balance -= request.Amount;

            var transaction = new FundTransaction
            {
                TransactionId = GenerateGUIDNumber.GenerateNumberUsingGuid(),
                AccountNumber = request.AccountNumber,
                Type = TransactionType.Withdrawal,
                Amount = request.Amount,
                Account = account
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new TransactionResponse
            {
                TransactionId = transaction.TransactionId,
                AccountNumber = transaction.AccountNumber,
                Type = transaction.Type,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate
            };
        }
    }
}
