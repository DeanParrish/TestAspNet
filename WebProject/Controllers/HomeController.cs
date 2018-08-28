using WebProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Data;
using Microsoft.AspNet.Identity;

namespace WebProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Dean Test
            //DataModel db = new DataModel();
            using (var db = new DataModel())
            {
                var loan = new Loan()
                {
                    FirstName = "Dean",
                    LastName = "Parrish",
                    LoanAccountNumber = "354354",
                    LoanCompany = "company",
                    LoanCompanyEmail = "email",
                    LoanAmount = 1205.23,
                    LoanObject = Data.LoanObject.Hospital,
                    Gender = Data.Gender.Male,
                    PhoneNumber = "7065514180",
                    SSN = "",
                    UserId = User.Identity.GetUserId()

                };
                db.Loans.Add(loan);
                db.SaveChanges();
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult FAQPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactViewModels c)
        {
            ViewBag.Title = "Contact";

            if (string.IsNullOrWhiteSpace(c.FullName) == false)
            {
                if (c.FullName.Contains(" ") == false)
                {
                    ModelState.AddModelError(nameof(c.FullName), "Please input first and last name.");
                }

                if (string.IsNullOrWhiteSpace(c.Email) == false)
                {
                    if (c.Email.Contains("@") == false)
                    {
                        ModelState.AddModelError(nameof(c.Email), "Not a valid email address");
                    }
                }
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("ContactAcknowledgement");

            }
            //I tried removing c to see if the form would still remember and it did. I put c back anyways.
            return View(c);
        }

        public ActionResult ContactAcknowledgement()
        {

            return View();
        }

        //I need to access the database for current filed loans for the user signed in
        public ActionResult Loan()
        {
            ViewBag.Message = "Your Loan Page";
            
            var vm = new LoanListViewModel();
            //using (DataModel dm = new DataModel())
            //{

            //    foreach (var s in dm.Loans)
            //    {
            //        vm.Loans.Add(s);
            //    }
            //}

                return View(vm);
            
        }

        public ActionResult LoanAcknowledgement()
        {

            return View();
        }

        public ActionResult AddLoan()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddLoan(LoanViewModel c)
         {
             ViewBag.Title = "Add Loan";

             if (string.IsNullOrWhiteSpace(c.FirstName) == false)
             {
                 if (c.FirstName.Contains(" ") == true)
                 {
                     ModelState.AddModelError(nameof(c.FirstName), "Please no special characters or spaces in first name.");
                 }
             }

             if (string.IsNullOrWhiteSpace(c.LastName) == false)
             {
                 if (c.LastName.Contains(" ") == true)
                 {
                     ModelState.AddModelError(nameof(c.LastName), "Please no special characters or spaces in last name.");
                 }
             }

                     if (string.IsNullOrWhiteSpace(c.LoanCompanyEmail) == false)
                 {
                     if (c.LoanCompanyEmail.Contains("@") == false)
                     {
                         ModelState.AddModelError(nameof(c.LoanCompanyEmail), "Not a valid email address");
                     }
                 }         
             if (ModelState.IsValid)
             {
                 return RedirectToAction("LoanAcknowledgement");

             }
             //I tried removing c to see if the form would still remember and it did. I put c back anyways.
             return View(c);
         }
    }


}