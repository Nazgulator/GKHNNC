using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GKHNNC.DAL;
using GKHNNC.Models;

namespace GKHNNC.Controllers
{
    public class PassportWorkTypesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: PassportWorkTypes
        public ActionResult Index()
        {
            return View(db.PassportWorkType.ToList());
        }

        // GET: PassportWorkTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassportWorkType passportWorkType = db.PassportWorkType.Find(id);
            if (passportWorkType == null)
            {
                return HttpNotFound();
            }
            return View(passportWorkType);
        }

        // GET: PassportWorkTypes/Create
        public ActionResult Create()
        {
            return View();
        }



        [HttpPost]
        public JsonResult UploadPDF(int WorkId)
        {

            PassportWork PW = db.PassportWork.Where(x => x.Id == WorkId).First();
            //проверяем директорию и создаем если её нет
            if (Directory.Exists(Server.MapPath("~/Files")) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files"));

            }
            if (Directory.Exists(Server.MapPath("~/Files/Adres")) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files/Adres"));

            }
            if (Directory.Exists(Server.MapPath("~/Files/Adres/" + PW.AdresId.ToString())) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files/Adres/" + PW.AdresId.ToString()));

            }



            string fileName = "";
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    // получаем имя файла
                    fileName = System.IO.Path.GetFileName(upload.FileName);

                    var path = Server.MapPath("~/Files/Adres/" + PW.AdresId.ToString() + "/" + fileName);
                    upload.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                    upload.SaveAs(Server.MapPath("~/Files/Adres/" + PW.AdresId.ToString() + "/" + fileName));
                    PW.FilePath = fileName;
                    PW.Est = true;
                    try
                    {
                        db.Entry(PW).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch
                    {

                    }
                }
            }
            return Json(fileName);
        }


        [HttpPost]
        public JsonResult DeletePassport(int PassportId)
        {

            PassportWork PW = db.PassportWork.Where(x => x.Id == PassportId).First();

            string fileName = "";
           
             
                    PW.FilePath = "";
                    PW.Est = false;
                    try
                    {
                        db.Entry(PW).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch
                    {

                    }
                
            
            return Json(fileName);
        }

        public ActionResult PassportWorks(int AdresId =0)
        {
            List<PassportWork> PW = new List<PassportWork>();
           
            int CountEst = db.PassportWork.Where(x => x.AdresId == AdresId).Count();
            int CountNugno = db.PassportWorkType.Count();

            if (CountNugno > 0)
            {
                Adres A = db.Adres.Where(x => x.Id == AdresId).First();
                List<PassportWorkType> PWT = new List<PassportWorkType>();
                try
                {
                    PW = db.PassportWork.Where(x => x.AdresId == AdresId).Include(x => x.PassportWorkType).Include(x => x.Adres).ToList();
                    PWT = db.PassportWorkType.ToList();

                }
                catch (Exception e)
                {

                }
                if (CountEst < CountNugno)
                {
                    foreach (var p in PWT)
                    {
                        if (PW.Where(x=>x.PassportWorkTypeId ==p.Id).Count()==0)
                        {
                            try
                            {
                                PassportWork pw = new PassportWork();
                                pw.Est = false;
                                pw.FilePath = "";
                                pw.AdresId = AdresId;
                                pw.PassportWorkTypeId = p.Id;
                             
                                db.PassportWork.Add(pw);
                                db.SaveChanges();
                                pw.Adres = A;
                                pw.PassportWorkType = p;
                                PW.Add(pw);
                            }
                            catch
                            {

                            }
                        }
                    }
            }

            }

            //  List<int> WorksInSpisok = PW.Select(x => x.PassportWorkTypeId).Distinct().ToList();




            return View(PW);
        }

        // POST: PassportWorkTypes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] PassportWorkType passportWorkType)
        {
            if (ModelState.IsValid)
            {
                db.PassportWorkType.Add(passportWorkType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(passportWorkType);
        }

        // GET: PassportWorkTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassportWorkType passportWorkType = db.PassportWorkType.Find(id);
            if (passportWorkType == null)
            {
                return HttpNotFound();
            }
            return View(passportWorkType);
        }

        // POST: PassportWorkTypes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] PassportWorkType passportWorkType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(passportWorkType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(passportWorkType);
        }

        // GET: PassportWorkTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassportWorkType passportWorkType = db.PassportWorkType.Find(id);
            if (passportWorkType == null)
            {
                return HttpNotFound();
            }
            return View(passportWorkType);
        }

        // POST: PassportWorkTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PassportWorkType passportWorkType = db.PassportWorkType.Find(id);
            db.PassportWorkType.Remove(passportWorkType);
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
