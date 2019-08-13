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
    public class RoomTypesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: RoomTypes
        public ActionResult Index()
        {
            return View(db.RoomTypes.ToList());
        }

        // GET: RoomTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomType roomType = db.RoomTypes.Find(id);
            if (roomType == null)
            {
                return HttpNotFound();
            }
            return View(roomType);
        }

        // GET: RoomTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomTypes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type")] RoomType roomType)
        {
            if (ModelState.IsValid)
            {
                db.RoomTypes.Add(roomType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roomType);
        }

        // GET: RoomTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomType roomType = db.RoomTypes.Find(id);
            if (roomType == null)
            {
                return HttpNotFound();
            }
            return View(roomType);
        }

        // POST: RoomTypes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type")] RoomType roomType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roomType);
        }

        // GET: RoomTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomType roomType = db.RoomTypes.Find(id);
            if (roomType == null)
            {
                return HttpNotFound();
            }
            return View(roomType);
        }

        // POST: RoomTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoomType roomType = db.RoomTypes.Find(id);
            db.RoomTypes.Remove(roomType);
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
