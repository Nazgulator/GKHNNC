using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office2010.Excel;
using GKHNNC.DAL;
using GKHNNC.Models;

namespace GKHNNC.Controllers
{
    public class OsmotrWorksController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: OsmotrWorks
        public ActionResult Index()
        {
            var osmotrWorks = db.OsmotrWorks.Include(o => o.DOMPart).Include(o => o.Izmerenie).OrderBy(x=>x.DOMPartId).ThenBy(x=>x.Name).ToList();
            List<SelectListItem> OW = db.OsmotrWorks.OrderBy(x => x.DOMPartId).ThenBy(x=>x.Name).Select(a => new SelectListItem { Value = a.Id.ToString(), Text =a.DOMPart.Name+" "+ a.Name }).ToList();
            ViewBag.OW = OW;
            return View(osmotrWorks);
        }

        // GET: OsmotrWorks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OsmotrWork osmotrWork = db.OsmotrWorks.Find(id);
            if (osmotrWork == null)
            {
                return HttpNotFound();
            }
            return View(osmotrWork);
        }

        // GET: OsmotrWorks/Create
        public ActionResult Create()
        {
            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name");
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name");
            return View();
        }

        // POST: OsmotrWorks/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult Actual(int Id)
        {
           var w = db.OsmotrWorks.Where(x => x.Id == Id).First();
            w.Archive = false;
            db.Entry(w).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Ok");
        }

        [HttpPost]
        public JsonResult Archive(int Id)
        {
            var w = db.OsmotrWorks.Where(x => x.Id == Id).First();
            w.Archive = true;
            db.Entry(w).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Ok");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IzmerenieId,Cost,DOMPartId,OtchetId")] OsmotrWork osmotrWork)
        {
            //  if (ModelState.IsValid)
            //   {
            db.OsmotrWorks.Add(osmotrWork);
            db.SaveChanges();
            return RedirectToAction("Index");
            //  }

            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name", osmotrWork.DOMPartId);
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", osmotrWork.IzmerenieId);
            return View(osmotrWork);
        }

        // GET: OsmotrWorks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OsmotrWork osmotrWork = db.OsmotrWorks.Find(id);
            if (osmotrWork == null)
            {
                return HttpNotFound();
            }
            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name", osmotrWork.DOMPartId);
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", osmotrWork.IzmerenieId);


            return View(osmotrWork);
        }

        // POST: OsmotrWorks/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,IzmerenieId,Cost,DOMPartId,OtchetId")] OsmotrWork osmotrWork)
        {
            if (ModelState.IsValid)
            {
                db.Entry(osmotrWork).State = EntityState.Modified;
                db.SaveChanges();


                //List<ActiveOsmotrWork> AOW = db.ActiveOsmotrWorks.Where(x => x.OsmotrWorkId == osmotrWork.Id&&x.ElementId == osmotrWork.).ToList();
                //foreach (var aow in AOW)
                //{
                //    aow.ElementId = osmotrWork.;
                //    db.Entry(aow).State = EntityState.Modified;
                //    db.SaveChanges();
                //}
                return RedirectToAction("Index");
            }

            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name", osmotrWork.DOMPartId);
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", osmotrWork.IzmerenieId);
            return View(osmotrWork);
        }

        // GET: OsmotrWorks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OsmotrWork osmotrWork = db.OsmotrWorks.Find(id);
            if (osmotrWork == null)
            {
                return HttpNotFound();
            }
            return View(osmotrWork);
        }
        [HttpPost]
        public JsonResult Count(int? id)
        {
            if (id == null)
            {
                return Json("Error");
            }
            OsmotrWork osmotrWork = db.OsmotrWorks.Find(id);
            int Col = db.ActiveOsmotrWorks.Where(x => x.OsmotrWorkId == id).Count();
            
            return Json("Таких активных работ в осмотрах "+Col);
        }
        [HttpPost]
        public JsonResult Replace(int? FromId,int?ToId)
        {
            if (FromId == null&&ToId==null)
            {
                return Json("Error");
            }
      
            List<ActiveOsmotrWork> From = db.ActiveOsmotrWorks.Where(x => x.OsmotrWorkId == FromId).ToList();

            int Count = 0;
            foreach (ActiveOsmotrWork F in From)
            {
                try
                {
                    F.OsmotrWorkId = ToId.Value;
                    db.Entry(F).State = EntityState.Modified;
                    db.SaveChanges();
                    Count++;
                }
                catch
                {
                    return Json("Error");
                }
            }
            return Json("Произведено замен " + Count);
        }

        // POST: OsmotrWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OsmotrWork osmotrWork = db.OsmotrWorks.Find(id);
           
            int Col = db.ActiveOsmotrWorks.Where(x => x.OsmotrWorkId == id).Count();
            if (Col == 0)
            {
                db.OsmotrWorks.Remove(osmotrWork);
                db.SaveChanges();
            }
            else
            {
                return Json("Работы не пустые!!!");
            }
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
