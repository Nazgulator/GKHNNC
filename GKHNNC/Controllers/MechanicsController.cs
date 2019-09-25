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
    public class MechanicsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Mechanics
        public ActionResult Index()
        {
            return View(db.Mechanics.ToList());
        }

        // GET: Mechanics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mechanic mechanic = db.Mechanics.Find(id);
            if (mechanic == null)
            {
                return HttpNotFound();
            }
            return View(mechanic);
        }

        // GET: Mechanics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Mechanics/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Mechanic mechanic)
        {
            if (ModelState.IsValid)
            {
                db.Mechanics.Add(mechanic);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mechanic);
        }

        // GET: Mechanics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mechanic mechanic = db.Mechanics.Find(id);
            if (mechanic == null)
            {
                return HttpNotFound();
            }
            return View(mechanic);
        }

        // POST: Mechanics/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Mechanic mechanic)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mechanic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mechanic);
        }

        // GET: Mechanics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mechanic mechanic = db.Mechanics.Find(id);
            if (mechanic == null)
            {
                return HttpNotFound();
            }
            return View(mechanic);
        }

        // POST: Mechanics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mechanic mechanic = db.Mechanics.Find(id);
            db.Mechanics.Remove(mechanic);
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
