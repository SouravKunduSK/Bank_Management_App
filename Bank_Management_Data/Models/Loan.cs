using Bank_Management_Data.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Management_Data.Models
{
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }

        [Required]
        public string LoanNumber { get; set; } 

        [Required]
        public decimal LoanAmount { get; set; }

        [Required]
        public decimal InterestRate { get; set; }

        public LoanStatus Status { get; set; } = LoanStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? NextDueDate { get;set; }
        public decimal PenaltyRate { get; set; } = 0.02m;
        // Loan Type
        [Required]
        public int LoanTypeId { get; set; }
        public LoanType LoanType { get; set; }

        // Account Relationship
        [Required]
        public int AccountId { get; set; }
        public Account Account { get; set; }
        // Navigation Property for Loan Repayments
        public List<LoanRepayment> Repayments { get; set; }
    }
}
