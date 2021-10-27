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
    public class AdresTypesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: AdresTypes
        public ActionResult Index()
        {
            return View(db.AdresTypes.ToList());
        }

        // GET: AdresTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdresType adresType = db.AdresTypes.Find(id);
            if (adresType == null)
            {
                return HttpNotFound();
            }
            return View(adresType);
        }

        // GET: AdresTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdresTypes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] AdresType adresType)
        {
            if (ModelState.IsValid)
            {
                db.AdresTypes.Add(adresType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(adresType);
        }

        // GET: AdresTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdresType adresType = db.AdresTypes.Find(id);
            if (adresType == null)
            {
                return HttpNotFound();
            }
            return View(adresType);
        }

        // POST: AdresTypes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] AdresType adresType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adresType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(adresType);
        }

        // GET: AdresTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdresType adresType = db.AdresTypes.Find(id);
            if (adresType == null)
            {
                return HttpNotFound();
            }
            return View(adresType);
        }

        // POST: AdresTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AdresType adresType = db.AdresTypes.Find(id);
            db.AdresTypes.Remove(adresType);
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
