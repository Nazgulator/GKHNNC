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
using System.Threading;
using System.Threading.Tasks;

namespace GKHNNC.Controllers
{
    public class OBSDsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: OBSDs
        public ActionResult IndexMain()
        {
            var oBSDs = db.OBSDs.Include(o => o.Adres).Include(o => o.TableService);
            return View(oBSDs.ToList());
        }

        // GET: OBSDs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBSD oBSD = db.OBSDs.Find(id);
            if (oBSD == null)
            {
                return HttpNotFound();
            }
            return View(oBSD);
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

        public ActionResult PoiskOBSD(DateTime date)
        {

            //ищем все данные за этот месяц, если они есть выводим предупреждение что уже есть данные и они удалятся если сюда грузить UEV
            int dbOBSD = db.OBSDs.Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month).Count();
            return Json(dbOBSD);
        }

        public ActionResult Index()
        {//Поменять на ОБСД
            List<string> OBSDKI = new List<string>();
            List<string> Head = new List<string>();
            List<DateTime> dates = db.OBSDs.Select(x => x.Date).Distinct().ToList();//даты загрузок без повторений
            foreach (DateTime D in dates)
            {
                Head.Add(Opr.MonthOpred(D.Month) + " " + D.Year.ToString());
                //доделать вывод по файлам
                OBSDKI.Add(db.OBSDs.Where(y => y.Date == D).Count().ToString());

            }



            ViewBag.Head = Head;
            ViewBag.OBSDKI = OBSDKI;
            return View();
        }
        public async void ZapuskPoiska(List<List<string>> excel, DateTime Date, List<Adres> Adresa, List<TableService> Services)
        {
            ProgressHub.SendMessage("Запускаем обработку файла ...", 0);
            await Task.Run(()=>Poisk(excel,Date,Adresa,Services));
            ProgressHub.SendMessage("Файл успешно обработан ...", 100);
        }

        public async void Poisk (List<List<string>> excel, DateTime Date,List<Adres>Adresa,List<TableService>Services)
        {
            int progress = 0;
            double pro100 = 0;
            int procount = 0;
            pro100 = excel.Count;
            OBSD OBSDKA = new OBSD();
          
            //для каждой строки в экселе
            int lastadres = 0;
            foreach (List<string> L in excel)
            {

                bool EstService = false;
                string ser = L[2].Replace(" ", "").ToUpper();
                string adr = L[0].Replace(" ", "").ToUpper();
                decimal Saldo = 0;
                decimal Nach = 0;
                bool ignore = false;
                try
                {
                    Saldo = Convert.ToDecimal(L[5]);

                }
                catch
                { }
                try
                {

                    Nach = Convert.ToDecimal(L[3]);
                }
                catch
                { }

                if (Nach + Saldo == 0)
                {
                    ignore = true;
                }

                if (!ignore)//Если не игнорить то ищем
                {
                    foreach (TableService S in Services)
                    {

                        if (S.Type.Equals(ser))
                        {
                            OBSDKA.TableServiceId = S.Id;
                            EstService = true;
                            break;
                        }
                    }
                }



                if (EstService)//если есть такой сервис
                {
                    bool nashli = false;

                    for (int A = lastadres; A < Adresa.Count; A++)
                    {

                        if (Adresa[A].Adress.Equals(adr))//и адрес совпал 
                        {

                            //если в массиве адресов есть адрес из строчки то сохраняем айдишник

                            OBSDKA.AdresId = Adresa[A].Id;
                            lastadres = A;
                            nashli = true;
                            int licevoi = 0;
                            try
                            {
                                licevoi = Convert.ToInt32(L[1]);
                            }
                            catch
                            { }
                            OBSDKA.Licevoi = licevoi;
                            OBSDKA.Date = Date;


                            OBSDKA.Nachislenie = Nach;
                            OBSDKA.Saldo = Saldo;
                            OBSDKA.FIO = L[4];
                            OBSDKA.Kvartira = L[6];
                            using (WorkContext db = new WorkContext())
                            {


                                try//сохраняем в БД
                                {
                                    db.OBSDs.Add(OBSDKA);
                                    await db.SaveChangesAsync();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Ошибка записи в базу данных " + e.Message);
                                }
                            }
                            // OBSDKA.Name = "";
                            break;
                        }

                    }
                    if (!nashli && lastadres > 0)//если адрес не нашли то продолжаем поиск с начала списка
                    {

                        for (int A = 0; A < lastadres; A++)
                        {
                            if (Adresa[A].Adress.Equals(adr))//и адрес совпал 
                            {
                                //если в массиве адресов есть адрес из строчки то сохраняем айдишник

                                OBSDKA.AdresId = Adresa[A].Id;
                                lastadres = A;
                                nashli = true;
                                int licevoi = 0;
                                try
                                {
                                    licevoi = Convert.ToInt32(L[1]);
                                }
                                catch
                                { }
                                OBSDKA.Licevoi = licevoi;
                                OBSDKA.Date = Date;


                                OBSDKA.Nachislenie = Nach;
                                OBSDKA.Saldo = Saldo;
                                OBSDKA.FIO = L[4];
                                OBSDKA.Kvartira = L[6];

                                try//сохраняем в БД
                                {
                                    db.OBSDs.Add(OBSDKA);
                                    db.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Ошибка записи в базу данных " + e.Message);
                                }
                                // OBSDKA.Name = "";
                                break;
                            }
                        }
                    }
                }
                procount++;
                progress = Convert.ToInt16(50 + procount / pro100 * 50);
                ProgressHub.SendMessage("Обрабатываем файл ОБСД...", progress);
                if (procount > pro100) { procount = Convert.ToInt32(pro100); }

            }
            List<string> Adr = Adresa.Select(x => x.Adress).ToList();
            for (int a = 0; a < Adr.Count; a++)
            {

                Adr[a] = Adr[a].Replace(" ", "").ToUpper();
            }

           
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, DateTime Date)
        {
            int progress = 0;
            double pro100 = 0;
            int procount = 0;
            if (upload != null)
            {
               

                //найдем старые данные за этот месяц и заменим их не щадя
                List<OBSD> dbOBSD = db.OBSDs.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList();
                pro100 = dbOBSD.Count;
                foreach (OBSD S in dbOBSD)
                {
                    try
                    {
                        db.OBSDs.Remove(S);
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
                string Vkladka = "Общая";
                string[] Names = new string[] { "адрес", "лицевой", "услуга", "начислениефактическое","фио","сальдоисходящее","квартира" };
                List<List<string>> excel = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName), Names,Vkladka);
                if (excel.Count < 1)
                {
                    //если нифига не загрузилось то 
                    Console.WriteLine("Пустой массив значит файл не загрузился!(он уже удалился)");
                    return View("NotUpload");
                }
                else
                {

                    List<Adres> Adresa = db.Adres.ToList();// грузим все адреса из БД
                    foreach (Adres A in Adresa)
                    {
                        A.Adress = A.Adress.Replace(" ", "");
                    }
                    List<TableService> Services = db.TableServices.ToList();
                    foreach (TableService S in Services)//Преобразуем в беспробельных
                    {
                        S.Type = S.Type.Replace(" ", "");
                    }
                    ZapuskPoiska(excel, Date,Adresa,Services);
                    ViewBag.date = Date;
                    ViewBag.file = fileName;

                    return View("UploadComplete");

                }
            }
            return RedirectToAction("Index");
        }


        // GET: OBSDs/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            ViewBag.TableServiceId = new SelectList(db.TableServices, "Id", "Type");
            return View();
        }

        // POST: OBSDs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,TableServiceId,Nachislenie,Licevoi,Date")] OBSD oBSD)
        {
            if (ModelState.IsValid)
            {
                db.OBSDs.Add(oBSD);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", oBSD.AdresId);
            ViewBag.TableServiceId = new SelectList(db.TableServices, "Id", "Type", oBSD.TableServiceId);
            return View(oBSD);
        }

        // GET: OBSDs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBSD oBSD = db.OBSDs.Find(id);
            if (oBSD == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", oBSD.AdresId);
            ViewBag.TableServiceId = new SelectList(db.TableServices, "Id", "Type", oBSD.TableServiceId);
            return View(oBSD);
        }

        // POST: OBSDs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,TableServiceId,Nachislenie,Licevoi,Date")] OBSD oBSD)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oBSD).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", oBSD.AdresId);
            ViewBag.TableServiceId = new SelectList(db.TableServices, "Id", "Type", oBSD.TableServiceId);
            return View(oBSD);
        }

        // GET: OBSDs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBSD oBSD = db.OBSDs.Find(id);
            if (oBSD == null)
            {
                return HttpNotFound();
            }
            return View(oBSD);
        }

        // POST: OBSDs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OBSD oBSD = db.OBSDs.Find(id);
            db.OBSDs.Remove(oBSD);
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
