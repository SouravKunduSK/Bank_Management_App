using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Management_Data.Models
{
    public class ExchangeRate
    {
        public int ExchangeRateId { get; set; }
        public string BaseCurrencyCode { get; set; } // E.g., USD
        public string TargetCurrencyCode { get; set; } // E.g., EUR
        public decimal Rate { get; set; } // Exchange rate between base and target currency
        public DateTime Date { get; set; } // Date when the rate was fetched
    }

}
