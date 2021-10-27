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
    public class ConstructiveTypesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: ConstructiveTypes
        public ActionResult Index()
        {
            var constructiveTypes = db.ConstructiveTypes.Include(c => c.DOMPart);
            return View(constructiveTypes.ToList());
        }

        // GET: ConstructiveTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConstructiveType constructiveType = db.ConstructiveTypes.Find(id);
            if (constructiveType == null)
            {
                return HttpNotFound();
            }
            return View(constructiveType);
        }

        // GET: ConstructiveTypes/Create
        public ActionResult Create()
        {
            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name");
            return View();
        }

        // POST: ConstructiveTypes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,DOMPartId")] ConstructiveType constructiveType)
        {
            if (ModelState.IsValid)
            {
                db.ConstructiveTypes.Add(constructiveType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name", constructiveType.DOMPartId);
            return View(constructiveType);
        }

        // GET: ConstructiveTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConstructiveType constructiveType = db.ConstructiveTypes.Find(id);
            if (constructiveType == null)
            {
                return HttpNotFound();
            }
            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name", constructiveType.DOMPartId);
            return View(constructiveType);
        }

        // POST: ConstructiveTypes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,DOMPartId")] ConstructiveType constructiveType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(constructiveType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name", constructiveType.DOMPartId);
            return View(constructiveType);
        }

        // GET: ConstructiveTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConstructiveType constructiveType = db.ConstructiveTypes.Find(id);
            if (constructiveType == null)
            {
                return HttpNotFound();
            }
            return View(constructiveType);
        }

        // POST: ConstructiveTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConstructiveType constructiveType = db.ConstructiveTypes.Find(id);
            db.ConstructiveTypes.Remove(constructiveType);
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
