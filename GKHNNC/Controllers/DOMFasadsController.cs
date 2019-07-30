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
    public class DOMFasadsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: DOMFasads
        public ActionResult Index()
        {
            var dOMFasads = db.DOMFasads.Include(d => d.Adres).Include(d => d.Material).Include(d => d.Type).Include(d => d.Uteplenie);
            return View(dOMFasads.ToList());
        }

        // GET: DOMFasads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMFasad dOMFasad = db.DOMFasads.Find(id);
            if (dOMFasad == null)
            {
                return HttpNotFound();
            }
            return View(dOMFasad);
        }

        // GET: DOMFasads/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            ViewBag.MaterialId = new SelectList(db.FasadMaterials, "Id", "Material");
            ViewBag.TypeId = new SelectList(db.FasadTypes, "Id", "Type");
            ViewBag.UteplenieId = new SelectList(db.FasadUteplenies, "Id", "Uteplenie");
            return View();
        }

        // POST: DOMFasads/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,Iznos,Year,MaterialId,TypeId,UteplenieId,Date")] DOMFasad dOMFasad)
        {
            if (ModelState.IsValid)
            {
                db.DOMFasads.Add(dOMFasad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMFasad.AdresId);
            ViewBag.MaterialId = new SelectList(db.FasadMaterials, "Id", "Material", dOMFasad.MaterialId);
            ViewBag.TypeId = new SelectList(db.FasadTypes, "Id", "Type", dOMFasad.TypeId);
            ViewBag.UteplenieId = new SelectList(db.FasadUteplenies, "Id", "Uteplenie", dOMFasad.UteplenieId);
            return View(dOMFasad);
        }

        // GET: DOMFasads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMFasad dOMFasad = db.DOMFasads.Find(id);
            if (dOMFasad == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMFasad.AdresId);
            ViewBag.MaterialId = new SelectList(db.FasadMaterials, "Id", "Material", dOMFasad.MaterialId);
            ViewBag.TypeId = new SelectList(db.FasadTypes, "Id", "Type", dOMFasad.TypeId);
            ViewBag.UteplenieId = new SelectList(db.FasadUteplenies, "Id", "Uteplenie", dOMFasad.UteplenieId);
            return View(dOMFasad);
        }

        // POST: DOMFasads/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,Iznos,Year,MaterialId,TypeId,UteplenieId,Date")] DOMFasad dOMFasad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dOMFasad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMFasad.AdresId);
            ViewBag.MaterialId = new SelectList(db.FasadMaterials, "Id", "Material", dOMFasad.MaterialId);
            ViewBag.TypeId = new SelectList(db.FasadTypes, "Id", "Type", dOMFasad.TypeId);
            ViewBag.UteplenieId = new SelectList(db.FasadUteplenies, "Id", "Uteplenie", dOMFasad.UteplenieId);
            return View(dOMFasad);
        }

        // GET: DOMFasads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMFasad dOMFasad = db.DOMFasads.Find(id);
            if (dOMFasad == null)
            {
                return HttpNotFound();
            }
            return View(dOMFasad);
        }

        // POST: DOMFasads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DOMFasad dOMFasad = db.DOMFasads.Find(id);
            db.DOMFasads.Remove(dOMFasad);
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
