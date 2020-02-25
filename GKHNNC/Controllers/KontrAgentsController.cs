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
    public class KontrAgentsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: KontrAgents
        public ActionResult Index()
        {
            return View(db.KontrAgents.ToList());
        }

        // GET: KontrAgents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontrAgent kontrAgent = db.KontrAgents.Find(id);
            if (kontrAgent == null)
            {
                return HttpNotFound();
            }
            return View(kontrAgent);
        }

        // GET: KontrAgents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KontrAgents/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] KontrAgent kontrAgent)
        {
            if (ModelState.IsValid)
            {
                db.KontrAgents.Add(kontrAgent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kontrAgent);
        }

        // GET: KontrAgents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontrAgent kontrAgent = db.KontrAgents.Find(id);
            if (kontrAgent == null)
            {
                return HttpNotFound();
            }
            return View(kontrAgent);
        }

        // POST: KontrAgents/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] KontrAgent kontrAgent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kontrAgent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kontrAgent);
        }

        // GET: KontrAgents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontrAgent kontrAgent = db.KontrAgents.Find(id);
            if (kontrAgent == null)
            {
                return HttpNotFound();
            }
            return View(kontrAgent);
        }

        // POST: KontrAgents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KontrAgent kontrAgent = db.KontrAgents.Find(id);
            db.KontrAgents.Remove(kontrAgent);
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
