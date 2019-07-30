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
    public class FasadUtepleniesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: FasadUteplenies
        public ActionResult Index()
        {
            return View(db.FasadUteplenies.ToList());
        }

        // GET: FasadUteplenies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FasadUteplenie fasadUteplenie = db.FasadUteplenies.Find(id);
            if (fasadUteplenie == null)
            {
                return HttpNotFound();
            }
            return View(fasadUteplenie);
        }

        // GET: FasadUteplenies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FasadUteplenies/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Uteplenie")] FasadUteplenie fasadUteplenie)
        {
            if (ModelState.IsValid)
            {
                db.FasadUteplenies.Add(fasadUteplenie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fasadUteplenie);
        }

        // GET: FasadUteplenies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FasadUteplenie fasadUteplenie = db.FasadUteplenies.Find(id);
            if (fasadUteplenie == null)
            {
                return HttpNotFound();
            }
            return View(fasadUteplenie);
        }

        // POST: FasadUteplenies/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Uteplenie")] FasadUteplenie fasadUteplenie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fasadUteplenie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fasadUteplenie);
        }

        // GET: FasadUteplenies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FasadUteplenie fasadUteplenie = db.FasadUteplenies.Find(id);
            if (fasadUteplenie == null)
            {
                return HttpNotFound();
            }
            return View(fasadUteplenie);
        }

        // POST: FasadUteplenies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FasadUteplenie fasadUteplenie = db.FasadUteplenies.Find(id);
            db.FasadUteplenies.Remove(fasadUteplenie);
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
