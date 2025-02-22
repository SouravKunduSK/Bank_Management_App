using Bank_Management_Api.Helpers;
using Bank_Management_Api.Services.Interfaces;
using Bank_Management_Data.Data;
using Bank_Management_Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank_Management_Api.Services
{
    public class InterestService : IInterestService
    {
        private readonly AppDbContext _context;

        public InterestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CalculateAndApplyInterestAsync()
        {
            var activeLoans = await _context.Loans
                                            .Where(l => l.Status == LoanStatus.Active)
                                            .Include(l => l.LoanType)
                                            .ToListAsync();


            foreach (var loan in activeLoans)
            {
                var interest = InterestCalculation.CalculateInterest(loan.LoanAmount, loan.LoanType.DefaultInterestRate, 1);
                loan.LoanAmount += interest;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task CalculateAndCreditInterestAsync()
        {
            var completedLoans = await _context.Loans
                                               .Where(l => l.Status == LoanStatus.Completed)
                                               .ToListAsync();

            foreach (var loan in completedLoans)
            {
                var interest = InterestCalculation.CalculateInterest(loan.LoanAmount, loan.LoanType.DefaultInterestRate, 1);
                // Logic to credit to customer account can be added here
            }
        }

        public async Task<List<Interest>> GetInterestDetailsAsync(string accountNumber)
        {
            // Implement logic to retrieve interest details for a given account number.
            return await _context.Interests.Where(i => i.AccountNumber == accountNumber).ToListAsync();
        }
    }

}
