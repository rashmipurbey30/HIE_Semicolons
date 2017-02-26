using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VirtualHIE.Models;

namespace VirtualHIE.Controllers
{
    [SessionTimeout]
    public class PatientDataRequestController : Controller
    {
        private HealthInformationExchangeEntities db = new HealthInformationExchangeEntities();

        // GET: /PatientDataRequest/
        public ActionResult Index()
        {

            if (TempData["dat"] == null)
            {
                string loggedinUserID = Session["UserID"].ToString();
                var user = db.Users.Where(u => u.UserId == loggedinUserID).First();
                if (user.Role.RoleName.ToString().Trim() == "HIEAdmin")
                {
                    // if user role is HIEAdmin then display all records of all hosipitals
                    var patientdatarequests = db.PatientDataRequests.Include(p => p.Hospital).Include(p => p.Patient).Include(p => p.PatientDataRequestStatu);
                    return View(patientdatarequests.ToList().OrderByDescending(p=> p.RequestDate));
                }
                else
                {
                    // if user role is HospitalAdmin then display records for logged in hospital only with filtered status as "Broadcast"
                    var hospital = db.Hospitals.Where(h => h.UserId == user.id).First();
                    var patientdatarequests = db.PatientDataRequests.Where(p => p.Hospital.Id != hospital.Id && p.PatientDataRequestStatu.Id == 3).ToList();
                    return View(patientdatarequests.ToList().OrderByDescending(p=> p.RequestDate));
                }
            }
            else
            {
                return View(TempData["dat"]);
            }

        }

        // GET: /PatientDataRequest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientDataRequest patientdatarequest = db.PatientDataRequests.Find(id);
            if (patientdatarequest == null)
            {
                return HttpNotFound();
            }
            return View(patientdatarequest);
        }

        // GET: /PatientDataRequest/Create
        public ActionResult Create(int? id)
        {
            var hospitalList = db.Hospitals.Where(h => h.EnrollmentStatus == 1).ToList();
            ViewBag.HospitalId = new SelectList(hospitalList, "Id", "HospitalName");

            if (Session["UserID"] != null && Session["UserID"].ToString().Trim() != "HIE_Admin")
            {
                if (Session["LoggedinHospID"] != null)
                {
                    int hospId = Convert.ToInt32(Session["LoggedinHospID"]);


                    var patientIDList = db.PatientDataRequests.Where(p => p.HospitalId == hospId && p.Status != 2).ToList().Select(s=> s.PatientId);
                    var patientList1 = from s in db.Patients.ToList()
                                      where !patientIDList.Contains(s.Id)
                                      select s;
                    var patientList2 = db.Patients.Where(h => !patientIDList.Contains(h.Id)).ToList();

                    ViewBag.PatientId = new SelectList(patientList2, "Id", "Name");

                }
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            // PatientDataRequest patientdatarequest = db.PatientDataRequests.Where(p => p.PatientId == id).FirstOrDefault();
            PatientDataRequest patientdatarequest = new PatientDataRequest();
            patientdatarequest.Patient = patient;
            patientdatarequest.PatientId = patient.Id;
            if (patientdatarequest == null)
            {
                return HttpNotFound();
            }
                      
           return View(patientdatarequest);
           
        }

        // POST: /PatientDataRequest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,HospitalId,PatientId,RequestDate,Status")] PatientDataRequest patientdatarequest)
        {
            if (Session["LoggedinHospID"] != null)
            {
                int hospId = Convert.ToInt32(Session["LoggedinHospID"]);
                patientdatarequest.HospitalId = hospId;
            }
            patientdatarequest.Status = 1;
            if (ModelState.IsValid)
            {
                db.PatientDataRequests.Add(patientdatarequest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HospitalId = new SelectList(db.Hospitals, "Id", "HospitalName", patientdatarequest.HospitalId);
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "AadharId", patientdatarequest.PatientId);
            ViewBag.Status = new SelectList(db.PatientDataRequestStatus, "Id", "Status", patientdatarequest.Status);
            return View(patientdatarequest);
        }

        // GET: /PatientDataRequest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientDataRequest patientdatarequest = db.PatientDataRequests.Find(id);
            if (patientdatarequest == null)
            {
                return HttpNotFound();
            }
            ViewBag.HospitalId = new SelectList(db.Hospitals, "Id", "HospitalName", patientdatarequest.HospitalId);
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "AadharId", patientdatarequest.PatientId);
            ViewBag.Status = new SelectList(db.PatientDataRequestStatus, "Id", "Status", patientdatarequest.Status);
            return View(patientdatarequest);
        }

        // POST: /PatientDataRequest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,HospitalId,PatientId,RequestDate,Status")] PatientDataRequest patientdatarequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patientdatarequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HospitalId = new SelectList(db.Hospitals, "Id", "HospitalName", patientdatarequest.HospitalId);
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "AadharId", patientdatarequest.PatientId);
            ViewBag.Status = new SelectList(db.PatientDataRequestStatus, "Id", "Status", patientdatarequest.Status);
            return View(patientdatarequest);
        }

        // GET: /PatientDataRequest/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientDataRequest patientdatarequest = db.PatientDataRequests.Find(id);
            if (patientdatarequest == null)
            {
                return HttpNotFound();
            }
            return View(patientdatarequest);
        }

        // POST: /PatientDataRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PatientDataRequest patientdatarequest = db.PatientDataRequests.Find(id);
            db.PatientDataRequests.Remove(patientdatarequest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Search(string searchBy)
        {
            List<PatientDataRequest> lstResult = null;

            lstResult = db.PatientDataRequests.Include(p => p.Hospital).Include(p => p.Patient).Include(p => p.PatientDataRequestStatu).Where(x => x.PatientDataRequestStatu.Status.Contains(searchBy)).ToList();
            TempData["dat"] = lstResult.OrderByDescending(p=> p.RequestDate).ToList();
            return RedirectToAction("Index");
        }

        public ActionResult ArrayHandler(IEnumerable<int> PDRids, string Status)
        {

            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "PatientDataRequest", new { });
            try
            {
                var PatientDataRequested = db.PatientDataRequests.Where(h => PDRids.Contains(h.id)).ToList();
                int PDRstatusId = Convert.ToInt32(db.PatientDataRequestStatus.Where(e => e.Status == Status).First().Id);

                foreach (var item in PatientDataRequested)
                {
                    item.Status = PDRstatusId;
                    db.Entry(item).State = EntityState.Modified;
                }

                db.SaveChanges();

                if (Status == "Closed")
                {

                    var patients = db.PatientDataAccesses.ToList().Where(pt => PatientDataRequested.Any(pdr => pt.PatientId == pdr.PatientId)).Select(s => s);
                    foreach (var item in patients)
                    {
                        item.IsLatest = false;
                        db.Entry(item).State = EntityState.Modified;
                    }

                    db.SaveChanges();

                    PatientDataAccess pdrdata = new PatientDataAccess();
                    foreach (var item in PatientDataRequested)
                    {
                        pdrdata.HospitalId = item.HospitalId;
                        pdrdata.PatientId = item.PatientId;
                        pdrdata.IsLatest = true;
                        pdrdata.AccessRequestDate = DateTime.Now;
                        db.PatientDataAccesses.Add(pdrdata);
                        db.SaveChanges();
                    }
                }
                return Json(new { Url = redirectUrl, status = "OK" });
            }
            catch (Exception)
            {
                return Json(new { Url = redirectUrl, status = "Error" });

            }

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
