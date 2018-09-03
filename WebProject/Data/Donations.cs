
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebProject.Data
{
    public partial class Donations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DonationId { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AspNetUser User { get; set; }

        [Required]
        public int AppliedLoanId { get; set; }

        [Required]
        public DateTime TimeSubmitted { get; set; }

        //for future user with API/whatnot
        public string ConfirmationId { get; set; }

        public int Amount { get; set; }


    }
}