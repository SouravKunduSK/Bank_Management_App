namespace Bank_Management_Api.Models.Account
{
    public class AccountTypeResponse
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public int DailyTransactionNumberLimit { get; set; }
        public decimal DailyMoneyTransactionLimit { get; set; }
        public decimal TransactionFee { get; set; }
    }
}
