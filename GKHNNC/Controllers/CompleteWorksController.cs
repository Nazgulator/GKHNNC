using GKHNNC.DAL;
using GKHNNC.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static System.Web.HttpCookie;
using Opredelenie;





namespace GKHNNC.Controllers
{
    public class CompleteWorksController : Controller
    {
        // public string Adress = "";
        // public string Group = "";
        //  public List<Work> WORKS = new List<Work>();
        // public int g;

        private  WorkContext db = new WorkContext();

        public List<string> MenuPoiskGeu()
        {
            string xxx = User.Identity.Name.Replace(" ", "");
            List<string> GEUAll = new List<string>();
            if (xxx.Contains("ЖЭУ"))
            {
                //имя пользователя если содержит жэу то только его в список
                GEUAll.Add(xxx);
            }
            else
            {
                //иначе список всех жэу
                GEUAll = db.GEUs.Select(x => x.Name).ToList();
            }

            return GEUAll;
        }
        public ActionResult MenuPoiskAdresaPoGeu(string GEU)
        {

            int EUId = 0;
         
               EUId = Convert.ToInt16(GEU.Replace("ЭУ-", ""));

               
           

            List<string> Adresadb = db.Adres.Where(x => x.EUId==EUId&&x.MKD).Select(y=>y.Adress).ToList();

            Adresadb.Sort();
            Adresadb.Insert(0, "Все адреса");
            
            return Json(Adresadb);
        }

        [Authorize]
        public ActionResult IndexMain()
        {
            return View();
        }
        
        public ActionResult IndexSpisok(string selection)
        {
            string Agent = "ЖЭУ-2";
            if (User.Identity.Name.Contains("ЖЭУ"))
            {
                Agent = User.Identity.Name.Replace(" ", "").Replace("НачальникЖЭУ", "");
            }
            string Adres = "Всеадреса";
            string Month = DateTime.Now.Month.ToString();
            string Year = DateTime.Now.Year.ToString();




            if (selection != null)
            {
                string[] s = selection.Split(';');
                Agent = s[0].Replace(" ", "");
                Adres = s[1].Replace(" ", "");
                Month = s[2];
                Year = s[3];
                // создаем cookie
                HttpContext.Response.Cookies["Month"].Value = Month;//Opr.MonthOpred(Convert.ToInt16(Month));
                HttpContext.Response.Cookies["Month"].Name = "Month";
                HttpContext.Response.Cookies["Month"].Expires = DateTime.Now.AddDays(1);
                

                //Response.Cookies["Month"].Value = Month;
                // задаем срок истечения срока действия cookie
                // Response.Cookies["Month"].Expires = DateTime.Now.AddDays(1);

            }
            else
            {
                if (Request.Cookies["Month"] != null)
                {
                    Month = Request.Cookies["Month"].Value;
                }
            }
            int Y = Convert.ToInt16(Year);
            int M = 0;
            int EU = Convert.ToInt16(Agent.Remove(0, Agent.Length - 1));
           
            List<string> GEUS = new List<string>();
            GEUS = db.GEUs.Where(x => x.EU == EU).Select(x => x.Name).ToList();
            Obratno(Month, out M);
            List<CompleteWork> CWdb = new List<CompleteWork>();
            if (Adres.Equals("Всеадреса"))
            {
                foreach (string S in GEUS)
                {
                    CWdb.AddRange( db.CompleteWorks.Where(p => p.Agent.Equals(S)).Where(g => g.WorkDate.Year == Y && g.WorkDate.Month == M).ToList());
                }
            }
            else
            {
               
                    CWdb = db.CompleteWorks.Where(x=> x.WorkDate.Year == Y && x.WorkDate.Month == M&&x.WorkAdress.Equals(Adres)).ToList();
               
                //CWdb = db.CompleteWorks.Where(p => p.Agent.Contains(Agent)).Where(f => f.WorkAdress.Replace(" ", "").Equals(Adres)).Where(g => g.WorkDate.Year == Y && g.WorkDate.Month == M).ToList();
            }

                return View(CWdb);

        }

        // GET: CompleteWorks
        [Authorize]
        public ActionResult IndexMenu()
        {
            string xxx = User.Identity.Name.Replace(" ", "");
            List<string> GEUAll = new List<string>();
            int EUId = 0;
            if (xxx.Contains("ЖЭУ"))
            {
                int GeuId = Convert.ToInt16(xxx.Replace("ЖЭУ-", "").Replace("НачальникЖЭУ",""));
            
                try
                {
                    EUId = db.GEUs.Where(x => x.GEUN == GeuId).Select(x => x.EU).First();
                }
                catch
                {

                }
                //имя пользователя если содержит жэу то только его в список
                GEUAll.Add("ЭУ-"+EUId);
            }
            else
            {
                //иначе список всех жэу
                GEUAll = db.GEUs.Select(x =>"ЭУ-"+ x.EU.ToString()).Distinct().ToList();
          
            }
            GEUAll.Sort();
            
            ViewBag.GEU = GEUAll;
            string G = GEUAll[0];


            List<string> Adresadb = new List<string>();
            if (EUId ==0)
            {
              Adresadb =  db.Adres.Where(x=>x.MKD).Select(y => y.Adress).ToList();
            }
            else
            {
               Adresadb = db.Adres.Where(x => x.MKD&&x.EUId == EUId).Select(y => y.Adress).ToList();
            }
                
                
            Adresadb.Sort();
            Adresadb.Insert(0, "Все адреса");
            ViewBag.Adres = Adresadb;
            ViewBag.Works = db.Works.Where(x => x.Group.Contains("ТО конструктивных элементов")).OrderBy(x=>x.Name).Select(a => new SelectListItem { Value = a.WorkId.ToString(), Text = a.Name }).ToList();
        //    ViewBag.Works = new SelectList( db.Works.Where(x => x.Code.Contains("01-")).Select(x=>x.Name).ToList());

            //ищем год
            ViewBag.Year = new string[DateTime.Now.Year - 2018 + 1];
                int counter = 0;
                for (int i = DateTime.Now.Year; i >= 2018; i--)
                {
                    ViewBag.Year[counter] = i.ToString();
                    counter++;
            }

            //var worksFromBase = db.CompleteWorks.Where(p => p.Agent.Replace(" ", "") == xxx);
            //Делаем список месяцев из них первый тот что в куки записан
            List < SelectListItem > Month = new List<SelectListItem>();
            for (int i=1;i<13;i++)
            {
                string mon = "";
                mon = Opr.MonthOpred(i);
                SelectListItem SLI = new SelectListItem();
                SLI.Text = mon;
                SLI.Value = mon;//i.ToString();
                Month.Add(SLI);

            }
            SelectListItem M = new SelectListItem();
            //если в куки что-то есть
            if (HttpContext.Request.Cookies["Month"]!=null)
            {
                M.Text = HttpContext.Request.Cookies["Month"].Value;
                M.Value = HttpContext.Request.Cookies["Month"].Value;//Opr.MonthObratno(M.Text).ToString();
                Month.RemoveAt(Opr.MonthObratno(M.Text)-1);
                Month.Insert(0, M);
            }

            ViewBag.Month = Month;

          
            ViewBag.Month = Month;

            // SelectList MSL = new SelectList(new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" }, new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" });


            return View();
                
        }

