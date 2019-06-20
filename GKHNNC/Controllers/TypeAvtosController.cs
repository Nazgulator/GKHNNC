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
    public class TypeAvtosController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: TypeAvtos
        public ActionResult Index()
        {
            return View(db.TypeAvtos.ToList());
        }

        // GET: TypeAvtos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeAvto typeAvto = db.TypeAvtos.Find(id);
            if (typeAvto == null)
            {
                return HttpNotFound();
            }
            return View(typeAvto);
        }

        // GET: TypeAvtos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TypeAvtos/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type")] TypeAvto typeAvto)
        {
            if (ModelState.IsValid)
            {
                db.TypeAvtos.Add(typeAvto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(typeAvto);
        }

        // GET: TypeAvtos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeAvto typeAvto = db.TypeAvtos.Find(id);
            if (typeAvto == null)
            {
                return HttpNotFound();
            }
            return View(typeAvto);
        }

        // POST: TypeAvtos/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type")] TypeAvto typeAvto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(typeAvto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(typeAvto);
        }

        // GET: TypeAvtos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeAvto typeAvto = db.TypeAvtos.Find(id);
            if (typeAvto == null)
            {
                return HttpNotFound();
            }
            return View(typeAvto);
        }

        // POST: TypeAvtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TypeAvto typeAvto = db.TypeAvtos.Find(id);
            db.TypeAvtos.Remove(typeAvto);
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
