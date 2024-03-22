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
    public class AdresaMKDsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: AdresaMKDs
        public ActionResult Index()
        {
            return View(db.AdresMKDs.ToList());
        }

        // GET: AdresaMKDs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdresaMKDs adresaMKDs = db.AdresMKDs.Find(id);
            if (adresaMKDs == null)
            {
                return HttpNotFound();
            }
            return View(adresaMKDs);
        }

        // GET: AdresaMKDs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdresaMKDs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ORC,ASU,FileName")] AdresaMKDs adresaMKDs)
        {
            if (ModelState.IsValid)
            {
                db.AdresMKDs.Add(adresaMKDs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(adresaMKDs);
        }

        // GET: AdresaMKDs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdresaMKDs adresaMKDs = db.AdresMKDs.Find(id);
            if (adresaMKDs == null)
            {
                return HttpNotFound();
            }
            return View(adresaMKDs);
        }

        // POST: AdresaMKDs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ORC,ASU,FileName")] AdresaMKDs adresaMKDs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adresaMKDs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(adresaMKDs);
        }

        // GET: AdresaMKDs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdresaMKDs adresaMKDs = db.AdresMKDs.Find(id);
            if (adresaMKDs == null)
            {
                return HttpNotFound();
            }
            return View(adresaMKDs);
        }

        // POST: AdresaMKDs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AdresaMKDs adresaMKDs = db.AdresMKDs.Find(id);
            db.AdresMKDs.Remove(adresaMKDs);
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
