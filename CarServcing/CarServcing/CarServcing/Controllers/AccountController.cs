using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CarServcing.Models;

namespace CarServcing.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
      /*  public ActionResult Index()
        {
            return View();
        }*/
        public ActionResult Signup()
        {
            UserDetail userModel = new UserDetail();
            return View(userModel);
        }

        [HttpPost]
        public ActionResult Signup(UserDetail userModel)
        {
            using (CarServiceEntities2 dbModel = new CarServiceEntities2())
            {
                if (dbModel.UserDetails.Any(x => x.Email == userModel.Email))
                {
                    ViewBag.DuplicateMessage = "Email Already Exists";
                    return View("Signup", userModel);
                }
                dbModel.UserDetails.Add(userModel);
                dbModel.SaveChanges();
            }
            ModelState.Clear();
            ViewBag.SuccessMessage = "Your Details are submitted successfully " + userModel.FirstName + " " + userModel.LastName;
            if (userModel.UserType == "Vendor")
            {
                ViewBag.SuccessMessage2 = "Your Vendor ID is: " + userModel.Id;
                CarServiceEntities2 dbModel = new CarServiceEntities2();
                Vendor vendor = new Vendor();
                vendor.UserId = userModel.Id;
                vendor.VendorStatus = "Waiting For Approval";
                dbModel.Vendors.Add(vendor);
                dbModel.SaveChanges();
                return View("Signup", new UserDetail());
            }
            else
            {
                CarServiceEntities2 dbModel = new CarServiceEntities2();
                UserRole role = new UserRole();
                role.UserId = userModel.Id;
                role.Role = "Customer";
                dbModel.UserRoles.Add(role);
                dbModel.SaveChanges();
                return View("Signup", new UserDetail());
            }

        }
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Account");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login( UserDetail model)
        {
            using (var context = new CarServiceEntities2 ())
            {
                bool isValid = context.UserDetails.Any(x => x.Email == model.Email && x.Password == model.Password);
                if (isValid)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("Dashboard", "Account");
                }
                ModelState.AddModelError("", "Invalid UserName and/or Password");
                return View();
            }
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            CarServiceEntities2 db = new CarServiceEntities2();
            var vendorList=db.Vendors.Where(m => m.VendorStatus == "Waiting For Approval" || m.VendorStatus == "Rejected");
            var vendorRow=vendorList.SingleOrDefault(m => m.UserDetail.Email == User.Identity.Name);
            return View(vendorRow);
        }
        public ActionResult VendorList()
        {
            CarServiceEntities2 db = new CarServiceEntities2();
            var VendorList = db.UserDetails.Where(userdetails => userdetails.UserType == "Vendor");
            return View(VendorList.ToList());
        }

        [Authorize(Roles ="Admin")]
        public ActionResult VendorApproved(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarServiceEntities2 db = new CarServiceEntities2();
            UserDetail userDetail = db.UserDetails.Find(id);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            UserRole userRole = new UserRole();
            userRole.UserId = userDetail.Id;
            userRole.Role = "Vendor";
            db.UserRoles.Add(userRole);
            db.SaveChanges();
            Vendor vendor = new Vendor();
            CarServiceEntities2 carServiceEntities = new CarServiceEntities2();
            var adminRow=carServiceEntities.UserDetails.Single(m => m.Email == User.Identity.Name);
            vendor.UserId = (int)id;
            vendor.ApproverId = adminRow.Id;
            vendor.ApproveDate = DateTime.Now;
            vendor.VendorStatus = "Approved";
            carServiceEntities.Entry(vendor).State = EntityState.Modified;
            carServiceEntities.SaveChanges();
            return RedirectToAction("VendorList");
        }

        [Authorize(Roles ="Admin")]
        public ActionResult VendorRejected(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarServiceEntities2 db = new CarServiceEntities2();
            UserDetail userDetail = db.UserDetails.Find(id);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            Vendor vendor = new Vendor();
            var adminRow = db.UserDetails.Single(m => m.Email == User.Identity.Name);
            vendor.UserId = (int)id;
            vendor.ApproverId = adminRow.Id;
            vendor.ApproveDate = DateTime.Now;
            vendor.VendorStatus = "Rejected";
            db.Entry(vendor).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("VendorList");

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

    }
}