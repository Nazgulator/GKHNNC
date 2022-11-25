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
    public class MaterialStandartsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: MaterialStandarts
        public ActionResult Index()
        {
            var materialStandasrts = db.MaterialStandasrts.Include(m => m.Izmerenie);
            return View(materialStandasrts.ToList());
        }

        // GET: MaterialStandarts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaterialStandart materialStandart = db.MaterialStandasrts.Find(id);
            if (materialStandart == null)
            {
                return HttpNotFound();
            }
            return View(materialStandart);
        }

        // GET: MaterialStandarts/Create
        public ActionResult Create()
        {
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name");
            return View();
        }

        // POST: MaterialStandarts/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IzmerenieId,Cost")] MaterialStandart materialStandart)
        {
            if (ModelState.IsValid)
            {
                db.MaterialStandasrts.Add(materialStandart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", materialStandart.IzmerenieId);
            return View(materialStandart);
        }

        // GET: MaterialStandarts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaterialStandart materialStandart = db.MaterialStandasrts.Find(id);
            if (materialStandart == null)
            {
                return HttpNotFound();
            }
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", materialStandart.IzmerenieId);
            return View(materialStandart);
        }

        // POST: MaterialStandarts/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,IzmerenieId,Cost")] MaterialStandart materialStandart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(materialStandart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IzmerenieId = new SelectList(db.Izmerenies, "Id", "Name", materialStandart.IzmerenieId);
            return View(materialStandart);
        }

        // GET: MaterialStandarts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaterialStandart materialStandart = db.MaterialStandasrts.Find(id);
            if (materialStandart == null)
            {
                return HttpNotFound();
            }
            return View(materialStandart);
        }

        // POST: MaterialStandarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MaterialStandart materialStandart = db.MaterialStandasrts.Find(id);
            db.MaterialStandasrts.Remove(materialStandart);
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
