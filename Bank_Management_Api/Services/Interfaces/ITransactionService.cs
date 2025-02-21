using Bank_Management_Api.Models.Transactions;

namespace Bank_Management_Api.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponse> DepositAsync(TransactionRequest request, string userId);
        Task<TransactionResponse> WithdrawAsync(TransactionRequest request, string userId);
        Task<TransactionResponse> TransferAsync(TransactionRequest request, string userId);
        Task<List<TransactionResponse>> GetTransactionHistoryAsync(string accountNumber, string userId);
    }
}
