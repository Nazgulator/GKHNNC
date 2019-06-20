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
    public class UslugasController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Uslugas
        public ActionResult Index()
        {
            var uslug = db.Usluga.Include(p => p.Periodichnost);
            return View(uslug.ToList());
        }

        // GET: Uslugas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usluga usluga = db.Usluga.Find(id);
            if (usluga == null)
            {
                return HttpNotFound();
            }
            return View(usluga);
        }

        // GET: Uslugas/Create
        public ActionResult Create()
        {
            List<Periodichnost> Period = db.Periodichnosts.ToList();
            
            List<SelectListItem> SL = new List<SelectListItem>();
            foreach (Periodichnost P in Period)
            {
                SelectListItem SLI = new SelectListItem();
                SLI.Text = P.PeriodichnostName;
                SLI.Value = P.Id.ToString();
                SL.Add(SLI);
            }
            SelectList SLE = new SelectList(SL, "Value", "Text");
            ViewBag.Periodichnost = SLE;
            return View();
        }

        // POST: Uslugas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Periodichnost,PeriodichnostId,Name")] Usluga usluga)
        {
            if (ModelState.IsValid)
            {
                db.Usluga.Add(usluga);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usluga);
        }

        // GET: Uslugas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usluga usluga = db.Usluga.Find(id);
            if (usluga == null)
            {
                return HttpNotFound();
            }
            List<Periodichnost> Period = db.Periodichnosts.ToList();

            List<SelectListItem> SL = new List<SelectListItem>();
            foreach (Periodichnost P in Period)
            {
                SelectListItem SLI = new SelectListItem();
                SLI.Text = P.PeriodichnostName;
                SLI.Value = P.Id.ToString();
                SL.Add(SLI);
            }
            SelectList SLE = new SelectList(SL, "Value", "Text");
            ViewBag.Periodichnost = SLE;
            return View(usluga);
        }

        // POST: Uslugas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Periodichnost,PeriodichnostId,Name")] Usluga usluga)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usluga).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usluga);
        }

        // GET: Uslugas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usluga usluga = db.Usluga.Find(id);
            if (usluga == null)
            {
                return HttpNotFound();
            }
            return View(usluga);
        }

        // POST: Uslugas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usluga usluga = db.Usluga.Find(id);
            db.Usluga.Remove(usluga);
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
