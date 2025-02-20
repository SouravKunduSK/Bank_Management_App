using Bank_Management_Data.Data;
using System.ComponentModel.DataAnnotations;

namespace Bank_Management_Api.Models.Transactions
{
    public class TransactionRequest
    {
        public string AccountNumber { get; set; }
        [Required(ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }
        public string? TargetAccountNumber { get; set; }
    }
}
