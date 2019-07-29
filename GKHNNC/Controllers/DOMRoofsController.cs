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
    public class DOMRoofsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: DOMRoofs
        public ActionResult Index()
        {
            var dOMRoofs = db.DOMRoofs.Include(d => d.Adres).Include(d => d.Form).Include(d => d.Type).Include(d => d.Uteplenie).Include(d => d.Vid);
            return View(dOMRoofs.ToList());
        }

        // GET: DOMRoofs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMRoof dOMRoof = db.DOMRoofs.Find(id);
            if (dOMRoof == null)
            {
                return HttpNotFound();
            }
            return View(dOMRoof);
        }

        // GET: DOMRoofs/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres.OrderBy(x=>x.Adress), "Id", "Adress");
            ViewBag.FormId = new SelectList(db.RoofForms, "Id", "Form");
            ViewBag.TypeId = new SelectList(db.RoofTypes, "Id", "Type");
            ViewBag.UteplenieId = new SelectList(db.RoofUteplenies, "Id", "Uteplenie");
            ViewBag.VidId = new SelectList(db.RoofVids, "Id", "Vid");
            return View();
        }

        // POST: DOMRoofs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,VidId,TypeId,FormId,UteplenieId,YearKrovlya,Year,Ploshad,IznosKrovlya,Iznos,Date")] DOMRoof dOMRoof)
        {
            if (ModelState.IsValid)
            {
                db.DOMRoofs.Add(dOMRoof);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMRoof.AdresId);
            ViewBag.FormId = new SelectList(db.RoofForms, "Id", "Form", dOMRoof.FormId);
            ViewBag.TypeId = new SelectList(db.RoofTypes, "Id", "Type", dOMRoof.TypeId);
            ViewBag.UteplenieId = new SelectList(db.RoofUteplenies, "Id", "Uteplenie", dOMRoof.UteplenieId);
            ViewBag.VidId = new SelectList(db.RoofVids, "Id", "Vid", dOMRoof.VidId);
            return View(dOMRoof);
        }

        // GET: DOMRoofs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMRoof dOMRoof = db.DOMRoofs.Find(id);
            if (dOMRoof == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMRoof.AdresId);
            ViewBag.FormId = new SelectList(db.RoofForms, "Id", "Form", dOMRoof.FormId);
            ViewBag.TypeId = new SelectList(db.RoofTypes, "Id", "Type", dOMRoof.TypeId);
            ViewBag.UteplenieId = new SelectList(db.RoofUteplenies, "Id", "Uteplenie", dOMRoof.UteplenieId);
            ViewBag.VidId = new SelectList(db.RoofVids, "Id", "Vid", dOMRoof.VidId);
            return View(dOMRoof);
        }

        // POST: DOMRoofs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,VidId,TypeId,FormId,UteplenieId,YearKrovlya,Year,Ploshad,IznosKrovlya,Iznos,Date")] DOMRoof dOMRoof)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dOMRoof).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMRoof.AdresId);
            ViewBag.FormId = new SelectList(db.RoofForms, "Id", "Form", dOMRoof.FormId);
            ViewBag.TypeId = new SelectList(db.RoofTypes, "Id", "Type", dOMRoof.TypeId);
            ViewBag.UteplenieId = new SelectList(db.RoofUteplenies, "Id", "Uteplenie", dOMRoof.UteplenieId);
            ViewBag.VidId = new SelectList(db.RoofVids, "Id", "Vid", dOMRoof.VidId);
            return View(dOMRoof);
        }

        // GET: DOMRoofs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMRoof dOMRoof = db.DOMRoofs.Find(id);
            if (dOMRoof == null)
            {
                return HttpNotFound();
            }
            return View(dOMRoof);
        }

        // POST: DOMRoofs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DOMRoof dOMRoof = db.DOMRoofs.Find(id);
            db.DOMRoofs.Remove(dOMRoof);
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
