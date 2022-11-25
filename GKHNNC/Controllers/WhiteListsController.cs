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
    public class WhiteListsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: WhiteLists
        public ActionResult Index()
        {
            return View(db.WhiteLists.ToList());
        }

        // GET: WhiteLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WhiteList whiteList = db.WhiteLists.Find(id);
            if (whiteList == null)
            {
                return HttpNotFound();
            }
            return View(whiteList);
        }

        // GET: WhiteLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WhiteLists/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nomer,Marka,Obiem,Kontragent")] WhiteList whiteList)
        {
            if (ModelState.IsValid)
            {
                db.WhiteLists.Add(whiteList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(whiteList);
        }

        // GET: WhiteLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WhiteList whiteList = db.WhiteLists.Find(id);
            if (whiteList == null)
            {
                return HttpNotFound();
            }
            return View(whiteList);
        }

        // POST: WhiteLists/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nomer,Marka,Obiem,Kontragent")] WhiteList whiteList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(whiteList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(whiteList);
        }

        // GET: WhiteLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WhiteList whiteList = db.WhiteLists.Find(id);
            if (whiteList == null)
            {
                return HttpNotFound();
            }
            return View(whiteList);
        }

        // POST: WhiteLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WhiteList whiteList = db.WhiteLists.Find(id);
            db.WhiteLists.Remove(whiteList);
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
