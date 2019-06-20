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
    public class WorksController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Works
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Works.ToList());
        }

        // GET: Works/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Work work = db.Works.Find(id);
            if (work == null)
            {
                return HttpNotFound();
            }
            return View(work);
        }

        // GET: Works/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Works/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Group,Izmerenie")] Work work)
        {
            try
            {
                if (ModelState.IsValid)

                {
                    //work.Date = DateTime.Now;
                    string wo = "";
                    string rk = "";
                    bool go = false;
                    int ind = 0;
                    
                    int ID = db.Works.Count()+1;
                    while (!go)
                    {
                        go = true;
                        foreach (Work w in db.Works)
                        {
                            int C = Convert.ToInt16(w.Code.Remove(0, 3));
                            if (C ==ID+ind)
                            {
                                go = false;
                            }
                           
                        }
                        if (!go) { ind++; }


                    }
                    ID += ind;
                    if (work.Group.Equals("ТО конструктивных элементов"))
                    {
                         wo = "01";
                    }
                    else
                    {
                        wo = "02";
                    }

                    rk = ID.ToString();//work.WorkId.ToString();
                    while (rk.Length < 4)
                    {
                        rk = "0" + rk;
                    }
                     //00-0000 группа-работа (не более 10 разрядов)
                    work.Code = wo+"-"+rk;
                    db.Works.Add(work);
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
            }
            catch (DataException  dex )
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator."+ dex);
            }

            return View(work);
        }





        // GET: Works/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Work work = db.Works.Find(id);
            if (work == null)
            {
                return HttpNotFound();
            }
            return View(work);
        }

        // POST: Works/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WorkId,Name,Group,Izmerenie,Code")] Work work)
        {
            if (ModelState.IsValid)
            {
                db.Entry(work).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(work);
        }

        // GET: Works/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Work work = db.Works.Find(id);
            if (work == null)
            {
                return HttpNotFound();
            }
            return View(work);
        }

        // POST: Works/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Work work = db.Works.Find(id);
            db.Works.Remove(work);
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
