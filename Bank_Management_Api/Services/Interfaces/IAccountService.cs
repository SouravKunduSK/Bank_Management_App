using Bank_Management_Api.Models.Account;

namespace Bank_Management_Api.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponse> CreateAccountAsync(AccountRequest request, string userId);
        Task<AccountResponse> GetAccountDetailsAsync(string accountNumber, string userId);
        Task<List<AccountResponse>> GetAllAccountsAsync(string userId);
        Task<bool> ApproveAccountAsync(string accountNumber);
        Task<bool> FreezeAccountAsync(string accountNumber);
        Task<bool> CloseAccountAsync(string accountNumber);
        Task<bool> DeleteAccountAsync(string accountNumber);
    }
}
