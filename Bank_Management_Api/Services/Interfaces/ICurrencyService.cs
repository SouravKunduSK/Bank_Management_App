namespace Bank_Management_Api.Services.Interfaces
{
    public interface ICurrencyService
    {
        public Task<decimal> GetExchangeRateAsync(string baseCurrencyCode, string targetCurrencyCode);
    }
}
