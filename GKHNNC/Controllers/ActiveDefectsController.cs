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
    public class ActiveDefectsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: ActiveDefects
        public ActionResult Index()
        {
            var activeDefects = db.ActiveDefects.Include(a => a.Adres).Include(a => a.Defect).Include(a => a.Element);
            return View(activeDefects.ToList());
        }

        // GET: ActiveDefects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActiveDefect activeDefect = db.ActiveDefects.Find(id);
            if (activeDefect == null)
            {
                return HttpNotFound();
            }
            return View(activeDefect);
        }

        // GET: ActiveDefects/Create
        public ActionResult Create(int ElementId = 1118,int AdresId=13)
        {
            List<SelectListItem> Elements = new SelectList(db.Elements, "Id", "Name").ToList();
            SelectListItem S = new SelectListItem();
            S.Value = ElementId.ToString();
            S.Text = db.Elements.Where(x => x.Id == ElementId).First().Name;
            Elements.Remove(S);
            Elements.Insert(0, S);

            List<SelectListItem> Adress = new SelectList(db.Adres, "Id", "Adress").ToList();
            SelectListItem A = new SelectListItem();
            A.Value = AdresId.ToString();
            A.Text = db.Adres.Where(x => x.Id == AdresId).First().Adress;
            Adress.Remove(A);
            Adress.Insert(0, A);

            ViewBag.AdresId = Adress;
            ViewBag.DefectId = new SelectList(db.Defects.Where(x=>x.ElementId==ElementId), "Id", "Def");
            ViewBag.ElementId = Elements;
            return View();
        }

        // POST: ActiveDefects/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ElementId,DefectId,AdresId,Sostoyanie,Opisanie,Date")] ActiveDefect activeDefect)
        {
            if (activeDefect.Sostoyanie > 10) { activeDefect.Sostoyanie = 10; }
            if (activeDefect.Sostoyanie < 1) { activeDefect.Sostoyanie = 1; }
            if (activeDefect.Date.Year == 1) { activeDefect.Date = DateTime.Now; }


            db.ActiveDefects.Add(activeDefect);
                db.SaveChanges();
                return RedirectToAction("Index");
           

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", activeDefect.AdresId);
            ViewBag.DefectId = new SelectList(db.Defects, "Id", "Def", activeDefect.DefectId);
            ViewBag.ElementId = new SelectList(db.Elements, "Id", "Name", activeDefect.ElementId);
            return View(activeDefect);
        }

        // GET: ActiveDefects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActiveDefect activeDefect = db.ActiveDefects.Find(id);
            if (activeDefect == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", activeDefect.AdresId);
            ViewBag.DefectId = new SelectList(db.Defects, "Id", "Def", activeDefect.DefectId);
            ViewBag.ElementId = new SelectList(db.Elements, "Id", "Name", activeDefect.ElementId);
            return View(activeDefect);
        }

        // POST: ActiveDefects/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ElementId,DefectId,AdresId,Sostoyanie,Opisanie,Date")] ActiveDefect activeDefect)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activeDefect).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", activeDefect.AdresId);
            ViewBag.DefectId = new SelectList(db.Defects, "Id", "Def", activeDefect.DefectId);
            ViewBag.ElementId = new SelectList(db.Elements, "Id", "Name", activeDefect.ElementId);
            return View(activeDefect);
        }

        // GET: ActiveDefects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActiveDefect activeDefect = db.ActiveDefects.Find(id);
            if (activeDefect == null)
            {
                return HttpNotFound();
            }
            return View(activeDefect);
        }

        // POST: ActiveDefects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ActiveDefect activeDefect = db.ActiveDefects.Find(id);
            db.ActiveDefects.Remove(activeDefect);
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
