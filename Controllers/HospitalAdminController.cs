using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualHIE.Models;

namespace VirtualHIE.Controllers
{
    [SessionTimeout]
    public class HospitalAdminController : Controller
    {
        private HealthInformationExchangeEntities db = new HealthInformationExchangeEntities();

        // GET: /HospitalAdmin/
        public ActionResult HospitalHome()
        {
            if (Session["UserID"] != null)
            {
                string loggedinUserID = Session["UserID"].ToString();
                var user = db.Users.Where(u => u.UserId == loggedinUserID).First();
                var enrollmentStatus = db.Hospitals.Where(h => h.UserId == user.id).Select(i => new { i.EnrollmentStatu.Status, i.Id });

                Session["LoggedinHospID"] = enrollmentStatus.ToArray()[0].Id.ToString().Trim();
                Session["enrollmentStatus"] = enrollmentStatus.ToArray()[0].Status.ToString().Trim();
                if (enrollmentStatus.ToArray()[0].Status.ToString().Trim() != "Enrolled")
                {
                    return View();
                }
                else
                {
                    return View("Enrolled");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult HospitalHome(string Status)
        {
            string loggedinUserID = Session["UserID"].ToString();
            var user = db.Users.Where(u => u.UserId == loggedinUserID).First();
            var hos = db.Hospitals.Where(h => h.UserId == user.id).First();
            hos.EnrollmentStatus = db.EnrollmentStatus.Where(e => e.Status == Status).First().Id;
            db.Entry(hos).State = EntityState.Modified;
            db.SaveChanges();
            if (Status == "Enrolled")
            {
                return View("Enrolled");

            }
            else
            {
                ModelState.AddModelError("Information", "You have Refused to Enroll to HIE.");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Enrolled()
        {
            string loggedinUserID = Session["UserID"].ToString();
            var user = db.Users.Where(u => u.UserId == loggedinUserID).First();

            return View();
        }

    }
}