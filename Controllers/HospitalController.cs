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
    public class HospitalController : Controller
    {
        private HealthInformationExchangeEntities db = new HealthInformationExchangeEntities();

        // GET: /Hospital/
        public ActionResult Index()
        {
            var hospitals = db.Hospitals.Include(h => h.EnrollmentStatu).Include(h => h.State).Include(h => h.User);
            return View(hospitals.ToList());
        }

        // GET: /Hospital/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Find(id);
            if (hospital == null)
            {
                return HttpNotFound();
            }
            return View(hospital);
        }

        // GET: /Hospital/Create
        public ActionResult Create()
        {
            ViewBag.EnrollmentStatus = new SelectList(db.EnrollmentStatus, "Id", "Status",2);
            ViewBag.StateId = new SelectList(db.States, "StateId", "Name");
            ViewBag.UserId = new SelectList(db.Users, "id", "UserId");
            return View();
        }

        // POST: /Hospital/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,HospitalName,Address,StateId,UserId,EnrollmentStatus")] Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                db.Hospitals.Add(hospital);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EnrollmentStatus = new SelectList(db.EnrollmentStatus, "Id", "Status", hospital.EnrollmentStatus);
            ViewBag.StateId = new SelectList(db.States, "StateId", "Name", hospital.StateId);
            ViewBag.UserId = new SelectList(db.Users, "id", "UserId", hospital.UserId);
            return View(hospital);
        }

        // GET: /Hospital/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Find(id);
            if (hospital == null)
            {
                return HttpNotFound();
            }
            ViewBag.EnrollmentStatus = new SelectList(db.EnrollmentStatus, "Id", "Status", hospital.EnrollmentStatus);
            ViewBag.StateId = new SelectList(db.States, "StateId", "Name", hospital.StateId);
            ViewBag.UserId = new SelectList(db.Users, "id", "UserId", hospital.UserId);
            return View(hospital);
        }

        // POST: /Hospital/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,HospitalName,Address,StateId,UserId,EnrollmentStatus")] Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hospital).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EnrollmentStatus = new SelectList(db.EnrollmentStatus, "Id", "Status", hospital.EnrollmentStatus);
            ViewBag.StateId = new SelectList(db.States, "StateId", "Name", hospital.StateId);
            ViewBag.UserId = new SelectList(db.Users, "id", "UserId", hospital.UserId);
            return View(hospital);
        }

        // GET: /Hospital/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Find(id);
            if (hospital == null)
            {
                return HttpNotFound();
            }
            return View(hospital);
        }

        // POST: /Hospital/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hospital hospital = db.Hospitals.Find(id);
            db.Hospitals.Remove(hospital);
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
