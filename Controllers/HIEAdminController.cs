using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualHIE.Models;

namespace VirtualHIE.Controllers
{
    public class HIEAdminController : Controller
    {
        private HealthInformationExchangeEntities db = new HealthInformationExchangeEntities();
        //
        // GET: /HIEAdmin/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdminHome(string sortOrder, string searchString)
        {
            ViewBag.HospitalNameParm = String.IsNullOrEmpty(sortOrder) ? "HospitalName" : "";
           
            var hospitals = db.Hospitals.Include(h => h.EnrollmentStatu).Include(h => h.State).Include(h => h.User);
            if (!String.IsNullOrEmpty(searchString))
            {

                hospitals = hospitals.Where(s => s.HospitalName.Contains(searchString));

            }

            switch (sortOrder)
            {
                case "HospitalName":
                    hospitals = hospitals.OrderByDescending(p => p.HospitalName);
                    break;               
            }

            return View(hospitals.ToList());
        }
        public ActionResult PatientRequestStatus()
        {
            //var hospitals = db.Hospitals.Include(h => h.EnrollmentStatu).Include(h => h.State).Include(h => h.User);
            //return View(hospitals.ToList());
            return RedirectToAction("Index", "PatientDataRequest");
        }

        public ActionResult ArrayHandler(IEnumerable<int> Hospitalids)
        {
            if (Hospitalids != null)
            {
                string enrolledstatus = "Awaiting";
                var hospitalsRequested = db.Hospitals.Where(h => Hospitalids.Contains(h.Id)).ToList();
                int enrolledstatusId = Convert.ToInt32(db.EnrollmentStatus.Where(e => e.Status == enrolledstatus).First().Id);
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("AdminHome", "HIEAdmin", new { });
                foreach (var item in hospitalsRequested)
                {
                    item.EnrollmentStatus = enrolledstatusId;
                    db.Entry(item).State = EntityState.Modified;
                }
                try
                {
                    db.SaveChanges();
                    return Json(new { Url = redirectUrl, status = "OK" });
                }
                catch (Exception)
                {
                    return Json(new { Url = redirectUrl, status = "Error" });
                }
            }
            else
            {
                return RedirectToAction("AdminHome");
            }
           
        }

    }
}
