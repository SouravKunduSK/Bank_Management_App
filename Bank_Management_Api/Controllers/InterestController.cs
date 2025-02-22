
using Bank_Management_Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class InterestController : ControllerBase
    {
        private readonly IInterestService _interestService;

        public InterestController(IInterestService interestService)
        {
            _interestService = interestService;
        }

        // Calculate and Apply Interest to Loan Balance
        [HttpPost("CalculateAndApplyInterest")]
        public async Task<IActionResult> CalculateAndApplyInterest()
        {
            var response = await _interestService.CalculateAndApplyInterestAsync();
            if (response)
                return Ok("Interest calculated and applied successfully.");
            return BadRequest(response);
        }

        // Get Interest Details for a Specific Account
        [HttpGet("GetInterestDetails/{accountNumber}")]
        public async Task<IActionResult> GetInterestDetails(string accountNumber)
        {
            var interestDetails = await _interestService.GetInterestDetailsAsync(accountNumber);
            if (interestDetails == null || !interestDetails.Any())
                return NotFound("No interest details found for the specified account.");

            return Ok(interestDetails);
        }

       
    }

}
