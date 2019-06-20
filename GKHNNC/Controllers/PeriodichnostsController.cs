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
    public class PeriodichnostsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Periodichnosts
        public ActionResult Index()
        {
            return View(db.Periodichnosts.ToList());
        }

        // GET: Periodichnosts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Periodichnost periodichnost = db.Periodichnosts.Find(id);
            if (periodichnost == null)
            {
                return HttpNotFound();
            }
            return View(periodichnost);
        }

        // GET: Periodichnosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Periodichnosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PeriodichnostName")] Periodichnost periodichnost)
        {
            if (ModelState.IsValid)
            {
                db.Periodichnosts.Add(periodichnost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(periodichnost);
        }

        // GET: Periodichnosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Periodichnost periodichnost = db.Periodichnosts.Find(id);
            if (periodichnost == null)
            {
                return HttpNotFound();
            }
            return View(periodichnost);
        }

        // POST: Periodichnosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PeriodichnostName")] Periodichnost periodichnost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(periodichnost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(periodichnost);
        }

        // GET: Periodichnosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Periodichnost periodichnost = db.Periodichnosts.Find(id);
            if (periodichnost == null)
            {
                return HttpNotFound();
            }
            return View(periodichnost);
        }

        // POST: Periodichnosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Periodichnost periodichnost = db.Periodichnosts.Find(id);
            db.Periodichnosts.Remove(periodichnost);
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
