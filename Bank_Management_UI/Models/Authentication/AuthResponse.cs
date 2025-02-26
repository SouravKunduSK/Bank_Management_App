namespace Bank_Management_UI.Models.Authentication
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserRole { get; set; }
        public string Message { get; set; }
    }
}
