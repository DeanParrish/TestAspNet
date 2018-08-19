using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Data;

namespace WebProject.Models
{
    public class LoanListViewModel
    {
        public List<Loan> Loans { get; set; } = new List<Loan>();
    }
}