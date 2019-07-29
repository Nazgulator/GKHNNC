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
    public class RoofUtepleniesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: RoofUteplenies
        public ActionResult Index()
        {
            return View(db.RoofUteplenies.ToList());
        }

        // GET: RoofUteplenies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoofUteplenie roofUteplenie = db.RoofUteplenies.Find(id);
            if (roofUteplenie == null)
            {
                return HttpNotFound();
            }
            return View(roofUteplenie);
        }

        // GET: RoofUteplenies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoofUteplenies/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Uteplenie")] RoofUteplenie roofUteplenie)
        {
            if (ModelState.IsValid)
            {
                db.RoofUteplenies.Add(roofUteplenie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roofUteplenie);
        }

        // GET: RoofUteplenies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoofUteplenie roofUteplenie = db.RoofUteplenies.Find(id);
            if (roofUteplenie == null)
            {
                return HttpNotFound();
            }
            return View(roofUteplenie);
        }

        // POST: RoofUteplenies/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Uteplenie")] RoofUteplenie roofUteplenie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roofUteplenie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roofUteplenie);
        }

        // GET: RoofUteplenies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoofUteplenie roofUteplenie = db.RoofUteplenies.Find(id);
            if (roofUteplenie == null)
            {
                return HttpNotFound();
            }
            return View(roofUteplenie);
        }

        // POST: RoofUteplenies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoofUteplenie roofUteplenie = db.RoofUteplenies.Find(id);
            db.RoofUteplenies.Remove(roofUteplenie);
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
