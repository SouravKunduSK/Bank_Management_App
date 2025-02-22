using Bank_Management_Data.Models;

namespace Bank_Management_Api.Services.Interfaces
{
    public interface IInterestService
    {

            Task<bool> CalculateAndApplyInterestAsync();  // This matches your implementation
            Task<List<Interest>> GetInterestDetailsAsync(string accountNumber);
    }
}
