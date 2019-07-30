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
    public class FasadMaterialsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: FasadMaterials
        public ActionResult Index()
        {
            return View(db.FasadMaterials.ToList());
        }

        // GET: FasadMaterials/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FasadMaterial fasadMaterial = db.FasadMaterials.Find(id);
            if (fasadMaterial == null)
            {
                return HttpNotFound();
            }
            return View(fasadMaterial);
        }

        // GET: FasadMaterials/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FasadMaterials/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Material")] FasadMaterial fasadMaterial)
        {
            if (ModelState.IsValid)
            {
                db.FasadMaterials.Add(fasadMaterial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fasadMaterial);
        }

        // GET: FasadMaterials/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FasadMaterial fasadMaterial = db.FasadMaterials.Find(id);
            if (fasadMaterial == null)
            {
                return HttpNotFound();
            }
            return View(fasadMaterial);
        }

        // POST: FasadMaterials/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Material")] FasadMaterial fasadMaterial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fasadMaterial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fasadMaterial);
        }

        // GET: FasadMaterials/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FasadMaterial fasadMaterial = db.FasadMaterials.Find(id);
            if (fasadMaterial == null)
            {
                return HttpNotFound();
            }
            return View(fasadMaterial);
        }

        // POST: FasadMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FasadMaterial fasadMaterial = db.FasadMaterials.Find(id);
            db.FasadMaterials.Remove(fasadMaterial);
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
