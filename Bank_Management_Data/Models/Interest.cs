using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Management_Data.Models
{
    public class Interest
    {
        public int InterestId { get; set; }
        public string AccountNumber { get; set; }
        public decimal InterestRate { get; set; }
        public decimal InterestAmount { get; set; }
        public DateTime CalculationDate { get; set; }

        // Navigation Property
        public Account Account { get; set; }
    }
}
