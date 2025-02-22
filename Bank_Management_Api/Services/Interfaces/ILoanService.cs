using Bank_Management_Data.Models;

namespace Bank_Management_Api.Services.Interfaces
{
    public interface ILoanService
    {
        Task<Loan> ApplyForLoanAsync(Loan loan);
        Task<bool> ApproveLoanAsync(int loanId);
        Task<bool> RejectLoanAsync(int loanId);
        Task<bool> RepayLoanAsync(int loanId, decimal amount);
        Task<List<Loan>> GetLoanDetailsAsync(string accountNumber);
        Task <Loan> GetLoanByIdAsync(int id);
        Task<List<Loan>> GetAllLoanAsync();
    }
}
