using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Management_Data.Models
{
    public class AccountType
    {
        [Key]
        public int Id { get; set; }
        public string TypeName { get; set; } // e.g., Savings, Current, Business
        public int DailyTransactionNumberLimit { get; set; }
        public decimal DailyMoneyTransactionLimit { get; set; }
        public decimal TransactionFee { get; set; }

        // Navigation Property
        public List<Account> Accounts { get; set; }
    }
}
