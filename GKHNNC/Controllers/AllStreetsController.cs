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
    public class AllStreetsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: AllStreets
        public ActionResult Index()
        {
            return View(db.AllStreets.ToList());
        }

        // GET: AllStreets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllStreet allStreet = db.AllStreets.Find(id);
            if (allStreet == null)
            {
                return HttpNotFound();
            }
            return View(allStreet);
        }

        // GET: AllStreets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AllStreets/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] AllStreet allStreet)
        {
            if (ModelState.IsValid)
            {
                db.AllStreets.Add(allStreet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(allStreet);
        }

        // GET: AllStreets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllStreet allStreet = db.AllStreets.Find(id);
            if (allStreet == null)
            {
                return HttpNotFound();
            }
            return View(allStreet);
        }

        // POST: AllStreets/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] AllStreet allStreet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(allStreet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(allStreet);
        }

        // GET: AllStreets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllStreet allStreet = db.AllStreets.Find(id);
            if (allStreet == null)
            {
                return HttpNotFound();
            }
            return View(allStreet);
        }

        // POST: AllStreets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AllStreet allStreet = db.AllStreets.Find(id);
            db.AllStreets.Remove(allStreet);
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
