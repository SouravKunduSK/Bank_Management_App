using Bank_Management_Api.Services.Interfaces;
using Bank_Management_Data.Data;
using Bank_Management_Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank_Management_Api.Services
{
    public class LoanRepaymentService : ILoanRepaymentService
    {
        private readonly AppDbContext _context;

        public LoanRepaymentService(AppDbContext context)
        {
            _context = context;
        }

        // Process Loan Repayment
        public async Task<bool> RepayLoan(int loanId, decimal amount)
        {
            var loan = await _context.Loans
                .Include(l => l.Account)
                .FirstOrDefaultAsync(l => l.LoanId == loanId);

            if (loan == null || loan.Status != LoanStatus.Active)
                return false;

            // Calculate penalty if overdue
            var penalty = await CalculatePenalty(loanId);
            var totalAmount = amount + penalty;

            // Check if repayment amount is sufficient
            if (totalAmount > loan.Account.Balance)
                return false;

            // Deduct from account balance
            loan.Account.Balance -= totalAmount;
            loan.LoanAmount -= amount;

            // If loan fully repaid, mark as Closed
            if (loan.LoanAmount <= 0)
                loan.Status = LoanStatus.Completed;

            // Record Repayment
            var repayment = new LoanRepayment
            {
                LoanId = loanId,
                Amount = amount,
                Penalty = penalty,
                RepaymentDate = DateTime.UtcNow
            };
            _context.LoanRepayments.Add(repayment);

            await _context.SaveChangesAsync();
            return true;
        }

        // Calculate Penalty for Overdue Payments
        public async Task<decimal> CalculatePenalty(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);

            if (loan == null)
                return 0;

            // Check if overdue
            if (loan.NextDueDate < DateTime.UtcNow)
            {
                var overdueDays = (DateTime.UtcNow - loan.NextDueDate).Value.Days;
                var penalty = overdueDays * loan.PenaltyRate;
                return penalty;
            }

            return 0;
        }

        // Get Repayment History
        public async Task<List<LoanRepayment>> GetRepaymentHistory(int loanId)
        {
            return await _context.LoanRepayments
                .Where(r => r.LoanId == loanId)
                .OrderByDescending(r => r.RepaymentDate)
                .ToListAsync();
        }

        public async Task<List<LoanRepayment>> GetRepaymentsByLoanIdAsync(int loanId)
        {
            return await _context.LoanRepayments
                           .Where(r => r.LoanId == loanId)
                           .OrderByDescending(r => r.RepaymentDate)
                           .ToListAsync();
        }
    }
}
