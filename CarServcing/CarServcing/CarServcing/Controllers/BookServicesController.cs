using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarServcing.Models;

namespace CarServcing.Controllers
{
    [Authorize]
    public class BookServicesController : Controller
    {
        private CarServiceEntities2 db = new CarServiceEntities2();

        // GET: BookServices
        [Authorize(Roles = "Admin,Vendor,Customer")]
        public ActionResult Index()
        {
            if (User.IsInRole("Vendor"))
            {
                var vBookServices = db.BookServices.Include(b => b.ServiceDetail).Include(b => b.Status).Include(b => b.UserDetail).Include(b => b.Feedback).Include(b => b.TransactionDetail).Where(b => b.ServiceDetail.UserDetail.Email == User.Identity.Name);
                return View(vBookServices.ToList());
            }
            if (User.IsInRole("Customer"))
            {
                var cBookServices = db.BookServices.Include(b => b.ServiceDetail).Include(b => b.Status).Include(b => b.UserDetail).Include(b => b.Feedback).Include(b => b.TransactionDetail).Where(b => b.UserDetail.Email == User.Identity.Name);
                return View(cBookServices.ToList());
            }
            var bookServices = db.BookServices.Include(b => b.ServiceDetail).Include(b => b.Status).Include(b => b.UserDetail).Include(b => b.Feedback).Include(b => b.TransactionDetail);
            return View(bookServices.ToList());
        }

        [Authorize(Roles = "Vendor")]
        public ActionResult WaitingApproval()
        {
            var bookServices = db.BookServices.Include(b => b.ServiceDetail).Include(b => b.Status).Include(b => b.UserDetail).Include(b => b.Feedback).Include(b => b.TransactionDetail).Where(b => b.StatusId == 1);
            return View(bookServices.ToList());
        }

        /* public ActionResult CustomerBookings()
        {
            var bookServices = db.BookServices.Include(b => b.ServiceDetail).Include(b => b.Status).Include(b => b.UserDetail).Include(b => b.Feedback).Include(b => b.TransactionDetail).Where(b => b.UserId == 5);
            return View(bookServices.ToList());
        }*/

        // GET: BookServices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookService bookService = db.BookServices.Find(id);
            if (bookService == null)
            {
                return HttpNotFound();
            }
            return View(bookService);
        }

        // GET: BookServices/Create
        public ActionResult Create()
        {
            ViewBag.ServiceDetailsId = new SelectList(db.ServiceDetails, "ServiceDetailsId", "ServiceName");
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "CurrentStatus");
            ViewBag.UserId = new SelectList(db.UserDetails.Where(m => m.Email == User.Identity.Name), "Id", "Email");
            ViewBag.BookingId = new SelectList(db.Feedbacks, "BookingId", "Comment");
            ViewBag.BookingId = new SelectList(db.TransactionDetails, "BookingId", "BookingId");

