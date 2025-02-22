using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_Api.Controllers
{
    using Bank_Management_Api.Models.Loan;
    using Bank_Management_Api.Services.Interfaces;
    using Bank_Management_Data.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        // Customer Apply for a Loan
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForLoan([FromBody] Loan loan)
        {
            var result = await _loanService.ApplyForLoanAsync(loan);
            return Ok(result);
        }

        // Admin/BankStaff Approve Loan Request
        [HttpPost("approve/{loanId}")]
        public async Task<IActionResult> ApproveLoan(int loanId)
        {
            var result = await _loanService.ApproveLoanAsync(loanId);
            return Ok(result);
        }

        // Admin/BankStaff Reject Loan Request
        [HttpPost("reject/{loanId}")]
        public async Task<IActionResult> RejectLoan(int loanId)
        {
            var result = await _loanService.RejectLoanAsync(loanId);
            return Ok(result);
        }

        // Loan Repayment
        [HttpPost("repay")]
        public async Task<IActionResult> RepayLoan([FromBody] LoanRepaymentRequest repayment)
        {
            var result = await _loanService.RepayLoanAsync(repayment.LoanId, repayment.Amount);
            return Ok(result);
        }

        // View Loan Details
        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetLoanDetails(string accountNumber)
        {
            var loan = await _loanService.GetLoanDetailsAsync(accountNumber);
            if (loan == null)
                return NotFound();
            return Ok(loan);
        }
    }

}
