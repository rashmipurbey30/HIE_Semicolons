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
    public class EnrollmentStatusController : Controller
    {
        private HealthInformationExchangeEntities db = new HealthInformationExchangeEntities();

        // GET: /EnrollmentStatus/
        public ActionResult Index()
        {
            return View(db.EnrollmentStatus.ToList());
        }

        // GET: /EnrollmentStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EnrollmentStatu enrollmentstatu = db.EnrollmentStatus.Find(id);
            if (enrollmentstatu == null)
            {
                return HttpNotFound();
            }
            return View(enrollmentstatu);
        }

        // GET: /EnrollmentStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /EnrollmentStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Status")] EnrollmentStatu enrollmentstatu)
        {
            if (ModelState.IsValid)
            {
                db.EnrollmentStatus.Add(enrollmentstatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(enrollmentstatu);
        }

        // GET: /EnrollmentStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EnrollmentStatu enrollmentstatu = db.EnrollmentStatus.Find(id);
            if (enrollmentstatu == null)
            {
                return HttpNotFound();
            }
            return View(enrollmentstatu);
        }

        // POST: /EnrollmentStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Status")] EnrollmentStatu enrollmentstatu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enrollmentstatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(enrollmentstatu);
        }

        // GET: /EnrollmentStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EnrollmentStatu enrollmentstatu = db.EnrollmentStatus.Find(id);
            if (enrollmentstatu == null)
            {
                return HttpNotFound();
            }
            return View(enrollmentstatu);
        }

        // POST: /EnrollmentStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EnrollmentStatu enrollmentstatu = db.EnrollmentStatus.Find(id);
            db.EnrollmentStatus.Remove(enrollmentstatu);
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
