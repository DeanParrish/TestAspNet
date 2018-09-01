using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Data;
using WebProject.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace WebProject.Controllers
{
    public class DonationsController : Controller
    {
        // GET: Donations
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Donations model)
        {
            using (DataModel db = new DataModel())
            {
                Loan activeLoan = db.Loans.Where(x => x.isLoanActive == true).FirstOrDefault();
                List<Donations> donationsTowardLoan = db.Donations.Where(x => x.AppliedLoanId == activeLoan.Id).ToList();
                double totalDonations = 0;

                foreach (var don in donationsTowardLoan)
                {
                    totalDonations += don.Amount;
                }

                //completed loan
                if ((totalDonations += model.Amount) >= activeLoan.LoanAmount)
                {
                    TempData["Winner"] = "Congratulations you're a winner!";

                    //This is where it will send an email if user won. 
                    var mess = "This person won on " + DateTime.Now;
                    var body = "<p>Email From: {0} </p> <p> {1})</p> <p>Message: Stuff that may be rev.</p><p>{2}</p>";
                    var message = new MailMessage();
                        message.To.Add(new MailAddress("playitfor@gmail.com"));   
                        message.From = new MailAddress("playitfor@gmail.com");  
                        message.Subject = "Winner";
                        message.Body = string.Format(body, User.Identity.GetUserName(), mess, " ");
                        message.IsBodyHtml = true;

                        using (var smtp = new SmtpClient())
                        {
                            var credential = new NetworkCredential
                            {
                                UserName = "playitfor@gmail.com",  
                                Password = "DeanandChad123!"  
                            };
                            smtp.Credentials = credential;
                            smtp.Host = "smtp.gmail.com";
                            smtp.Port = 587;
                            smtp.EnableSsl = true;
                             smtp.Send(message);                          
                        }

                activeLoan.isLoanComplete = true;
                activeLoan.isLoanActive = false;
                var userId = User.Identity.GetUserId();
                Loan newActiveLoan = db.Loans.Where(x => x.UserId == userId).FirstOrDefault();
                if (newActiveLoan != null)
                {
                    newActiveLoan.isLoanActive = true;
                }

                db.SaveChanges();
            } 
                else
                {
                    TempData["Winner"] = "Unfortunately this isn't your time! Try again!";
                }

                var donation = new Donations()
                {
                    Amount = model.Amount,
                    ConfirmationId = "Confirmation Static",
                    TimeSubmitted = DateTime.Now,
                    UserId = User.Identity.GetUserId(),
                    AppliedLoanId = activeLoan.Id
                };

                db.Donations.Add(donation);
                db.SaveChanges();

            }      
            TempData["DonationConfirmation"] = "Your donation was a success!";
            return RedirectToAction("Index", "Home");
            //return View(ViewBag);
        }
    }
}