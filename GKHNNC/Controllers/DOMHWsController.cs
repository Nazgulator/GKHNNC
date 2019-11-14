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
    public class DOMHWsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: DOMHWs
        public ActionResult Index()
        {
            var dOMHWs = db.DOMHWs.Include(d => d.Adres).Include(d => d.MaterialHW);
            return View(dOMHWs.ToList());
        }

        // GET: DOMHWs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMHW dOMHW = db.DOMHWs.Find(id);
            if (dOMHW == null)
            {
                return HttpNotFound();
            }
            return View(dOMHW);
        }

        // GET: DOMHWs/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            ViewBag.MaterialHWId = new SelectList(db.Materials, "Id", "Name");
            return View();
        }

        // POST: DOMHWs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,IznosHW,MaterialHWId,RemontHW")] DOMHW dOMHW)
        {
            if (ModelState.IsValid)
            {
                db.DOMHWs.Add(dOMHW);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMHW.AdresId);
            ViewBag.MaterialHWId = new SelectList(db.Materials, "Id", "Name", dOMHW.MaterialHWId);
            return View(dOMHW);
        }

        // GET: DOMHWs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMHW dOMHW = db.DOMHWs.Find(id);
            if (dOMHW == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMHW.AdresId);
            ViewBag.MaterialHWId = new SelectList(db.Materials, "Id", "Name", dOMHW.MaterialHWId);
            return View(dOMHW);
        }

        // POST: DOMHWs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,IznosHW,MaterialHWId,RemontHW")] DOMHW dOMHW)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dOMHW).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMHW.AdresId);
            ViewBag.MaterialHWId = new SelectList(db.Materials, "Id", "Name", dOMHW.MaterialHWId);
            return View(dOMHW);
        }

        // GET: DOMHWs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMHW dOMHW = db.DOMHWs.Find(id);
            if (dOMHW == null)
            {
                return HttpNotFound();
            }
            return View(dOMHW);
        }

        // POST: DOMHWs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DOMHW dOMHW = db.DOMHWs.Find(id);
            db.DOMHWs.Remove(dOMHW);
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
