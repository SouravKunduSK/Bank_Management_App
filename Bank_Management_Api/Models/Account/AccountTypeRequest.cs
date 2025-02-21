namespace Bank_Management_Api.Models.Account
{
    public class AccountTypeRequest
    {
        public string TypeName { get; set; }
        public int DailyTransactionNumberLimit { get; set; }
        public decimal DailyMoneyTransactionLimit { get; set; }
        public decimal TransactionFee { get; set; }
    }
}
