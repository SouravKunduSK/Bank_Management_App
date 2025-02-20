using Bank_Management_Data.Data;

namespace Bank_Management_Api.Models.Transactions
{
    public class TransactionResponse
    {
        public string TransactionId { get; set; }
        public string AccountNumber { get; set; }
        public  TransactionType Type { get; set; } // Deposit, Withdrawal, Transfer
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TargetAccountNumber { get; set; }
    }
}
