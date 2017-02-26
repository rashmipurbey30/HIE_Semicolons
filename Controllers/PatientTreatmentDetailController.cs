using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VirtualHIE.Models;

namespace VirtualHIE.Controllers
{
    [SessionTimeout]
    public class PatientTreatmentDetailController : Controller
    {
        private HealthInformationExchangeEntities db = new HealthInformationExchangeEntities();

        // GET: /PatientTreatmentDetail/
        public ActionResult Index(int? id)
        {
            var patienttreatmentdetails = db.PatientTreatmentDetails.Include(p => p.Hospital).Include(p => p.Patient).OrderByDescending(p => p.CreatedOn);
            if (Session["UserId"].ToString().Trim() != "HIE_Admin")
            {

                if (id != null)
                {
                    int hospId = Convert.ToInt32(Session["LoggedinHospID"]);
                    patienttreatmentdetails = db.PatientTreatmentDetails.Where(p => p.PatientId == id).OrderByDescending(p => p.CreatedOn);
                    var patientaccessrecord = db.PatientDataAccesses.Where(p => p.PatientId == id && p.HospitalId == hospId).Select(s => s);
                    var IsLatestHospital = patientaccessrecord.Count(p => p.IsLatest == true);
                    if (IsLatestHospital == 0)
                    {
                        patienttreatmentdetails = patienttreatmentdetails.Where(p => p.HospitalId == hospId).OrderByDescending(p => p.CreatedOn);
                    }
                }
            }
            else
            {
                if (id != null)
                {
                    patienttreatmentdetails = db.PatientTreatmentDetails.Where(p => p.PatientId == id).OrderByDescending(p => p.CreatedOn);
                }
            }
            return View(patienttreatmentdetails.ToList());
        }

        // GET: /PatientTreatmentDetail/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientTreatmentDetail patienttreatmentdetail = db.PatientTreatmentDetails.Find(id);
            if (patienttreatmentdetail == null)
            {
                return HttpNotFound();
            }
            return View(patienttreatmentdetail);
        }

        // GET: /PatientTreatmentDetail/Create
        public ActionResult Create(int? id)
        {


            // Get Broadcasted Patient names for other requested hospitals

            if (Session["UserId"].ToString().Trim() != "HIE_Admin")
            {
                if (Session["LoggedinHospID"] != null)
                {
                    int hospId = Convert.ToInt32(Session["LoggedinHospID"]);

                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Patient patient = db.Patients.Find(id);

                    PatientTreatmentDetail patientTreatmentDetail = new PatientTreatmentDetail();
                    patientTreatmentDetail.Patient = patient;
                    patientTreatmentDetail.PatientId = patient.Id;
                    if (patientTreatmentDetail == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.HospitalId = new SelectList(db.Hospitals, "Id", "HospitalName");
                    return View(patientTreatmentDetail);

                }
                else
                {
                    return RedirectToAction("Index", "PatientTreatmentDetail");
                }
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Patient patient = db.Patients.Find(id);

                PatientTreatmentDetail patientTreatmentDetail = new PatientTreatmentDetail();
                patientTreatmentDetail.Patient = patient;
                patientTreatmentDetail.PatientId = patient.Id;
                if (patientTreatmentDetail == null)
                {
                    return HttpNotFound();
                }


                ViewBag.HospitalId = new SelectList(db.Hospitals.ToList().Where(h => h.EnrollmentStatus == 1), "Id", "HospitalName");
                //ViewBag.PatientId = new SelectList(db.Patients, "Id", "Name");
                return View(patientTreatmentDetail);
            }
        }

        // POST: /PatientTreatmentDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PatientId,HospitalId,TreatmentDetails,Files,CreatedBy,CreatedOn")] PatientTreatmentDetail patienttreatmentdetail)
        {
            if (ModelState.IsValid)
            {
                db.PatientTreatmentDetails.Add(patienttreatmentdetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HospitalId = new SelectList(db.Hospitals, "Id", "HospitalName", patienttreatmentdetail.HospitalId);
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "AadharId", patienttreatmentdetail.PatientId);
            return View(patienttreatmentdetail);
        }

        // GET: /PatientTreatmentDetail/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientTreatmentDetail patienttreatmentdetail = db.PatientTreatmentDetails.Find(id);
            if (patienttreatmentdetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.HospitalId = new SelectList(db.Hospitals, "Id", "HospitalName", patienttreatmentdetail.HospitalId);
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "AadharId", patienttreatmentdetail.PatientId);
            return View(patienttreatmentdetail);
        }

        // POST: /PatientTreatmentDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PatientId,HospitalId,TreatmentDetails,Files,CreatedBy,CreatedOn")] PatientTreatmentDetail patienttreatmentdetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patienttreatmentdetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HospitalId = new SelectList(db.Hospitals, "Id", "HospitalName", patienttreatmentdetail.HospitalId);
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "AadharId", patienttreatmentdetail.PatientId);
            return View(patienttreatmentdetail);
        }

        // GET: /PatientTreatmentDetail/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientTreatmentDetail patienttreatmentdetail = db.PatientTreatmentDetails.Find(id);
            if (patienttreatmentdetail == null)
            {
                return HttpNotFound();
            }
            return View(patienttreatmentdetail);
        }

        // POST: /PatientTreatmentDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PatientTreatmentDetail patienttreatmentdetail = db.PatientTreatmentDetails.Find(id);
            db.PatientTreatmentDetails.Remove(patienttreatmentdetail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Createdata([Bind(Include = "Id,PatientId,HospitalId,TreatmentDetails,Files,CreatedBy,CreatedOn")] PatientTreatmentDetail model)
        {
            if (Session["UserId"].ToString().Trim() != "HIE_Admin")
            {
                int hospId = Convert.ToInt32(Session["LoggedinHospID"]);
                model.HospitalId = hospId;
            }

            HttpPostedFileBase file = Request.Files["ImageData"];
            int i = UploadImageInDataBase(file, model);
            if (i == 1)
            {
                if (Session["UserId"].ToString().Trim() != "HIE_Admin")
                {
                    return RedirectToAction("Index", "PatientTreatmentDetail", new { id=model.PatientId});
                }
                else
                {
                    return RedirectToAction("Index", "PatientTreatmentDetail", new { id = model.PatientId });
                }


            }
            return View(model);
        }

        public int UploadImageInDataBase(HttpPostedFileBase file, PatientTreatmentDetail patientTreatmentmodel)
        {
            patientTreatmentmodel.Files = ConvertToBytes(file);
            var PatientTreatmentData = new PatientTreatmentDetail
            {
                PatientId = patientTreatmentmodel.PatientId,
                HospitalId = patientTreatmentmodel.HospitalId,
                TreatmentDetails = patientTreatmentmodel.TreatmentDetails,
                Files = patientTreatmentmodel.Files,
                CreatedBy = patientTreatmentmodel.CreatedBy,
                CreatedOn = patientTreatmentmodel.CreatedOn

            };
            db.PatientTreatmentDetails.Add(PatientTreatmentData);
            int i = db.SaveChanges();
            if (i == 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }


        /// <summary>
        /// Retrive Image from database 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RetrieveImage(int id)
        {
            byte[] cover = GetImageFromDataBase(id);
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public byte[] GetImageFromDataBase(int Id)
        {
            var q = from temp in db.PatientTreatmentDetails where temp.Id == Id select temp.Files;
            byte[] cover = q.First();
            return cover;
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
