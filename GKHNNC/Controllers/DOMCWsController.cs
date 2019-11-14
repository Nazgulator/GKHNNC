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
    public class DOMCWsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: DOMCWs
        public ActionResult Index()
        {
            var dOMCWs = db.DOMCWs.Include(d => d.Adres).Include(d => d.MaterialCW);
            return View(dOMCWs.ToList());
        }

        // GET: DOMCWs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMCW dOMCW = db.DOMCWs.Find(id);
            if (dOMCW == null)
            {
                return HttpNotFound();
            }
            return View(dOMCW);
        }

        // GET: DOMCWs/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            ViewBag.MaterialCWId = new SelectList(db.Materials, "Id", "Name");
            return View();
        }

        // POST: DOMCWs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,IznosCW,MaterialCWId,RemontCW")] DOMCW dOMCW)
        {
            if (ModelState.IsValid)
            {
                db.DOMCWs.Add(dOMCW);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMCW.AdresId);
            ViewBag.MaterialCWId = new SelectList(db.Materials, "Id", "Name", dOMCW.MaterialCWId);
            return View(dOMCW);
        }

        // GET: DOMCWs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMCW dOMCW = db.DOMCWs.Find(id);
            if (dOMCW == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMCW.AdresId);
            ViewBag.MaterialCWId = new SelectList(db.Materials, "Id", "Name", dOMCW.MaterialCWId);
            return View(dOMCW);
        }

        // POST: DOMCWs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,IznosCW,MaterialCWId,RemontCW")] DOMCW dOMCW)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dOMCW).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMCW.AdresId);
            ViewBag.MaterialCWId = new SelectList(db.Materials, "Id", "Name", dOMCW.MaterialCWId);
            return View(dOMCW);
        }

        // GET: DOMCWs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMCW dOMCW = db.DOMCWs.Find(id);
            if (dOMCW == null)
            {
                return HttpNotFound();
            }
            return View(dOMCW);
        }

        // POST: DOMCWs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DOMCW dOMCW = db.DOMCWs.Find(id);
            db.DOMCWs.Remove(dOMCW);
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
