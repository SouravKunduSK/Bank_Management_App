using Azure;
using Bank_Management_Api.Models.ExchangeRate;
using Bank_Management_Api.Services.Interfaces;
using Bank_Management_Data.Data;
using Bank_Management_Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Bank_Management_Api.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public CurrencyService(HttpClient httpClient, IConfiguration configuration, AppDbContext context)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _context = context;
        }

        public async Task<decimal> GetExchangeRateAsync(string baseCurrencyCode, string targetCurrencyCode)
        {
            // Check if the rate is already in the database
            var existingRate = await _context.ExchangeRates
                .FirstOrDefaultAsync(r => r.BaseCurrencyCode == baseCurrencyCode &&
                                          r.TargetCurrencyCode == targetCurrencyCode &&
                                          r.Date.Date == DateTime.UtcNow.Date);

            if (existingRate != null)
            {
                return existingRate.Rate;
            }
            if(baseCurrencyCode == targetCurrencyCode)
            {
                var eRate = 1;
                return eRate;
            }
            // If not in the database, fetch from API
            var baseUrl = _configuration["ExchangeApi:BaseUrl"];
            var apiKey = _configuration["ExchangeApi:ApiKey"];
            var response = await _httpClient.GetStringAsync($"{baseUrl}live?access_key={apiKey}&source={baseCurrencyCode}");
            var exchangeData = JsonConvert.DeserializeObject<ExchangeRateResponse>(response);

            if (exchangeData == null || !exchangeData.Success)
            {
                throw new Exception("Failed to fetch exchange rate data.");
            }

            var key = $"{baseCurrencyCode}{targetCurrencyCode}";
            if (exchangeData.Quotes == null || !exchangeData.Quotes.ContainsKey(key))
            {
                throw new Exception("Exchange rate data not available for the requested currencies.");
            }



            var exchangeRate  = exchangeData.Quotes[key];

            // Store the rate in the database for caching
            var exchangeRateEntry = new ExchangeRate
            {
                BaseCurrencyCode = baseCurrencyCode,
                TargetCurrencyCode = targetCurrencyCode,
                Rate = exchangeRate,
                Date = DateTime.UtcNow
            };

            await _context.ExchangeRates.AddAsync(exchangeRateEntry);
            await _context.SaveChangesAsync();

            return exchangeRate;
        }
    }

}
