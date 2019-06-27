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
    public class SVNsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: SVNs
        public ActionResult Index()
        {
            List<List<string>> SVNKI = new List<List<string>>();
            List<string> Head = new List<string>();
            List<TableService> Services = db.TableServices.ToList();
            List<DateTime> dates = db.SVNs.Select(x => x.Date).Distinct().ToList();//даты загрузок без повторений
            foreach (DateTime D in dates)
            {
                Head.Add(Opr.MonthOpred(D.Month) +" "+ D.Year.ToString());
                foreach (TableService T in Services)
                {
                    SVNKI.Add(db.SVNs.Where(y => y.Date == D).Include(s => s.Adres).Include(g=>g.Service).Where(h=>h.Service.Id==T.Id).Select(z => z.Adres.Adress +" F="+ z.Fact+" P="+ z.Plan).Distinct().ToList());
                }
            }



            ViewBag.Head = Head;
            ViewBag.SVNKI = SVNKI;
            return View();
        }


        public ActionResult IndexMain()
        {
            //старый индексный файл где можно добавить запись и изменить ее
            var sVNs = db.SVNs.Include(s => s.Adres).Include(s => s.Service);

            return View(sVNs.ToList());
        }

        // GET: SVNs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SVN sVN = db.SVNs.Find(id);
            if (sVN == null)
            {
                return HttpNotFound();
            }
            return View(sVN);
        }

        public ActionResult PoiskSVN(DateTime date)
        {
            //ищем все данные за этот месяц, если они есть выводим предупреждение что уже есть данные и они удалятся если сюда грузить свн
            int dbSVN = db.SVNs.Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month).Count();
            return Json(dbSVN);
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
                List<SVN> dbSVN = db.SVNs.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList();


                pro100 = dbSVN.Count;
                foreach (SVN S in dbSVN)
                {
                    try
                    {
                        db.SVNs.Remove(S);
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
               
              

                string[] Names = new string[] { "STREET_HOUSE", "SERVICE", "DELIVER", "CHARGE_PLAN", "CHARGE_FACT", "MAKET" };
                List<List<string>> excel = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName),Names);
                if (excel.Count < 1)
                {
                    //если нифига не загрузилось то 
                    Console.WriteLine("Пустой массив значит файл не загрузился!(он уже удалился)");
                    return View("NotUpload");
                }
                else
                {
                    pro100 = excel.Count;
                    SVN SVNKA = new SVN();
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
                    foreach (List<string> L in excel)
                    {
                        string Service = L[1].Replace(" ", "").ToUpper();
                        
                        bool EstService = false;
                       
                        foreach(TableService TS in TSdb)
                        {
                           
                            if (TS.Type.Equals(Service))
                            {
                                EstService = true;
                                SVNKA.ServiceId = TS.Id;
                                ser = TS.Id-1;//номер сервиса по порядку с 0
                                break;
                            }
                           
                        }
                        //если сервис не найден в списке то и адрес не проверяем идем дальше
                        if (EstService)
                        {
                            bool EstName = false;
                            string Name = L[0].Replace(" ", "");
                            foreach (Adres A in Adresa)
                            {
                                string AName = A.Adress.Replace(" ", "");
                                if (AName.Equals(Name))
                                {
                                    //если в массиве адресов есть адрес из строчки то сохраняем айдишник
                                    EstName = true;
                                    SVNKA.AdresId = A.Id;
                                    if (ser < 4)
                                    {
                                        Services[ser].Add(Name);
                                    }
                                   // Adresa.Remove(A);//уменьшаем массив для дальнейшего ускорения поиска
                                    break;
                                }
                            }
                            //если имени нет в списке то и сохранять не будем
                            if (EstName)
                            {
                                try
                                {
                                    SVNKA.Plan = Convert.ToDecimal(L[3]);
                                    SVNKA.Fact = Convert.ToDecimal(L[4]);
                                    SVNKA.Maket = Convert.ToDecimal(L[5]);
                                    SVNKA.Agent = L[2];
                                    SVNKA.Date = Date;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Не преобразуется в децимал "+ SVNKA.AdresId+ " " +e.Message);
                                }

                                try
                                {
                                    db.SVNs.Add(SVNKA);
                                    db.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Ошибка записи в базу данных "+e.Message);
                                }
                            }
                        }
                        procount++;
                        progress = Convert.ToInt16(50+procount / pro100 * 50);
                        ProgressHub.SendMessage("Обрабатываем файл СВН...", progress);
                        if (procount > pro100) { procount = Convert.ToInt32(pro100); }

                    }
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

        // GET: SVNs/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            ViewBag.ServiceId = new SelectList(db.TableServices, "Id", "Type");
            return View();
        }

        // POST: SVNs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,Agent,ServiceId,Fact,Plan,Maket")] SVN sVN)
        {
            if (ModelState.IsValid)
            {
                db.SVNs.Add(sVN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", sVN.AdresId);
            ViewBag.ServiceId = new SelectList(db.TableServices, "Id", "Type", sVN.ServiceId);
            return View(sVN);
        }

        // GET: SVNs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SVN sVN = db.SVNs.Find(id);
            if (sVN == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", sVN.AdresId);
            ViewBag.ServiceId = new SelectList(db.TableServices, "Id", "Type", sVN.ServiceId);
            return View(sVN);
        }

        // POST: SVNs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,Agent,ServiceId,Fact,Plan,Maket")] SVN sVN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sVN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", sVN.AdresId);
            ViewBag.ServiceId = new SelectList(db.TableServices, "Id", "Type", sVN.ServiceId);
            return View(sVN);
        }

        // GET: SVNs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SVN sVN = db.SVNs.Find(id);
            if (sVN == null)
            {
                return HttpNotFound();
            }
            return View(sVN);
        }

        // POST: SVNs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SVN sVN = db.SVNs.Find(id);
            db.SVNs.Remove(sVN);
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
