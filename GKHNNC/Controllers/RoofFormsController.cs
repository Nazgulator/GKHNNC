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
    public class RoofFormsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: RoofForms
        public ActionResult Index()
        {
            return View(db.RoofForms.ToList());
        }

        // GET: RoofForms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoofForm roofForm = db.RoofForms.Find(id);
            if (roofForm == null)
            {
                return HttpNotFound();
            }
            return View(roofForm);
        }

        // GET: RoofForms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoofForms/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Form")] RoofForm roofForm)
        {
            if (ModelState.IsValid)
            {
                db.RoofForms.Add(roofForm);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roofForm);
        }

        // GET: RoofForms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoofForm roofForm = db.RoofForms.Find(id);
            if (roofForm == null)
            {
                return HttpNotFound();
            }
            return View(roofForm);
        }

        // POST: RoofForms/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Form")] RoofForm roofForm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roofForm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roofForm);
        }

        // GET: RoofForms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoofForm roofForm = db.RoofForms.Find(id);
            if (roofForm == null)
            {
                return HttpNotFound();
            }
            return View(roofForm);
        }

        // POST: RoofForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoofForm roofForm = db.RoofForms.Find(id);
            db.RoofForms.Remove(roofForm);
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
