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
    public class PodpisantsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Podpisants
        public ActionResult Index()
        {
            return View(db.Podpisnts.ToList());
        }

        // GET: Podpisants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Podpisant podpisant = db.Podpisnts.Find(id);
            if (podpisant == null)
            {
                return HttpNotFound();
            }
            return View(podpisant);
        }

        // GET: Podpisants/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Podpisants/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StringId,Dolgnost,Name")] Podpisant podpisant)
        {
            if (ModelState.IsValid)
            {
                db.Podpisnts.Add(podpisant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(podpisant);
        }

        // GET: Podpisants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Podpisant podpisant = db.Podpisnts.Find(id);
            if (podpisant == null)
            {
                return HttpNotFound();
            }
            return View(podpisant);
        }

        // POST: Podpisants/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StringId,Dolgnost,Name")] Podpisant podpisant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(podpisant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(podpisant);
        }

        // GET: Podpisants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Podpisant podpisant = db.Podpisnts.Find(id);
            if (podpisant == null)
            {
                return HttpNotFound();
            }
            return View(podpisant);
        }

        // POST: Podpisants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Podpisant podpisant = db.Podpisnts.Find(id);
            db.Podpisnts.Remove(podpisant);
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
