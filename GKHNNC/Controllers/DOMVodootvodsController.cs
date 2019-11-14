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
    public class DOMVodootvodsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: DOMVodootvods
        public ActionResult Index()
        {
            return View(db.DOMVodootvods.ToList());
        }

        // GET: DOMVodootvods/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMVodootvod dOMVodootvod = db.DOMVodootvods.Find(id);
            if (dOMVodootvod == null)
            {
                return HttpNotFound();
            }
            return View(dOMVodootvod);
        }

        // GET: DOMVodootvods/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DOMVodootvods/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id")] DOMVodootvod dOMVodootvod)
        {
            if (ModelState.IsValid)
            {
                db.DOMVodootvods.Add(dOMVodootvod);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dOMVodootvod);
        }

        // GET: DOMVodootvods/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMVodootvod dOMVodootvod = db.DOMVodootvods.Find(id);
            if (dOMVodootvod == null)
            {
                return HttpNotFound();
            }
            return View(dOMVodootvod);
        }

        // POST: DOMVodootvods/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id")] DOMVodootvod dOMVodootvod)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dOMVodootvod).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dOMVodootvod);
        }

        // GET: DOMVodootvods/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMVodootvod dOMVodootvod = db.DOMVodootvods.Find(id);
            if (dOMVodootvod == null)
            {
                return HttpNotFound();
            }
            return View(dOMVodootvod);
        }

        // POST: DOMVodootvods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DOMVodootvod dOMVodootvod = db.DOMVodootvods.Find(id);
            db.DOMVodootvods.Remove(dOMVodootvod);
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
