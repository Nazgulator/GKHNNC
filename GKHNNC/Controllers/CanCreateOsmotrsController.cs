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
    public class CanCreateOsmotrsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: CanCreateOsmotrs
        public ActionResult Index()
        {
            return View(db.CanCreateOsmotrs.ToList());
        }

        // GET: CanCreateOsmotrs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CanCreateOsmotr canCreateOsmotr = db.CanCreateOsmotrs.Find(id);
            if (canCreateOsmotr == null)
            {
                return HttpNotFound();
            }
            return View(canCreateOsmotr);
        }

        // GET: CanCreateOsmotrs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CanCreateOsmotrs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Sozdanie,DateTime")] CanCreateOsmotr canCreateOsmotr)
        {
            if (ModelState.IsValid)
            {
                db.CanCreateOsmotrs.Add(canCreateOsmotr);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(canCreateOsmotr);
        }

        // GET: CanCreateOsmotrs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CanCreateOsmotr canCreateOsmotr = db.CanCreateOsmotrs.Find(id);
            if (canCreateOsmotr == null)
            {
                return HttpNotFound();
            }
            return View(canCreateOsmotr);
        }

        // POST: CanCreateOsmotrs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Sozdanie,DateTime")] CanCreateOsmotr canCreateOsmotr)
        {
            if (ModelState.IsValid)
            {
                db.Entry(canCreateOsmotr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(canCreateOsmotr);
        }

        // GET: CanCreateOsmotrs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CanCreateOsmotr canCreateOsmotr = db.CanCreateOsmotrs.Find(id);
            if (canCreateOsmotr == null)
            {
                return HttpNotFound();
            }
            return View(canCreateOsmotr);
        }

        // POST: CanCreateOsmotrs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CanCreateOsmotr canCreateOsmotr = db.CanCreateOsmotrs.Find(id);
            db.CanCreateOsmotrs.Remove(canCreateOsmotr);
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
