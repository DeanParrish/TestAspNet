using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Data
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LoanCompany { get; set; }

        [Required]
        [MaxLength(500)]
        public string LoanCompanyEmail { get; set; }

        [Required]
        [MaxLength(500)]
        public string PhoneNumber { get; set; }

        [Required]
        
        public LoanObject LoanObject { get; set; }

        [Required]
        [MaxLength(500)]
        public string LoanAccountNumber { get; set; }

        [Required]
        public double LoanAmount { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [ForeignKey("")]
        public string UserId { get; set; }

        public bool isLoanComplete { get; set; }

        public bool isLoanActive { get; set; }
        public bool isLoanPrimary { get; set; }



        // NAVIGATION PROPERTIES

    }
    public enum Gender
    {
        Male,
        Female
    }
    public enum LoanObject
    {
        Vehicle,
        Mortgage,
        Hospital,
        Other

    }
}