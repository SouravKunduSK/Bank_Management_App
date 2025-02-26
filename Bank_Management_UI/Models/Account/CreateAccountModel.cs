using System.ComponentModel.DataAnnotations;

namespace Bank_Management_UI.Models.Account
{
    public class CreateAccountModel
    {
        [Required(ErrorMessage ="Account Type shouldn't be null!")]
        public int AccountTypeId { get; set; }

        [Required(ErrorMessage = "Deposit is required!")]
        [Range(500, 1000000, ErrorMessage = "Deposit shouldn't be less than 500 BDT and greater than 1000000 BDT!")]
        public decimal InitialDeposit { get; set; }

        [Required(ErrorMessage = "Currency Code is required!")]
        public string CurrencyCode { get; set; } = "BDT";

        // Readonly user details
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
