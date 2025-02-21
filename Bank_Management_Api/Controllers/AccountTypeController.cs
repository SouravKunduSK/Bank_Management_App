using Bank_Management_Api.Models.Account;
using Bank_Management_Api.Services;
using Bank_Management_Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bank_Management_Api.Controllers
{
    [Authorize(Roles = "Admin, BankStaff")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountTypeController : ControllerBase
    {
        private readonly IAccountTypeService _accountTypeService;

        public AccountTypeController(IAccountTypeService accountTypeService)
        {
            _accountTypeService = accountTypeService;
        }
        // GET: api/<AccountTypeController>
        [HttpGet("all-account-types")]
        public async Task<IActionResult> GetAllTypes()
        {
            var accountTypes = await _accountTypeService.GetAllAccountTypesAsync();
            if (accountTypes == null)
            {
                return NotFound();
            }
            return Ok(accountTypes);
        }

        // GET api/<AccountTypeController>/5
        [HttpGet("detail/{typeName}")]
        public async Task<IActionResult> GetType(string typeName)
        {
            if (typeName == null)
            {
                return BadRequest("Type name cannot be null!");
            }
            var type = await _accountTypeService.GetAccountTypeByNameAsync(typeName);
            if (type == null)
            {
                return NotFound("No type detail found!");
            }
            return Ok(type);
        }

        // POST api/<AccountTypeController>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewTypeWithLimit([FromBody] AccountTypeRequest accountType)
        {
            if (accountType == null)
            {
                return BadRequest("Cannot be null!");
            }
            var response = await _accountTypeService.AddAccountTypeAsync(accountType);
            if (response == null)
            {
                return BadRequest("Cannot be null!");
            }
            return Ok(response);
        }

        // PUT api/<AccountTypeController>/5
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditTypeOrLimit(int id, [FromBody] AccountTypeRequest accountType)
        {
            if(id!>0 || accountType == null)
            {
                return BadRequest("Edit is not possible!");
            }
            var response = await _accountTypeService.UpdateAccountTypeAsync(id, accountType);
            if(response == null)
            {
                return BadRequest();
            }
            return Ok(response);

        }

        // DELETE api/<AccountTypeController>/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var response = await _accountTypeService.DeleteAccountTypeAsync(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);

        }
    }
}
