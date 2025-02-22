using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Management_Data.Models
{
    public class LoanType
    {
        [Key]
        public int LoanTypeId { get; set; }

        [Required]
        public string TypeName { get; set; }

        [Required]
        public decimal DefaultInterestRate { get; set; }

        public int DurationInMonths { get; set; }

        // Navigation Property
        public List<Loan> Loans { get; set; }
    }
}
