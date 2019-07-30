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
    public class FasadTypesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: FasadTypes
        public ActionResult Index()
        {
            return View(db.FasadTypes.ToList());
        }

        // GET: FasadTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FasadType fasadType = db.FasadTypes.Find(id);
            if (fasadType == null)
            {
                return HttpNotFound();
            }
            return View(fasadType);
        }

        // GET: FasadTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FasadTypes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type")] FasadType fasadType)
        {
            if (ModelState.IsValid)
            {
                db.FasadTypes.Add(fasadType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fasadType);
        }

        // GET: FasadTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FasadType fasadType = db.FasadTypes.Find(id);
            if (fasadType == null)
            {
                return HttpNotFound();
            }
            return View(fasadType);
        }

        // POST: FasadTypes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type")] FasadType fasadType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fasadType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fasadType);
        }

        // GET: FasadTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FasadType fasadType = db.FasadTypes.Find(id);
            if (fasadType == null)
            {
                return HttpNotFound();
            }
            return View(fasadType);
        }

        // POST: FasadTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FasadType fasadType = db.FasadTypes.Find(id);
            db.FasadTypes.Remove(fasadType);
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
