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
    public class AvtomobilsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Avtomobils
        public ActionResult Index()
        {
            var avtomobils = db.Avtomobils.Include(a => a.Marka).Include(a => a.Type).Include(a => a.KontrAgent);
            return View(avtomobils.ToList());
        }
        public ActionResult AvtomobilsSpisok(string Selection)
        {
            if (Selection != null && Selection != "")
            {
                string[] s = Selection.Split(';');
                int AT = Convert.ToInt16(s[1]);
                int AN = Convert.ToInt16(s[0]);
                bool AG = Convert.ToBoolean(s[2]);

                List<Avtomobil> avtomobils = new List<Avtomobil>();
                if (AT + AN == 0)
                {
                     avtomobils = db.Avtomobils.Include(a => a.Marka).Include(a => a.Type).Include(a=>a.KontrAgent).Where(x => x.Glonass == AG).ToList();
                }

                if (AT==0 && AN!=0)
                {
                    avtomobils = db.Avtomobils.Include(a => a.Marka).Include(a => a.Type).Include(a => a.KontrAgent).Where(x =>x.Id == AN && x.Glonass == AG).ToList();
                }
                if (AT != 0 && AN == 0)
                {
                    avtomobils = db.Avtomobils.Include(a => a.Marka).Include(a => a.Type).Include(a => a.KontrAgent).Where(x => x.Type.Id == AT && x.Glonass == AG).ToList();
                }
                if (AT != 0 && AN != 0)
                {
                    avtomobils = db.Avtomobils.Include(a => a.Marka).Include(a => a.Type).Include(a => a.KontrAgent).Where(x => x.Type.Id == AT && x.Id == AN && x.Glonass == AG).ToList();
                }
                return View(avtomobils);
            }
            else
            {
                var avtomobils = db.Avtomobils.Include(a => a.Marka).Include(a => a.Type);
                return View(avtomobils.ToList());
            }
        }

        // GET: Avtomobils/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Avtomobil avtomobil = db.Avtomobils.Find(id);
            if (avtomobil == null)
            {
                return HttpNotFound();
            }
            return View(avtomobil);
        }

        // GET: Avtomobils/Create
        public ActionResult Create()
        {
            ViewBag.MarkaId = new SelectList(db.MarkaAvtomobils, "Id", "Name");
            ViewBag.TypeId = new SelectList(db.TypeAvtos, "Id", "Type");
            return View();
        }

        public ActionResult AvtomobilsMenu()
        {//заполняем номера и на 1 строке ВСЕ АВТО
            List<SelectListItem> AN = new SelectList(db.Avtomobils.OrderBy(a => a.Number), "Id", "Number").ToList();
            SelectListItem ANI = new SelectListItem();
            ANI.Text = "Все автомобили";
            ANI.Value = "0";
            AN.Insert(0,ANI);
            ViewBag.Number = AN;
            //заполняем типы и на 1 строке ВСЕ ТИПЫ
            List<SelectListItem> AT = new SelectList(db.TypeAvtos, "Id", "Type").ToList();
            SelectListItem ATI = new SelectListItem();
            ATI.Text = "Любой тип";
            ATI.Value = "0";
            AT.Insert(0, ATI);
            ViewBag.Type = AT;

            ViewBag.Glo = false;
            return View();
        }

        // POST: Avtomobils/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MarkaId,TypeId,Number,Date,Garage,Glonass")] Avtomobil avtomobil)
        {
            if (ModelState.IsValid)
            {
                if (avtomobil.Garage == null) { avtomobil.Garage = 0; }
                avtomobil.KontrAgentId = 1;
                db.Avtomobils.Add(avtomobil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MarkaId = new SelectList(db.MarkaAvtomobils, "Id", "Name", avtomobil.MarkaId);
            ViewBag.TypeId = new SelectList(db.TypeAvtos, "Id", "Type", avtomobil.TypeId);
            return View(avtomobil);
        }

        public  JsonResult White(int id)
        {
            Avtomobil A =db.Avtomobils.Where(x => x.Id == id).First();
            A.WhiteSpisok = !A.WhiteSpisok;
            db.Entry(A).State = EntityState.Modified;
            db.SaveChanges();

            return Json("");
        }
        // GET: Avtomobils/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Avtomobil avtomobil = db.Avtomobils.Find(id);
            if (avtomobil == null)
            {
                return HttpNotFound();
            }
            ViewBag.MarkaId = new SelectList(db.MarkaAvtomobils, "Id", "Name", avtomobil.MarkaId);
            ViewBag.TypeId = new SelectList(db.TypeAvtos, "Id", "Type", avtomobil.TypeId);
            ViewBag.KontragentId = new SelectList(db.KontrAgents, "Id", "Name", avtomobil.KontrAgentId);
            return View(avtomobil);
        }

        // POST: Avtomobils/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MarkaId,TypeId,Number,Date,Garage,Glonass,WhiteSpisok,KontragentId")] Avtomobil avtomobil)
        {
            if (ModelState.IsValid)
            {
                db.Entry(avtomobil).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MarkaId = new SelectList(db.MarkaAvtomobils, "Id", "Name", avtomobil.MarkaId);
            ViewBag.TypeId = new SelectList(db.TypeAvtos, "Id", "Type", avtomobil.TypeId);
            return View(avtomobil);
        }

        // GET: Avtomobils/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Avtomobil avtomobil = db.Avtomobils.Find(id);
            if (avtomobil == null)
            {
                return HttpNotFound();
            }
            return View(avtomobil);
        }

        // POST: Avtomobils/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Avtomobil avtomobil = db.Avtomobils.Find(id);
            db.Avtomobils.Remove(avtomobil);
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
