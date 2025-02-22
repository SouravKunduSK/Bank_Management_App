using Bank_Management_Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoanRepaymentController : ControllerBase
    {
        private readonly ILoanRepaymentService _loanRepaymentService;

        public LoanRepaymentController(ILoanRepaymentService loanRepaymentService)
        {
            _loanRepaymentService = loanRepaymentService;
        }

        // Process Loan Repayment
        [HttpPost("RepayLoan/{loanId}")]
        public async Task<IActionResult> RepayLoan(int loanId, [FromBody] decimal amount)
        {
            var result = await _loanRepaymentService.RepayLoan(loanId, amount);
            if (result)
                return Ok("Loan repayment successful.");
            else
                return BadRequest("Loan repayment failed.");
        }

        // Calculate Penalty for Overdue Payments
        [HttpGet("CalculatePenalty/{loanId}")]
        public async Task<IActionResult> CalculatePenalty(int loanId)
        {
            var penalty = await _loanRepaymentService.CalculatePenalty(loanId);
            return Ok(new { LoanId = loanId, Penalty = penalty });
        }

        // Get Repayment History
        [HttpGet("GetRepaymentHistory/{loanId}")]
        public async Task<IActionResult> GetRepaymentHistory(int loanId)
        {
            var history = await _loanRepaymentService.GetRepaymentHistory(loanId);
            if (history == null || !history.Any())
                return NotFound("No repayment history found for the specified loan.");

            return Ok(history);
        }

        // Get Repayments By Loan Id
        [HttpGet("GetRepaymentsByLoanId/{loanId}")]
        public async Task<IActionResult> GetRepaymentsByLoanId(int loanId)
        {
            var repayments = await _loanRepaymentService.GetRepaymentsByLoanIdAsync(loanId);
            if (repayments == null || !repayments.Any())
                return NotFound("No repayments found for the specified loan.");

            return Ok(repayments);
        }
    }
}
