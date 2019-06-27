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
    public class ArendatorsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Arendators
        public ActionResult Index()
        {
            List<List<string>> ArendatorKI = new List<List<string>>();
            List<string> Head = new List<string>();
       
            List<DateTime> dates = db.Arendators.Select(x => x.Date).Distinct().ToList();//даты загрузок без повторений
            foreach (DateTime D in dates)
            {
                Head.Add(Opr.MonthOpred(D.Month) + " " + D.Year.ToString());
                for (int i = 0;i< db.Arendators.Where(y => y.Date == D).Count();i++)
                {
                    ArendatorKI.Add(db.Arendators.Where(y => y.Date == D).Include(s => s.Adres).Select(z => z.Adres.Adress + z.Name+" S=" + z.Ploshad).Distinct().ToList());
                }
            }



            ViewBag.Head = Head;
            ViewBag.ArendatorKI = ArendatorKI;
            return View();
        }

        // GET: Arendators/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arendator arendator = db.Arendators.Find(id);
            if (arendator == null)
            {
                return HttpNotFound();
            }
            return View(arendator);
        }

        // GET: Arendators/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            return View();
        }

        // POST: Arendators/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,Name,Ploshad,Teplota,Teplota12,HotWater,ColdWater,Date")] Arendator arendator)
        {
            if (ModelState.IsValid)
            {
                db.Arendators.Add(arendator);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", arendator.AdresId);
            return View(arendator);
        }

        // GET: Arendators/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arendator arendator = db.Arendators.Find(id);
            if (arendator == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", arendator.AdresId);
            return View(arendator);
        }

        // POST: Arendators/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,Name,Ploshad,Teplota,Teplota12,HotWater,ColdWater,Date")] Arendator arendator)
        {
            if (ModelState.IsValid)
            {
                db.Entry(arendator).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", arendator.AdresId);
            return View(arendator);
        }

        // GET: Arendators/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arendator arendator = db.Arendators.Find(id);
            if (arendator == null)
            {
                return HttpNotFound();
            }
            return View(arendator);
        }

        public ActionResult PoiskArendator(DateTime date)
        {
            //ищем все данные за этот месяц, если они есть выводим предупреждение что уже есть данные и они удалятся если сюда грузить свн
            int dbArendators = db.Arendators.Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month).Count();
            return Json(dbArendators);
        }
        // POST: Arendators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Arendator arendator = db.Arendators.Find(id);
            db.Arendators.Remove(arendator);
            db.SaveChanges();
            return RedirectToAction("Index");
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
                List<Arendator> dbArendator = db.Arendators.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList();


                pro100 = dbArendator.Count;
                foreach (Arendator S in dbArendator)
                {
                    try
                    {
                        db.Arendators.Remove(S);
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

                // Установить значения в нем
                cookie["Download"] = "0";
                // Добавить куки в ответ
                Response.Cookies.Add(cookie);




                //call this method inside your working action
                ProgressHub.SendMessage("Инициализация и подготовка...", 0);

                // получаем имя файла
                string fileName = System.IO.Path.GetFileName(upload.FileName);
                // сохраняем файл в папку Files в проекте
                if (Directory.Exists(Server.MapPath("~/Files/")) == false)
                {
                    Directory.CreateDirectory(Server.MapPath("~/Files/"));

                }
                upload.SaveAs(Server.MapPath("~/Files/" + fileName));
                //обрабатываем файл после загрузки



                string OpenText = Server.MapPath("~/Files/" + fileName);
              
                string Vkladka = Date.Month.ToString();//укажем номер вкладки
                List<List<string>> Dannie;

                int[] X = new int[] {1,2,3,4,6,8,10};
                string[] Names = new string[] { "адрес", "№дома", "теплотаотопл,гкалфактич", "площадь", "теплота1/12(гкал,)", "гвкуб,м", "хвкуб,м" };
                List<List<string>> excel = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName), Names,Vkladka,X);
                if (excel.Count < 1)
                {
                    //если нифига не загрузилось то 
                    Console.WriteLine("Пустой массив значит файл не загрузился!(он уже удалился)");
                    return View("NotUpload");
                }
                

                    string HomeAdress;
                string ADRESS = "";
                string CODE = "";
             
                for (int i = 0; i < excel.Count; i++)
                {
                    Arendator Ar = new Arendator();
                    bool swsave = false;
                    // for (int j = 0; j < Dannie[i].Count; j++)
                    //{
                    if (excel[i][1] != null)
                    {
                        excel[i][1] = excel[i][1].Replace(" ", "");
                    }
                    if (excel[i][1] != null && excel[i][1] != "")//Когда дошли до номера дома значит это дом и в нем есть арендаторы
                    {
                        string adr = excel[i][0].ToString();
                        adr = adr.ToUpper();
                        adr = adr.Replace("ПР.", "");
                        adr = adr.Replace("М.ДЖАЛИЛЯ", "МУСЫ ДЖАЛИЛЯ");
                        if (adr.Equals("ШЛЮЗОВАЯ")) { }
                        int ii = adr.IndexOf(" ");
                        if (ii == adr.Length - 1) { adr = adr.Replace(" ", ""); }
                        if (ii < adr.Length - 1 && ii != -1)
                        {
                            //  adr = adr.Replace(" ", "");
                            adr = adr.Replace(",", "");
                            string s1 = adr.Remove(ii);
                            s1 = s1.Replace(",", "");
                            s1 = s1.Replace(" ", "");

                            string s2 = adr.Remove(0, ii);
                            s2 = s2.Replace(",", "");

                            s2 = s2.Replace(" ", "");

                            adr = s1 + " " + s2;

                        }

                        string adr2 = excel[i][1].ToString();
                        adr += " " + adr2;//сохраняем в формате [ХХХХ 123]
                        adr = adr.ToUpper();
                        adr = adr.Replace(",", "").Replace(" ","");
                        
                        int AdrId = 0;
                        try
                        {
                           AdrId =  db.Adres.Where(x => x.Adress.Equals(adr)).Select(y => y.Id).Single();//сохранили адрес
                        }
                        catch { continue; }//если адрес не нашли то пропустим данный шаг в цикле
                        
                        Ar.AdresId = AdrId;//пишем в адрес
                        Ar.Date = Date;
                        int j = i+1;
                        
                        while (excel[j][1] == "0"&&j<excel.Count)
                        {
                            try { Ar.Name = excel[j][0]; } catch { }
                            try { Ar.Teplota = Convert.ToDecimal(excel[j][2]); } catch { }
                            try { Ar.Ploshad = Convert.ToDecimal(excel[j][3]); } catch { }
                            try { Ar.Teplota12 = Convert.ToDecimal(excel[j][4]); } catch { }
                            try { Ar.HotWater = Convert.ToDecimal(excel[j][5]); } catch { }
                            try { Ar.ColdWater = Convert.ToDecimal(excel[j][6]); } catch { }
                            db.Arendators.Add(Ar);
                            db.SaveChanges();
                            j++;
                        }
                        i = j-1;
                    }
                  
                   
                }
            





                if (excel.Count < 1)
                {
                    //если нифига не загрузилось то 
                    Console.WriteLine("Пустой массив значит файл не загрузился!(он уже удалился)");
                    return View("NotUpload");
                }
                else
                {
                    pro100 = excel.Count;
                    Arendator ArendatorKA = new Arendator();
                    List<Adres> Adresa = db.Adres.ToList();// грузим все адреса из БД
                    
                    List<TableService> TSdb = db.TableServices.ToList();
                    List<List<string>> Services = new List<List<string>>();
                    //один раз преобразуем таблицу сервисов для сравнения чтоб в цикле не вызывать
                    int ser = 0;
                    foreach (TableService T in TSdb)
                    {
                        
                        T.Type = T.Type.Replace(" ", "").ToUpper();
                        Services.Add(new List<string>());
                        Services[ser].Add(T.Type);//для проверки сохраняем
                        ser++;

                    }
                    //для каждой строки в экселе

                    List<string> Adr = Adresa.Select(x=>x.Adress).ToList();
                    for (int a=0; a<Adr.Count;a++)
                    {

                        Adr[a] = Adr[a].Replace(" ", "").ToUpper();
                    }
                   

                    ViewBag.VsegoServices = TSdb.Count;
                    ViewBag.Services = Services;
                    ViewBag.date = Date;
                    ViewBag.file = fileName;

                  

                    return View("UploadComplete");
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult UploadComplete ()
        {

            return View();
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
