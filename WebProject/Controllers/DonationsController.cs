﻿using Microsoft.AspNet.Identity;
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
                if ((totalDonations+=1) >= activeLoan.LoanAmount)
                {
                  //This shows the previous winner userid. this will show on email to help as well. email shows current winner and previous winner
                    Loan persons = db.Loans.Where(p => p.isLoanActive == true).FirstOrDefault();
                    
                    TempData["Winner"] = "Congratulations you're a winner!";
                    activeLoan.isLoanComplete = true;
                    activeLoan.isLoanActive = false;
                    activeLoan.isLoanPrimary = false;
                    db.SaveChanges();

                    var userId = User.Identity.GetUserId();
                  
                        var body = "<p>Winner email: " + User.Identity.GetUserName() + " </p><p>Winner time stamp: {0}</p><p>Previous winner UserId: {1}";
                        var message = new MailMessage();
                        message.To.Add(new MailAddress("playitfor@gmail.com"));  // replace with valid value 
                        message.From = new MailAddress("playitfor@gmail.com");  // replace with valid value
                        message.Subject = "Winner";
                        message.Body = string.Format(body, DateTime.Now, persons.UserId);
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
                    

                    Loan newActiveLoan = db.Loans.Where(x => x.UserId == userId && x.isLoanActive == false && x.isLoanComplete == false && x.isLoanPrimary == true).FirstOrDefault();
                    if (newActiveLoan != null)
                    {
                        newActiveLoan.isLoanActive = true;
                    }
                    else//no primary
                    {
                        TempData["Winner"] = "Congratulations you're a winner! You had no primary loan, so we chose your first loan for you!";
                        newActiveLoan = db.Loans.Where(x => x.UserId == userId && x.isLoanActive == false && x.isLoanComplete == false).FirstOrDefault();
                        if (newActiveLoan != null)
                        {
                            newActiveLoan.isLoanActive = true;
                        }
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
            
            TempData["DonationConfirmation"] = "Your donation was a success!";
            return RedirectToAction("Index", "Home");
            //return View(ViewBag);
        }

        [HttpPost]
        public void AuthorizeNetPay(string confirmRes)//void for now
        {
            WebProject.Helper.AuthorizeNetPay authorizeNetPay = new Helper.AuthorizeNetPay();
            authorizeNetPay.Pay(confirmRes);
        }
    }
}