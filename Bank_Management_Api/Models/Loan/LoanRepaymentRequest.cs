namespace Bank_Management_Api.Models.Loan
{
    public class LoanRepaymentRequest
    {
        public int LoanId { get; set; }
        public decimal Amount { get; set; }
    }
}
