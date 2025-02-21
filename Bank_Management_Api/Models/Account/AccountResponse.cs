using Bank_Management_Data.Data;

namespace Bank_Management_Api.Models.Account
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } // Account number, e.g., 1234567890
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; } // Account Status
        public DateTime CreatedAt { get; set; }

        // Currency-related info
        public string CurrencyCode { get; set; } // E.g., USD, EUR
        public string CurrencySymbol { get; set; } // Currency symbol, e.g., $, €
        public decimal CurrencyExchangeRate { get; set; } // Exchange rate to the base currency

    }
}
