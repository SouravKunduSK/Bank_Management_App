using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Management_Data.Models
{
    public class Currency
    {
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; } // E.g., USD, EUR, GBP
        public string CurrencySymbol { get; set; } // E.g., $, €, £
        public decimal ExchangeRate { get; set; } // Exchange rate to a base currency (e.g., USD)

        // Navigation Property
        public List<Account> Accounts { get; set; } //one currency mayh be used in diff accounts
    }
}
