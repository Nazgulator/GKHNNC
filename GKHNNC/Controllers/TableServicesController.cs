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
    public class TableServicesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: TableServices
        public ActionResult Index()
        {
            return View(db.TableServices.ToList());
        }

        // GET: TableServices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TableService tableService = db.TableServices.Find(id);
            if (tableService == null)
            {
                return HttpNotFound();
            }
            return View(tableService);
        }

        // GET: TableServices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TableServices/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,Summ")] TableService tableService)
        {
            if (ModelState.IsValid)
            {
                db.TableServices.Add(tableService);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tableService);
        }

        // GET: TableServices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TableService tableService = db.TableServices.Find(id);
            if (tableService == null)
            {
                return HttpNotFound();
            }
            return View(tableService);
        }

        // POST: TableServices/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type,Summ")] TableService tableService)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tableService).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tableService);
        }

        // GET: TableServices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TableService tableService = db.TableServices.Find(id);
            if (tableService == null)
            {
                return HttpNotFound();
            }
            return View(tableService);
        }

        // POST: TableServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TableService tableService = db.TableServices.Find(id);
            db.TableServices.Remove(tableService);
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
