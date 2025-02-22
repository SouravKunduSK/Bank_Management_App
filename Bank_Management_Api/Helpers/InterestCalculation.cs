namespace Bank_Management_Api.Helpers
{
    public static class InterestCalculation
    {
        public static decimal CalculateInterest(decimal principal, decimal rate, int timeInYears)
        {
            return principal * rate * timeInYears / 100;
        }
    }
}
