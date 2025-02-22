using Bank_Management_Api.Services.Interfaces;
using Bank_Management_Data.Data;
using Bank_Management_Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Bank_Management_Api.Services
{
    public class LoanService : ILoanService
    {
        private readonly AppDbContext _context;

        public LoanService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Loan> ApplyForLoanAsync(Loan loan)
        {
            loan.Status = LoanStatus.Pending;
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return loan;
        }

        public async Task<bool> ApproveLoanAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null)
            {
                return false; // Or handle the case where the loan is not found
            }
            loan.Status = LoanStatus.Approved;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectLoanAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);

            if (loan == null)
            {
                return false; // Or handle the case where the loan is not found
            }

            loan.Status = LoanStatus.Rejected;
            await _context.SaveChangesAsync();

            return true; // Indicating success
        }

        public async Task<bool> RepayLoanAsync(int loanId, decimal amount)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null)
            {
                return false; // Or handle the case where the loan is not found
            }
            loan.LoanAmount -= amount;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Loan>> GetLoanDetailsAsync(string accountNumber)
        {
            return await _context.Loans
                .Where(l => l.Account.AccountNumber == accountNumber)
                .ToListAsync();
        }

        public async Task<List<Loan>> GetAllLoanAsync()
        {
            return await _context.Loans.Include(l => l.LoanType).ToListAsync();
        }
        public async Task<Loan> GetLoanByIdAsync(int id)
        {
            var loan = await _context.Loans.Include(l => l.LoanType).FirstOrDefaultAsync(l => l.LoanId == id);
            if (loan == null)
                throw new Exception("Not Found!");
            return loan;
        }
    }

}
