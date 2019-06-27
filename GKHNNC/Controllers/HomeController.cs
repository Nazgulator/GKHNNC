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
using System.Collections;
using Microsoft.AspNet.SignalR;
using Opredelenie;
using System.Web;
using System.Reflection;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections;

using System.IO;

using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using System.Threading;
using GKHNNC.Models;
using Microsoft.AspNet.Identity;
using System.Web.Helpers;
using Opredelenie;
using System.Diagnostics;



namespace GKHNNC.Controllers
{
    public class HomeController : Controller
    {
        private WorkContext db = new WorkContext();
        public ActionResult Index()
        {
            return View();
        }
        /*
        public ActionResult ProverkaVodaMonth(int Month)
        {
            return View();
        }
        */


        public ActionResult VodaMonth(int Month)
        {
            
           int Year = DateTime.Now.Year;
            List<Adres> dbAdresa = db.Adres.ToList();//список всех адресов
            //Сервис айди 1 = отопление, 2 = ГВ, 3 = ГВ на общее имущество берем только гв и гв на общее и смотрим складывать ли их
            List<SVN> dbSVNs = db.SVNs.Where(a => a.Date.Year == DateTime.Now.Year && a.Date.Month == Month&&(a.ServiceId == 2||a.ServiceId==3)).Include(b=>b.Service).ToList();
            List<UEV> dbUEV = db.UEVs.Where(c => c.Date.Year == Year && c.Date.Month == Month).ToList();
            List<OPU> dbOPU = db.OPUs.Where(c => c.Date.Year == Year && c.Date.Month == Month).ToList();
            ViewBag.SVN = false;
            if (dbSVNs.Count > 0)
            {
                ViewBag.SVN = true;
            }
            ViewBag.UEV = false;
            if (dbUEV.Count > 0)
            {
                ViewBag.UEV = true;
            }
            ViewBag.OPU = false;
            if (dbOPU.Count > 0)
            {
                ViewBag.OPU = true;
            }
            List<ViewVoda> Result = new List<ViewVoda>();//пишем сюда результат
            List<ViewVoda> RedResult = new List<ViewVoda>();//пишем сюда результат
            List<ViewVoda> NullResult = new List<ViewVoda>();//пишем сюда результат

            //для каждого адреса ищем данные уэв и сумму данных SVN
            bool skladivat = false;
            List<TableService> TS = db.TableServices.Where(g => g.Id == 2 || g.Id == 3).ToList();//проверка складывать ли если числа в поле сумм равны то складываем
            if (TS[0].Summ == TS[1].Summ) { skladivat = true; }
            foreach(Adres A in dbAdresa)
            {
                ViewVoda V = new ViewVoda();
                decimal Plan = 0;
                decimal Fact = 0;
                SVN GVSVN = new SVN();
                SVN GVOSVN = new SVN();
                try
                {
                    GVSVN = dbSVNs.Where(d => d.AdresId == A.Id && d.ServiceId == 2).Single();//горячая вода
                }
                catch { }
                try
                {
                     GVOSVN = dbSVNs.Where(d => d.AdresId == A.Id && d.ServiceId == 3).Single();//горячая вода на общее имущество
                }
                catch { }
                    if (skladivat)//Если суммы равны то значит складываем ГВ общее и ГВ 
                {
                    Plan = GVSVN.Plan + GVOSVN.Plan;//Складываем плановые показатели 
                    Fact = GVSVN.Fact + GVOSVN.Fact;//Складываем фактические показатели

                }
                else
                {
                    Plan = GVSVN.Plan;//если не складывать то берем данные только из свн
                    Fact = GVSVN.Fact;
                }
                //Выставленные показания в рублях
                decimal GVUEV = 0;
                try
                {
                   GVUEV =  dbUEV.Where(e => e.AdresId == A.Id).Sum(f => f.HwVodaRub + f.HwEnergyRub);//ищем выставленную сумму в рублях по горячей воде в данном доме УЭВ

                }
                catch
                {   }
                //ищем прибор учета и если он есть то выводим галку
                bool pu = false;
                try
                {
                    int Pribor = dbUEV.Where(e => e.AdresId == A.Id).Select(f => f.Pribor).Single();
                    if (Pribor > 0) { pu = true; }
                }
                catch { }
                decimal GVUEVM3 = 0;
                try
                {
                    GVUEVM3 = dbUEV.Where(e => e.AdresId == A.Id).Sum(f => f.HwVodaM3);//ищем выставленную сумму в рублях по горячей воде в данном доме УЭВ
                }
                catch { }

                decimal RaznPlan = GVUEV - Plan;//Разница с планом
                decimal RaznFact = GVUEV - Fact;//Разница с фактом

                //ищем в базе все опушки
                string Primech = "";
                decimal VFact = 0;
                try
                {
                    VFact = dbOPU.Where(h => h.AdresId == A.Id).Select(k => k.GWM3).Single();
                    Primech = dbOPU.Where(h => h.AdresId == A.Id).Select(k => k.Primech).Single();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }


                //сохраняем данные для вывода
                V.Primech = Primech;
                V.VFact = VFact;
                V.Fact = Fact;
                V.Plan = Plan;
                V.RaznFact = RaznFact;
                V.RaznPlan = RaznPlan;
                V.Uev = GVUEV;
                V.Adres = A.Adress;
                V.PU = pu;//прибор учета галкой
                V.GVUEVM3 = GVUEVM3;
                if (Plan + Fact + GVUEV == 0)
                {
                    NullResult.Add(V);
                }
                else
                {
                    if (GVUEV > VFact && pu)
                    {
                        RedResult.Add(V);
                    }
                    Result.Add(V);
                }

            }


            /*
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо



            // Сохранение файла Excel.

            WbExcel.SaveCopyAs("C:\\inetpub\\Otchets\\" + "OtchetMonth" + GEU + ".xlsx");//сохраняем в папку

            ApExcel.Visible = true;//невидимо
            ApExcel.ScreenUpdating = true;//и не обновляемо
                                          // Закрытие книги.
            WbExcel.Close(false, "", Type.Missing);
            // Закрытие приложения Excel.

            ApExcel.Quit();

            Marshal.FinalReleaseComObject(WbExcel);
            Marshal.FinalReleaseComObject(WB);
            Marshal.FinalReleaseComObject(ApExcel);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            */
            List<ViewVoda> MainResult = new List<ViewVoda>(RedResult);//пишем сюда результат
            MainResult.AddRange(Result);
            MainResult.AddRange(NullResult);
            ViewBag.Year = Year;
            ViewBag.Month = Opr.MonthOpred(Month);
            return View(MainResult);
        }

