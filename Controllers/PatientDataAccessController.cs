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
    public class PatientDataAccessController : Controller
    {
        private HealthInformationExchangeEntities db = new HealthInformationExchangeEntities();

        // GET: /PatientDataAccess/
        public ActionResult Index()
        {
            var patientdataaccesses = db.PatientDataAccesses.Include(p => p.Hospital).Include(p => p.Patient);
            return View(patientdataaccesses.ToList());
        }

        // GET: /PatientDataAccess/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientDataAccess patientdataaccess = db.PatientDataAccesses.Find(id);
            if (patientdataaccess == null)
            {
                return HttpNotFound();
            }
            return View(patientdataaccess);
        }

        // GET: /PatientDataAccess/Create
        public ActionResult Create()
        {
            ViewBag.HospitalId = new SelectList(db.Hospitals, "Id", "HospitalName");
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "AadharId");
            return View();
        }

        // POST: /PatientDataAccess/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,PatientId,HospitalId,AccessRequestDate,IsLatest")] PatientDataAccess patientdataaccess)
        {
            if (ModelState.IsValid)
            {
                db.PatientDataAccesses.Add(patientdataaccess);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HospitalId = new SelectList(db.Hospitals, "Id", "HospitalName", patientdataaccess.HospitalId);
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "AadharId", patientdataaccess.PatientId);
            return View(patientdataaccess);
        }

        // GET: /PatientDataAccess/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientDataAccess patientdataaccess = db.PatientDataAccesses.Find(id);
            if (patientdataaccess == null)
            {
                return HttpNotFound();
            }
            ViewBag.HospitalId = new SelectList(db.Hospitals, "Id", "HospitalName", patientdataaccess.HospitalId);
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "AadharId", patientdataaccess.PatientId);
            return View(patientdataaccess);
        }

        // POST: /PatientDataAccess/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,PatientId,HospitalId,AccessRequestDate,IsLatest")] PatientDataAccess patientdataaccess)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patientdataaccess).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HospitalId = new SelectList(db.Hospitals, "Id", "HospitalName", patientdataaccess.HospitalId);
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "AadharId", patientdataaccess.PatientId);
            return View(patientdataaccess);
        }

        // GET: /PatientDataAccess/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientDataAccess patientdataaccess = db.PatientDataAccesses.Find(id);
            if (patientdataaccess == null)
            {
                return HttpNotFound();
            }
            return View(patientdataaccess);
        }

        // POST: /PatientDataAccess/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PatientDataAccess patientdataaccess = db.PatientDataAccesses.Find(id);
            db.PatientDataAccesses.Remove(patientdataaccess);
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
