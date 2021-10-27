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
    public class TypeElementsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: TypeElements
        public ActionResult Index()
        {
            var typeElements = db.TypeElements.Include(t => t.Adres).Include(t => t.ConstructiveType).Include(t => t.DOMPart).Include(t => t.Material);
            return View(typeElements.ToList());
        }

        // GET: TypeElements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeElement typeElement = db.TypeElements.Find(id);
            if (typeElement == null)
            {
                return HttpNotFound();
            }
            return View(typeElement);
        }

        // GET: TypeElements/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            ViewBag.ConstructiveTypeId = new SelectList(db.ConstructiveTypes, "Id", "Name");
            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name");
            ViewBag.MaterialId = new SelectList(db.Materials, "Id", "Name");
            return View();
        }

        // POST: TypeElements/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ConstructiveTypeId,AdresId,Date,MaterialId,DOMPartId,UserName")] TypeElement typeElement)
        {
            if (ModelState.IsValid)
            {
                db.TypeElements.Add(typeElement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", typeElement.AdresId);
            ViewBag.ConstructiveTypeId = new SelectList(db.ConstructiveTypes, "Id", "Name", typeElement.ConstructiveTypeId);
            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name", typeElement.DOMPartId);
            ViewBag.MaterialId = new SelectList(db.Materials, "Id", "Name", typeElement.MaterialId);
            return View(typeElement);
        }

        // GET: TypeElements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeElement typeElement = db.TypeElements.Find(id);
            if (typeElement == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", typeElement.AdresId);
            ViewBag.ConstructiveTypeId = new SelectList(db.ConstructiveTypes, "Id", "Name", typeElement.ConstructiveTypeId);
            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name", typeElement.DOMPartId);
            ViewBag.MaterialId = new SelectList(db.Materials, "Id", "Name", typeElement.MaterialId);
            return View(typeElement);
        }

        // POST: TypeElements/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ConstructiveTypeId,AdresId,Date,MaterialId,DOMPartId,UserName")] TypeElement typeElement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(typeElement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", typeElement.AdresId);
            ViewBag.ConstructiveTypeId = new SelectList(db.ConstructiveTypes, "Id", "Name", typeElement.ConstructiveTypeId);
            ViewBag.DOMPartId = new SelectList(db.DOMParts, "Id", "Name", typeElement.DOMPartId);
            ViewBag.MaterialId = new SelectList(db.Materials, "Id", "Name", typeElement.MaterialId);
            return View(typeElement);
        }

        // GET: TypeElements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeElement typeElement = db.TypeElements.Find(id);
            if (typeElement == null)
            {
                return HttpNotFound();
            }
            return View(typeElement);
        }

        // POST: TypeElements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TypeElement typeElement = db.TypeElements.Find(id);
            db.TypeElements.Remove(typeElement);
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
