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
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Donations model)
        {
            using (DataModel db = new DataModel())
            {
                var id = User.Identity.GetUserId();
                if (id != null)
                {
                    int userLoanCount = db.Loans.Where(x => x.UserId == id).Count();
                    if (userLoanCount == 0)
                    {
                        TempData["DonationConfirmation"] = "Your donation failed. You must add a loan before you donate!";
                        return RedirectToAction("Index", "Home");
                    }
                }

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
                    activeLoan.isLoanComplete = true;
                    activeLoan.isLoanActive = false;
                    var userId = User.Identity.GetUserId();
                    Loan newActiveLoan = db.Loans.Where(x => x.UserId == userId && x.isLoanActive == false && x.isLoanComplete == false && x.isLoanPrimary == true).FirstOrDefault();
                    if (newActiveLoan != null)
                    {
                        newActiveLoan.isLoanActive = true;
                    }

                   
                        var body = "<p>Winner email: " + User.Identity.GetUserName() + " </p><p>Winner time stamp: {0}</p>";
                        var message = new MailMessage();
                        message.To.Add(new MailAddress("playitfor@gmail.com"));  // replace with valid value 
                        message.From = new MailAddress("playitfor@gmail.com");  // replace with valid value
                        message.Subject = "Winner";
                        message.Body = string.Format(body, DateTime.Now);
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
                            smtp.Send(message);

                        }
                    

                    db.SaveChanges();
                }
                else
                {
                    TempData["Winner"] = "Unfortunately this isn't your time! Try again!";
                }

                var donation = new Donations()
                {
                    Amount = 1,
                    ConfirmationId = "Confirmation Static",
                    TimeSubmitted = DateTime.Now,
                    UserId = User.Identity.GetUserId(),
                    AppliedLoanId = activeLoan.Id
                };

                db.Donations.Add(donation);
                db.SaveChanges();

            }
           // var test = 3;
            
            TempData["DonationConfirmation"] = "Your donation was a success!";
            return RedirectToAction("Index", "Home");
            //return View(ViewBag);
        }
    }
}