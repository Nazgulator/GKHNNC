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
    public class DOMElectroesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: DOMElectroes
        public ActionResult Index()
        {
            var dOMElectroes = db.DOMElectroes.Include(d => d.Adres);
            return View(dOMElectroes.ToList());
        }

        // GET: DOMElectroes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMElectro dOMElectro = db.DOMElectroes.Find(id);
            if (dOMElectro == null)
            {
                return HttpNotFound();
            }
            return View(dOMElectro);
        }

        // GET: DOMElectroes/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            return View();
        }

        // POST: DOMElectroes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,Electrovvods,RemontElectro,IznosElectro")] DOMElectro dOMElectro)
        {
            if (ModelState.IsValid)
            {
                db.DOMElectroes.Add(dOMElectro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMElectro.AdresId);
            return View(dOMElectro);
        }

        // GET: DOMElectroes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMElectro dOMElectro = db.DOMElectroes.Find(id);
            if (dOMElectro == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMElectro.AdresId);
            return View(dOMElectro);
        }

        // POST: DOMElectroes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,Electrovvods,RemontElectro,IznosElectro")] DOMElectro dOMElectro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dOMElectro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMElectro.AdresId);
            return View(dOMElectro);
        }

        // GET: DOMElectroes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMElectro dOMElectro = db.DOMElectroes.Find(id);
            if (dOMElectro == null)
            {
                return HttpNotFound();
            }
            return View(dOMElectro);
        }

        // POST: DOMElectroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DOMElectro dOMElectro = db.DOMElectroes.Find(id);
            db.DOMElectroes.Remove(dOMElectro);
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
