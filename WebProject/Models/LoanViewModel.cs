using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
    public class LoanViewModel
    {
        [Display(Name = "First Name:")]
        [Required(ErrorMessage = "*Full name is required")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name:")]
        [Required(ErrorMessage = "*last name is required")]
        public string LastName { get; set; }

        [Display(Name = "Social Security Number:")]
        [StringLength(60, MinimumLength = 9)]
        [Required(ErrorMessage = "*SSN number is required")]
        public string SSN { get; set; }

        [Display(Name = "Loan Company:")]
        [Required(ErrorMessage = "*Name of loan company is required")]
        public string LoanCompany { get; set; }

        [Display(Name = "Loan Companys Email Address:")]
        [Required(ErrorMessage = "*Loan Companys Email is required")]
        public string LoanCompanyEmail { get; set; }

        [Display(Name = "Loan Companys Phone Number:")]
        [Required(ErrorMessage = "*Loan Companys number is required")]
        [StringLength(60, MinimumLength = 10)]
        public string PhoneNumber { get; set; }

        [Display(Name = "What is this loan for:")]
        [Required(ErrorMessage = "*What is the loan for?")]
        public LoanObject LoanObject { get; set; }

        [Display(Name = "Loan account number:")]
        [Required(ErrorMessage = "*Loan Account number is required")]
        public string LoanAccountNumber { get; set; }

        [Required(ErrorMessage = "*Gender Required")]
        public Gender Gender{ get; set; }

        //[ForeignKey("AspNetUsers")]
        //public string userId { get; internal set; }


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