        [Authorize]
        public ActionResult Index(string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            IEnumerable<CompleteWork> cw = db.CompleteWorks;
            List<CompleteWork> ww = new List<CompleteWork>();
            if (User.IsInRole("Администратор") == false)
            {
                //если не админ то видно только твои данные
              
                ViewBag.Year = new string[DateTime.Now.Year - 2018 + 1];
                int counter = 0;
                for (int i = DateTime.Now.Year; i >= 2018; i--)
                {
                    ViewBag.Year[counter] = i.ToString();
                    counter++;
                }
              

                //var worksFromBase = from s in db.CompleteWorks select s;
                string xxx = User.Identity.Name.Replace(" ", "");
                ViewBag.GEU2 = xxx;
                //готовим адреса из БД по дому

                List<SelectListItem> Ad = new List<SelectListItem>();
                // var ADD = db.Adres.ToList();//все элементы в массив
                foreach (Adres A in db.Adres)
                {
                    if (A.GEU!=null&&A.GEU.Replace(" ", "").Equals(xxx))
                    {
                        SelectListItem AA = new SelectListItem();
                        AA.Value = A.Adress.Replace(" ", "");
                        AA.Text = A.Adress.Replace(" ","");
                        Ad.Add(AA);
                    }
                }

                SelectList SL = new SelectList(Ad, "Value", "Text");
                ViewBag.Adres = SL;

                var worksFromBase = db.CompleteWorks.Where(p => p.Agent.Replace(" ", "") == xxx);
               // matchedManager = managers.Where(x => x.EngineerId == matchedEngineer.PersonId).ToList();
                switch (sortOrder)
                {
                    case "name_desc":
                        worksFromBase = worksFromBase.OrderByDescending(s => s.WorkAdress);
                        break;
                    case "Date":
                        worksFromBase = worksFromBase.OrderBy(s => s.WorkDate);
                        break;
                    case "date_desc":
                        worksFromBase = worksFromBase.OrderByDescending(s => s.WorkDate);
                        break;
                    default:
                        worksFromBase = worksFromBase.OrderBy(s => s.WorkAdress);
                        break;
                }

                return View(worksFromBase.ToList());
            }
            else
            {
                //если админ возвращаем все данные
                ViewBag.Year = new string[DateTime.Now.Year - 2018 + 1];
                int counter = 0;
                for (int i = DateTime.Now.Year; i >= 2018 ; i--)
                {
                    ViewBag.Year[counter] = i.ToString();
                    counter++;
                }
                ViewBag.GEU = new string[] { "ЖЭУ-2", "ЖЭУ-3", "ЖЭУ-4", "ЖЭУ-5", "ЖЭУ-7" };
                //разбиваем адреса по жэу в массив
              

                List<SelectListItem> Ad = new List<SelectListItem>();
                // var ADD = db.Adres.ToList();//все элементы в массив
                foreach (Adres A in db.Adres)
                {
                    SelectListItem AA = new SelectListItem();
                    AA.Value = A.Adress.Replace(" ", "");
                    AA.Text = A.Adress.Replace(" ", "");
                    Ad.Add(AA);
                }

                SelectList SL = new SelectList(Ad, "Value", "Text");
                ViewBag.Adres = SL;
               

                //ViewBag.Adres = db.Adres;
                return View(cw.ToList());
            }
           
           // db.CompleteWorks.ToList()
        }
        //меню для формирования актов
        public ActionResult PartialViewMenu(string Selection)
        {
            List<string> GGEU = new List<string>();
            if (User.Identity.Name.Contains("ЖЭУ"))
            {
                //GEU = User.Identity.Name.Replace(" ", "");
                GGEU.Add(User.Identity.Name.Replace(" ", ""));
            }
            else
            {
                foreach (GEU G in db.GEUs)
                {
                    GGEU.Add(G.Name);
                }
            }
            ViewBag.GEU = GGEU;
            //создаем список годов
            ViewBag.Year = new string[DateTime.Now.Year - 2018 + 1];
            int counter = 0;
            for (int i = DateTime.Now.Year; i >= 2018; i--)
            {
                ViewBag.Year[counter] = i.ToString();
                counter++;
            }

            //Делаем список месяцев из них первый тот что в куки записан
            List<SelectListItem> Month = new List<SelectListItem>();
            for (int i = 1; i < 13; i++)
            {
                string mon = "";
                mon = Opr.MonthOpred(i);
                SelectListItem SLI = new SelectListItem();
                SLI.Text = mon;
                SLI.Value = mon;//i.ToString();
                Month.Add(SLI);

            }
            SelectListItem M = new SelectListItem();
            //если в куки что-то есть
            if (HttpContext.Request.Cookies["Month"] != null)
            {
                M.Text = HttpContext.Request.Cookies["Month"].Value;
                M.Value = HttpContext.Request.Cookies["Month"].Value;//Opr.MonthObratno(M.Text).ToString();
                Month.RemoveAt(Opr.MonthObratno(M.Text) - 1);
                Month.Insert(0, M);
            }

            ViewBag.Month = Month;


            return View();
           
        }

        public ActionResult PartialViewMenuEu(string Selection)
        {
            List<string> GGEU = new List<string>();
            if (User.Identity.Name.Contains("ЖЭУ"))
            {
                //GEU = User.Identity.Name.Replace(" ", "");
                string eid = db.GEUs.Where(x => x.Name.Contains(User.Identity.Name.Replace("НачальникЖЭУ",""))).Select(x => x.EU).First().ToString();
                GGEU.Add(eid);

            }
            else
            {
                foreach (GEU G in db.GEUs.OrderBy(x=>x.EU))
                {
                    GGEU.Add(G.EU.ToString());
                }
            }
            ViewBag.GEU = GGEU.Distinct();
            //создаем список годов
            ViewBag.Year = new string[DateTime.Now.Year - 2018 + 1];
            int counter = 0;
            for (int i = DateTime.Now.Year; i >= 2018; i--)
            {
                ViewBag.Year[counter] = i.ToString();
                counter++;
            }

            //Делаем список месяцев из них первый тот что в куки записан
            List<SelectListItem> Month = new List<SelectListItem>();
            for (int i = 1; i < 13; i++)
            {
                string mon = "";
                mon = Opr.MonthOpred(i);
                SelectListItem SLI = new SelectListItem();
                SLI.Text = mon;
                SLI.Value = mon;//i.ToString();
                Month.Add(SLI);

            }
            SelectListItem M = new SelectListItem();
            //если в куки что-то есть
            if (HttpContext.Request.Cookies["Month"] != null)
            {
                M.Text = HttpContext.Request.Cookies["Month"].Value;
                M.Value = HttpContext.Request.Cookies["Month"].Value;//Opr.MonthObratno(M.Text).ToString();
                Month.RemoveAt(Opr.MonthObratno(M.Text) - 1);
                Month.Insert(0, M);
            }

            ViewBag.Month = Month;


            return View();

        }


