using System.Net.Http.Headers;

namespace Bank_Management_UI.Helpers
{
    public static class AuthorizeToken
    {
        public static void DoAuthorization(HttpClient client, HttpContext httpContext)
        {
            var accessToken = httpContext.User.Claims.FirstOrDefault(t => t.Type == "Token").Value;
            // Add the Bearer token to the Authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
