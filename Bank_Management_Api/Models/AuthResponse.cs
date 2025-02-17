namespace Bank_Management_Api.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
    }
}
