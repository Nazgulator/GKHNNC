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
    public class RoomWindowsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: RoomWindows
        public ActionResult Index()
        {
            return View(db.RoomWindows.ToList());
        }

        // GET: RoomWindows/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomWindow roomWindow = db.RoomWindows.Find(id);
            if (roomWindow == null)
            {
                return HttpNotFound();
            }
            return View(roomWindow);
        }

        // GET: RoomWindows/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomWindows/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Window")] RoomWindow roomWindow)
        {
            if (ModelState.IsValid)
            {
                db.RoomWindows.Add(roomWindow);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roomWindow);
        }

        // GET: RoomWindows/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomWindow roomWindow = db.RoomWindows.Find(id);
            if (roomWindow == null)
            {
                return HttpNotFound();
            }
            return View(roomWindow);
        }

        // POST: RoomWindows/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Window")] RoomWindow roomWindow)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomWindow).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roomWindow);
        }

        // GET: RoomWindows/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomWindow roomWindow = db.RoomWindows.Find(id);
            if (roomWindow == null)
            {
                return HttpNotFound();
            }
            return View(roomWindow);
        }

        // POST: RoomWindows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoomWindow roomWindow = db.RoomWindows.Find(id);
            db.RoomWindows.Remove(roomWindow);
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
