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
using Opredelenie;

namespace GKHNNC.Controllers
{
    public class LogsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Logs
        public ActionResult Index()
        { DateTime D = DateTime.Now;

            List<Log> L = db.Logs.Where(x => x.Date.Year == D.Year && x.Date.Month == D.Month && x.Date.Day == D.Day-1).ToList();
            return View(L);
        }

        public ActionResult MenuLogs(string Selection)
        {
            int Month = 1;
            if (Selection != "" && Selection != null) {
                string[] S = Selection.Split(';');
                int Y = Convert.ToInt16(S[0]);
               
                Month = Opr.MonthObratno(S[1]);
                List<string> Day = new List<string>();
                for (int i = 1; i <= DateTime.DaysInMonth(Y, Month); i++)
                {
                    Day.Add(i.ToString());
                }
                ViewBag.Day = Day; 
                }
            else
            {
                List<string> Day = new List<string>();
                for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
                {
                    Day.Add(i.ToString());
                }
                ViewBag.Day = Day;

            }
            List<string> M = new List<string> { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
            M.RemoveAt(Month - 1);
            M.Insert(0, Opr.MonthOpred(Month));
            ViewBag.Month = M;
            List<string> Year = new List<string>();
            for (int i = DateTime.Now.Year; i >= 2018; i--)
            {
                Year.Add(i.ToString());
            }

            ViewBag.Year = Year;

            return View();
        }
        public ActionResult SpisokLogs(string Selection)
        {
            DateTime D = DateTime.Now;
            
            if (Selection != null && Selection != "")
            {
                string[] s = Selection.Split(';');
                int Day = Convert.ToInt16(s[0]);
                int Month = Opr.MonthObratno(s[1]);
                int Year = Convert.ToInt16(s[2]);
                List<Log> L = db.Logs.Where(x => x.Date.Year == Year && x.Date.Month == Month && x.Date.Day == Day).ToList();
                return View(L);
            }
            else
            {
                List<Log> L = db.Logs.Where(x => x.Date.Year == D.Year && x.Date.Month == D.Month && x.Date.Day == D.Day - 1).ToList();
                return View(L);
            }
           
           
        }

        // GET: Logs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Log log = db.Logs.Find(id);
            if (log == null)
            {
                return HttpNotFound();
            }
            return View(log);
        }

        // GET: Logs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Logs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,WhatToDo,Date")] Log log)
        {
            if (ModelState.IsValid)
            {
                db.Logs.Add(log);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(log);
        }

        // GET: Logs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Log log = db.Logs.Find(id);
            if (log == null)
            {
                return HttpNotFound();
            }
            return View(log);
        }

        // POST: Logs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,WhatToDo,Date")] Log log)
        {
            if (ModelState.IsValid)
            {
                db.Entry(log).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(log);
        }

        // GET: Logs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Log log = db.Logs.Find(id);
            if (log == null)
            {
                return HttpNotFound();
            }
            return View(log);
        }

        // POST: Logs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Log log = db.Logs.Find(id);
            db.Logs.Remove(log);
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
