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
    public class DOMFundamentsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: DOMFundaments
        public ActionResult Index()
        {
            var dOMFundaments = db.DOMFundaments.Include(d => d.Adres).Include(d => d.Material).Include(d => d.Type);
            return View(dOMFundaments.ToList());
        }

        // GET: DOMFundaments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMFundament dOMFundament = db.DOMFundaments.Find(id);
            if (dOMFundament == null)
            {
                return HttpNotFound();
            }
            return View(dOMFundament);
        }

        // GET: DOMFundaments/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres.OrderBy(x=>x.Adress), "Id", "Adress");
            ViewBag.MaterialId = new SelectList(db.FundamentMaterials, "Id", "Material");
            ViewBag.TypeId = new SelectList(db.FundamentTypes, "Id", "Type");
            return View();
        }

        // POST: DOMFundaments/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Ploshad,MaterialId,TypeId,AdresId,Date")] DOMFundament dOMFundament)
        {
            if (ModelState.IsValid)
            {
                db.DOMFundaments.Add(dOMFundament);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMFundament.AdresId);
            ViewBag.MaterialId = new SelectList(db.FundamentMaterials, "Id", "Material", dOMFundament.MaterialId);
            ViewBag.TypeId = new SelectList(db.FundamentTypes, "Id", "Type", dOMFundament.TypeId);
            return View(dOMFundament);
        }

        // GET: DOMFundaments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMFundament dOMFundament = db.DOMFundaments.Find(id);
            if (dOMFundament == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMFundament.AdresId);
            ViewBag.MaterialId = new SelectList(db.FundamentMaterials, "Id", "Material", dOMFundament.MaterialId);
            ViewBag.TypeId = new SelectList(db.FundamentTypes, "Id", "Type", dOMFundament.TypeId);
            return View(dOMFundament);
        }

        // POST: DOMFundaments/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Ploshad,MaterialId,TypeId,AdresId,Date")] DOMFundament dOMFundament)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dOMFundament).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMFundament.AdresId);
            ViewBag.MaterialId = new SelectList(db.FundamentMaterials, "Id", "Material", dOMFundament.MaterialId);
            ViewBag.TypeId = new SelectList(db.FundamentTypes, "Id", "Type", dOMFundament.TypeId);
            return View(dOMFundament);
        }

        // GET: DOMFundaments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMFundament dOMFundament = db.DOMFundaments.Find(id);
            if (dOMFundament == null)
            {
                return HttpNotFound();
            }
            return View(dOMFundament);
        }

        // POST: DOMFundaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DOMFundament dOMFundament = db.DOMFundaments.Find(id);
            db.DOMFundaments.Remove(dOMFundament);
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
