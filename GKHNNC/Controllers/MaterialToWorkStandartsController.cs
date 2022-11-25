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
    public class MaterialToWorkStandartsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: MaterialToWorkStandarts
        public ActionResult Index()
        {
            var materialToWorkStandarts = db.MaterialToWorkStandarts.Include(m => m.MaterialStandart).Include(m => m.WorkStandart);
            return View(materialToWorkStandarts.ToList());
        }

        // GET: MaterialToWorkStandarts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaterialToWorkStandart materialToWorkStandart = db.MaterialToWorkStandarts.Find(id);
            if (materialToWorkStandart == null)
            {
                return HttpNotFound();
            }
            return View(materialToWorkStandart);
        }

        // GET: MaterialToWorkStandarts/Create
        public ActionResult Create()
        {
            ViewBag.MaterialStandartId = new SelectList(db.MaterialStandasrts, "Id", "Name");
            ViewBag.WorkStandartId = new SelectList(db.WorkStandarts, "Id", "Name");
            return View();
        }

        // POST: MaterialToWorkStandarts/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MaterialStandartId,WorkStandartId,QTY")] MaterialToWorkStandart materialToWorkStandart)
        {
            if (ModelState.IsValid)
            {
                db.MaterialToWorkStandarts.Add(materialToWorkStandart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaterialStandartId = new SelectList(db.MaterialStandasrts, "Id", "Name", materialToWorkStandart.MaterialStandartId);
            ViewBag.WorkStandartId = new SelectList(db.WorkStandarts, "Id", "Name", materialToWorkStandart.WorkStandartId);
            return View(materialToWorkStandart);
        }

        // GET: MaterialToWorkStandarts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaterialToWorkStandart materialToWorkStandart = db.MaterialToWorkStandarts.Find(id);
            if (materialToWorkStandart == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaterialStandartId = new SelectList(db.MaterialStandasrts, "Id", "Name", materialToWorkStandart.MaterialStandartId);
            ViewBag.WorkStandartId = new SelectList(db.WorkStandarts, "Id", "Name", materialToWorkStandart.WorkStandartId);
            return View(materialToWorkStandart);
        }

        // POST: MaterialToWorkStandarts/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MaterialStandartId,WorkStandartId,QTY")] MaterialToWorkStandart materialToWorkStandart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(materialToWorkStandart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaterialStandartId = new SelectList(db.MaterialStandasrts, "Id", "Name", materialToWorkStandart.MaterialStandartId);
            ViewBag.WorkStandartId = new SelectList(db.WorkStandarts, "Id", "Name", materialToWorkStandart.WorkStandartId);
            return View(materialToWorkStandart);
        }

        // GET: MaterialToWorkStandarts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaterialToWorkStandart materialToWorkStandart = db.MaterialToWorkStandarts.Find(id);
            if (materialToWorkStandart == null)
            {
                return HttpNotFound();
            }
            return View(materialToWorkStandart);
        }

        // POST: MaterialToWorkStandarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MaterialToWorkStandart materialToWorkStandart = db.MaterialToWorkStandarts.Find(id);
            db.MaterialToWorkStandarts.Remove(materialToWorkStandart);
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
