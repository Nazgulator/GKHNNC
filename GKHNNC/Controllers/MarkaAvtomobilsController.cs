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
    public class MarkaAvtomobilsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: MarkaAvtomobils
        public ActionResult Index()
        {
            return View(db.MarkaAvtomobils.OrderBy(a=>a.Id).ToList());
        }

        // GET: MarkaAvtomobils/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MarkaAvtomobil markaAvtomobil = db.MarkaAvtomobils.Find(id);
            if (markaAvtomobil == null)
            {
                return HttpNotFound();
            }
            return View(markaAvtomobil);
        }

        // GET: MarkaAvtomobils/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MarkaAvtomobils/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,SNorm,WNorm,Toplivo,KmMoto,EzdkaPricep,NormaEzdka")] MarkaAvtomobil markaAvtomobil)
        {
            if (ModelState.IsValid)
            {
                db.MarkaAvtomobils.Add(markaAvtomobil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(markaAvtomobil);
        }

        // GET: MarkaAvtomobils/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MarkaAvtomobil markaAvtomobil = db.MarkaAvtomobils.Find(id);
            if (markaAvtomobil == null)
            {
                return HttpNotFound();
            }
            return View(markaAvtomobil);
        }

        // POST: MarkaAvtomobils/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,SNorm,WNorm,Toplivo,KmMoto,EzdkaPricep,NormaEzdka")] MarkaAvtomobil markaAvtomobil)
        {
            if (ModelState.IsValid)
            {
                db.Entry(markaAvtomobil).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(markaAvtomobil);
        }

        // GET: MarkaAvtomobils/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MarkaAvtomobil markaAvtomobil = db.MarkaAvtomobils.Find(id);
            if (markaAvtomobil == null)
            {
                return HttpNotFound();
            }
            return View(markaAvtomobil);
        }

        // POST: MarkaAvtomobils/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MarkaAvtomobil markaAvtomobil = db.MarkaAvtomobils.Find(id);
            db.MarkaAvtomobils.Remove(markaAvtomobil);
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
