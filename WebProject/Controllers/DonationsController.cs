using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Data;
using WebProject.Models;

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
            using(DataModel db = new DataModel())
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
                    TempData["Winner"] = "Congratulations you're a winner!";
                    activeLoan.isLoanComplete = true;
                    activeLoan.isLoanActive = false;
                    activeLoan.isLoanPrimary = false;
                    db.SaveChanges();

                    var userId = User.Identity.GetUserId();
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
    }
}