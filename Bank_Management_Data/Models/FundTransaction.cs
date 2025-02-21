using Bank_Management_Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Management_Data.Models
{
    public class FundTransaction
    {
        public string TransactionId { get; set; }
        public string AccountNumber { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string? TargetAccountNumber { get; set; }

        // Navigation Properties
        public Account Account { get; set; }
    }

}
