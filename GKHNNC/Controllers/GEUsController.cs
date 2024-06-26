﻿using System;
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
    public class GEUsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: GEUs
        public ActionResult Index()
        {
            return View(db.GEUs.ToList());
        }

        // GET: GEUs
        public ActionResult PrintConstantIndex()
        {
            return View(db.PrintConstants.ToList());
        }

        // GET: GEUs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GEU gEU = db.GEUs.Find(id);
            if (gEU == null)
            {
                return HttpNotFound();
            }
            return View(gEU);
        }

        // GET: GEUs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GEUs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Director,DirectorIP,Doverennost,IngenerPTO,IngenerOEGF,EU,GEUN")] GEU gEU)
        {
            if (ModelState.IsValid)
            {
                db.GEUs.Add(gEU);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gEU);
        }

        // GET: GEUs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GEU gEU = db.GEUs.Find(id);
            if (gEU == null)
            {
                return HttpNotFound();
            }
            return View(gEU);
        }

        // POST: GEUs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,Name,Director,DirectorIP,Doverennost,IngenerPTO,IngenerOEGF,EU,GEUN,DirectorDolgnost,IngenerPTODolgnost,IngenerOEGFDolgnost")] GEU gEU)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gEU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gEU);
        }

        // GET: GEUs/PCEdit/5
        public ActionResult PCEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrintConstant pc = db.PrintConstants.Find(id);
            if (pc == null)
            {
                return HttpNotFound();
            }
            return View(pc);
        }

        // POST: GEUs/PCEdit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult PCEdit([Bind(Include = "Id,Name,NameRP,Dolgnost,Poisk")] PrintConstant printConstant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(printConstant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(printConstant);
        }

        // GET: GEUs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GEU gEU = db.GEUs.Find(id);
            if (gEU == null)
            {
                return HttpNotFound();
            }
            return View(gEU);
        }

        // POST: GEUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GEU gEU = db.GEUs.Find(id);
            db.GEUs.Remove(gEU);
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
