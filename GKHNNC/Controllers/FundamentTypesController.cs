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
    public class FundamentTypesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: FundamentTypes
        public ActionResult Index()
        {
            return View(db.FundamentTypes.ToList());
        }

        // GET: FundamentTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FundamentType fundamentType = db.FundamentTypes.Find(id);
            if (fundamentType == null)
            {
                return HttpNotFound();
            }
            return View(fundamentType);
        }

        // GET: FundamentTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FundamentTypes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type")] FundamentType fundamentType)
        {
            if (ModelState.IsValid)
            {
                db.FundamentTypes.Add(fundamentType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fundamentType);
        }

        // GET: FundamentTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FundamentType fundamentType = db.FundamentTypes.Find(id);
            if (fundamentType == null)
            {
                return HttpNotFound();
            }
            return View(fundamentType);
        }

        // POST: FundamentTypes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type")] FundamentType fundamentType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fundamentType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fundamentType);
        }

        // GET: FundamentTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FundamentType fundamentType = db.FundamentTypes.Find(id);
            if (fundamentType == null)
            {
                return HttpNotFound();
            }
            return View(fundamentType);
        }

        // POST: FundamentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FundamentType fundamentType = db.FundamentTypes.Find(id);
            db.FundamentTypes.Remove(fundamentType);
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
