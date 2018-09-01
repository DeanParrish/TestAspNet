using WebProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Data;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

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

        ///This is the part that will allow user to send email to us. 
        [HttpPost]
        [ValidateAntiForgeryToken]
       public async Task<ActionResult> Contact(EmailFormModel model)
        {
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("playitfor@gmail.com"));  // replace with valid value 
                message.From = new MailAddress("playitfor@gmail.com");  // replace with valid value
                message.Subject = "Contact form submission";
                message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                message.IsBodyHtml = true;

                //This part below can be put in the web config file if we want to do that i think.
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "playitfor@gmail.com",  // replace with valid value user@outlook.com
                        Password = "DeanandChad123!"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    return RedirectToAction("Sent");
                }
            }
            return View(model);
        }

        public ActionResult Sent()
        {
            return View();
        }
        //99999999999999999999999999999999999999999999999999999999999999999999999999
        [HttpPost]
        public ActionResult Contacts(ContactViewModels c)
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
        [AuthorizeUserFilter]
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