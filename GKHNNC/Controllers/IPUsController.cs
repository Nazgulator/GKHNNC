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
    public class IPUsController : Controller
    {
        private WorkContext db = new WorkContext();

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        public ActionResult NotUpload()
        {
            return View();
        }


        public ActionResult Index()
        {//Поменять на ОБСД
            List<string> IPUKI = new List<string>();
            List<string> Head = new List<string>();
            List<DateTime> dates = db.IPUs.Select(x => x.Date).Distinct().ToList();//даты загрузок без повторений
            foreach (DateTime D in dates)
            {
                Head.Add(Opr.MonthOpred(D.Month) + " " + D.Year.ToString());
                //доделать вывод по файлам
                IPUKI.Add(db.IPUs.Where(y => y.Date == D).Count().ToString());

            }



            ViewBag.Head = Head;
            ViewBag.IPUKI = IPUKI;
            return View();
        }


        public async void Poisk(List<List<string>> excel, DateTime Date, List<Adres> Adresa)
        {
            int progress = 0;
            double pro100 = 0;
            int procount = 0;
            pro100 = excel.Count;
            IPU IPUKA = new IPU();

            //для каждой строки в экселе
            int lastadres = 0;
            foreach (List<string> L in excel)
            {

                bool EstService = false;
                string adr = L[0].Replace(" ", "").ToUpper();
                decimal Normativ = 0;
                decimal Schetchik = 0;
                bool ignore = false;
                try
                {
                    Normativ = Convert.ToDecimal(L[1].Replace(".",","));

                }
                catch
                { }
                try
                {

                    Schetchik = Convert.ToDecimal(L[2].Replace(".", ","));
                }
                catch
                { }

                if (Normativ + Schetchik == 0)
                {
                    ignore = true;
                }

               



                if (!ignore)//если ненулевые показания
                {
                    bool nashli = false;

                    for (int A = lastadres; A < Adresa.Count; A++)
                    {

                        if (Adresa[A].Adress.Equals(adr))//и адрес совпал 
                        {

                            //если в массиве адресов есть адрес из строчки то сохраняем айдишник

                            IPUKA.AdresId = Adresa[A].Id;
                            lastadres = A;
                            nashli = true;
                            IPUKA.Normativ = Normativ;
                            IPUKA.Schetchik = Schetchik;
                            IPUKA.NomerSchetchika = L[3];
                            IPUKA.Date = Date;

                            using (WorkContext db = new WorkContext())
                            {


                                try//сохраняем в БД
                                {
                                    db.IPUs.Add(IPUKA);
                                    await db.SaveChangesAsync();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Ошибка записи в базу данных " + e.Message);
                                }
                            }
                            // IPUKA.Name = "";
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

                                IPUKA.AdresId = Adresa[A].Id;
                                lastadres = A;
                                nashli = true;
                                IPUKA.Normativ = Normativ;
                                IPUKA.Schetchik = Schetchik;
                                IPUKA.NomerSchetchika = L[3];

                                try//сохраняем в БД
                                {
                                    db.IPUs.Add(IPUKA);
                                    db.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Ошибка записи в базу данных " + e.Message);
                                }
                                // IPUKA.Name = "";
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

        public ActionResult PoiskIPU(DateTime date)
        {

            //ищем все данные за этот месяц, если они есть выводим предупреждение что уже есть данные и они удалятся если сюда грузить UEV
            int dbIPU = db.IPUs.Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month).Count();
            return Json(dbIPU);
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
                List<IPU> dbIPU = db.IPUs.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList();
                pro100 = dbIPU.Count;
                foreach (IPU S in dbIPU)
                {
                    try
                    {
                        db.IPUs.Remove(S);
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
                string Vkladka = "для УЭВ по ГВС без ОПУ";
                string[] Names = new string[] { "адрес", "начислениепонормативу(руб.)", "начислениепосчетчику(куб.м.)","Номер счетчика" };
                List<List<string>> excel = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName), Names, Vkladka);
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
                  
                    ZapuskPoiska(excel, Date, Adresa);
                    ViewBag.date = Date;
                    ViewBag.file = fileName;

                    return View("UploadComplete");

                }
            }
            return RedirectToAction("NotUpload");
        }
        public async void ZapuskPoiska(List<List<string>> excel, DateTime Date, List<Adres> Adresa)
        {
            ProgressHub.SendMessage("Запускаем обработку файла ...", 0);
            await Task.Run(() => Poisk(excel, Date, Adresa));
            ProgressHub.SendMessage("Файл успешно обработан ...", 100);
        }

        // GET: IPUs
        public ActionResult IndexMain()
        {
            var iPUs = db.IPUs.Include(i => i.Adres);
            return View(iPUs.ToList());
        }

        // GET: IPUs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IPU iPU = db.IPUs.Find(id);
            if (iPU == null)
            {
                return HttpNotFound();
            }
            return View(iPU);
        }

        // GET: IPUs/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            return View();
        }

        // POST: IPUs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,OtEnergyRub,OtEnergyGkal,NomerSchetchika,Date")] IPU iPU)
        {
            if (ModelState.IsValid)
            {
                db.IPUs.Add(iPU);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", iPU.AdresId);
            return View(iPU);
        }

        // GET: IPUs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IPU iPU = db.IPUs.Find(id);
            if (iPU == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", iPU.AdresId);
            return View(iPU);
        }

        // POST: IPUs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,OtEnergyRub,OtEnergyGkal,NomerSchetchika,Date")] IPU iPU)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iPU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", iPU.AdresId);
            return View(iPU);
        }

        // GET: IPUs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IPU iPU = db.IPUs.Find(id);
            if (iPU == null)
            {
                return HttpNotFound();
            }
            return View(iPU);
        }

        // POST: IPUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IPU iPU = db.IPUs.Find(id);
            db.IPUs.Remove(iPU);
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
