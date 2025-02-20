using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Management_Data.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        // Navigation Property
        public List<Account> Accounts { get; set; } // one use may have multiple accounts
    }
}
