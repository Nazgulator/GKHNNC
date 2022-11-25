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
    public class SchetchikTipsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: SchetchikTips
        public ActionResult Index()
        {
            return View(db.SchetchikTips.ToList());
        }

        // GET: SchetchikTips/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchetchikTip schetchikTip = db.SchetchikTips.Find(id);
            if (schetchikTip == null)
            {
                return HttpNotFound();
            }
            return View(schetchikTip);
        }

        // GET: SchetchikTips/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SchetchikTips/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] SchetchikTip schetchikTip)
        {
            if (ModelState.IsValid)
            {
                db.SchetchikTips.Add(schetchikTip);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(schetchikTip);
        }

        // GET: SchetchikTips/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchetchikTip schetchikTip = db.SchetchikTips.Find(id);
            if (schetchikTip == null)
            {
                return HttpNotFound();
            }
            return View(schetchikTip);
        }

        // POST: SchetchikTips/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] SchetchikTip schetchikTip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(schetchikTip).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(schetchikTip);
        }

        // GET: SchetchikTips/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchetchikTip schetchikTip = db.SchetchikTips.Find(id);
            if (schetchikTip == null)
            {
                return HttpNotFound();
            }
            return View(schetchikTip);
        }

        // POST: SchetchikTips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SchetchikTip schetchikTip = db.SchetchikTips.Find(id);
            db.SchetchikTips.Remove(schetchikTip);
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
