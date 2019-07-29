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
    public class RoofVidsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: RoofVids
        public ActionResult Index()
        {
            return View(db.RoofVids.ToList());
        }

        // GET: RoofVids/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoofVid roofVid = db.RoofVids.Find(id);
            if (roofVid == null)
            {
                return HttpNotFound();
            }
            return View(roofVid);
        }

        // GET: RoofVids/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoofVids/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Vid")] RoofVid roofVid)
        {
            if (ModelState.IsValid)
            {
                db.RoofVids.Add(roofVid);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roofVid);
        }

        // GET: RoofVids/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoofVid roofVid = db.RoofVids.Find(id);
            if (roofVid == null)
            {
                return HttpNotFound();
            }
            return View(roofVid);
        }

        // POST: RoofVids/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Vid")] RoofVid roofVid)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roofVid).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roofVid);
        }

        // GET: RoofVids/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoofVid roofVid = db.RoofVids.Find(id);
            if (roofVid == null)
            {
                return HttpNotFound();
            }
            return View(roofVid);
        }

        // POST: RoofVids/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoofVid roofVid = db.RoofVids.Find(id);
            db.RoofVids.Remove(roofVid);
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
