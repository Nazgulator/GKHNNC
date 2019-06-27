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
using Microsoft.AspNet.Identity;
using System.Web.Helpers;
using GKHNNC.Utilites;
using System;
using System.IO;
using Opredelenie;
using System.Collections;
using Microsoft.AspNet.SignalR;
namespace GKHNNC.Controllers
{
    public class UEVsController : Controller
    {
        private WorkContext db = new WorkContext();


        public ActionResult Index()
        {
            List<List<string>> UEVKI = new List<List<string>>();
            List<string> Head = new List<string>();
            List<DateTime> dates = db.UEVs.Select(x => x.Date).Distinct().ToList();//даты загрузок без повторений
            foreach (DateTime D in dates)
            {
                Head.Add(Opr.MonthOpred(D.Month) + " " + D.Year.ToString());
                //доделать вывод по файлам
                    UEVKI.Add(db.UEVs.Where(y => y.Date == D).Include(s => s.Adres).Select(z => z.Adres.Adress + " Отопление Гкал.=" + z.OtEnergyGkal + " Вода Гкал.=" + z.HwEnergyGkal + " ВодаМ3=" + z.HwVodaM3).Distinct().ToList());

            }



            ViewBag.Head = Head;
            ViewBag.UEVKI = UEVKI;
            return View();
        }

        // GET: UEVs
        public ActionResult IndexMain()
        {
            var uEVs = db.UEVs.Include(u => u.Adres);
            return View(uEVs.ToList());
        }

        // GET: UEVs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UEV uEV = db.UEVs.Find(id);
            if (uEV == null)
            {
                return HttpNotFound();
            }
            return View(uEV);
        }