            return View();
        }

        // POST: BookServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingId,Date,StartTime,EndTime,Amount,CouponCode,ServiceDetailsId,UserId,StatusId")] BookService bookService)
        {
            if (ModelState.IsValid)
            {
                var row = db.ServiceDetails.Find(bookService.ServiceDetailsId);
                bookService.Amount = row.Cost;
                bookService.StatusId = 1;
                db.BookServices.Add(bookService);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ServiceDetailsId = new SelectList(db.ServiceDetails, "ServiceDetailsId", "ServiceName", bookService.ServiceDetailsId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "CurrentStatus", bookService.StatusId);
            ViewBag.UserId = new SelectList(db.UserDetails.Where(m => m.Email == User.Identity.Name), "Id", "Email", bookService.UserId);
            ViewBag.BookingId = new SelectList(db.Feedbacks, "BookingId", "Comment", bookService.BookingId);
            ViewBag.BookingId = new SelectList(db.TransactionDetails, "BookingId", "BookingId", bookService.BookingId);
            return View(bookService);
        }

        // GET: BookServices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookService bookService = db.BookServices.Find(id);
            if (bookService == null)
            {
                return HttpNotFound();
            }
            // For updating the data in database
            ViewBag.ServiceDetailsId = new SelectList(db.ServiceDetails, "ServiceDetailsId", "ServiceName", bookService.ServiceDetailsId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "CurrentStatus", bookService.StatusId);
            ViewBag.UserId = new SelectList(db.UserDetails, "Id", "Email", bookService.UserId);
            ViewBag.BookingId = new SelectList(db.Feedbacks, "BookingId", "Comment", bookService.BookingId);
            ViewBag.BookingId = new SelectList(db.TransactionDetails, "BookingId", "BookingId", bookService.BookingId);
            return View(bookService);
        }

        // POST: BookServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingId,Date,StartTime,EndTime,Amount,CouponCode,ServiceDetailsId,UserId,StatusId")] BookService bookService)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookService).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ServiceDetailsId = new SelectList(db.ServiceDetails, "ServiceDetailsId", "ServiceName", bookService.ServiceDetailsId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "CurrentStatus", bookService.StatusId);
            ViewBag.UserId = new SelectList(db.UserDetails, "Id", "Email", bookService.UserId);
            ViewBag.BookingId = new SelectList(db.Feedbacks, "BookingId", "Comment", bookService.BookingId);
            ViewBag.BookingId = new SelectList(db.TransactionDetails, "BookingId", "BookingId", bookService.BookingId);
            return View(bookService);
        }

        // GET: BookServices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookService bookService = db.BookServices.Find(id);
            if (bookService == null)
            {
                return HttpNotFound();
            }
            return View(bookService);
        }

        // POST: BookServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BookService bookService = db.BookServices.Find(id);
            db.BookServices.Remove(bookService);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Approved(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookService bookService = db.BookServices.Find(id);
            if (bookService == null)
            {
                return HttpNotFound();
            }
            bookService.StatusId = 2;
            db.Entry(bookService).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Reallocate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookService bookService = db.BookServices.Find(id);
            if (bookService == null)
            {
                return HttpNotFound();
            }
            ViewBag.ServiceDetailsId = new SelectList(db.ServiceDetails, "ServiceDetailsId", "ServiceName", bookService.ServiceDetailsId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "CurrentStatus", bookService.StatusId);
            ViewBag.UserId = new SelectList(db.UserDetails, "Id", "Email", bookService.UserId);
            ViewBag.BookingId = new SelectList(db.Feedbacks, "BookingId", "Comment", bookService.BookingId);
            ViewBag.BookingId = new SelectList(db.TransactionDetails, "BookingId", "BookingId", bookService.BookingId);
            var centerList = db.ServiceDetails.Where(m => m.UserId == bookService.ServiceDetail.UserId && m.ServiceName == bookService.ServiceDetail.ServiceName);
            ViewBag.ServiceCenter = new SelectList(centerList, "ServiceCenter", "ServiceCenter", bookService.ServiceDetail.ServiceCenter);
            return View(bookService);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reallocate([Bind(Include = "BookingId,Date,StartTime,EndTime,Amount,CouponCode,ServiceDetailsId,UserId,StatusId")] BookService bookService)
        {
            if (ModelState.IsValid)
            {
                bookService.StatusId = 3;
                db.Entry(bookService).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ServiceDetailsId = new SelectList(db.ServiceDetails, "ServiceDetailsId", "ServiceName", bookService.ServiceDetailsId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "CurrentStatus", bookService.StatusId);
            ViewBag.UserId = new SelectList(db.UserDetails, "Id", "Email", bookService.UserId);
            ViewBag.BookingId = new SelectList(db.Feedbacks, "BookingId", "Comment", bookService.BookingId);
            ViewBag.BookingId = new SelectList(db.TransactionDetails, "BookingId", "BookingId", bookService.BookingId);
            var centerList = db.ServiceDetails.Where(m => m.UserId == bookService.ServiceDetail.UserId && m.ServiceName == bookService.ServiceDetail.ServiceName);
            ViewBag.ServiceCenter = new SelectList(centerList, "ServiceCenter", "ServiceCenter", bookService.ServiceDetail.ServiceCenter);
            return View(bookService);
        }

        public ActionResult PaymentPortal(int? id)
        {
            var model = db.BookServices.SingleOrDefault(m => m.BookingId == id);
            return View(model);
        }
        public ActionResult Transaction(int? id)
        {
            var bookService = db.BookServices.Find(id);
            bookService.StatusId = 4;
            db.Entry(bookService).State = EntityState.Modified;
            var TransactionRow = new TransactionDetail();
            TransactionRow.BookingId = bookService.BookingId;
            TransactionRow.Amount = bookService.Amount;
            TransactionRow.TransactionDateTime = DateTime.Now;
            db.TransactionDetails.Add(TransactionRow);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult FeedbackForm(int? id)
        {
            var model = db.BookServices.Find(id);
            var tr = model.UserDetail.Email.Trim();
            if (!(model.UserDetail.Email.Trim() == User.Identity.Name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedbackmodel = new Feedback();
            feedbackmodel.BookingId = model.BookingId;
            return View(feedbackmodel);
        }
        [HttpPost]
        public ActionResult FeedbackForm(Feedback feedback)
        {
            Feedback row = new Feedback();
            row.BookingId = feedback.BookingId;
            row.Q1Rating = feedback.Q1Rating;
            row.Q2Rating = feedback.Q2Rating;
            row.Comment = feedback.Comment;
            db.Feedbacks.Add(row);
            var bookService = db.BookServices.Find(feedback.BookingId);
            bookService.StatusId = 5;
            db.Entry(bookService).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "BookServices");
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
