namespace Bank_Management_UI.Models.Transaction
{
    public class ResponseVM
    {
        public string AccountNumber { get; set; }
        public string UserName { get; set; } // User's name to display
        public decimal Amount { get; set; } // The deposit amount that was deposited
        public decimal PreviousBalance { get; set; } // The balance before the deposit
        public decimal CurrentBalance { get; set; } // The balance after the deposit
        public string TransactionId { get; set; } // The transaction ID
        public string? TargetAccountNumber { get; set; } // The other account number to transfer money
    }
}