        // GET: UEVs/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            return View();
        }

        // POST: UEVs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,Name,KodUEV,Pribor,OtEnergyRub,OtEnergyGkal,HwEnergyRub,HwEnergyGkal,HwVodaRub,HwVodaM3,Date")] UEV uEV)
        {
            if (ModelState.IsValid)
            {
                db.UEVs.Add(uEV);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", uEV.AdresId);
            return View(uEV);
        }

        // GET: UEVs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UEV uEV = db.UEVs.Find(id);
            if (uEV == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", uEV.AdresId);
            return View(uEV);
        }

        // POST: UEVs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,Name,KodUEV,Pribor,OtEnergyRub,OtEnergyGkal,HwEnergyRub,HwEnergyGkal,HwVodaRub,HwVodaM3,Date")] UEV uEV)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uEV).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", uEV.AdresId);
            return View(uEV);
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }
        public ActionResult NotUpload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, DateTime Date)
        {
            int progress = 0;
            double pro100 = 0;
            int procount = 0;
            if (upload != null)
            {
                HttpCookie cookie = new HttpCookie("My localhost cookie");

                //найдем старые данные за этот месяц и заменим их не щадя
                List<UEV> dbUEV = db.UEVs.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList();
                pro100 = dbUEV.Count;
                foreach (UEV S in dbUEV)
                {
                    try
                    {
                        db.UEVs.Remove(S);
                        db.SaveChanges();
                        procount++;
                        progress = Convert.ToInt16(procount / pro100 * 100);
                        if (procount > pro100) { procount = Convert.ToInt32(pro100); }
                        ProgressHub.SendMessage("Удаляем старые данные...", progress);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }


                }

                //call this method inside your working action
                ProgressHub.SendMessage("Инициализация и подготовка...", 0);

                // получаем имя файла
                string fileName = Path.GetFileName(upload.FileName);
                // сохраняем файл в папку Files в проекте
                if (Directory.Exists(Server.MapPath("~/Files/")) == false)
                {
                    Directory.CreateDirectory(Server.MapPath("~/Files/"));

                }
                upload.SaveAs(Server.MapPath("~/Files/" + fileName));
                
                //обрабатываем файл после загрузки
                
                string[] Names = new string[] { "Kp", "№прибора", "Тариф", "Общийотпуск", "ИтогосуммасНДСруб.", "Тариф", "Общийотпуск", "ИтогосуммасНДСруб.", "Тариф", "Общийотпуск", "ИтогосуммасНДСруб." };
                List<List<string>> excel = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName), Names);
                if (excel.Count < 1)
                {
                    //если нифига не загрузилось то 
                    Console.WriteLine("Пустой массив значит файл не загрузился!(он уже удалился)");
                    return View("NotUpload");
                }
                else
                {
                    pro100 = excel.Count;
                    UEV UEVKA = new UEV();
                    List<Adres> Adresa = db.Adres.ToList();// грузим все адреса из БД


                    //для каждой строки в экселе
                    foreach (List<string> L in excel)
                    {
                        
                            bool EstName = false;
                        int CodUEV = 0;
                        try
                        {
                            CodUEV= Convert.ToInt32(L[0]);
                        }
                        catch
                        { }
                        if (CodUEV != 0)
                        { 
                            foreach (Adres A in Adresa)
                            {
                                
                                if (A.UEV==CodUEV)
                                {
                                   
                                    //если в массиве адресов есть адрес из строчки то сохраняем айдишник
                                    EstName = true;
                                    UEVKA.AdresId = A.Id;
                                    UEVKA.KodUEV = A.UEV;
                                    UEVKA.Date = Date;
                                    UEVKA.Name = "";
                                    break;
                                }
                            }
                            //если имени нет в списке то и сохранять не будем
                            if (EstName)
                            {
                                int Pribor = 0;
                                decimal OtEnergyGkal = 0;
                                decimal OtEnergyRub = 0;
                                decimal HwEnergyGkal = 0;
                                decimal HwEnergyRub = 0;
                                decimal HwVodaM3 = 0;
                                decimal HwVodaRub = 0;
                                try
                                {
                                    Pribor = Convert.ToInt32(L[1]);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Не преобразуется в инт " + UEVKA.AdresId + " " + e.Message);
                                }
                                try
                                {
                                    OtEnergyGkal = Convert.ToDecimal(L[3]);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Не преобразуется в децимал " + UEVKA.AdresId + " " + e.Message);
                                }
                                try
                                {
                                    OtEnergyRub = Convert.ToDecimal(L[4]);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Не преобразуется в децимал " + UEVKA.AdresId + " " + e.Message);
                                }
                                try
                                {
                                    HwEnergyGkal = Convert.ToDecimal(L[6]);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Не преобразуется в децимал " + UEVKA.AdresId + " " + e.Message);
                                }
                                try
                                {
                                    HwEnergyRub = Convert.ToDecimal(L[7]);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Не преобразуется в децимал " + UEVKA.AdresId + " " + e.Message);
                                }
                                try
                                {
                                    HwVodaM3 = Convert.ToDecimal(L[9]);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Не преобразуется в децимал " + UEVKA.AdresId + " " + e.Message);
                                }
                                try
                                {
                                    HwVodaRub = Convert.ToDecimal(L[10]);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Не преобразуется в децимал " + UEVKA.AdresId + " " + e.Message);
                                }
                                UEVKA.Pribor = Pribor;
                                UEVKA.OtEnergyGkal = OtEnergyGkal;
                                UEVKA.OtEnergyRub = OtEnergyRub;
                                UEVKA.HwEnergyGkal = HwEnergyGkal;
                                UEVKA.HwEnergyRub = HwEnergyRub;
                                UEVKA.HwVodaM3 = HwVodaM3;
                                UEVKA.HwVodaRub = HwVodaRub;


                                try
                                {
                                    db.UEVs.Add(UEVKA);
                                    db.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Ошибка записи в базу данных " + e.Message);
                                }
                            }
                        }
                        procount++;
                        progress = Convert.ToInt16(50 + procount / pro100 * 50);
                        ProgressHub.SendMessage("Обрабатываем файл УЭВ...", progress);
                        if (procount > pro100) { procount = Convert.ToInt32(pro100); }

                    }
                    List<string> Adr = Adresa.Select(x => x.Adress).ToList();
                    for (int a = 0; a < Adr.Count; a++)
                    {

                        Adr[a] = Adr[a].Replace(" ", "").ToUpper();
                    }


                    ViewBag.VsegoUEV = db.UEVs.Where(x => x.Date == Date).Count();
                    //ViewBag.Services = Services;
                    ViewBag.UEV = db.UEVs.Where(x => x.Date == Date).Include(z=>z.Adres.Adress).Select(y => y.Adres.Adress + "ОТ(энергия руб.)=" + y.OtEnergyRub + " ГВ(энергия руб.)=" + y.HwEnergyRub + "ГВ(теплоноситель руб.)=" + y.HwVodaRub).ToList();
                    ViewBag.date = Date;
                    ViewBag.file = fileName;



                    return View("UploadComplete");
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult UploadComplete()
        {

            return View();
        }

        public ActionResult PoiskUEV(DateTime date)
        {
            //ищем все данные за этот месяц, если они есть выводим предупреждение что уже есть данные и они удалятся если сюда грузить UEV
            int dbUEV = db.UEVs.Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month).Count();
            return Json(dbUEV);
        }

        // GET: UEVs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UEV uEV = db.UEVs.Find(id);
            if (uEV == null)
            {
                return HttpNotFound();
            }
            return View(uEV);
        }

        // POST: UEVs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UEV uEV = db.UEVs.Find(id);
            db.UEVs.Remove(uEV);
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
