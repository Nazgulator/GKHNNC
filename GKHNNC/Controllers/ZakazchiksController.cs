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
    public class ZakazchiksController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Zakazchiks
        public ActionResult Index()
        {
            return View(db.Zakazchiks.ToList());
        }

        // GET: Zakazchiks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zakazchik zakazchik = db.Zakazchiks.Find(id);
            if (zakazchik == null)
            {
                return HttpNotFound();
            }
            return View(zakazchik);
        }

        // GET: Zakazchiks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Zakazchiks/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Zakazchik zakazchik)
        {
            if (ModelState.IsValid)
            {
                db.Zakazchiks.Add(zakazchik);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(zakazchik);
        }

        // GET: Zakazchiks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zakazchik zakazchik = db.Zakazchiks.Find(id);
            if (zakazchik == null)
            {
                return HttpNotFound();
            }
            return View(zakazchik);
        }

        // POST: Zakazchiks/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Zakazchik zakazchik)
        {
            if (ModelState.IsValid)
            {
                db.Entry(zakazchik).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(zakazchik);
        }

        // GET: Zakazchiks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zakazchik zakazchik = db.Zakazchiks.Find(id);
            if (zakazchik == null)
            {
                return HttpNotFound();
            }
            return View(zakazchik);
        }

        // POST: Zakazchiks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Zakazchik zakazchik = db.Zakazchiks.Find(id);
            db.Zakazchiks.Remove(zakazchik);
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
