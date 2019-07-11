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
    public class NegilayasController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Negilayas
        public ActionResult Index()
        {
            return View(db.Negilayas.ToList());
        }

        // GET: Negilayas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negilaya negilaya = db.Negilayas.Find(id);
            if (negilaya == null)
            {
                return HttpNotFound();
            }
            return View(negilaya);
        }

        // GET: Negilayas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Negilayas/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CodeOBSD")] Negilaya negilaya)
        {
            if (ModelState.IsValid)
            {
                db.Negilayas.Add(negilaya);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(negilaya);
        }

        // GET: Negilayas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negilaya negilaya = db.Negilayas.Find(id);
            if (negilaya == null)
            {
                return HttpNotFound();
            }
            return View(negilaya);
        }

        // POST: Negilayas/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CodeOBSD")] Negilaya negilaya)
        {
            if (ModelState.IsValid)
            {
                db.Entry(negilaya).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(negilaya);
        }

        // GET: Negilayas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negilaya negilaya = db.Negilayas.Find(id);
            if (negilaya == null)
            {
                return HttpNotFound();
            }
            return View(negilaya);
        }

        // POST: Negilayas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Negilaya negilaya = db.Negilayas.Find(id);
            db.Negilayas.Remove(negilaya);
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