        public JsonResult DeleteWork(int ID)
        {
            string Alert = "";
            var CW = db.CompleteWorks.Where(x => x.ID == ID).First();
            try
            {
                db.CompleteWorks.Remove(CW);
                db.SaveChanges();
                Alert = "Работа удалена!";
            }
            catch (Exception e)
            {
                Alert = "Ошибка удаления "+ e.Message;
            }
            return Json(Alert);
        }
        //формируем список для вывода по домам
        public ActionResult PartialViewSpisokEu(string Selection)
        {
            string GEU = "";
            string Year = "";
            string Month = "";
            List<VipolnennieUslugi> VUAll = new List<VipolnennieUslugi>();
            List<string> VUString = new List<string>();
            List<int> VUNumber = new List<int>();

           

            GEU = "1";
            if (Selection == null)
            {
                Year = DateTime.Now.Year.ToString();
                Month = DateTime.Now.Month.ToString();
                string g = User.Identity.Name.Replace(" ","").Replace("НачальникЖЭУ","");
                if (Request.Cookies["Month"] != null)
                {
                    Month = Request.Cookies["Month"].Value;
                }
                try
                {
                    GEU = db.GEUs.Where(x => x.Name.Contains(g)).Select(x=>x.EU).First().ToString();
                }
                catch
                {

                }

            }
            else
            {
                string[] s = Selection.Split(';');
                GEU = s[2];
                Year = s[0];
                Month = s[1];
                // создаем cookie
                HttpContext.Response.Cookies["Month"].Value = Month;//Opr.MonthOpred(Convert.ToInt16(Month));
                HttpContext.Response.Cookies["Month"].Name = "Month";
                HttpContext.Response.Cookies["Month"].Expires = DateTime.Now.AddDays(1);
            }




            if (Selection != null)
            {
                if (User.Identity.Name.Contains("ЭУ"))
                {
                    //GEU = User.Identity.Name.Replace(" ", "");
                    string eid = db.GEUs.Where(x => x.Name.Contains(User.Identity.Name.Replace("НачальникЖЭУ", ""))).Select(x => x.EU).First().ToString();
                    GEU = eid;

                }
                int EU = Convert.ToInt16(GEU);
                int M = 0;
                Obratno(Month, out M);
                int Y = Convert.ToInt16(Year);
                Month = M.ToString();

                DateTime D = new DateTime(Y, M, 1);
                DateTime ToD = new DateTime(Y, M, 1).AddMonths(1);
                List<Adres> Adresadb = db.Adres.Where(a => a.EUId == EU).Distinct().ToList();
                List<string> AdresDist = Adresadb.Select(x => x.Adress.Replace(" ", "").ToUpper()).ToList();
                List<CompleteWork> completeWorksDB = db.CompleteWorks.Where(x => x.WorkDate >= D && x.WorkDate <= ToD && AdresDist.Contains(x.WorkAdress.Replace(" ", ""))).OrderBy(x => x.WorkAdress).ToList();
                List<CompleteWork> CWSpisok = new List<CompleteWork>();
                List<VipolnennieUslugi> completeUslugsDB = db.VipolnennieUslugis.Include(X => X.Adres).Include(X => X.Usluga).Where(x => x.Date == D  && x.StoimostNaM2 + x.StoimostNaMonth!=0 && AdresDist.Contains(x.Adres.Adress.Replace(" ", ""))).ToList();//.OrderBy(x => x.Adres.Adress)
                List<VipolnennieUslugi> VUSpisok = new List<VipolnennieUslugi>();
                List<string> CWAdresa = new List<string>();
                try
                {
                    CWAdresa = completeWorksDB.Select(x => x.WorkAdress.Replace(" ", "")).Distinct().ToList();
                }
                catch (Exception e)
                {

                }
                //   CWAdresa = AdresDist;
                List<int> CWNumber = new List<int>();
                List<string> CWString = new List<string>();
                //создаем список всех адресов из выполненных работ
                int progress = 0;
                int saveprogress = 0;
                int cc = 0;
                /*       foreach (CompleteWork C in completeWorksDB)
                       {
                           cc++;
                           progress = Convert.ToInt16(Convert.ToDecimal(cc) / completeWorksDB.Count * 10);
                           ProgressHub.SendMessage("Загружено...", progress);
                           bool go = false;
                           for (int i = 0; i < CWAdresa.Count; i++)
                           {

                               if (C.WorkAdress.Replace(" ", "").Equals(CWAdresa[i]))
                               {
                                   go = true;
                                   break;
                               }

                           }
                           if (!go)
                           {
                               CWAdresa.Add(C.WorkAdress.Replace(" ", ""));
                               CWNumber.Add(0);
                               CWString.Add("");


                           }
                           else
                           {

                           }
                       }
                       */
                CWAdresa.Sort();
                foreach (var CWA in CWAdresa)
                {
                    try
                    {
                        List<string> CWS = completeWorksDB.Where(x => x.WorkAdress.Replace(" ", "").Equals(CWA)).Select(x => x.WorkName).ToList();
                        string cws = "";
                        int num = CWS.Count();
                        foreach (string s in CWS)
                        {
                            cws += s + ";";
                        }
                        cws = cws.Remove(cws.Length - 1, 1);
                        CWString.Add(cws);
                        CWNumber.Add(num);

                    }
                    catch
                    {

                    }

                    try
                    {
                        List<VipolnennieUslugi> CUS = completeUslugsDB.Where(x => x.Adres.Adress.Equals(CWA)).ToList();
                        string cus = "";
                        int num = CUS.Count();
                        foreach (var s in CUS)
                        {
                            cus += s.Usluga.Name + ";";
                            // VipolnennieUslugi F = new VipolnennieUslugi();
                            // DateTime FFF = new DateTime(Y, M, 1);
                            //  F.Adres = s.Adres;
                            //   F.Date = FFF;
                           
                        }
                        VUAll.Add(CUS[0]);
                        cus = cus.Remove(cus.Length - 1, 1);
                        VUString.Add(cus);
                        VUNumber.Add(num);
                    }
                    catch
                    {

                    }
                }

                saveprogress = 10;
                // сверяем адреса с БД по ЖЭУ
                cc = 0;
                /*    for (int i = CWAdresa.Count - 1; i >= 0; i--)
                    {
                        cc++;
                        progress = Convert.ToInt16(saveprogress + Convert.ToDecimal(cc) / CWAdresa.Count * 10);
                        ProgressHub.SendMessage("Загружено...", progress);
                        bool go = false;
                        foreach (Adres A in Adresadb)
                        {
                            if (A.Adress.Replace(" ", "").Equals(CWAdresa[i].Replace(" ", "")))
                            {
                                go = true;
                                break;
                            }
                        }
                        if (!go)
                        {
                            CWAdresa.RemoveAt(i);
                        }

                    }
                    */
                saveprogress = 20;
                cc = 0;
                /*        for (int j = completeWorksDB.Count - 1; j >= 0; j--)//для каждой работы
                        {
                            cc++;
                            progress = Convert.ToInt16(saveprogress + Convert.ToDecimal(cc) / completeWorksDB.Count * 10);
                            ProgressHub.SendMessage("Загружено...", progress);
                            for (int i = 0; i < CWAdresa.Count; i++)//для каждого адреса
                            {
                                if (completeWorksDB[j].WorkAdress.Replace(" ", "").Equals(CWAdresa[i]))//если адрес работы совпал с адресом дома
                                {
                                    if (CWString[i].Replace(" ", "").Contains(completeWorksDB[j].WorkName.Replace(" ", "")) == false)//проверка на типы услуг
                                    {
                                        CWNumber[i]++;
                                        CWString[i] += completeWorksDB[j].WorkName + ";";

                                    }
                                    break;
                                }

                            }
                        }
        */

                ViewBag.CWString = CWString;//названия услуг через ;
                ViewBag.CWNumber = CWNumber;//количество услуг
                ViewBag.CWAdresa = CWAdresa;
                ViewBag.GEU = GEU;

                int counter = 0;
                saveprogress = 30;
                cc = 0;
                /*      foreach (string Adres in CWAdresa)
                      {
                          cc++;
                          progress = Convert.ToInt16(saveprogress + Convert.ToDecimal(cc) / CWAdresa.Count * 70);
                          ProgressHub.SendMessage("Загружено...", progress);

                          VUString.Add("");
                          VUNumber.Add(0);
                          List<VipolnennieUslugi> VUDB = db.VipolnennieUslugis.Include(z => z.Adres).Include(f => f.Usluga).Where(x => x.Adres.Adress.Replace(" ", "").Equals(Adres.Replace(" ", "")) && x.Date.Year == Y && x.Date.Month == M).ToList();
                          VipolnennieUslugi V = new VipolnennieUslugi();

                          foreach (VipolnennieUslugi VU in VUDB)
                          {

                              if (VUString[counter].Contains(VU.Usluga.Name) == false)
                              {
                                  if (VU.StoimostNaM2 + VU.StoimostNaMonth != 0)
                                  {
                                      VUString[counter] += VU.Usluga.Name + ";";
                                      VUNumber[counter]++;
                                  }

                              }

                          }
                          if (VUDB.Count > 0)
                          {
                              VUAll.Add(VUDB.First());
                          }
                          else
                          {
                              VipolnennieUslugi F = new VipolnennieUslugi();
                              Adres FF = new Adres();
                              DateTime FFF = new DateTime(Y, M, 1);
                              FF.Adress = Adres;
                              F.Adres = FF;
                              F.Date = FFF;
                              VUAll.Add(F);
                          }
                          counter++;
                      }
                  }
                  */
                ViewBag.VUNumber = VUNumber;
                ViewBag.VUString = VUString;
            }
            return View(VUAll);
        }
  

