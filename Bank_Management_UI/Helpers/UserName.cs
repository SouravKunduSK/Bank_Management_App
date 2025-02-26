using Bank_Management_UI.Models.Account;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Bank_Management_UI.Helpers
{
    public static class UserName
    {
        public static async Task<string> FindUserName(HttpClient client, HttpContext httpContext, string userMail)
        {
            var url = $"Auth/user-data?email={Uri.EscapeDataString(userMail)}";
            var userResponse =await client.GetAsync(url);

            var userData = userResponse.Content.ReadAsStringAsync().Result;
            var user = JsonConvert.DeserializeObject<UserResponse>(userData);
            return user.FullName;
        }
    }
}
