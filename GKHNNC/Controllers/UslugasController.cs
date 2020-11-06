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
    public class UslugasController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Uslugas
        public ActionResult Index()
        {
            var usluga = db.Usluga.Include(u => u.Periodichnost);
            return View(usluga.ToList());
        }

        // GET: Uslugas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usluga usluga = db.Usluga.Find(id);
            if (usluga == null)
            {
                return HttpNotFound();
            }
            return View(usluga);
        }

        // GET: Uslugas/Create
        public ActionResult Create()
        {
            ViewBag.PeriodichnostId = new SelectList(db.Periodichnosts, "Id", "PeriodichnostName");
            return View();
        }

        // POST: Uslugas/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PeriodichnostId,Poryadok,Name")] Usluga usluga)
        {
            if (ModelState.IsValid)
            {
                db.Usluga.Add(usluga);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PeriodichnostId = new SelectList(db.Periodichnosts, "Id", "PeriodichnostName", usluga.PeriodichnostId);
            return View(usluga);
        }

        // GET: Uslugas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usluga usluga = db.Usluga.Find(id);
            if (usluga == null)
            {
                return HttpNotFound();
            }
            ViewBag.PeriodichnostId = new SelectList(db.Periodichnosts, "Id", "PeriodichnostName", usluga.PeriodichnostId);
            return View(usluga);
        }

        // POST: Uslugas/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PeriodichnostId,Poryadok,Name")] Usluga usluga)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usluga).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PeriodichnostId = new SelectList(db.Periodichnosts, "Id", "PeriodichnostName", usluga.PeriodichnostId);
            return View(usluga);
        }

        // GET: Uslugas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usluga usluga = db.Usluga.Find(id);
            if (usluga == null)
            {
                return HttpNotFound();
            }
            return View(usluga);
        }

        // POST: Uslugas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usluga usluga = db.Usluga.Find(id);
            db.Usluga.Remove(usluga);
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
