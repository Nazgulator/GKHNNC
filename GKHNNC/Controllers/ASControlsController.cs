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
    public class ASControlsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: ASControls
        public ActionResult Index()
        {
            DateTime Date = DateTime.Now;
            ViewBag.Date = Date;

            //берем автомобили вышедшие в рейс
             List<int> ASId = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).Select(y=>y.Avto.Id).ToList();
              List<Avtomobil> Avtos = new List<Avtomobil>();
              foreach (int i in ASId)
              {
                  Avtos.Add(db.Avtomobils.Where(x => x.Id == i).First());
              }

            //берем все записи контрола за этот день и пробиваем по базе АС24
            List<ASControl> ASC = new List<ASControl>();
            try
            {
                ASC = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).Include(x => x.Avto).ToList();
            }
            catch(Exception e)
            {            }
            //берем все записи ас24 за день
            List<AS24> AS24db = new List<AS24>();
            try
            {
               
               AS24db = db.AS24.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).ToList();

            }
            catch { }
            //пробиваем каждую запись и суммируем километраж и ДУТ
            List<int> Nabludenii = new List<int>();
            foreach (ASControl AC in ASC)
            {
                int counter = 0;
                decimal KMAS = 0;
                decimal DUT = 0;
                List<AS24> A24 = new List<AS24>();
                try
                {//берем все записи с данной тачкой
                    A24 = AS24db.Where(x => x.AvtoId == AC.AvtoId&&x.Date.Hour>=AC.Date.Hour).ToList();
                }
                catch { }
                foreach (AS24 A in A24)
                {
                    KMAS += A.KM;
                    DUT += A.DUT;
                    counter++;
                }
                Nabludenii.Add(counter);
                AC.KMAS = KMAS;
                AC.DUT = DUT;

            }

            //Берем все не закрытые записи за предыдущие дни. Километраж и ДУТ автоматически взят из автоскана при ночном обновлении.
            List<ASControl> ASCOld = new List<ASControl>();
            try
            {
                ASCOld = db.ASControls.Where(x => x.DateClose<x.Date&&x.Date.Day!=Date.Day).Include(x => x.Avto).OrderBy(x=>x.Date).ToList();
            }
            catch { }


            
            ViewBag.Counter = ASC.Count();//сохраняем количество записей за текущий день.
            ASC.AddRange(ASCOld);//добавляем в конец списка все не закрытые записи
            ViewBag.Nabludenii = Nabludenii;//массив с числом наблюдений по часам
            ViewBag.Avto = db.Avtomobils.OrderBy(x => x.Number).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Number, }).ToList(); 
            return View(ASC);
        }


        // GET: ASControls
        public ActionResult AvtoWarnings()
        {
            DateTime Date = DateTime.Now;
            ViewBag.Date = Date;



            //берем все записи контрола за этот день и пробиваем по базе АС24
            List<ASControl> ASC = new List<ASControl>();
            try
            {
                ASC = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).Include(x => x.Avto).ToList();
            }
            catch (Exception e)
            { }
            //берем все записи ас24 за день
            List<AS24> AS24db = new List<AS24>();
            try
            {

                AS24db = db.AS24.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).ToList();

            }
            catch { }
            //пробиваем каждую запись и суммируем километраж и ДУТ
            List<int> Nabludenii = new List<int>();
            foreach (ASControl AC in ASC)
            {
                int counter = 0;
                decimal KMAS = 0;
                decimal DUT = 0;
                List<AS24> A24 = new List<AS24>();
                try
                {//берем все записи с данной тачкой
                    A24 = AS24db.Where(x => x.AvtoId == AC.AvtoId && x.Date.Hour >= AC.Date.Hour).ToList();
                }
                catch { }
                foreach (AS24 A in A24)
                {
                    KMAS += A.KM;
                    DUT += A.DUT;
                    counter++;
                }

                    Nabludenii.Add(counter);
                    AC.KMAS = KMAS;
                    AC.DUT = DUT;
              

            }

            //Берем все не закрытые записи за предыдущие дни. Километраж и ДУТ автоматически взят из автоскана при ночном обновлении.
            List<ASControl> ASCOld = new List<ASControl>();
            try
            {
                ASCOld = db.ASControls.Where(x =>  x.Date.Year==Date.Year&&x.Date.Month == Date.Month&& x.Warning==true).Include(x => x.Avto).OrderBy(x => x.Date).ToList();
            }
            catch { }



            ViewBag.Counter = ASC.Count();//сохраняем количество записей за текущий день.
            ASC.AddRange(ASCOld);//добавляем в конец списка все не закрытые записи
            ViewBag.Nabludenii = Nabludenii;//массив с числом наблюдений по часам
            ViewBag.Avto = db.Avtomobils.OrderBy(x => x.Number).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Number, }).ToList();
            return View(ASC);
        }



        // GET: ASControls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ASControl aSControl = db.ASControls.Find(id);
            if (aSControl == null)
            {
                return HttpNotFound();
            }
            return View(aSControl);
        }

        // GET: ASControls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ASControls/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AvtoId,Name,Date,Go,Primech,KMAS,KM,DUT,Start,End,Zapravleno,Sliv")] ASControl aSControl)
        {
            if (ModelState.IsValid)
            {
                db.ASControls.Add(aSControl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aSControl);
        }
        //добавление нового автомобиля скриптом на индексе
        [HttpPost]
        public ActionResult AddAvto(string selection)
        {
            string[] S = selection.Split(';');
            int AvtoId = Convert.ToInt32(S[0]);

            ASControl ASC = new ASControl();
            Avtomobil Avto = db.Avtomobils.Where(a => a.Id == AvtoId).First();

            ASC.Go = true;
            ASC.KM = 0;
            ASC.Primech = S[1];
            ASC.Sliv = 0;
            ASC.Start = 0;
            ASC.Zapravleno = 0;
            ASC.KMAS = 0;
            ASC.AvtoId = AvtoId;
            ASC.Date = DateTime.Now;
            ASC.DateClose = new DateTime(2001, 1, 1, 0, 0, 0);
            

            string Data = "";

            try
            {
                db.ASControls.Add(ASC);
                db.SaveChanges();
                Data = "Успешно добавлена";
            }
            catch (Exception e)
            {
                Data = "Неудача";
            }

            return Json(Data);
        }


        [HttpPost]
        public ActionResult CloseAvto(string selection)
        {
            string[] S = selection.Split(';');
            int Id = Convert.ToInt32(S[0]);
            //если адекватно написаны километры то сохраняем иначе 0
            decimal KM = 0;
            try
            {
                KM = Convert.ToDecimal(S[1]);
            }
            catch
            {

            }
            
            
            ASControl ASC = db.ASControls.Where(a => a.Id == Id).First();

            
            ASC.KM = KM;
            ASC.Primech = S[2];
            ASC.DateClose = DateTime.Now;
            string Data = "";
            //если не вбит пробег то возврат обратно
            
            if (KM == 0) { Data = "Чтобы закрыть выезд, введите километраж автомобиля (записанный водителем в путёвке) в соответствующее поле. Километраж точно больше нуля!"; return Json(Data); }
            try
            {
                db.Entry(ASC).State = EntityState.Modified;
                db.SaveChanges();
                Data = "";
            }
            catch (Exception e)
            {
                Data = "Неудача";
            }

            return Json(Data);
        }


        // GET: ASControls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ASControl aSControl = db.ASControls.Find(id);
            if (aSControl == null)
            {
                return HttpNotFound();
            }
            return View(aSControl);
        }

        // POST: ASControls/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AvtoId,Name,Date,Go,Primech,KMAS,KM,DUT,Start,End,Zapravleno,Sliv")] ASControl aSControl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aSControl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aSControl);
        }

        // GET: ASControls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ASControl aSControl = db.ASControls.Find(id);
            if (aSControl == null)
            {
                return HttpNotFound();
            }
            return View(aSControl);
        }

        // POST: ASControls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ASControl aSControl = db.ASControls.Find(id);
            db.ASControls.Remove(aSControl);
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