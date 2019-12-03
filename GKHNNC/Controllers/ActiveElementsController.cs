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
    public class ActiveElementsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: ActiveElements
        public ActionResult Index()
        {
            var activeElements = db.ActiveElements.Include(a => a.Adres).Include(a => a.Element).Include(a => a.Osmotr);
            return View(activeElements.ToList());
        }

        // GET: ActiveElements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActiveElement activeElement = db.ActiveElements.Find(id);
            if (activeElement == null)
            {
                return HttpNotFound();
            }
            return View(activeElement);
        }

        // GET: ActiveElements/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            ViewBag.ElementId = new SelectList(db.Elements, "Id", "Name");
            ViewBag.OsmotrId = new SelectList(db.Osmotrs, "Id", "Id");
            return View();
        }

        // POST: ActiveElements/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ElementId,AdresId,OsmotrId,Sostoyanie,Date")] ActiveElement activeElement)
        {
            if (ModelState.IsValid)
            {
                db.ActiveElements.Add(activeElement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", activeElement.AdresId);
            ViewBag.ElementId = new SelectList(db.Elements, "Id", "Name", activeElement.ElementId);
            ViewBag.OsmotrId = new SelectList(db.Osmotrs, "Id", "Id", activeElement.OsmotrId);
            return View(activeElement);
        }

        // GET: ActiveElements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActiveElement activeElement = db.ActiveElements.Find(id);
            if (activeElement == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", activeElement.AdresId);
            ViewBag.ElementId = new SelectList(db.Elements, "Id", "Name", activeElement.ElementId);
            ViewBag.OsmotrId = new SelectList(db.Osmotrs, "Id", "Id", activeElement.OsmotrId);
            return View(activeElement);
        }

        // POST: ActiveElements/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ElementId,AdresId,OsmotrId,Sostoyanie,Date")] ActiveElement activeElement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activeElement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", activeElement.AdresId);
            ViewBag.ElementId = new SelectList(db.Elements, "Id", "Name", activeElement.ElementId);
            ViewBag.OsmotrId = new SelectList(db.Osmotrs, "Id", "Id", activeElement.OsmotrId);
            return View(activeElement);
        }

        // GET: ActiveElements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActiveElement activeElement = db.ActiveElements.Find(id);
            if (activeElement == null)
            {
                return HttpNotFound();
            }
            return View(activeElement);
        }

        // POST: ActiveElements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ActiveElement activeElement = db.ActiveElements.Find(id);
            db.ActiveElements.Remove(activeElement);
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
