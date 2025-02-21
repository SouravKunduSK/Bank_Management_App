using Bank_Management_Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Management_Data.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; } = AccountStatus.OnProcessing; // OnProcessing, Active, Frozen, Closed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //Account type
        public int AccountTypeId { get; set; }
        public AccountType AccountType { get; set; }

        // Currency-related
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        // Navigation Property
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public List<FundTransaction> Transactions { get; set; }
    }
}
