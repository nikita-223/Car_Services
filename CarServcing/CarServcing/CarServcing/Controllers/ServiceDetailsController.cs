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
    public class ServiceDetailsController : Controller
    {
        private CarServiceEntities2 db = new CarServiceEntities2();

        // GET: ServiceDetails
        public ActionResult Index(string searchBy, string search)
        {
            
            if (User.IsInRole("Vendor"))
            {
                var VserviceDetails = db.ServiceDetails.Include(s => s.ServiceCategory).Include(s => s.UserDetail).Where(s=>s.UserDetail.Email==User.Identity.Name);
                if (searchBy == "ServiceCenter")
                {
                    return View(VserviceDetails.Where(x => x.ServiceCenter.Contains(search) || search == null).ToList());
                }
                else
                {
                    return View(VserviceDetails.Where(x => x.ServiceName.Contains(search) || search == null).ToList());

                }
            }
            var serviceDetails = db.ServiceDetails.Include(s => s.ServiceCategory).Include(s => s.UserDetail);
            if (searchBy == "ServiceCenter")
            {
                return View(serviceDetails.Where(x => x.ServiceCenter.Contains(search) || search == null).ToList());
            }
            else
            {
                return View(serviceDetails.Where(x => x.ServiceName.Contains(search) || search == null).ToList());

            }
        }

        // GET: ServiceDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceDetail serviceDetail = db.ServiceDetails.Find(id);
            if (serviceDetail == null)
            {
                return HttpNotFound();
            }
            return View(serviceDetail);
        }

        // GET: ServiceDetails/Create
        [Authorize (Roles="Vendor")]
        public ActionResult Create()
        {
            
            ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "ServiceCategoryId", "CategoryName");
            //ViewBag.UserId = new SelectList(db.UserDetails, "Id", "Email");
            ViewBag.UserId = new SelectList(db.UserDetails.Where(m => m.Email == User.Identity.Name), "Id", "Email");
            return View();
        }

        // POST: ServiceDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles ="Vendor")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ServiceDetailsId,ServiceName,Cost,OpeningTime,ClosingTime,ServiceCenter,UserId,ServiceCategoryId")] ServiceDetail serviceDetail)
        {
            if (ModelState.IsValid)
            {
                db.ServiceDetails.Add(serviceDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "ServiceCategoryId", "CategoryName", serviceDetail.ServiceCategoryId);
            ViewBag.UserId = new SelectList(db.UserDetails.Where(m => m.Email == User.Identity.Name), "Id", "Email", serviceDetail.UserId);
            //ViewBag.UserId = db.UserDetails.SingleOrDefault(m => m.Email == User.Identity.Name).Id;
            return View(serviceDetail);
        }

        // GET: ServiceDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceDetail serviceDetail = db.ServiceDetails.Find(id);
            if (serviceDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "ServiceCategoryId", "CategoryName", serviceDetail.ServiceCategoryId);
            ViewBag.UserId = new SelectList(db.UserDetails, "Id", "Email", serviceDetail.UserId);
            return View(serviceDetail);
        }

        // POST: ServiceDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ServiceDetailsId,ServiceName,Cost,OpeningTime,ClosingTime,ServiceCenter,UserId,ServiceCategoryId")] ServiceDetail serviceDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serviceDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ServiceCategoryId = new SelectList(db.ServiceCategories, "ServiceCategoryId", "CategoryName", serviceDetail.ServiceCategoryId);
            ViewBag.UserId = new SelectList(db.UserDetails, "Id", "Email", serviceDetail.UserId);
            return View(serviceDetail);
        }

        // GET: ServiceDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceDetail serviceDetail = db.ServiceDetails.Find(id);
            if (serviceDetail == null)
            {
                return HttpNotFound();
            }
            return View(serviceDetail);
        }

        // POST: ServiceDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServiceDetail serviceDetail = db.ServiceDetails.Find(id);
            db.ServiceDetails.Remove(serviceDetail);
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
