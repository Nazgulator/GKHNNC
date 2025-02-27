using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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

        public JsonResult YearToSession(string Year = "")
        {
            if (Year != null && Year.Equals("") == false)
            {

                Session["CurrentYear"] = Year;
            }
            return Json(Year);
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

        public JsonResult RemoveYearFromSession()
        {
            Session["CurrentYear"] = "";
            return Json("Ok");
        }

        public ActionResult OstatkiPoNakopitelnymSchetam()
        {
            return View();
        }

        public ActionResult PerenosOstatkovPoNakopitelnymSchetam()
        {
            return View();
        }

        public ActionResult ObrabotkaAktov()
        {
            return View();
        }

        public ActionResult ObrabotkaORC()
        {
            return View();
        }

        public ActionResult ObrabotkaArenda()
        {
            string[] fileEntries = Directory.GetFiles(Server.MapPath("~/Files/Arenda/"));
            List<string> Result = new List<string>();

            foreach (string f in fileEntries)
            {
                string FileName = System.IO.Path.GetFileName(f);
                if (FileName.Contains(".csv") == false)
                {
                    
                    continue;
                }
                Result.Add(FileName);

                
            }
            return View(Result);
        }

        public ActionResult WordWorks(string Adres = "", string Dom = "", string Year = "")
        {
            List<MKDCompleteWork> Result = new List<MKDCompleteWork>();
            //try
            //{
            //    Result = db.MKDCompleteWork.ToList();
            //}
            //catch
            //{

            //}

            if (Adres == null || Adres.Equals("") == true)
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

            if (Year == null || Year.Equals("") == true)
            {
                Year = (string)Session["CurrentYear"];
            }
            else
            {
                YearToSession(Year);
            }


            List<int> Adresa = new List<int>();
            List<int> Doma = new List<int>();
            if (Dom != null && Dom.Equals("") == false)
            {
                try
                {
                    Doma.AddRange(db.AdresMKDs.Where(x => x.ASU.EndsWith(" " + Dom)).Select(x => x.Id).ToList());
                }
                catch
                {

                }
            }


            if (Adres != null && Adres.Equals("") == false)
            {
                Adresa.AddRange(db.AdresMKDs.Where(x => x.ASU.Contains(Adres)).Select(x=>x.Id).ToList());

            }
            else
            {
                //if (Year != null && Year.Equals("") == true)
                //{
                //    //int Y = DateTime.Now.Year;
                //   // Result = db.MKDCompleteWork.Where(x => x.WorkDate.Year == Y).Take(100).ToList();
                //}

            }

            if (Adresa.Count>0 && Doma.Count>0)
            {
                Adresa = Adresa.Where(x => Doma.Contains(x)).ToList();
            }

            if (Adresa.Count == 0 && Doma.Count > 0)
            {
                Adresa = Doma;
            }

            if (Adresa.Count()>0)
            {
                Adresa = Adresa.Distinct().ToList();
                List<AdresaMKDs> AdresAll = db.AdresMKDs.Where(x => Adresa.Contains(x.Id)).ToList(); 
                Result = db.MKDCompleteWork.Where(x => Adresa.Contains(x.AdresMKDID)).OrderBy(x=>x.AdresMKDID).ThenBy(x=>x.WorkTip).ThenBy(x=>x.WorkDate).ToList();
                foreach ( var r in Result)
                {
                    r.AdresMKD = AdresAll.Where(x => x.Id == r.AdresMKDID).First();
                }


            }

            

            if (Year != null && Year.Equals("") == false)
            {

                try
                {
                    int Y = Convert.ToInt16(Year);
                    Result = Result.Where(x => x.WorkDate.Year == Y).ToList();
                }
                catch
                {

                }
            }

            ViewBag.CurrentAdres = Adres;
            ViewBag.CurrentDom = Dom;
            ViewBag.CurrentYear = Year;

            return View(Result);

        }

        // GET: MKDYearResults
        public ActionResult Index(string Adres ="", string Dom="", string Year = "")
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

            if (Year == null || Year.Equals("") == true)
            {
                Year = (string)Session["CurrentYear"];
            }
            else
            {
                YearToSession(Year);
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
                    Res = Res.Where(x => x.AdresMKD.EndsWith(" "+ Dom)).ToList();
                }
                catch
                {

                }
            }

            if (Year != null && Year.Equals("") == false)
            {
                
                try
                {
                    int Y = Convert.ToInt16(Year);
                    Res = Res.Where(x => x.PeriodYear == Y).ToList();
                }
                catch
                {

                }
            }

            ViewBag.CurrentAdres = Adres;
            ViewBag.CurrentDom = Dom;
            ViewBag.CurrentYear = Year;

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