        //формируем список для вывода по домам
        public ActionResult PartialViewSpisok(string Selection)
        {
            string GEU = "";
            string Year = "";
            string Month = "";
            

            if (Selection == null)
            {
                Year = DateTime.Now.Year.ToString();
                Month = DateTime.Now.Month.ToString();
                GEU = User.Identity.Name;
                if (Request.Cookies["Month"] != null)
                {
                    Month = Request.Cookies["Month"].Value;
                }

            }
            else
            {
               string[] s = Selection.Split(';');
               GEU = s[2];
               Year = s[0];
               Month = s[1];
                // создаем cookie
                HttpContext.Response.Cookies["Month"].Value = Month;//Opr.MonthOpred(Convert.ToInt16(Month));
                HttpContext.Response.Cookies["Month"].Name = "Month";
                HttpContext.Response.Cookies["Month"].Expires = DateTime.Now.AddDays(1);
            }

            if (GEU.Contains("ЖЭУ")==false)
            {
                GEU = "ЖЭУ-5";
            }
            if (User.Identity.Name.Contains("ЖЭУ"))
            {
                GEU = User.Identity.Name.Replace(" ", "");
            }
            int M = 0;
            Obratno(Month, out M);
            int Y = Convert.ToInt16(Year);
            Month = M.ToString();
            List<Adres> Adresadb = db.Adres.Where(a => a.GEU.Contains(GEU)).ToList();
            List<CompleteWork> completeWorksDB = db.CompleteWorks.Where(x => x.WorkDate.Year == Y).Where(y => y.WorkDate.Month == M).ToList();
            List<CompleteWork> CWSpisok = new List<CompleteWork>();
            List<string> CWAdresa = new List<string>();
            List<int> CWNumber = new List<int>();
            List<string> CWString = new List<string>();
            //создаем список всех адресов из выполненных работ
            int progress = 0;
            int saveprogress=0;
            int cc = 0;
            foreach (CompleteWork C in completeWorksDB)
            {
                cc++;
                progress = Convert.ToInt16(Convert.ToDecimal(cc) / completeWorksDB.Count * 10);
                ProgressHub.SendMessage("Загружено...", progress);
                bool go = false;
                for (int i = 0; i < CWAdresa.Count; i++)
                { 
              
                    if (C.WorkAdress.Replace(" ","").Equals(CWAdresa[i]))
                    {
                        
                       

                        go = true;
                        break;
                    }

                }
                if (!go)
                {
                    CWAdresa.Add(C.WorkAdress.Replace(" ", ""));
                    CWNumber.Add(0);
                    CWString.Add("");
                   
                   
                }
                else
                {

                }
            }
            CWAdresa.Sort();
            saveprogress = 10;
            // сверяем адреса с БД по ЖЭУ
            cc = 0;
            for (int i = CWAdresa.Count-1;i>=0;i--)
            {
                cc++;
                progress = Convert.ToInt16(saveprogress+Convert.ToDecimal(cc) / CWAdresa.Count * 10);
                ProgressHub.SendMessage("Загружено...", progress);
                bool go = false;
                foreach (Adres A in Adresadb)
                {
                    if (A.Adress.Replace(" ","").Equals(CWAdresa[i].Replace(" ","")))
                    {
                        go = true;
                        break;
                    }
                }
                if (!go)
                {
                    CWAdresa.RemoveAt(i);
                }

            }
            saveprogress = 20;
            cc = 0;
            for (int j=completeWorksDB.Count-1;j>=0;j--)//для каждой работы
            {
                cc++;
                progress = Convert.ToInt16(saveprogress + Convert.ToDecimal(cc) / completeWorksDB.Count * 10);
                ProgressHub.SendMessage("Загружено...", progress);
                for (int i = 0; i < CWAdresa.Count; i++)//для каждого адреса
                {
                    if (completeWorksDB[j].WorkAdress.Replace(" ", "").Equals(CWAdresa[i]))//если адрес работы совпал с адресом дома
                    {
                        if (CWString[i].Replace(" ", "").Contains(completeWorksDB[j].WorkName.Replace(" ", "")) == false)//проверка на типы услуг
                        {
                            CWNumber[i]++;
                            CWString[i] += completeWorksDB[j].WorkName + ";";

                        }
                        break;
                    }

                }
            }
            
               
                ViewBag.CWString = CWString;//названия услуг через ;
            ViewBag.CWNumber = CWNumber;//количество услуг
            ViewBag.CWAdresa = CWAdresa;
            ViewBag.GEU = GEU;
            List<VipolnennieUslugi> VUAll = new List<VipolnennieUslugi>();
            List<string> VUString = new List<string>();
            List<int> VUNumber = new List<int>();
            int counter = 0;
            saveprogress = 30;
            cc = 0;
            foreach (string Adres in CWAdresa)
            {
                cc++;
                progress = Convert.ToInt16(saveprogress + Convert.ToDecimal(cc) / CWAdresa.Count * 70);
                ProgressHub.SendMessage("Загружено...", progress);

                VUString.Add("");
                VUNumber.Add(0);
                List<VipolnennieUslugi> VUDB = db.VipolnennieUslugis.Include(z => z.Adres).Include(f=>f.Usluga).Where(x => x.Adres.Adress.Replace(" ", "").Equals(Adres.Replace(" ", ""))&&x.Date.Year==Y&&x.Date.Month==M).ToList();
                VipolnennieUslugi V = new VipolnennieUslugi();
              
                foreach(VipolnennieUslugi VU in VUDB)
                {
                    
                    if (VUString[counter].Contains(VU.Usluga.Name)==false)
                    {
                        if (VU.StoimostNaM2 + VU.StoimostNaMonth != 0)
                        {
                            VUString[counter] += VU.Usluga.Name + ";";
                            VUNumber[counter]++;
                        }

                    }
                
                }
                if (VUDB.Count > 0)
                {
                    VUAll.Add(VUDB.First());
                }
                else
                {
                    VipolnennieUslugi F = new VipolnennieUslugi();
                    Adres FF = new Adres();
                    DateTime FFF = new DateTime(Y, M, 1);
                    FF.Adress = Adres;
                    F.Adres = FF;
                    F.Date = FFF;
                    VUAll.Add(F);
                }
                counter++;
            }
            ViewBag.VUNumber = VUNumber;
            ViewBag.VUString = VUString;
            return View(VUAll);
        }
        //создание актов
        public ActionResult SozdanieAktov ()
        {
            return View();
        }
        // GET: CompleteWorks/Details/5
        //[HttpPost]
        //поиск по имени жэу
        public ActionResult PoiskPoGeu(string selection)
        {
            //сделать поиск домов по жэу (p => p.Agent.Replace(" ", "") == xxx);
            List<string> data2 = db.Adres.Where(x => x.GEU.Contains(selection)).Select(y=>y.Adress.Replace(" ","")).ToList();
            

            return Json(data2);
        }

