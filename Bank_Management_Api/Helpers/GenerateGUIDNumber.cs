namespace Bank_Management_Api.Helpers
{
    public static class GenerateGUIDNumber
    {
        public static string GenerateNumberUsingGuid()
        {
            string guidNumbers = new string(Guid.NewGuid().ToString("N").Where(char.IsDigit).ToArray());
            string part1 = guidNumbers.Substring(0, 5);
            string part2 = guidNumbers.Substring(5, 5);

            return $"{part1}-{part2}";
        }
    }
}
