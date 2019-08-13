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
    public class RoomDoorsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: RoomDoors
        public ActionResult Index()
        {
            return View(db.RoomDoors.ToList());
        }

        // GET: RoomDoors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomDoor roomDoor = db.RoomDoors.Find(id);
            if (roomDoor == null)
            {
                return HttpNotFound();
            }
            return View(roomDoor);
        }

        // GET: RoomDoors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomDoors/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Door")] RoomDoor roomDoor)
        {
            if (ModelState.IsValid)
            {
                db.RoomDoors.Add(roomDoor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roomDoor);
        }

        // GET: RoomDoors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomDoor roomDoor = db.RoomDoors.Find(id);
            if (roomDoor == null)
            {
                return HttpNotFound();
            }
            return View(roomDoor);
        }

        // POST: RoomDoors/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Door")] RoomDoor roomDoor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomDoor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roomDoor);
        }

        // GET: RoomDoors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomDoor roomDoor = db.RoomDoors.Find(id);
            if (roomDoor == null)
            {
                return HttpNotFound();
            }
            return View(roomDoor);
        }

        // POST: RoomDoors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoomDoor roomDoor = db.RoomDoors.Find(id);
            db.RoomDoors.Remove(roomDoor);
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