        public void Obratno(string month, out int mes)
        {
            mes = 1;
            switch (month)
            {
                case "Январь":
                    mes = 1;
                    break;
                case "Февраль":
                    mes = 2;
                    break;
                case "Март":
                    mes = 3;
                    break;
                case "Апрель":
                    mes = 4;
                    break;
                case "Май":
                    mes = 5;
                    break;
                case "Июнь":
                    mes = 6;
                    break;
                case "Июль":
                    mes = 7;
                    break;
                case "Август":
                    mes = 8;
                    break;
                case "Сентябрь":
                    mes = 9;
                    break;
                case "Октябрь":
                    mes = 10;
                    break;
                case "Ноябрь":
                    mes = 11;
                    break;
                case "Декабрь":
                    mes = 12;
                    break;

            }
        }

        public void MonthOpred(int mes, out string messtr)
        {
            messtr = "Январь";
            switch (mes)
            {
                case 1:
                    messtr = "Январь";
                    break;
                case 2:
                    messtr = "Февраль";
                    break;
                case 3:
                    messtr = "Март";
                    break;
                case 4:
                    messtr = "Апрель";
                    break;
                case 5:
                    messtr = "Май";
                    break;
                case 6:
                    messtr = "Июнь";
                    break;
                case 7:
                    messtr = "Июль";
                    break;
                case 8:
                    messtr = "Август";
                    break;
                case 9:
                    messtr = "Сентябрь";
                    break;
                case 10:
                    messtr = "Октябрь";
                    break;
                case 11:
                    messtr = "Ноябрь";
                    break;
                case 12:
                    messtr = "Декабрь";
                    break;

            }
        }


        public ActionResult PodgotovkaKAktu (string Selection)
        {
            string data = "";
            string[] s = Selection.Split(';');
            string Adres = s[0].Replace(" ", "");
            string Month = s[2];
            string Year = s[1];
            string GEU = db.Adres.Where(a => a.Adress.Replace(" ", "").Equals(Adres)).Select(b => b.GEU).First() ;
            data = Adres + ";" + Year + ";" + Month + ";" + GEU;
            SformirovatAkt(data);
            return Json("SformirovatAkt",data);
        }

