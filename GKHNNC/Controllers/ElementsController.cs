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
    public class ElementsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Elements
        public ActionResult Index()
        {
            var elements = db.Elements.Include(e => e.ElementType);
            return View(elements.ToList());
        }

        // GET: Elements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Element element = db.Elements.Find(id);
            if (element == null)
            {
                return HttpNotFound();
            }
            return View(element);
        }

        public ActionResult DomParts()
        {
            return View(db.DOMParts.ToList());
        }

        // GET: Elements/Create
        public ActionResult Create()
        {
            ViewBag.ElementTypeId = new SelectList(db.DOMParts, "Id", "Name");
            ViewBag.MaxId = db.Elements.Select(x=>x.ElementId).Max();
            return View();
        }

        // GET: Elements/Create
        public ActionResult CreatePart()
        {
            return View();
        }

        // POST: Elements/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePart([Bind(Include = "Id,Name")] DOMPart element)
        {
            if (ModelState.IsValid)
            {
                db.DOMParts.Add(element);
                db.SaveChanges();

            }

            return RedirectToAction("DomParts");

        }

        // GET: Elements/Edit/5
        public ActionResult EditPart(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMPart element = db.DOMParts.Find(id);
            if (element == null)
            {
                return HttpNotFound();
            }
       
            return View(element);
        }

        // POST: Elements/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPart([Bind(Include = "Id,Name")] DOMPart element)
        {
            if (ModelState.IsValid)
            {
                db.Entry(element).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DomParts");
            }
            return View(element);
        }

        // POST: Elements/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ElementId,ElementTypeId,PhotoNeeded,Podsvetit")] Element element)
        {
            if (ModelState.IsValid)
            {
                db.Elements.Add(element);
                db.SaveChanges();
              
            }

            ViewBag.ElementTypeId = new SelectList(db.DOMParts, "Id", "Name", element.ElementTypeId);

            try
            {
                DateTime dateTime = DateTime.Now;
               List<Osmotr> Osmotrs = db.Osmotrs.Where(x => x.Date.Year == dateTime.Year).ToList();
                if (element.Id > 0)
                {
                    foreach (var O in Osmotrs)
                    {
                        ActiveElement AE = new ActiveElement();
                        AE.Sostoyanie = 3;
                        AE.Date = dateTime;
                        AE.DateIzmeneniya = dateTime;
                        AE.ElementId = element.Id;
                        AE.Est = true;
                        AE.IsOld1 = false;
                        AE.IsOld2 = false;
                        AE.AdresId = O.AdresId.Value;
                        AE.Photo1 = null;
                        AE.Photo2 = null;
                        AE.IzmerenieId = 3;
                        AE.Izmerenie2Id = 1;
                        AE.Kolichestvo = 0;
                        AE.Kolichestvo2 = 0;
                        AE.MaterialId = 1;
                        AE.OsmotrId = O.Id;
                        try
                        {
                            db.ActiveElements.Add(AE);
                            db.SaveChanges();
                        }
                        catch (Exception ex) 
                        {

                        }

                    }
                }

            }
            catch
            {

            }

            return RedirectToAction("Index");

        }

        // GET: Elements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Element element = db.Elements.Find(id);
            if (element == null)
            {
                return HttpNotFound();
            }
            ViewBag.ElementTypeId = new SelectList(db.DOMParts, "Id", "Name", element.ElementTypeId);
            return View(element);
        }

        // POST: Elements/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ElementId,ElementTypeId,PhotoNeeded,Podsvetit")] Element element)
        {
            if (ModelState.IsValid)
            {
                db.Entry(element).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ElementTypeId = new SelectList(db.DOMParts, "Id", "Name", element.ElementTypeId);
            return View(element);
        }

        // GET: Elements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Element element = db.Elements.Find(id);
            if (element == null)
            {
                return HttpNotFound();
            }
            return View(element);
        }

        // POST: Elements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Element element = db.Elements.Find(id);
            db.Elements.Remove(element);
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
