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
    public class FundamentMaterialsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: FundamentMaterials
        public ActionResult Index()
        {
            return View(db.FundamentMaterials.ToList());
        }

        // GET: FundamentMaterials/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FundamentMaterial fundamentMaterial = db.FundamentMaterials.Find(id);
            if (fundamentMaterial == null)
            {
                return HttpNotFound();
            }
            return View(fundamentMaterial);
        }

        // GET: FundamentMaterials/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FundamentMaterials/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Material")] FundamentMaterial fundamentMaterial)
        {
            if (ModelState.IsValid)
            {
                db.FundamentMaterials.Add(fundamentMaterial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fundamentMaterial);
        }

        // GET: FundamentMaterials/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FundamentMaterial fundamentMaterial = db.FundamentMaterials.Find(id);
            if (fundamentMaterial == null)
            {
                return HttpNotFound();
            }
            return View(fundamentMaterial);
        }

        // POST: FundamentMaterials/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Material")] FundamentMaterial fundamentMaterial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fundamentMaterial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fundamentMaterial);
        }

        // GET: FundamentMaterials/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FundamentMaterial fundamentMaterial = db.FundamentMaterials.Find(id);
            if (fundamentMaterial == null)
            {
                return HttpNotFound();
            }
            return View(fundamentMaterial);
        }

        // POST: FundamentMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FundamentMaterial fundamentMaterial = db.FundamentMaterials.Find(id);
            db.FundamentMaterials.Remove(fundamentMaterial);
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
