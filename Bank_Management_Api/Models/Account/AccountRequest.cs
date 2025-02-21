using Bank_Management_Data.Data;

namespace Bank_Management_Api.Models.Account
{
    public class AccountRequest
    {
        public int AccountTypeId { get; set; }
        public decimal InitialDeposit { get; set; }
        public string CurrencyCode { get; set; } = "BDT"; // Reference to Currency model
    }
}
