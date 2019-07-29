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
    public class RoofTypesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: RoofTypes
        public ActionResult Index()
        {
            return View(db.RoofTypes.ToList());
        }

        // GET: RoofTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoofType roofType = db.RoofTypes.Find(id);
            if (roofType == null)
            {
                return HttpNotFound();
            }
            return View(roofType);
        }

        // GET: RoofTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoofTypes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type")] RoofType roofType)
        {
            if (ModelState.IsValid)
            {
                db.RoofTypes.Add(roofType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roofType);
        }

        // GET: RoofTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoofType roofType = db.RoofTypes.Find(id);
            if (roofType == null)
            {
                return HttpNotFound();
            }
            return View(roofType);
        }

        // POST: RoofTypes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type")] RoofType roofType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roofType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roofType);
        }

        // GET: RoofTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoofType roofType = db.RoofTypes.Find(id);
            if (roofType == null)
            {
                return HttpNotFound();
            }
            return View(roofType);
        }

        // POST: RoofTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoofType roofType = db.RoofTypes.Find(id);
            db.RoofTypes.Remove(roofType);
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
