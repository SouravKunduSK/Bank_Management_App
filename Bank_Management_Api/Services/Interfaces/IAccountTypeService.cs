using Bank_Management_Api.Models.Account;

namespace Bank_Management_Api.Services.Interfaces
{
    public interface IAccountTypeService
    {
        Task<List<AccountTypeResponse>> GetAllAccountTypesAsync();
        Task<AccountTypeResponse> GetAccountTypeByNameAsync(string typeName);
        Task<AccountTypeResponse> AddAccountTypeAsync(AccountTypeRequest request);
        Task<AccountTypeResponse> UpdateAccountTypeAsync(int id, AccountTypeRequest request);
        Task<AccountTypeResponse> DeleteAccountTypeAsync(int id);
    }
}
