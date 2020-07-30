using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GKHNNC.DAL;
using GKHNNC.Models;

namespace GKHNNC.Controllers
{
    public class OsmotrRecommendWorksController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: OsmotrWorks
        public ActionResult Index()
        {
            var osmotrWorks = db.OsmotrRecommendWorks.Include(o => o.DOMPart).Include(o => o.Izmerenie);
            return View(osmotrWorks.ToList());
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
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IzmerenieId,Cost,DOMPartId,Smeta")] OsmotrRecommendWork ORW)
        {
            if (ModelState.IsValid)
            {
                db.OsmotrRecommendWorks.Add(ORW);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name", ORW.DOMPartId);
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", ORW.IzmerenieId);
            return View(ORW);
        }

        // GET: OsmotrWorks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OsmotrRecommendWork osmotrWork = db.OsmotrRecommendWorks.Find(id);
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
        public ActionResult Edit([Bind(Include = "Id,Name,IzmerenieId,Cost,DOMPartId,Smeta")] OsmotrRecommendWork osmotrWork)
        {
            if (ModelState.IsValid)
            {
                db.Entry(osmotrWork).State = EntityState.Modified;
                db.SaveChanges();
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
            OsmotrRecommendWork osmotrWork = db.OsmotrRecommendWorks.Find(id);
            if (osmotrWork == null)
            {
                return HttpNotFound();
            }

            return View(osmotrWork);
        }

        // POST: OsmotrWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OsmotrRecommendWork osmotrWork = db.OsmotrRecommendWorks.Find(id);
            db.OsmotrRecommendWorks.Remove(osmotrWork);
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
