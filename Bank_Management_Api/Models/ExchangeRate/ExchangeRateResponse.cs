namespace Bank_Management_Api.Models.ExchangeRate
{
    public class ExchangeRateResponse
    {
        public bool Success { get; set; }
        public Dictionary<string, decimal> Quotes { get; set; }
    }
}
