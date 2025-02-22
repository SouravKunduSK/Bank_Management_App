using Bank_Management_Data.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Management_Data.Models
{
    public class LoanRepayment
    {
        [Key]
        public int RepaymentId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime RepaymentDate { get; set; } = DateTime.UtcNow;

        public RepaymentStatus Status { get; set; } = RepaymentStatus.Pending;

        public decimal Penalty { get; set; } = 0;

        // Loan Relationship
        [Required]
        public int LoanId { get; set; }
        public Loan Loan { get; set; }
    }
}
