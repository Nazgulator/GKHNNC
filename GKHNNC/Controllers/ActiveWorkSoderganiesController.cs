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
    public class ActiveWorkSoderganiesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: ActiveWorkSoderganies
        public ActionResult Index()
        {
            var activeWorkSoderganies = db.ActiveWorkSoderganies.Include(a => a.Adres).Include(a => a.WorkSoderganie);
            return View(activeWorkSoderganies.ToList());
        }

        // GET: ActiveWorkSoderganies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActiveWorkSoderganie activeWorkSoderganie = db.ActiveWorkSoderganies.Find(id);
            if (activeWorkSoderganie == null)
            {
                return HttpNotFound();
            }
            return View(activeWorkSoderganie);
        }

        // GET: ActiveWorkSoderganies/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            ViewBag.WorkSoderganieId = new SelectList(db.WorkSoderganies, "Id", "Name");
            return View();
        }

        // POST: ActiveWorkSoderganies/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,WorkSoderganieId,Val,Date")] ActiveWorkSoderganie activeWorkSoderganie)
        {
            if (ModelState.IsValid)
            {
                db.ActiveWorkSoderganies.Add(activeWorkSoderganie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", activeWorkSoderganie.AdresId);
            ViewBag.WorkSoderganieId = new SelectList(db.WorkSoderganies, "Id", "Name", activeWorkSoderganie.WorkSoderganieId);
            return View(activeWorkSoderganie);
        }

        // GET: ActiveWorkSoderganies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActiveWorkSoderganie activeWorkSoderganie = db.ActiveWorkSoderganies.Find(id);
            if (activeWorkSoderganie == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", activeWorkSoderganie.AdresId);
            ViewBag.WorkSoderganieId = new SelectList(db.WorkSoderganies, "Id", "Name", activeWorkSoderganie.WorkSoderganieId);
            return View(activeWorkSoderganie);
        }

        // POST: ActiveWorkSoderganies/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,WorkSoderganieId,Val,Date")] ActiveWorkSoderganie activeWorkSoderganie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activeWorkSoderganie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", activeWorkSoderganie.AdresId);
            ViewBag.WorkSoderganieId = new SelectList(db.WorkSoderganies, "Id", "Name", activeWorkSoderganie.WorkSoderganieId);
            return View(activeWorkSoderganie);
        }

        // GET: ActiveWorkSoderganies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActiveWorkSoderganie activeWorkSoderganie = db.ActiveWorkSoderganies.Find(id);
            if (activeWorkSoderganie == null)
            {
                return HttpNotFound();
            }
            return View(activeWorkSoderganie);
        }

        // POST: ActiveWorkSoderganies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ActiveWorkSoderganie activeWorkSoderganie = db.ActiveWorkSoderganies.Find(id);
            db.ActiveWorkSoderganies.Remove(activeWorkSoderganie);
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
