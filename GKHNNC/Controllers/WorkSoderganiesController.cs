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
    public class WorkSoderganiesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: WorkSoderganies
        public ActionResult Index()
        {
            var workSoderganies = db.WorkSoderganies.Include(w => w.Izmerenie).Include(w => w.Tip);
            return View(workSoderganies.ToList());
        }

        // GET: WorkSoderganies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkSoderganie workSoderganie = db.WorkSoderganies.Find(id);
            if (workSoderganie == null)
            {
                return HttpNotFound();
            }
            return View(workSoderganie);
        }

        // GET: WorkSoderganies/Create
        public ActionResult Create()
        {
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name");
            ViewBag.TipId = new SelectList(db.Tips, "Id", "Name");
            return View();
        }

        // POST: WorkSoderganies/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IzmerenieId,Norma,Obiem,Opisanie,Code,Val,TipId,Periodichnost,Remont,CostMterials,CostWrok,ProcGood,ProcBad")] WorkSoderganie workSoderganie)
        {
            if (ModelState.IsValid)
            {
                db.WorkSoderganies.Add(workSoderganie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", workSoderganie.IzmerenieId);
            ViewBag.TipId = new SelectList(db.Tips, "Id", "Name", workSoderganie.TipId);
            return View(workSoderganie);
        }

        // GET: WorkSoderganies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkSoderganie workSoderganie = db.WorkSoderganies.Find(id);
            if (workSoderganie == null)
            {
                return HttpNotFound();
            }
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", workSoderganie.IzmerenieId);
            ViewBag.TipId = new SelectList(db.Tips, "Id", "Name", workSoderganie.TipId);
            return View(workSoderganie);
        }

        // POST: WorkSoderganies/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,IzmerenieId,Norma,Obiem,Opisanie,Code,Val,TipId,Periodichnost,Remont,CostMterials,CostWrok,ProcGood,ProcBad")] WorkSoderganie workSoderganie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(workSoderganie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", workSoderganie.IzmerenieId);
            ViewBag.TipId = new SelectList(db.Tips, "Id", "Name", workSoderganie.TipId);
            return View(workSoderganie);
        }

        // GET: WorkSoderganies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkSoderganie workSoderganie = db.WorkSoderganies.Find(id);
            if (workSoderganie == null)
            {
                return HttpNotFound();
            }
            return View(workSoderganie);
        }

        // POST: WorkSoderganies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WorkSoderganie workSoderganie = db.WorkSoderganies.Find(id);
            db.WorkSoderganies.Remove(workSoderganie);
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
