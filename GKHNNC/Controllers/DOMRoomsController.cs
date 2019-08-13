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
    public class DOMRoomsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: DOMRooms
        public ActionResult Index()
        {
            var dOMRooms = db.DOMRooms.Include(d => d.Adres).Include(d => d.Door).Include(d => d.Overlap).Include(d => d.Type).Include(d => d.Window);
            return View(dOMRooms.ToList());
        }

        // GET: DOMRooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMRoom dOMRoom = db.DOMRooms.Find(id);
            if (dOMRoom == null)
            {
                return HttpNotFound();
            }
            return View(dOMRoom);
        }

        // GET: DOMRooms/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            ViewBag.DoorId = new SelectList(db.RoomDoors, "Id", "Door");
            ViewBag.OverlapId = new SelectList(db.RoomOverlaps, "Id", "Overlap");
            ViewBag.TypeId = new SelectList(db.RoomTypes, "Id", "Type");
            ViewBag.WindowId = new SelectList(db.RoomWindows, "Id", "Window");
            return View();
        }

        // POST: DOMRooms/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,TypeId,OverlapId,WindowId,DoorId,Lodgi,Balkon,Date")] DOMRoom dOMRoom)
        {
            if (ModelState.IsValid)
            {
                db.DOMRooms.Add(dOMRoom);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMRoom.AdresId);
            ViewBag.DoorId = new SelectList(db.RoomDoors, "Id", "Door", dOMRoom.DoorId);
            ViewBag.OverlapId = new SelectList(db.RoomOverlaps, "Id", "Overlap", dOMRoom.OverlapId);
            ViewBag.TypeId = new SelectList(db.RoomTypes, "Id", "Type", dOMRoom.TypeId);
            ViewBag.WindowId = new SelectList(db.RoomWindows, "Id", "Window", dOMRoom.WindowId);
            return View(dOMRoom);
        }

        // GET: DOMRooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMRoom dOMRoom = db.DOMRooms.Find(id);
            if (dOMRoom == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMRoom.AdresId);
            ViewBag.DoorId = new SelectList(db.RoomDoors, "Id", "Door", dOMRoom.DoorId);
            ViewBag.OverlapId = new SelectList(db.RoomOverlaps, "Id", "Overlap", dOMRoom.OverlapId);
            ViewBag.TypeId = new SelectList(db.RoomTypes, "Id", "Type", dOMRoom.TypeId);
            ViewBag.WindowId = new SelectList(db.RoomWindows, "Id", "Window", dOMRoom.WindowId);
            return View(dOMRoom);
        }

        // POST: DOMRooms/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,TypeId,OverlapId,WindowId,DoorId,Lodgi,Balkon,Date")] DOMRoom dOMRoom)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dOMRoom).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMRoom.AdresId);
            ViewBag.DoorId = new SelectList(db.RoomDoors, "Id", "Door", dOMRoom.DoorId);
            ViewBag.OverlapId = new SelectList(db.RoomOverlaps, "Id", "Overlap", dOMRoom.OverlapId);
            ViewBag.TypeId = new SelectList(db.RoomTypes, "Id", "Type", dOMRoom.TypeId);
            ViewBag.WindowId = new SelectList(db.RoomWindows, "Id", "Window", dOMRoom.WindowId);
            return View(dOMRoom);
        }

        // GET: DOMRooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMRoom dOMRoom = db.DOMRooms.Find(id);
            if (dOMRoom == null)
            {
                return HttpNotFound();
            }
            return View(dOMRoom);
        }

        // POST: DOMRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DOMRoom dOMRoom = db.DOMRooms.Find(id);
            db.DOMRooms.Remove(dOMRoom);
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