      //  [Authorize]
      //  [HttpPost]
        public ActionResult SformirovatAkt (string Selection)
        {
            if (Selection == null||Selection.Contains(";")==false)
            {
                return Json("Ничего не найдено");
            }
            string[] s = Selection.Replace("'","").Split(';');
            string Adres = "";
            string AdresAll = "";
            string Month = "";
            string Year = "";
            string GEU = "";
            try
            {
               
                Adres = s[0].Replace(" ", "");
                AdresAll = s[0];
                Month = s[2];
                Year = s[1];
              
            }
            catch
            {

            }
            int G = 0;

            if (s.Length < 4)
            {
                GEU = db.Adres.Where(h => h.Adress.Replace(" ", "").Equals(Adres)).Select(g => g.GEU).First();
            }
           else
            {
                GEU = s[3];
            }
            int M = Convert.ToInt16(Month);
            int Y = Convert.ToInt16(Year);
            bool GG = false;
            GEU = db.Adres.Where(h => h.Adress.Replace(" ", "").Equals(Adres)).Select(g => g.GEU).First();
           //GEU = G.ToString();
            if (Y<2021 )
            {
              
                GG = false;

            }
            else
            {
                GG = true;
            }

            Adres ADRdb = db.Adres.Where(f => f.Adress.Replace(" ", "").Equals(Adres)).Single();
            List<CompleteWork> CWdb = db.CompleteWorks.Where(a => a.WorkAdress.Replace(" ", "").Equals(Adres) && a.WorkDate.Year == Y && a.WorkDate.Month == M).ToList();
            List<VipolnennieUslugi> VUdb = db.VipolnennieUslugis.Include(a => a.Adres).Include(b => b.Usluga).Include(v=>v.Usluga.Periodichnost).Where(c => c.Adres.Adress.Replace(" ", "").Equals(Adres) && c.Date.Year == Y && c.Date.Month == M).OrderBy(x=>x.Usluga.Poryadok).ToList();
            GEU geudb = db.GEUs.Where(a => a.Name.Contains(GEU)).First();
            decimal summa = 0;

            foreach(VipolnennieUslugi VU in VUdb)
            {
                summa += VU.StoimostNaMonth;
            }

            MonthOpred(M, out Month);

            string path = Server.MapPath("~/Content/ASP" + Adres.Replace(" ", "").Replace("/"," к.") + "_" + Year.Remove(0, 2) + "_" + Month + ".xlsx"); //@"C:\inetpub\Otchets\ASP" + "X" + Year.Remove(0, 2) + "X" + Month + ".xlsx";//Server.MapPath("~\\ASP" +"X"+ Year.Remove(0,2) +"X"+ Month + ".xlsx");
            string filename = "ASP" + Adres.Replace(" ", "").Replace("/", " к.") + "_" + Year.Remove(0, 2) + "_" + Month + ".xlsx";
            //формируем удобочитаемый адрес 
            string AA = "";
            //string ADR = ADRdb.Ulica;
            //int ind = 0;


            //ADR = ADR.Replace("  ", "").Replace(" ", "-");

            // VUdb.Insert(1, VUdb[7]);
            // VUdb.RemoveAt(8);
            string Director = geudb.Director;
            string Doverennost = geudb.Doverennost;
            string GEUEU = geudb.EU.ToString();
            if (!GG)
            {
                GEUEU = GEU.Replace("ЖЭУ-","");
            }
            ExcelExportDomVipolnennieUslugi.SFORMIROVATAKT(CWdb, VUdb, Month, VUdb[0].Adres.GEU, Year, ADRdb.Ulica, ADRdb.Dom, Director, Doverennost, path, summa.ToString(),GEUEU,GG);


            string path2 = Url.Content("~/Content/ASP" +Adres.Replace(" ","").Replace("/", " к.") + "_"+ Year.Remove(0, 2) + "_" + Month + ".xlsx");

            // RedirectToAction("DownloadPS", new {path,filename});
            string data = path2; //+ filename;
            string contentType = "application/vnd.ms-excel";
            //return File(path2, contentType, filename);

             return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SformirovatAktYear(string Year, string AdresId,string GEU)
        {
           // string[] s = Selection.Split(';');
           
            //   string AdresAll = s[0];
            //   string Month = s[2];
            int AId = Convert.ToInt16(AdresId);
         //   string GEU = "";
          
                GEU = db.Adres.Where(h => h.Id==AId).Select(g => g.GEU).First();
           
       //     int M = Convert.ToInt16(Month);
            int Y = Convert.ToInt16(Year);

            Adres ADRdb = db.Adres.Where(f => f.Id == AId).Single();
            string Adres = ADRdb.Adress;
            List<CompleteWork> CWdb = db.CompleteWorks.Where(a => a.WorkAdress.Replace(" ", "").Equals(Adres) && a.WorkDate.Year == Y).ToList();
            List<VipolnennieUslugi> VUdb = db.VipolnennieUslugis.Include(a => a.Adres).Include(b => b.Usluga).Include(v => v.Usluga.Periodichnost).Where(c => c.Adres.Adress.Replace(" ", "").Equals(Adres) && c.Date.Year == Y ).OrderBy(x => x.Usluga.Poryadok).ToList();
            GEU geudb = db.GEUs.Where(a => a.Name.Contains(GEU)).First();
            decimal summa = 0;
            foreach (VipolnennieUslugi VU in VUdb)
            {
                summa += VU.StoimostNaMonth;
            }

            string path = Server.MapPath("~/Content/ASP" + Adres.Replace(" ", "").Replace("/", " к.") + "_" + Year.Remove(0, 2) + ".xlsx"); //@"C:\inetpub\Otchets\ASP" + "X" + Year.Remove(0, 2) + "X" + Month + ".xlsx";//Server.MapPath("~\\ASP" +"X"+ Year.Remove(0,2) +"X"+ Month + ".xlsx");
            string filename = "ASP" + Adres.Replace(" ", "").Replace("/", " к.") + "_" + Year.Remove(0, 2) + ".xlsx";
            //формируем удобочитаемый адрес 
            string AA = "";
            //string ADR = ADRdb.Ulica;
            //int ind = 0;
          

            //ADR = ADR.Replace("  ", "").Replace(" ", "-");

            // VUdb.Insert(1, VUdb[7]);
            // VUdb.RemoveAt(8);
            ExcelExportDomVipolnennieUslugi.SFORMIROVATAKTYEAR(CWdb, VUdb, VUdb[0].Adres.GEU, Year, ADRdb.Ulica, ADRdb.Dom, geudb.Director, geudb.Doverennost, path, summa.ToString(), geudb.EU.ToString());
            string path2 = Url.Content("~/Content/ASP" + Adres.Replace(" ", "").Replace("/", " к.") + "_" + Year.Remove(0, 2) + ".xlsx");
            // RedirectToAction("DownloadPS", new {path,filename});
            string data = path2; //+ filename;
            string contentType = "application/vnd.ms-excel";
            //return File(path2, contentType, filename);

            return Json(data,JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult OtchetMonth(string Month, string GEU, string Year,string Adres)
        {
            if (Request.Form["b1"] != null)
            {
                // Code for function 1 
           
            int m = 0;
            Obratno(Month,out m);
            IEnumerable<CompleteWork> cw = db.CompleteWorks;
            List<CompleteWork> ww = new List<CompleteWork>();
            if (User.IsInRole("Администратор") == false)
            {
                //если не админ то видно только твои данные
                foreach (CompleteWork W in cw)
                {
                    
                    string www = W.Agent.Replace(" ", "");
                    string xxx = User.Identity.Name.Replace(" ", "");
                    if (www.Equals(xxx)&&W.WorkDate.Month == m && W.WorkDate.Year.ToString().Equals(Year))
                    {
                        ww.Add(W);
                        GEU = xxx;
                    }
                }
               
            }
            else
            {//если админ то выводим выбранные данные
                foreach (CompleteWork W in cw)
                {
                    //Если агент из поля ввода ЖЭУ совпал
                    string www = W.Agent.Replace(" ", "");
                    if (www.Equals(GEU)&&W.WorkDate.Month ==m&&W.WorkDate.Year.ToString().Equals(Year))//выбираем данные за жэу-месяц-год
                    {
                        ww.Add(W);

                    }
                }
                   // ww = db.CompleteWorks.ToList();
                //если админ возвращаем все данные

            }
                string eu = db.GEUs.Where(x => x.Name.Equals("ЖЭУ-" + GEU.ToString())).Select(x=>x.EU).First().ToString();
            ExcelExportMonth.EXPORT(ww,Month,GEU,Year,eu);
                ViewBag.patch = GEU;
            return View();
            }
            else if (Request.Form["b2"] != null)
            {
                int m = 0;
                //Obratno(Month, out m);
                
                IEnumerable<CompleteWork> cw = db.CompleteWorks;
                List<CompleteWork> ww = new List<CompleteWork>();
                if (User.IsInRole("Администратор") == false)
                {
                    //если не админ то видно только твои данные
                    foreach (CompleteWork W in cw)
                    {

                        string www = W.Agent.Replace(" ", "");
                        string xxx = User.Identity.Name.Replace(" ", "");
                        if (www.Equals(xxx)  && W.WorkDate.Year.ToString().Equals(Year))
                        {
                            ww.Add(W);
                            GEU = xxx;
                        }
                    }

                }
                else
                {//если админ то выводим выбранные данные
                    foreach (CompleteWork W in cw)
                    {
                        //Если агент из поля ввода ЖЭУ совпал
                        string www = W.Agent.Replace(" ", "");
                        if (www.Equals(GEU) && W.WorkDate.Year.ToString().Equals(Year))//выбираем данные за жэу-месяц-год
                        {
                            ww.Add(W);

                        }
                    }
                    // ww = db.CompleteWorks.ToList();
                    //если админ возвращаем все данные

                }
                Month = "год";
                string eu = db.GEUs.Where(x => x.Name.Equals("ЖЭУ-" + GEU.ToString())).Select(x => x.EU).First().ToString();
                ExcelExportMonth.EXPORT(ww, Month, GEU, Year,eu);
                ViewBag.patch = GEU;
                return View();
            }
            else if (Request.Form["b3"] != null)
            {
                // Code for function 3 

                int m = 0;
                Obratno(Month, out m);
                IEnumerable<CompleteWork> cw = db.CompleteWorks;
                List<CompleteWork> ww = new List<CompleteWork>();
                if (User.IsInRole("Администратор") == false)
                {
                    //если не админ то видно только твои данные
                    foreach (CompleteWork W in cw)
                    {

                        string www = W.Agent.Replace(" ", "");
                        string xxx = User.Identity.Name.Replace(" ", "");
                        string adr = W.WorkAdress.Replace(" ","");
                        if (www.Equals(xxx) && W.WorkDate.Month == m && W.WorkDate.Year.ToString().Equals(Year) && adr.Equals(Adres.Replace(" ","")))
                        {
                            ww.Add(W);
                            GEU = xxx;
                        }
                    }

                }
                else
                {//если админ то выводим выбранные данные
                    foreach (CompleteWork W in cw)
                    {
                        //Если агент из поля ввода ЖЭУ совпал
                        string www = W.Agent.Replace(" ", "");
                        string adr = W.WorkAdress.Replace(" ","");
                        if (www.Equals(GEU) && W.WorkDate.Month == m && W.WorkDate.Year.ToString().Equals(Year)&&adr.Equals(Adres.Replace(" ","")))//выбираем данные за жэу-месяц-год
                        {
                            ww.Add(W);

                        }
                    }
                    // ww = db.CompleteWorks.ToList();
                    //если админ возвращаем все данные

                }
                if (ww.Count == 0) { ViewBag.Adres = Adres; ViewBag.GEU = GEU; ViewBag.Month = Month; ViewBag.Year = Year; return View("Error"); }
                string eu = db.GEUs.Where(x => x.Name.Equals("ЖЭУ-" + GEU.ToString())).Select(x => x.EU).First().ToString();
                ExcelExportDom.EXPORT(ww, Month, GEU, Year,Adres);
                ViewBag.patch = GEU;
                return View();
            }
            return View();
        }
        public ActionResult Download(string GEU)
        {
            string file = @"C:\\inetpub\\Otchets\\OtchetMonth"+GEU+".xlsx";
            string filename = "OtchetMonth"+GEU+".xlsx";
            string contentType = "application/vnd.ms-excel";
                       //патч,тип файла,новое имя файла
            return File(file, contentType, filename);//отправка файла пользователю (сохранение, скачать файл)
        }
        public ActionResult DownloadPS(string path,string filename)
        {
           
            string contentType = "application/vnd.ms-excel";
            //патч,тип файла,новое имя файла
            return File(path, contentType, filename);//отправка файла пользователю (сохранение, скачать файл)
        }
        public ActionResult DownloadS(string selection)
        {
            string[] s = selection.Split(';');
            string path = s[0];
            string filename = s[1];

            string contentType = "application/vnd.ms-excel";
            //патч,тип файла,новое имя файла
            return File(path, contentType, filename);//отправка файла пользователю (сохранение, скачать файл)
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompleteWork completeWork = db.CompleteWorks.Find(id);
            if (completeWork == null)
            {
                return HttpNotFound();
            }
            return View(completeWork);
        }

        // GET: CompleteWorks/Create

        public ActionResult AddNewCompleteWork()
        {
            return View();
        }

        public ActionResult CreateFirst()
        {
            
            string GEU = "ЖЭУ-5";
            if (User.Identity.Name.Contains("ЖЭУ"))
            {
                GEU = User.Identity.Name.Replace(" ", "");
            }
            List<string> Adresadb = db.Adres.Where(a => a.GEU.Contains(GEU)).Select(c=>c.Adress.Replace(" ","")).ToList(); 
            ViewBag.Adresa = Adresadb;

            return View();
        }

        //the autocomplete 
        static List<Adres> adr = new List<Adres>();
        //выводим список адресов ВСЕ РАБОТАЕТ ЕСЛИ ВВОДИТЬ ЗАГЛАВНЫМИ так как все дома заглавными вбиты
        public ActionResult AutocompleteSearch(string term)
        {
            adr = db.Adres.ToList();
            string Name = User.Identity.Name;
            if (User.IsInRole("Пользователь")) {
                
                    var models = adr.Where(a => a.Adress.Contains(term)&&a.GEU!=null&& a.GEU.Contains(Name))
                                    .Select(a => new { value = a.Adress })
                                    .Distinct();
                   
                    return Json(models, JsonRequestBehavior.AllowGet);
                
               
                    
            }
            else
            {
                var models = adr.Where(a => a.Adress.Contains(term))
                            .Select(a => new { value = a.Adress })
                            .Distinct();
                return Json(models, JsonRequestBehavior.AllowGet);
            }
            


        }
        //автопоиск измерения при выборе работы
        [HttpPost]
        public ActionResult PoiskIzmereniya(string term)
        {
            //тестирую вариант поиска по БД не заходит в блок скрипт не робит
            var models = db.Works.Where(a => a.Name.Contains(term))
                            .Select(a => new { value = a.Izmerenie });
                            

            return Json(models, JsonRequestBehavior.AllowGet);
        }
        //Не используется висит как шаблон для автопоиска
        [HttpPost]
        public ActionResult BookSearch(string name)
        {
            var allbooks = db.Adres.Where(a => a.Adress.Contains(name)).ToList();
            if (allbooks.Count <= 0)
            {
                return HttpNotFound();
            }
            return PartialView(allbooks);
        }

        // return View();

        // POST: CompleteWorks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WorkWorkId,Agent,WorkGroup,WorkAdress,WorkNumber,Izmerenie,WorkDate")] CompleteWork completeWork)
        {
           
            //if (ModelState.IsValid)
            //{
                bool go = false;
                int ind = 0;
               
               
                int ID = Convert.ToInt16(completeWork.WorkWorkId);
                Work W = db.Works.Find(ID);
                completeWork.WorkCode = W.Code;
                completeWork.WorkIzmerenie = W.Izmerenie;
                completeWork.WorkName = W.Name;
                completeWork.Agent = User.Identity.GetUserName();
                completeWork.Date = System.DateTime.Now.Date;
            //completeWork.Date = Convert.ToDateTime(26);
            //  LoginViewModel lvm = new LoginViewModel();


            //completeWork.Agent = "Нет";
            
                db.CompleteWorks.Add(completeWork);
                db.SaveChanges();
                return RedirectToAction("Index");
           // }

          //  return View(completeWork);
        }

        public ActionResult NewCompleteWorkMass()
        {
            List<SelectListItem> Ad = new List<SelectListItem>();
            List<string> GEUAll = new List<string>();
            string GEU = "ЖЭУ-5";
            if (User.Identity.Name.Contains("ЖЭУ"))
            {
                GEU = User.Identity.Name.Replace(" ", "");
                GEUAll.Add(GEU);
            }
            else
            {
                GEUAll = db.GEUs.Select(b => b.Name).ToList();
            }
            GEUAll.Sort();
            int EUId = db.GEUs.Where(x => x.Name.Equals(GEU)).Select(x => x.EU).First();
            List<string> Adresadb = db.Adres.Where(a => a.EUId == EUId&&a.MKD).Select(c => c.Adress).ToList();
            Adresadb.Sort();
            Adresadb.Insert(0, "Все адреса");
            ViewBag.Adresa = Adresadb;
            ViewBag.GEU = GEUAll;

            return View();//передаем лист моделей в представление
        }

        public ActionResult NewCompleteWork()
        {
            List<SelectListItem> Ad = new List<SelectListItem>();
            List<string> GEUAll = new List<string>();
            string GEU = "ЖЭУ-5";
            if (User.Identity.Name.Contains("ЖЭУ"))
            {
                GEU = User.Identity.Name.Replace(" ", "");
                GEUAll.Add(GEU);
            }
            else
            {
                GEUAll = db.GEUs.Select(b => b.Name).ToList();
            }
            GEUAll.Sort();
            
            List<string> Adresadb = db.Adres.Where(a => a.GEU.Contains(GEU)).Select(c=>c.Adress).ToList();
            Adresadb.Sort();
            Adresadb.Insert(0, "Все адреса");
            ViewBag.Adresa = Adresadb;
            ViewBag.GEU = GEUAll;
            
            return View();//передаем лист моделей в представление
        }

        public ActionResult PoiskRabotPoGruppe(string selection)
        {

            List<string> WNdb = db.Works.Where(a => a.Group.Contains(selection)).Select(b => b.Name).ToList();
            List<int> WIDdb = db.Works.Where(a => a.Group.Contains(selection)).Select(b => b.WorkId).ToList();
            for (int i=0;i<WNdb.Count;i++)
            {
                WNdb[i] += ";" + WIDdb[i].ToString();
            }
            WNdb.Sort();
            return Json(WNdb);
        }
        public ActionResult PoiskIzmereniaPoRabote(string selection)
        {
            int ID = Convert.ToInt32(selection);
            string Worksdb = db.Works.Where(a => a.WorkId==ID).Select(b => b.Izmerenie).First();
            return Json(Worksdb);
        }

        //Y: Year, M: Month, A: Adres, G: GEU, W: Work, I: Izmerenie, K: Kolvo 
        public ActionResult AddNewWork(int Y,string M,string A, string G, int W, string I, string K, string WG, string Multiadres = "X")
        {
          //  var dbAdres = db.Adres;
            List<string> Adresa = new List<string>();
            if (Multiadres.Equals("X")==false)
            {
                string[] S = Multiadres.Split(',');
                foreach (string s in S)
                {
                   // int i = 0;
                    try
                    {
                       // i = dbAdres.Where(x => x.Adress.Equals(s)).Select(x => x.Id).First();
                        Adresa.Add(s);
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
               // int i = dbAdres.Where(x => x.Adress.Equals(A)).Select(x => x.Id).First();
                Adresa.Add(A);
            }

            //адрес дата группа измерение ИД номер 
            string data = "";
            Random R = new Random();

          
            int Mon = 0;
            Obratno(M, out Mon);
            //   MonthOpred(M, out M);
            if (Adresa.Count>0)
            {
                foreach (string z in Adresa)
                {
                    try
                    {
                        // string[] s = selection.Split(';');
                        CompleteWork CW = new CompleteWork();
                        CW.WorkAdress = z;

                        int Day = R.Next(29) + 1;
                        CW.WorkDate = new DateTime(Y, Mon, Day);
                        CW.WorkGroup = WG;
                        CW.WorkIzmerenie = I;
                        CW.WorkWorkId = W;
                        CW.WorkNumber = Convert.ToDecimal(K);
                        CW.Date = DateTime.Now;
                        CW.Agent = G;
                        CW.KtoSohranil = User.Identity.Name.Replace(" ", "");
                        Work Work = db.Works.Find(CW.WorkWorkId);
                        CW.WorkCode = Work.Code;
                        CW.WorkName = Work.Name;
                        db.CompleteWorks.Add(CW);
                        db.SaveChanges();
                        string Month = "";
                        MonthOpred(CW.WorkDate.Month, out Month);
                        // < p > Адрес </ p >           < p > Наименование </ p >            < p > Количество </ p >            < p > Измерение </ p >            < p > Дата </ p >           < p > Агент </ p >
                        data = CW.WorkAdress + ";" + Work.Name + ";" + CW.WorkNumber + ";" + CW.WorkIzmerenie + ";" + CW.WorkDate + ";" + CW.Agent;
                    }
                    catch
                    {
                        data = "0;alert-danger;Ошибка в добавлении работы!";
                    }
                }
            }

            return Json(data);
        }


        public ActionResult SaveCompleteWork (string selection)
        {
            //адрес дата группа измерение ИД номер 
            string data = "";
           
            try
            {
                string[] s = selection.Split(';');
                CompleteWork CW = new CompleteWork();
                CW.WorkAdress = s[0];
                CW.WorkDate = Convert.ToDateTime(s[1]);
                CW.WorkGroup = s[2];
                CW.WorkIzmerenie = s[4];
                CW.WorkWorkId = Convert.ToInt32(s[3]);
                CW.WorkNumber = Convert.ToDecimal(s[5]);
                CW.Date = DateTime.Now;
                CW.Agent = s[6].Replace(" ", "");
                CW.KtoSohranil = User.Identity.Name.Replace(" ","");
                Work W = db.Works.Find(CW.WorkWorkId);
                CW.WorkCode = W.Code;
                CW.WorkName = W.Name;
                db.CompleteWorks.Add(CW);
                db.SaveChanges();
                string Month = "";
                MonthOpred(CW.WorkDate.Month, out Month);
                data = "1;alert-success;Выполненная работа "+W.Name+" от "+Month+" "+CW.WorkDate.Year +" успешно добавлена!";
            }
            catch
            {
                data = "0;alert-danger;Ошибка в добавлении работы!";
            }

             
            return Json(data);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFirst([Bind(Include = "WorkAdress,WorkGroup")] CompleteWork completeWork)
        {
            
                //if (ModelState.IsValid)
               // {
                    //Adress = completeWork.WorkAdress;
                    
                    string Group = completeWork.WorkGroup;
                    int g = 0;
                    if (Group.Equals("ТО конструктивных элементов"))
                    {
                        g = 1;
                    }
                    else
                    {
                        g = 2;
                    }
                    ViewBag.g = g;
                    ViewBag.Adress = completeWork.WorkAdress;
                    ViewBag.Group = Group;
                    if (g != 0)
                    {
                        List<string> name = new List<string>();
                        List<string> code = new List<string>();
                   
                  //  SelectList workers = new SelectList(db.Works, "WorkId", "Name");
                   
                  //  ViewBag.workers = workers;


                  
                    List<SelectListItem> S1 = new List<SelectListItem>();
                    List<string[]> S2 = new List<string[]>();
                    foreach (Work w in db.Works)
                        {
                            string[] codes = w.Code.Split('-');
                            if (Convert.ToInt16(codes[0]) == g)
                            {
                                name.Add(w.Name);
                                code.Add(w.Code);
                            SelectListItem SS1 = new SelectListItem();
                            SS1.Text = w.Name;
                            SS1.Value = w.WorkId.ToString();
                            string[] SS2 = new string[3];
                            SS2[0] = w.Name;
                            SS2[1] = w.Izmerenie;
                            SS2[2] = w.WorkId.ToString();
                            S2.Add(SS2);
                            S1.Add(SS1);
                        }
                        }
                     //SelectList SS = new SelectList(S1);
                    SelectList workers = new SelectList(S1,"Value","Text");
                    ViewBag.workers = workers;
                    ViewBag.all = S2;
                    //ViewBag.name = name;
            //        ViewBag.code = code;
            //        ViewBag.group = group;
            //        ViewBag.WorkId = WorkId;
            //        ViewBag.izmerenie = izmerenie;
                    
                       
                        return View("Create");//передаем лист моделей в представление
                    //}
                    //else
                   // {
                        return View("Index");
                   // }

                    //return RedirectToAction("Create");
                }
            
           

            return View(completeWork);
        }

        // GET: CompleteWorks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompleteWork completeWork = db.CompleteWorks.Find(id);
            if (completeWork == null)
            {
                return HttpNotFound();
            }
            return View(completeWork);
        }

        // POST: CompleteWorks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,WorkName,WorkCode,WorkIzmerenie,Agent")] CompleteWork completeWork)
        {
            if (ModelState.IsValid)
            {
                db.Entry(completeWork).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(completeWork);
        }

        // GET: CompleteWorks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompleteWork completeWork = db.CompleteWorks.Find(id);
            if (completeWork == null)
            {
                return HttpNotFound();
            }
            return View(completeWork);
        }

        // POST: CompleteWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompleteWork completeWork = db.CompleteWorks.Find(id);
            db.CompleteWorks.Remove(completeWork);
            db.SaveChanges();
            return RedirectToAction("IndexMain");
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
