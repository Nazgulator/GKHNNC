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
    public class MKDYearResultsController : Controller
    {
        private WorkContext db = new WorkContext();


        public JsonResult AdresToSession(string Adres = "")
        {
            if (Adres!=null&&Adres.Equals("") == false)
            {

                Session["CurrentAdres"] = Adres;
            }
            return Json(Adres);
        }

        public JsonResult DomToSession(string Dom = "")
        {
            if (Dom!=null&&Dom.Equals("") == false)
            {

                Session["CurrentDom"] = Dom;
            }
            return Json(Dom);
        }

        public JsonResult RemoveAdresFromSession()
        {
           

                Session["CurrentAdres"] = "";
           
            return Json("Ok");
        }


        public JsonResult RemoveDomFromSession()
        {


            Session["CurrentDom"] = "";

            return Json("Ok");
        }

        // GET: MKDYearResults
        public ActionResult Index(string Adres ="", string Dom="")
        {
            List<MKDYearResult> Res = new List<MKDYearResult>();
            if (Adres == null || Adres.Equals("")==true)
            {
                Adres = (string)Session["CurrentAdres"];
            }
            else
            {
                AdresToSession(Adres);
            }
           
            if (Dom == null || Dom.Equals("") == true)
            {
                Dom = (string)Session["CurrentDom"];
            }
            else
            {
                DomToSession(Dom);
            }


            if ( Adres!=null &&Adres.Equals("") == false)
            {
                Res = db.MKDYearResults.Where(x => x.AdresMKD.Contains(Adres)).ToList();

                
            }
            else
            {
                Res = db.MKDYearResults.ToList();

            }

            if (Dom != null && Dom.Equals("") == false)
            {
                try
                {
                    Res = Res.Where(x => x.AdresMKD.Contains(Dom)).ToList();
                }
                catch
                {

                }
            }

            ViewBag.CurrentAdres = Adres;
            ViewBag.CurrentDom = Dom;

            return View(Res);
        }

        // GET: MKDYearResults/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MKDYearResult mKDYearResult = db.MKDYearResults.Find(id);
            if (mKDYearResult == null)
            {
                return HttpNotFound();
            }
            return View(mKDYearResult);
        }

        // GET: MKDYearResults/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MKDYearResults/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresMKD,AdresFGBU,Statya,AdresId,PeriodYear,BallStart,Nachisleno,Oplacheno,BallEnd,CompletedWorks")] MKDYearResult mKDYearResult)
        {
            if (ModelState.IsValid)
            {
                db.MKDYearResults.Add(mKDYearResult);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mKDYearResult);
        }

 

        // GET: MKDYearResults/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MKDYearResult mKDYearResult = db.MKDYearResults.Find(id);
            if (mKDYearResult == null)
            {
                return HttpNotFound();
            }
            return View(mKDYearResult);
        }

        // POST: MKDYearResults/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresMKD,AdresFGBU,Statya,AdresId,PeriodYear,BallStart,Nachisleno,Oplacheno,BallEnd")] MKDYearResult mKDYearResult)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mKDYearResult).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mKDYearResult);
        }

        // GET: MKDYearResults/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MKDYearResult mKDYearResult = db.MKDYearResults.Find(id);
            if (mKDYearResult == null)
            {
                return HttpNotFound();
            }
            return View(mKDYearResult);
        }

        // POST: MKDYearResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MKDYearResult mKDYearResult = db.MKDYearResults.Find(id);
            db.MKDYearResults.Remove(mKDYearResult);
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
