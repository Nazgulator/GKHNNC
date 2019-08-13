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
    public class RoomOverlapsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: RoomOverlaps
        public ActionResult Index()
        {
            return View(db.RoomOverlaps.ToList());
        }

        // GET: RoomOverlaps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomOverlap roomOverlap = db.RoomOverlaps.Find(id);
            if (roomOverlap == null)
            {
                return HttpNotFound();
            }
            return View(roomOverlap);
        }

        // GET: RoomOverlaps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomOverlaps/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Overlap")] RoomOverlap roomOverlap)
        {
            if (ModelState.IsValid)
            {
                db.RoomOverlaps.Add(roomOverlap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roomOverlap);
        }

        // GET: RoomOverlaps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomOverlap roomOverlap = db.RoomOverlaps.Find(id);
            if (roomOverlap == null)
            {
                return HttpNotFound();
            }
            return View(roomOverlap);
        }

        // POST: RoomOverlaps/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Overlap")] RoomOverlap roomOverlap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomOverlap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roomOverlap);
        }

        // GET: RoomOverlaps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomOverlap roomOverlap = db.RoomOverlaps.Find(id);
            if (roomOverlap == null)
            {
                return HttpNotFound();
            }
            return View(roomOverlap);
        }

        // POST: RoomOverlaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoomOverlap roomOverlap = db.RoomOverlaps.Find(id);
            db.RoomOverlaps.Remove(roomOverlap);
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
