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
    public class DOMOtopleniesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: DOMOtoplenies
        public ActionResult Index()
        {
            var dOMOtoplenies = db.DOMOtoplenies.Include(d => d.Adress).Include(d => d.MaterialOtop1).Include(d => d.MaterialOtop2).Include(d => d.MaterialOtopTrub).Include(d => d.MaterialTeplo);
            return View(dOMOtoplenies.ToList());
        }

        // GET: DOMOtoplenies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMOtoplenie dOMOtoplenie = db.DOMOtoplenies.Find(id);
            if (dOMOtoplenie == null)
            {
                return HttpNotFound();
            }
            return View(dOMOtoplenie);
        }

        // GET: DOMOtoplenies/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            ViewBag.MaterialOtop1Id = new SelectList(db.Materials, "Id", "Name");
            ViewBag.MaterialOtop2Id = new SelectList(db.Materials, "Id", "Name");
            ViewBag.MaterialOtopTrubId = new SelectList(db.Materials, "Id", "Name");
            ViewBag.MaterialTeploId = new SelectList(db.Materials, "Id", "Name");
            return View();
        }

        // POST: DOMOtoplenies/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,IznosOtop,VvodsOtop,MaterialOtop1Id,MaterialOtop2Id,MaterialOtopTrubId,MaterialTeploId,Date")] DOMOtoplenie dOMOtoplenie)
        {
            if (ModelState.IsValid)
            {
                db.DOMOtoplenies.Add(dOMOtoplenie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMOtoplenie.AdresId);
            ViewBag.MaterialOtop1Id = new SelectList(db.Materials, "Id", "Name", dOMOtoplenie.MaterialOtop1Id);
            ViewBag.MaterialOtop2Id = new SelectList(db.Materials, "Id", "Name", dOMOtoplenie.MaterialOtop2Id);
            ViewBag.MaterialOtopTrubId = new SelectList(db.Materials, "Id", "Name", dOMOtoplenie.MaterialOtopTrubId);
            ViewBag.MaterialTeploId = new SelectList(db.Materials, "Id", "Name", dOMOtoplenie.MaterialTeploId);
            return View(dOMOtoplenie);
        }

        // GET: DOMOtoplenies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMOtoplenie dOMOtoplenie = db.DOMOtoplenies.Find(id);
            if (dOMOtoplenie == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMOtoplenie.AdresId);
            ViewBag.MaterialOtop1Id = new SelectList(db.Materials, "Id", "Name", dOMOtoplenie.MaterialOtop1Id);
            ViewBag.MaterialOtop2Id = new SelectList(db.Materials, "Id", "Name", dOMOtoplenie.MaterialOtop2Id);
            ViewBag.MaterialOtopTrubId = new SelectList(db.Materials, "Id", "Name", dOMOtoplenie.MaterialOtopTrubId);
            ViewBag.MaterialTeploId = new SelectList(db.Materials, "Id", "Name", dOMOtoplenie.MaterialTeploId);
            return View(dOMOtoplenie);
        }

        // POST: DOMOtoplenies/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,IznosOtop,VvodsOtop,MaterialOtop1Id,MaterialOtop2Id,MaterialOtopTrubId,MaterialTeploId,Date")] DOMOtoplenie dOMOtoplenie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dOMOtoplenie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMOtoplenie.AdresId);
            ViewBag.MaterialOtop1Id = new SelectList(db.Materials, "Id", "Name", dOMOtoplenie.MaterialOtop1Id);
            ViewBag.MaterialOtop2Id = new SelectList(db.Materials, "Id", "Name", dOMOtoplenie.MaterialOtop2Id);
            ViewBag.MaterialOtopTrubId = new SelectList(db.Materials, "Id", "Name", dOMOtoplenie.MaterialOtopTrubId);
            ViewBag.MaterialTeploId = new SelectList(db.Materials, "Id", "Name", dOMOtoplenie.MaterialTeploId);
            return View(dOMOtoplenie);
        }

        // GET: DOMOtoplenies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMOtoplenie dOMOtoplenie = db.DOMOtoplenies.Find(id);
            if (dOMOtoplenie == null)
            {
                return HttpNotFound();
            }
            return View(dOMOtoplenie);
        }

        // POST: DOMOtoplenies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DOMOtoplenie dOMOtoplenie = db.DOMOtoplenies.Find(id);
            db.DOMOtoplenies.Remove(dOMOtoplenie);
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
