using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VirtualHIE.Models;

namespace VirtualHIE.Controllers
{
    public class PatientController : Controller
    {
        private HealthInformationExchangeEntities db = new HealthInformationExchangeEntities();

        // GET: /Patient/
 
        public ActionResult Index(string sortOrder, string searchString, string reqType)
        {
            ViewBag.ReqType = reqType;
            ViewBag.FirstNameSortParm = String.IsNullOrEmpty(sortOrder) ? "FirstName" : "";
            ViewBag.LastNameSortParm = sortOrder == "LastName" ? "LastName_desc" : "LastName";
            ViewBag.AadharIDSortParm = sortOrder == "AadharID" ? "AadharID_desc" : "AadharID";
            ViewBag.DateSortParm = sortOrder == "DOB_asc" ? "DOB_desc" : "DOB_asc";
            var patients = from p in db.Patients
                           select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstName.Contains(searchString) || s.AadharId.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "FirstName":
                    patients = patients.OrderByDescending(p => p.FirstName);
                    break;
                case "DOB_asc":
                    patients = patients.OrderBy(s => s.DateOfBirth);
                    break;
                case "DOB_desc":
                    patients = patients.OrderByDescending(s => s.DateOfBirth);
                    break;
                case "LastName":
                    patients = patients.OrderBy(s => s.LastName);
                    break;
                case "LastName_desc":
                    patients = patients.OrderByDescending(s => s.LastName);
                    break;
                case "AadharID":
                    patients = patients.OrderBy(s => s.AadharId);
                    break;
                case "AadharID_desc":
                    patients = patients.OrderByDescending(s => s.AadharId);
                    break;
                default:
                    patients = patients.OrderBy(s => s.FirstName);
                    break;
            }
            return View(patients.ToList());
        }

        // GET: /Patient/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // GET: /Patient/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Patient/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AadharId,FirstName,LastName,Address,DateOfBirth,MobileNumber,EmailId")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Patients.Add(patient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(patient);
        }

        // GET: /Patient/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: /Patient/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AadharId,FirstName,LastName,Address,DateOfBirth,MobileNumber,EmailId")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        // GET: /Patient/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: /Patient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            db.Patients.Remove(patient);
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
