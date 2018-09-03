using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebProject.Data;
using Microsoft.AspNet.Identity;

namespace WebProject.Controllers
{
    [AuthorizeUserFilter]
    public class LoansController : Controller
    {
        private DataModel db = new DataModel();

        // GET: Loans
        public ActionResult Index()
        {

            var id = User.Identity.GetUserId();
            var loanList = db.Loans.Where(model => model.UserId == id).OrderBy(x=>x.isLoanPrimary);
            return View(loanList.ToList());
        }

        //This will allow the admin to search the loan table and view information on that person.
        public ActionResult LoanSearch(string q, string S)
        {
            var persons = from p in db.Loans select p;
            int id = Convert.ToInt32(Request["SearchType"]);
            var searchParameter = "Searching";

            if (!string.IsNullOrWhiteSpace(q))
            {
                switch (id)
                {
                    case 0:
                        // int iQ = int.Parse(q);
                        persons = persons.Where(p => p.FirstName.Contains(q));
                        searchParameter += " First Name for ' " + q + " '";
                        break;
                    case 1:
                        persons = persons.Where(p => p.LastName.Contains(q));
                        searchParameter += " Last Name for ' " + q + " '";
                        break;
                    case 3:
                        persons = persons.Where(p => p.LoanAccountNumber.StartsWith(q));
                        searchParameter += " Loan Account number for ' " + q + "'";
                        break;

                }
            }
            else
            {
                searchParameter += "ALL";
            }
            ViewBag.SearchParameter = searchParameter;
            return View(persons);
        }
        // GET: Loans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        // GET: Loans/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            using (var db = new DataModel())
            {
                var hasPrimaryLoan = db.Loans.Where(x => x.UserId == userId && x.isLoanPrimary == true).FirstOrDefault();
                if (hasPrimaryLoan != null)
                {
                    TempData["hasPrimary"] = true;
                }
            }
            return View();

        }

        // POST: Loans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,LoanCompany,LoanCompanyEmail,isLoanPrimary,PhoneNumber,LoanObject,LoanAccountNumber,LoanAmount,Gender,UserId")] Loan loan)
        {
            if (ModelState.IsValid)
            {

                //This allows me to keep the same value in the UserId field.
                var userId = User.Identity.GetUserId();

                List<Loan> loans = db.Loans.Where(x => x.UserId == userId).ToList();
                var completedLoanCount = loans.Where(x => x.isLoanComplete == false).Count();
                var hasPrimary = loans.Where(x => x.isLoanPrimary == true).FirstOrDefault();
                //only allow 2 loans
                if (completedLoanCount < 2 && hasPrimary == null)
                {
                    loan.UserId = userId;
                    loan.isLoanActive = false;
                    db.Loans.Add(loan);
                    db.SaveChanges();
                }
                else
                {
                    if (hasPrimary != null)
                    {
                        TempData["LoanFailure"] = "You already have a specified primary loan. You may only have one primary loan.";
                    }
                    else
                    {
                        TempData["LoanFailure"] = "You already have two specified loans. Please delete one and add your new loan.";
                    }
                    
                }
                
                return RedirectToAction("Index");
            }

            return View(loan);
        }

        // GET: Loans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,LoanCompany,LoanCompanyEmail,PhoneNumber,LoanObject,LoanAccountNumber,Gender")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                //check to see if this was already primary/if user already has primary
                bool isloanPrimary = db.Loans.Where(x => x.Id == loan.Id).Select(x => x.isLoanPrimary).FirstOrDefault();
                if (isloanPrimary == false && loan.isLoanPrimary == true)//tried to make primary need to check for other primary
                {
                    Loan userPrimaryLoan = db.Loans.Where(x => x.UserId == userId && x.isLoanPrimary == true).FirstOrDefault();
                    if (userPrimaryLoan != null)//already has primary
                    {
                        TempData["hasLoan"] = "You already have a primary loan. Please remove your existing primary loan before making this one your primary.";
                    }
                    else
                    {
                        loan.UserId = userId;
                        db.Entry(loan).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    loan.UserId = userId;
                    db.Entry(loan).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return View(loan);
        }

        // GET: Loans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Loan loan = db.Loans.Find(id);
            db.Loans.Remove(loan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
