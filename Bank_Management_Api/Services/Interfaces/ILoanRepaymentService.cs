using Bank_Management_Data.Models;

namespace Bank_Management_Api.Services.Interfaces
{
    public interface ILoanRepaymentService
    {
        // Process Loan Repayment
        Task<bool> RepayLoan(int loanId, decimal amount);

        // Calculate Penalty for Overdue Payments
        Task<decimal> CalculatePenalty(int loanId);

        // Get Repayment History
        Task<List<LoanRepayment>> GetRepaymentHistory(int loanId);

        Task<List<LoanRepayment>> GetRepaymentsByLoanIdAsync(int loanId);
    }
}
