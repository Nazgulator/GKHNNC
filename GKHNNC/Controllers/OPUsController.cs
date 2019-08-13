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
    public class OPUsController : Controller
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
            List<string> OPUKI = new List<string>();
            List<string> Head = new List<string>();
            List<DateTime> dates = db.OPUs.Select(x => x.Date).Distinct().ToList();//даты загрузок без повторений
            foreach (DateTime D in dates)
            {
                Head.Add(Opr.MonthOpred(D.Month) + " " + D.Year.ToString());
                //доделать вывод по файлам
                OPUKI.Add(db.OPUs.Where(y => y.Date == D).Count().ToString());

            }



            ViewBag.Head = Head;
            ViewBag.OPUKI = OPUKI;
            return View();
        }


        public async void Poisk(List<List<string>> excel, DateTime Date, List<Adres> Adresa)
        {
            int progress = 0;
            double pro100 = 0;
            int procount = 0;
            pro100 = excel.Count;
            OPU OPUKA = new OPU();

            //для каждой строки в экселе
            int lastadres = 0;
            foreach (List<string> L in excel)
            {
                if (L[0].Contains("Правды"))
                {
                }
                string adr = L[0].Replace("пр.", "").Replace("М.", "МУСЫ");
                adr = adr.Replace(" ", "").ToUpper()+L[1].Replace(" ","").Replace("A", "А").ToUpper();//адрес составной из улицы и дома
               // "адрес"0, "№дом"1, "теплотаобщаясОПУ(Гкал)"2, "РасходГВСсОПУкуб,м"3, "РасходХВСкуб,мзамесяц"4, "РУБОТОПЛЕНИЕ"5, "РУБГОР.ВОДА"6, "РУБХОЛ.ВОДА"7
                decimal OtopGkal = 0;
                decimal OtopRub = 0;
                decimal GWM3 = 0;
                decimal GWRub = 0;
                decimal HWM3 = 0;
                decimal HWRub = 0;
                string Primech = "";
                string Primech2 = "";
                bool ignore = false;
                try
                {
                    OtopGkal = Convert.ToDecimal(L[2].Replace(".", ","));
                    OtopRub = Convert.ToDecimal(L[5].Replace(".", ","));
                }
                catch
                {
                    if (L[2] != "")
                    {
                        Primech = L[2].Replace(" ", "");
                    } }
                try
                {
                    GWM3 = Convert.ToDecimal(L[3].Replace(".", ","));
                    if (GWM3 < 0)
                    {
                        GWM3 = 0;
                    }
                    GWRub = Convert.ToDecimal(L[6].Replace(".", ","));
                    if (GWRub < 0){GWRub = 0;}
                }
                catch
                {
                }
                try
                {
                    HWM3 = Convert.ToDecimal(L[4].Replace(".", ","));
                    if (HWM3 < 0) { HWM3 = 0; }
                    HWRub = Convert.ToDecimal(L[7].Replace(".", ","));
                    if (HWRub < 0) { HWRub = 0; }
                }
                catch
                {
                    if (L[4] != "")
                    {
                        Primech2 = L[4].Replace(" ", "");
                        HWM3 = 0;
                        HWRub = 0;
                    }
                
               
                }
            

                if (OtopGkal + GWM3 + HWM3 == 0)
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

                            OPUKA.AdresId = Adresa[A].Id;
                            lastadres = A;
                            nashli = true;
                            OPUKA.GWM3 = GWM3;
                            OPUKA.HWM3 = HWM3;
                            OPUKA.OtopGkal = OtopGkal;
                            OPUKA.GWRub = GWRub;
                            OPUKA.HWRub = HWRub;
                            OPUKA.OtopRub = OtopRub;
                            string prim = "";
                            if (Primech.Length > Primech2.Length) { prim = Primech; }
                            else { prim = Primech2; }
                            OPUKA.Primech = prim;
                            OPUKA.Date = Date;

                            using (WorkContext db = new WorkContext())
                            {


                                try//сохраняем в БД
                                {
                                    db.OPUs.Add(OPUKA);
                                    await db.SaveChangesAsync();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Ошибка записи в базу данных " + e.Message);
                                }
                            }
                            // OPUKA.Name = "";
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

                                OPUKA.AdresId = Adresa[A].Id;
                                lastadres = A;
                                nashli = true;
                                OPUKA.GWM3 = GWM3;
                                OPUKA.HWM3 = HWM3;
                                OPUKA.OtopGkal = OtopGkal;
                                OPUKA.GWRub = GWRub;
                                OPUKA.HWRub = HWRub;
                                OPUKA.OtopRub = OtopRub;
                                OPUKA.Primech = Primech;
                                OPUKA.Date = Date;
                                using (WorkContext db = new WorkContext())
                                {
                                    try//сохраняем в БД
                                    {
                                        db.OPUs.Add(OPUKA);
                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Ошибка записи в базу данных " + e.Message);
                                    }
                                }
                                // OPUKA.Name = "";
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

        public ActionResult PoiskOPU(DateTime date)
        {

            //ищем все данные за этот месяц, если они есть выводим предупреждение что уже есть данные и они удалятся если сюда грузить UEV
            int dbOPU = db.OPUs.Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month).Count();
            return Json(dbOPU);
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
                List<OPU> dbOPU = db.OPUs.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList();
                pro100 = dbOPU.Count;
                foreach (OPU S in dbOPU)
                {
                    try
                    {
                        db.OPUs.Remove(S);
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
                string Vkladka = Date.Month.ToString();
                string[] Names = new string[] { "адрес", "№дом", "гкалотопления(нараспределение)", "расходгвссопукуб.м", "расходхвссопукуб.м", "руботопление", "рубгор.вода", "рубхол.вода" };
                string Error = "";
                List<List<string>> excel = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName), Names, out Error,Vkladka);
                if (excel.Count < 1)
                {
                    //если нифига не загрузилось то 
                    ViewBag.Error = Error;
                    ViewBag.Names = Names;
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

        // GET: OPUs
        public ActionResult IndexMain()
        {
            var oPUs = db.OPUs.Include(o => o.Adres);
            return View(oPUs.ToList());
        }

        // GET: OPUs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPU oPU = db.OPUs.Find(id);
            if (oPU == null)
            {
                return HttpNotFound();
            }
            return View(oPU);
        }

        // GET: OPUs/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            return View();
        }

        // POST: OPUs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,OtopGkal,GWM3,HWM3,OtopRub,GWRub,HWRub,Primech,Date")] OPU oPU)
        {
            if (ModelState.IsValid)
            {
                db.OPUs.Add(oPU);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", oPU.AdresId);
            return View(oPU);
        }

        // GET: OPUs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPU oPU = db.OPUs.Find(id);
            if (oPU == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", oPU.AdresId);
            return View(oPU);
        }

        // POST: OPUs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,OtopGkal,GWM3,HWM3,OtopRub,GWRub,HWRub,Primech,Date")] OPU oPU)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oPU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", oPU.AdresId);
            return View(oPU);
        }

        // GET: OPUs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPU oPU = db.OPUs.Find(id);
            if (oPU == null)
            {
                return HttpNotFound();
            }
            return View(oPU);
        }

        // POST: OPUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OPU oPU = db.OPUs.Find(id);
            db.OPUs.Remove(oPU);
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