        public ActionResult VODAIndex()
        {
            
            ViewBag.Month = Opr.MonthZabit();
            int[] Go =new int[12];
            for (int i=1;i<13;i++)
            {
                
                int x =db.SVNs.Where(a => a.Date.Year == DateTime.Now.Year && a.Date.Month == i).Count();
                if (x > 0) { Go[i - 1]++; }
                int y = db.UEVs.Where(a => a.Date.Year == DateTime.Now.Year && a.Date.Month == i).Count();
                if (y > 0) { Go[i - 1]++; }
                int z = db.OPUs.Where(a => a.Date.Year == DateTime.Now.Year && a.Date.Month == i).Count();
                if (z > 0) { Go[i - 1]++; }
            }
            ViewBag.Go = Go;
            return View();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, DateTime Date)
        {
            if (upload != null)
            {
                HttpCookie cookie = new HttpCookie("My localhost cookie");

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
                List<HouseToAkt> houses = ExcelUpload.IMPORT(Server.MapPath("~/Files/" + fileName));
              if (houses.Count < 1)
                {
                    
                    RedirectToAction("Warning");
                    
                  
                }
              else
                {
                    List<string> H = new List<string>();//дома списком
                    List<string> U = new List<string>();//услуги списком списком
                    List<bool> HTF = new List<bool>();//помечаем адреса, совпавшие с БД
                    List<int> HId = new List<int>();//помечаем адреса, совпавшие с БД
               
                    List<Adres> Adresa = db.Adres.ToList();// грузим все адреса из БД
                    List<Usluga> Usl = db.Usluga.ToList();// грузим все услуги из БД
                    int progress = 0;
                    decimal pro100 = houses.Count;
                    int procount = 0;
                    foreach (HouseToAkt ho in houses)
                    {
                        procount++;
                        progress = Convert.ToInt16(50+ procount / pro100 * 50);
                        if (progress > 100) { progress = 100; }
                        ProgressHub.SendMessage("Загрузка...", progress);
                        bool go = false;
                        int id = 0;
                        string Adr = "";
                        foreach (Adres A in Adresa)
                        {
                            
                            if (A.Adress.Replace(" ", "").Equals(ho.Adres))
                            {
                                Adr = A.Adress;
                                id = A.Id;
                                go = true;                       
                                break;
                            }
                        }
                        if (go)
                        {
                            H.Add(Adr);//если нашли адрес в БД то сохраним его в список (Он отформатирован верно)
                        }
                        else
                        {
                            H.Add(ho.Adres);// иначе сохраняем тот что в экселе
                        }
                        HTF.Add(go);
                        HId.Add(id);
                        ho.HId = id;
                    }
                    List<bool> UTF = new List<bool>();// помечаем услуги, совпавшие с БД
                    for (int d = 0; d < houses.Count; d++) {

                       
                        List<int> UId = new List<int>();
                        int Ucount = 0;
                        foreach (string us in houses[d].pokazateli)
                        {
                            
                            bool go = false;
                            int id = 0;
                            string PN = "";
                            foreach (Usluga P in Usl)
                            {
                              
                                if (P.Name.ToUpper().Replace(" ", "").Equals(us.ToUpper().Replace(" ", "")))
                                {
                                    PN = P.Name;
                                    id = P.Id;
                                    go = true;
                                    break;

                                }
                                else
                                {
                                    //если объединять все корректировки то этот блок работает
                                  //  if (us.Contains("Корректировка"))
                                  //  {
                                  //      PN = us;
                                  //      id = 17;//корректировки получают код 17
                                  //      go = true;
                                  //      break;
                                  //  }
                                }
                            }
                            if (go)
                            {
                                if (d == 0) { U.Add(PN); }//если нашли услугу в БД то сохраним его в список (Она отформатирована верно)
                            }
                            else
                            {
                                
                                if (d == 0) { U.Add(us); }// иначе сохраняем тот что в экселе
                            }
                            if (d == 0) { UTF.Add(go); }
                            UId.Add(id);

                            houses[d].UId.Add(id);
                            Ucount++;
                        }
                    }

                   

                    //Session["Act2House"] = houses;
                    SessionObjects.HouseToAktsSet(Session, houses);
                    ViewBag.file = fileName;
                    ViewBag.H = H;
                    ViewBag.U = U;
                    ViewBag.HTF = HTF;
                    ViewBag.HId = HId;
                    ViewBag.UTF = UTF;
                    
                    ViewBag.UId = houses[0].UId; 
                    ViewBag.Data = Date;//отправляем дату с загруженного файла
                    ViewBag.Houses = houses;
                    int c = houses.Count;
                    if (c < houses[0].pokazateli.Count) {
                        ViewBag.MaxCount = houses[0].pokazateli.Count;
                            }
                   else
                    {
                        ViewBag.MaxCount = c;
                    }
                    return View("UploadComplete");
                }
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult UploadComplete(DateTime Date)
        {
            //При подтверждении записываем в БД 
           
            if (Date != null)
            {

                var houses = SessionObjects.HouseToAktsGet(Session);

                int progress = 0;
                decimal pro100 = houses.Count;
                int procount = 0;
                ProgressHub.SendMessage("Ожидаем подтверждения...", progress);



                for (int j = 0; j < houses.Count; j++)
                {

                    procount++;
                    progress = Convert.ToInt16( procount / pro100 * 100);
                    if (progress > 100) { progress = 100; }
                    ProgressHub.SendMessage("Записываем в базу...", progress);

                    if (houses[j].HId != 0)//если адрес определен
                    {

                        //если определен адрес и дата не нулевая то чистим совпадающие области в БД
                        List<VipolnennieUslugi> homeDate = db.VipolnennieUslugis.Where(x => x.Date.Year.Equals(Date.Year) && x.Date.Month.Equals(Date.Month)).ToList();
                        for (int a=homeDate.Count-1;a>=0;a--)
                        {
                            if (homeDate[a].AdresId.Equals(houses[j].HId))//если в эту дату в базен есть такой адрес то удаляем услугу
                            {
                                db.VipolnennieUslugis.Remove(homeDate[a]);
                                db.SaveChanges();
                            }
                        }
                        for (int i = 0; i < houses[j].UId.Count; i++)
                        {
                            //Usluga U = new Usluga();
                            if (houses[j].UId[i] != 0)//если услуга определена
                            {
                                VipolnennieUslugi V = new VipolnennieUslugi();
                                V.UslugaId = houses[j].UId[i];//выполненная услуга ID
                                V.AdresId = houses[j].HId;
                                V.Date = Date;
                                V.StoimostNaM2 = houses[j].StoimostNaM2[i];
                                V.StoimostNaMonth = houses[j].StoimostNaMonth[i];
                               // V.Usluga
                                db.VipolnennieUslugis.Add(V);
                                
                                    db.SaveChanges();
                                
                               
                               
                               // db.SaveChanges();
                            }
                        }

                    }
                }
            }
           
            return View("UploadEnd");
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}