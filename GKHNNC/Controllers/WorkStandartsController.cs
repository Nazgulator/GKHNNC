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
    public class WorkStandartsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: WorkStandarts
        public ActionResult Index()
        {
            var workStandarts = db.WorkStandarts.Include(w => w.Izmerenie).Include(w => w.Tip);
            return View(workStandarts.ToList());
        }

        // GET: WorkStandarts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkStandart workStandart = db.WorkStandarts.Find(id);
            if (workStandart == null)
            {
                return HttpNotFound();
            }
            return View(workStandart);
        }

        // GET: WorkStandarts/Create
        public ActionResult Create()
        {
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name");
            ViewBag.TipId = new SelectList(db.Tips, "Id", "Name");
            return View();
        }

        // POST: WorkStandarts/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,TipId,IzmerenieId,Cost")] WorkStandart workStandart)
        {
            if (ModelState.IsValid)
            {
                db.WorkStandarts.Add(workStandart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", workStandart.IzmerenieId);
            ViewBag.TipId = new SelectList(db.Tips, "Id", "Name", workStandart.TipId);
            return View(workStandart);
        }

        // GET: WorkStandarts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkStandart workStandart = db.WorkStandarts.Find(id);
            if (workStandart == null)
            {
                return HttpNotFound();
            }
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", workStandart.IzmerenieId);
            ViewBag.TipId = new SelectList(db.Tips, "Id", "Name", workStandart.TipId);
            return View(workStandart);
        }

        // POST: WorkStandarts/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,TipId,IzmerenieId,Cost")] WorkStandart workStandart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(workStandart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", workStandart.IzmerenieId);
            ViewBag.TipId = new SelectList(db.Tips, "Id", "Name", workStandart.TipId);
            return View(workStandart);
        }

        // GET: WorkStandarts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkStandart workStandart = db.WorkStandarts.Find(id);
            if (workStandart == null)
            {
                return HttpNotFound();
            }
            return View(workStandart);
        }

        // POST: WorkStandarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WorkStandart workStandart = db.WorkStandarts.Find(id);
            db.WorkStandarts.Remove(workStandart);
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
