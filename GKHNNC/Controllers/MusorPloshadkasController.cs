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
    public class MusorPloshadkasController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: MusorPloshadkas
        public ActionResult Index(int StreetId = 0)
        {
            var StreetsSL = new SelectList(db.AllStreets, "Id", "Name");
            List<SelectListItem> SSL = new List<SelectListItem>();
            SSL.Add(new SelectListItem { Text = "Все", Value = "0" });
            SSL.AddRange(StreetsSL);
            ViewBag.Streets = SSL;
            List<MusorPloshadka>musorPloshadkas = db.MusorPloshadkas.ToList();
         
           

            foreach (MusorPloshadka M in musorPloshadkas)
            {
                string[] Streets = M.StreetId.Split(';');
                M.VseUlici = new List<AllStreet>();
                M.AllStreets = new List<string>();
                foreach (string S in Streets)
                {
                    try
                    {
                        int N = Convert.ToInt32(S);
                        AllStreet Name = db.AllStreets.Where(x => x.Id == N).First();
                        M.VseUlici.Add(Name);
                        M.AllStreets.Add(Name.Name);
                    }
                    catch (Exception e)
                    {

                    }
                }
                //загружаем Объёмы
                string[] Obiems = M.Obiem.Split(';');
                for (int i=0;i<7;i++)
                {
                    if (i<=Obiems.Length -1)
                    {
                        M.Obiem7[i] = Convert.ToDecimal(Obiems[i]);
                    }
                    else
                    {
                        M.Obiem7[i] = 0;
                    }
                }

            }
            List<MusorPloshadka> MP = new List<MusorPloshadka>();
            if (StreetId == 0)
            {
                ViewBag.StreetId = 0;
                MP = musorPloshadkas;
            }
            else
            {
                ViewBag.StreetId = StreetId;
               
              // MP =  musorPloshadkas.Where(x => x.VseUlici.Where(y => y.Id == StreetId).First() != null).ToList();
              foreach(MusorPloshadka P in musorPloshadkas)
                {
                    foreach(AllStreet S in P.VseUlici)
                    {
                        if(S.Id==StreetId)
                        {
                            MP.Add(P);
                            break;
                        }
                    }
                }
              
            }
            return View(MP);
        }
      

        // GET: MusorPloshadkas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MusorPloshadka musorPloshadka = db.MusorPloshadkas.Find(id);
            if (musorPloshadka == null)
            {
                return HttpNotFound();
            }
            return View(musorPloshadka);
        }

        // GET: MusorPloshadkas/Create
        public ActionResult Create()
        {
            ViewBag.StreetId = new SelectList(db.AllStreets, "Id", "Name");
            return View();
        }

        // POST: MusorPloshadkas/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStreet([Bind(Include = "StreetId,Id")] MusorPloshadka musorPloshadka)
        {
            string data = "";
            MusorPloshadka MP = new MusorPloshadka();
            try
            {
                MP = db.MusorPloshadkas.Where(x => x.Id == musorPloshadka.Id).First();
                MP.StreetId += ";" + musorPloshadka.StreetId;
                db.Entry(MP).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ReObiem(decimal Value = 0, int Day=0, int Id = 0)
        {//день, размер и ид площадки
            string Data = "";
            MusorPloshadka MP = new MusorPloshadka();
            string[] S = new string[7] { "0", "0", "0", "0", "0", "0", "0" };
            try
            {
                MP = db.MusorPloshadkas.Where(x => x.Id == Id).First();
                string[] SS = MP.Obiem.Split(';');
                for (int i = 0; i < SS.Length; i++)
                {
                    S[i] = SS[i];
                }
                S[Day] = Value.ToString();
                string result = "";
                for (int i=0;i<7;i++)
                {
                    result += S[i]+";";
                }
                MP.Obiem = result.Remove(result.Length - 1, 1);
                db.Entry(MP).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return Json(Data);
        }


        [HttpPost]
        public ActionResult AddPloshadka(bool TKO = true,string MKD="", string Obiem = "0;0;0;0;0;0;0", string UL = "", string StreetId = "", int Id = 0)
        {//день, размер и ид площадки
            string Data = "";
            MusorPloshadka MP = new MusorPloshadka();
           
            try
            {
               
                db.Entry(MP).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return Json(Data);
        }

        [HttpPost]
        public ActionResult DeleteStreet(int StreetId=0, int Id=0)
        {
            
            if (StreetId != 0 && Id != 0)
            {
                MusorPloshadka MP = new MusorPloshadka();
                try
                {
                    MP = db.MusorPloshadkas.Where(x => x.Id == Id).First();
                    string[] S = MP.StreetId.Split(';');
                    string newStreets = "";
                    foreach (string ss in S)
                    {
                        if (ss.Equals(StreetId.ToString()) == false)
                        {
                            newStreets += ss + ";";
                        }
                    }
                    newStreets = newStreets.Remove(newStreets.Length - 1, 1);
                    MP.StreetId = newStreets;
                    db.Entry(MP).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch
                {

                }
            }
            return RedirectToAction("Index",StreetId=0);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,StreetId,Obiem,IDPloshadki,NameUL,UL,TKO")] MusorPloshadka musorPloshadka)
        {
            if (ModelState.IsValid)
            {
                db.MusorPloshadkas.Add(musorPloshadka);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StreetId = new SelectList(db.AllStreets, "Id", "Name", musorPloshadka.StreetId);
            return View(musorPloshadka);
        }

        // GET: MusorPloshadkas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MusorPloshadka musorPloshadka = db.MusorPloshadkas.Find(id);
            if (musorPloshadka == null)
            {
                return HttpNotFound();
            }
            ViewBag.StreetId = new SelectList(db.AllStreets, "Id", "Name", musorPloshadka.StreetId);
            return View(musorPloshadka);
        }

        // POST: MusorPloshadkas/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,StreetId,Obiem,IDPloshadki,NameUL,UL,TKO")] MusorPloshadka musorPloshadka)
        {
            if (ModelState.IsValid)
            {
                db.Entry(musorPloshadka).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StreetId = new SelectList(db.AllStreets, "Id", "Name", musorPloshadka.StreetId);
            return View(musorPloshadka);
        }

        // GET: MusorPloshadkas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MusorPloshadka musorPloshadka = db.MusorPloshadkas.Find(id);
            if (musorPloshadka == null)
            {
                return HttpNotFound();
            }
            return View(musorPloshadka);
        }

        // POST: MusorPloshadkas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MusorPloshadka musorPloshadka = db.MusorPloshadkas.Find(id);
            db.MusorPloshadkas.Remove(musorPloshadka);
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
