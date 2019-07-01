using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GKHNNC.DAL;
using GKHNNC.Models;
using Opredelenie;

namespace GKHNNC.Controllers
{
    public class HousesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Houses
        public ActionResult Index()
        {
            List<House> H = new List<House>();

            List<Adres> houses = db.Adres.OrderBy(x=>x.Adress).ToList();
            DateTime Date = new DateTime(DateTime.Now.Year,DateTime.Now.Month-1,1);//берем прошлый месяц
            List<string> Primechanie = new List<string>();
            List<Arendator> Arendators = db.Arendators.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month).ToList();//Берем всех арендаторов за текущий месяц
            List<UEV> Uevs = db.UEVs.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month).ToList();
            List<OPU> Opus = db.OPUs.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month).ToList();
            foreach (Adres a in houses)
            {
                
                House ho = new House();
               List<Arendator> TekArend = Arendators.Where(d => d.AdresId == a.Id).ToList();//арендаторы в данном доме для ускорения поиска
                List<UEV> TekUevs = Uevs.Where(d => d.AdresId == a.Id).ToList();//выставлено в УЭВ применим позже
                List<OPU> TekOpus = Opus.Where(d => d.AdresId == a.Id).ToList();//Фактические затраты воды по ОПУ андрей Исх
                ho.AdresId = a.Id;
                ho.Adres = a.Adress;
                ho.Ploshad = a.Ploshad;//общая площадь
                ho.Teplota = TekOpus.Sum(e => e.OtopGkal);//TekUevs.Sum(e => e.OtEnergyGkal);//Сумма теплоты 
                ho.Teplota12 = 0;
                ho.HotWater = TekOpus.Sum(e => e.GWM3);//Сумма Горводы
                ho.ColdWater = TekOpus.Sum(e => e.HWM3);//Сумма Холводы
                ho.PloshadArendators = TekArend.Sum(e => e.Ploshad);//Сумма площадей арендаторов
                ho.TeplotaArendators = TekArend.Sum(e => e.Teplota);//Сумма теплоты арендаторов
                ho.Teplota12Arendators = TekArend.Sum(e => e.Teplota12);//Сумма теплоты 1/12 арендаторов
                ho.ColdWaterArendators = TekArend.Sum(e => e.ColdWater);//Сумма Холодной воды арендаторов
                ho.HotWaterArendators = TekArend.Sum(e => e.HotWater);//Сумма Горячей воды арендаторов
                ho.Date = Date;
                H.Add(ho);
            }
            return View(H);
        }

        // GET: Houses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adres adr = db.Adres.Find(id);
            if (adr == null)
            {
                return HttpNotFound();
            }
            Adres Adr = db.Adres.Where(c => c.Id == id).Single();
            List<DateTime> SelectDate = new List<DateTime>();//массив для выбора минимальной из максимальных дат по которой все и будем считать
            try { SelectDate.Add(db.CompleteWorks.Where(d => d.WorkDate == db.CompleteWorks.Max(x => x.WorkDate)).Select(c => c.WorkDate).First()); } catch { }//Выбираем макс дату из комплит воркс
            try { SelectDate.Add(db.Arendators.Where(c => c.Date == db.Arendators.Max(x => x.Date) && c.AdresId == id).Select(d => d.Date).First()); }catch{ }
            try { SelectDate.Add(db.UEVs.Where(c => c.AdresId == id && c.Date == db.UEVs.Max(x => x.Date)).Select(d => d.Date).First()); } catch { }
            try { SelectDate.Add(db.OPUs.Where(c => c.Date == db.OPUs.Max(x => x.Date) && c.AdresId == id).Select(d => d.Date).First()); } catch { }
            try { SelectDate.Add(db.VipolnennieUslugis.Where(c => c.Date == db.VipolnennieUslugis.Max(x => x.Date) && c.AdresId == id).Select(d => d.Date).First()); } catch { }
            DateTime MaxDate = SelectDate.Where(c => c.Date == SelectDate.Min(x => x.Date)).First();

            DateTime Date = new DateTime(MaxDate.Year,MaxDate.Month,1);//берем минимально максимальную дату //DateTime.Now.Year, DateTime.Now.Month - 1, 1);//берем прошлый месяц
            List<Arendator> Arendators = db.Arendators.Where(c => c.Date.Year == Date.Year&&c.Date.Month == Date.Month&& c.AdresId==id).ToList();//Берем выбранный дом и ищем в нем арендаторов
            List<UEV> Uevs = db.UEVs.Where(c => c.AdresId == id&& c.Date.Year == Date.Year && c.Date.Month == Date.Month).ToList();
            List<OPU> Opus = db.OPUs.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month && c.AdresId == id).ToList();
            
            List<string> Works = new List<string>();//ищем работы
            List<CompleteWork> CW = new List<CompleteWork>();
            try
            {
                
               CW= db.CompleteWorks.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month && c.WorkAdress.Replace(" ", "").Equals(Adr.Adress)).ToList();//.Select(d => d.WorkName + " " + d.WorkNumber.ToString())
            }
            catch { }
            foreach (CompleteWork CCW in CW)
            {
                Works.Add(CCW.WorkName + " " + CCW.WorkNumber.ToString() + " " + CCW.WorkIzmerenie);
            }
            List<string> Uslugis = new List<string>();//ищем услуги
            List<string> UslugisCost = new List<string>();//ищем услуги
            try
            {
                Uslugis = db.VipolnennieUslugis.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month && c.AdresId == id).Include(e => e.Usluga).Select(d => d.Usluga.Name).ToList();
                UslugisCost = db.VipolnennieUslugis.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month && c.AdresId == id).Include(e => e.Usluga).Select(d => d.StoimostNaMonth.ToString() + " руб.").ToList();
            }
            catch { }
           
                House ho = new House();
            ho.AdresId = id;
            ho.Adres = adr.Ulica +" "+ adr.Dom;
            ho.Ploshad = adr.Ploshad;//пока не знаем общую площадь
                ho.Teplota = Opus.Sum(e => e.OtopGkal);//Сумма теплоты
                ho.Teplota12 = adr.Teplota12;//пока не знаем общую площадь
                ho.HotWater = Opus.Sum(e => e.GWM3);//сумма ГВ
                ho.ColdWater = Opus.Sum(e => e.HWM3);//сумма ХВ
                ho.PloshadArendators = Arendators.Sum(e => e.Ploshad);//Сумма площадей арендаторов
                ho.TeplotaArendators = Arendators.Sum(e => e.Teplota);//Сумма теплоты арендаторов
                ho.Teplota12Arendators = Arendators.Sum(e => e.Teplota12);//Сумма теплоты 1/12 арендаторов
                ho.ColdWaterArendators = Arendators.Sum(e => e.ColdWater);//Сумма Холодной воды арендаторов
                ho.HotWaterArendators = Arendators.Sum(e => e.HotWater);//Сумма Горячей воды арендаторов
                ho.Date =  Date;//берем макс дату (Она единственная для всех)
            ViewBag.Date = Opr.MonthOpred(Date.Month) + " " + Date.Year.ToString();
            ViewBag.Arendators = Arendators.Select(x => x.Name).ToList();
            ViewBag.Works = Works;
            ViewBag.Uslugis = Uslugis;
            ViewBag.UslugisCost = UslugisCost;
            string prim = "ОК";
            try
            {
               prim = Opus.Select(c => c.Primech).First();
            }
            catch { }
            ViewBag.Primechanie = prim;


            return View(ho);
        }

        // POST: Houses/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,Ploshad,PloshadArendators,Teplota,TeplotaArendators,Teplota12,Teplota12Arendators,HotWater,HotWaterArendators,ColdWater,ColdWaterArendators,Date")] House house)
        {
            if (ModelState.IsValid)
            {
                db.Entry(house).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", house.AdresId);
            return View(house);
        }

        // GET: Houses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = db.Houses.Find(id);
            if (house == null)
            {
                return HttpNotFound();
            }
            return View(house);
        }

        // POST: Houses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            House house = db.Houses.Find(id);
            db.Houses.Remove(house);
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
