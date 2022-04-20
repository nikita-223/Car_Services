  using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarServcing.Models;

namespace CarServcing.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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

        [Authorize(Roles ="Admin")]
        public ActionResult AdminDashboard()
        {
            return View();
        }

        [Authorize(Roles ="Vendor")]
        public ActionResult VendorDashboard()
        {
            return View();
        }
               [Authorize(Roles ="Customer")]
        public ActionResult CustomerDashboard()
        {
            return View();
        }

        [Authorize]
        public ActionResult Report()
        {
            CarServiceEntities2 db = new CarServiceEntities2();
            var serviceDetailReport = db.ServiceDetails;
            return View(serviceDetailReport.ToList());
        }
        [Authorize]
        public ActionResult BookReport()
        {
            CarServiceEntities2 db = new CarServiceEntities2();
            var bookDetailReport = db.BookServices;
            return View(bookDetailReport.ToList());
        }
        
        public ActionResult Feedback()
        {
            CarServiceEntities2 db = new CarServiceEntities2();
            var feedReport = db.Feedbacks;
            return View(feedReport.ToList());
        }


    }
}