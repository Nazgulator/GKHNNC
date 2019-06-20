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



namespace GKHNNC.Controllers
{
    public class HomeController : Controller
    {
        private WorkContext db = new WorkContext();
        public ActionResult Index()
        {
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