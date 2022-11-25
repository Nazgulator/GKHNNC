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
    public class SchetchiksController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Schetchiks
        public ActionResult Index()
        {
            var schetchiks = db.Schetchiks.Include(s => s.Adres).Include(s => s.Group).Include(s => s.Tip);
            return View(schetchiks.ToList());
        }

        // GET: Schetchiks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schetchik schetchik = db.Schetchiks.Find(id);
            if (schetchik == null)
            {
                return HttpNotFound();
            }
            return View(schetchik);
        }

        // GET: Schetchiks/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            ViewBag.GroupId = new SelectList(db.Groups, "Id", "Name");
            ViewBag.TipId = new SelectList(db.SchetchikTips, "Id", "Name");
            return View();
        }

        // POST: Schetchiks/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TipId,GroupId,AdresId,Diameter,DateEnd,Name,Number")] Schetchik schetchik)
        {
            if (schetchik.Summa ==null)
            {
                schetchik.Summa = 0;
            }
            schetchik.DateStart = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Schetchiks.Add(schetchik);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", schetchik.AdresId);
            ViewBag.GroupId = new SelectList(db.Groups, "Id", "Name", schetchik.GroupId);
            ViewBag.TipId = new SelectList(db.SchetchikTips, "Id", "Name", schetchik.TipId);
            return View(schetchik);
        }

        // GET: Schetchiks/Edit/5
        public ActionResult Edit(int? id)
        {
          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schetchik schetchik = db.Schetchiks.Find(id);
            if (schetchik == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", schetchik.AdresId);
            ViewBag.GroupId = new SelectList(db.Groups, "Id", "Name", schetchik.GroupId);
            ViewBag.TipId = new SelectList(db.SchetchikTips, "Id", "Name", schetchik.TipId);
            return View(schetchik);
        }

        // POST: Schetchiks/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TipId,GroupId,AdresId,Diameter,DateEnd,Name,Number")] Schetchik schetchik)
        {
            if (schetchik.Summa == null)
            {
                schetchik.Summa = 0;
            }
            schetchik.DateStart = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(schetchik).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", schetchik.AdresId);
            ViewBag.GroupId = new SelectList(db.Groups, "Id", "Name", schetchik.GroupId);
            ViewBag.TipId = new SelectList(db.SchetchikTips, "Id", "Name", schetchik.TipId);
            return View(schetchik);
        }

        // GET: Schetchiks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schetchik schetchik = db.Schetchiks.Find(id);
            if (schetchik == null)
            {
                return HttpNotFound();
            }
            return View(schetchik);
        }

        // POST: Schetchiks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Schetchik schetchik = db.Schetchiks.Find(id);
            db.Schetchiks.Remove(schetchik);
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
