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
using Opredelenie;

namespace GKHNNC.Controllers
{
    public class ASControlsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: ASControls
        public ActionResult Index()
        {
            DateTime Date = DateTime.Now;
            ViewBag.Date = Date;

            //берем автомобили вышедшие в рейс
             List<int> ASId = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).Include(x => x.Avto).Select(y=>y.Avto.Id).ToList();
              List<Avtomobil> Avtos = new List<Avtomobil>();
              foreach (int i in ASId)
              {
                  Avtos.Add(db.Avtomobils.Where(x => x.Id == i).First());
              }

            //берем все записи контрола за этот день и пробиваем по базе АС24
            List<ASControl> ASC = new List<ASControl>();
            try
            {
                ASC = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).Include(x => x.Avto).Include(x=>x.Avto.Type).Include(x=>x.Voditel).OrderByDescending(x=>x.Id).ToList();
            }
            catch(Exception e)
            {            }
            //берем все записи ас24 за день
            List<AS24> AS24db = new List<AS24>();
            List<SelectListItem> HourSnyatia = new List<SelectListItem>();
            try
            {
               
               AS24db = db.AS24.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).ToList();
                HourSnyatia = AS24db.Select(x => x.Date.Hour).Distinct().OrderBy(x => x).Select(a => new SelectListItem { Value = a.ToString(), Text = a.ToString() + ":00" }).ToList();

            }
            catch { }

            ViewBag.HourSnyatia = HourSnyatia;

            //пробиваем каждую запись и суммируем километраж и ДУТ
            List<int> Nabludenii = new List<int>();
            List<int> NoNabludenii = new List<int>();
            List<string> Dut = new List<string>();//пишем сюда все дуты и все пробеги
            List<string> Km = new List<string>();
            List<int> RealGo = new List<int>();
            List<int> RealEnd = new List<int>();
            List<string> Zag = new List<string>();
            List<string> TimeDut = new List<string>();
            List<decimal> Zapravleno = new List<decimal>();
            foreach (ASControl AC in ASC)
            {
                int counter = 0;
                int nocounter = 0;//считаем сколько машина стояла в гараже 
                decimal KMAS = 0;
                decimal DUT = 0;
                List<AS24> A24 = new List<AS24>();
                try
                {//берем все записи с данной тачкой
                    A24 = AS24db.Where(x => x.AvtoId == AC.AvtoId && x.Date.Hour >= AC.Date.Hour).OrderBy(y=>y.Date).ToList();
                }
                catch { }
                string SS = "";
                try
                {
                    SS = db.AutoScans.Where(x => x.AvtoId == AC.AvtoId).OrderBy(x=>x.Date).Select(x=>x.Zagrugeno).First();
                }
                catch
                {

                }
                Zag.Add(SS);
                ViewBag.NoSvaz = "";
                //ViewBag.Mesta = "";
                AC.Mesta = new List<string>();
                AC.Mesta.Add("Движений нет.");
                AC.NoSvaz = new List<string>();
                //AC.NoSvaz.Add("Потерь нет.");
                int RealViezd = 0; //Час реального выезда 
                int RealEndd = 0;
                string kmkm = "";
                string dutdut = "";
                string timedut = "";
                decimal zap = 0;
                foreach (AS24 A in A24)
                {
                    bool Go = true;


                    if (A.Mesta!= null&&A.Mesta.Contains('@'))
                    {
                        string[] S = A.Mesta.Split('@');


                        AC.Mesta = new List<string>();
                        AC.Mesta.Add("Не двигается");
                        if (A.Mesta != "" && A.Mesta != null)
                        {
                            AC.Mesta = new List<string>();
                            if (S.Length > 0 && S[S.Length - 1].Contains("Молодежибульвар,36") && RealViezd > 0)
                            {
                                AC.Mesta = new List<string>();
                                AC.Mesta.Add("Вернулся на стоянку");
                                Go = false;
                                RealEndd = A.Date.Hour;
                            }
                            else
                            {
                                if (A.KM < 1 && (A.Mesta.Contains("Молодежибульвар,36") || A.Mesta.Contains("----")) && A.Mesta.Contains("Заправка") == false && RealViezd == 0)
                                {
                                    AC.Mesta.Add("На стоянке");
                                    Go = false;
                                }
                                else
                                {
                                    if (RealViezd == 0)
                                    {
                                        RealViezd = A.Date.Hour;
                                    }
                                    AC.Mesta.AddRange(A.Mesta.Split(';'));

                                }
                            }

                        }
                        else
                        {
                            //считаем что он на базе пока не засечен первый выезд
                            if (RealViezd == 0)
                            {
                                AC.Mesta = new List<string>();
                                AC.Mesta.Add("На стоянке");
                                // Go = false;
                            }
                        }
                    }
                    //A.NoSvaz = A.NoSvaz.Replace(" ", "");
                    if (A.NoSvaz != "" && A.NoSvaz != null&&Go)
                    {
                        //AC.NoSvaz = new List<string>();
                        AC.NoSvaz.AddRange(A.NoSvaz.Split(';'));
                        AC.NoSvaz.RemoveAt(AC.NoSvaz.Count-1);
                        nocounter++;
                    }

                        kmkm += A.KM + ";";
                        dutdut += A.DUT + ";";
                    zap += A.Zapravleno;
                    KMAS += A.KM;
                    DUT += A.DUT;
                    timedut += A.Date.Hour + ";";

                    // ViewBag.Mesta = A.Mesta;//места, посещённые автомобилем
                    counter++;
                }
                if (kmkm.Length > 0)
                {
                    kmkm = kmkm.Remove(kmkm.Length - 1, 1);
                }
                if (dutdut.Length > 0)
                {
                    dutdut = dutdut.Remove(dutdut.Length - 1, 1);
                }

                if(AC.DateClose>AC.Date)
                {
                    RealEndd = AC.DateClose.Hour;
                }
                if (AC.Kontrol)
                {
                    RealViezd = AC.Date.Hour;
                }
                AC.Zapravleno = zap;
                TimeDut.Add(timedut);
                Km.Add(kmkm);
                Dut.Add(dutdut);
                NoNabludenii.Add(nocounter);
                Nabludenii.Add(counter);
                RealGo.Add(RealViezd);
                RealEnd.Add(RealEndd);
                AC.KMAS = KMAS;
                AC.DUT = DUT;

            }

            //Берем все не закрытые записи за предыдущие дни. Километраж и ДУТ автоматически взят из автоскана при ночном обновлении.
            List<ASControl> ASCOld = new List<ASControl>();
            try
            {
                ASCOld = db.ASControls.Where(x => x.DateClose<x.Date&&x.Date.Day!=Date.Day).Include(x => x.Avto).Include(x=>x.Avto.Type).OrderByDescending(x=>x.Date).ToList();
            }
            catch { }
            ViewBag.TimeDut = TimeDut;
            ViewBag.Km = Km;//список всех пробегов и километражей разделенных ;
            ViewBag.Dut = Dut;
            ViewBag.Zagrugeno = Zag;
            ViewBag.Counter = ASC.Count();//сохраняем количество записей за текущий день.
            ASC.AddRange(ASCOld);//добавляем в конец списка все не закрытые записи
            ViewBag.RealGo = RealGo;//массив реальных выездов. 0 значит еще не выехал.
            ViewBag.RealEnd = RealEnd;//массив реальных возвратов. 0 значит еще не вернулся.
            ViewBag.Nabludenii = Nabludenii;//массив с числом наблюдений по часам
            ViewBag.NoNabludenii = NoNabludenii;//массив отброшенных наблюдений когда машина стояла на парковке
            //выводим только автомобили с глонасс
            ViewBag.Avto = db.Avtomobils.Where(x=>x.Glonass==true).OrderBy(x => x.Number).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Number, }).ToList();
            ViewBag.Voditel = db.Voditels.OrderBy(x => x.Name).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name, }).ToList();
            ViewBag.Zakazchik = db.Zakazchiks.OrderBy(x => x.Id).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name, }).ToList();
            return View(ASC);
        }
        public ASControlView RaschetAvto(ASControl AS)
        {
            ASControlView AC = new ASControlView();
            AC.Id = AS.Id;
            AC.AvtoId = AS.AvtoId;
            AC.Avto = AS.Avto;
            AC.Date = AS.Date;
            AC.DateClose = AS.DateClose;
            AC.Go = AS.Go;
            AC.Kontrol = AS.Kontrol;
            AC.Pricep = AS.Pricep;
            AC.Primech = AS.Primech;
            AC.ALLKm = new List<decimal>();
            AC.ALLDut = new List<decimal>();
            AC.TimeDut = new List<int>();
            AC.AllActions = new List<string>();
            AC.AllRashod = new List<string>();
            AC.Zapravleno = AS.Zapravleno;
            AC.Start = AS.Start;
            AC.End = AS.End;
            string Zag = "";
           
           
                int counter = 0;
                int nocounter = 0;//считаем сколько машина стояла в гараже 
                decimal KMAS = 0;
                decimal DUT = 0;
                List<AS24> A24 = new List<AS24>();
                try
                {//берем все записи с данной тачкой
                DateTime Date = AC.Date.AddDays(1);
                    A24 = db.AS24.Where(x => x.AvtoId == AC.AvtoId &&x.Date>=AC.Date&&x.Date<=AC.DateClose).OrderBy(y => y.Date).ToList();
                }
                catch { }
                string SS = "";
                try
                {
                    SS = db.AutoScans.Where(x => x.AvtoId == AC.AvtoId).OrderBy(x => x.Date).Select(x => x.Zagrugeno).First();
                }
                catch
                {

                }
                Zag=SS;
                ViewBag.NoSvaz = "";
                //ViewBag.Mesta = "";
                AC.Mesta = new List<string>();
                AC.Mesta.Add("Движений нет.");
                AC.NoSvaz = new List<string>();
                //AC.NoSvaz.Add("Потерь нет.");
                int RealViezd = 0; //Час реального выезда 
                int RealEndd = 0;
            decimal zap = 0;
            if (A24.Count > 0)
            {
                AC.StartAS24 = A24[0].Start;//начальный уровень топлива
                AC.EndAS24 = A24[A24.Count - 1].End;//конечный уровень топлива

                if (AC.Start == 0) { AC.Start = AC.StartAS24; }
                if (AC.End == 0) { AC.End = AC.EndAS24; }

                foreach (AS24 A in A24)
                {
                    bool Go = true;


                    if (A.Mesta != null && A.Mesta.Contains(';'))
                    {
                        string[] S = A.Mesta.Split(';');
                        

                        AC.Mesta = new List<string>();
                        AC.Mesta.Add("Не двигается");
                        if (A.Mesta != "" && A.Mesta != null)
                        {
                            AC.Mesta = new List<string>();
                            if (S.Length > 2 && (S[S.Length-1].Contains("Молодежибульвар,36")|| S[S.Length - 2].Contains("Молодежибульвар,36")) && RealViezd > 0&&RealEndd==0)
                            {
                                AC.Mesta = new List<string>();
                                AC.Mesta.Add("Вернулся на стоянку");
                                Go = false;
                                RealEndd = A.Date.Hour;
                            }
                            else
                            {
                                if (A.KM < 1 && (A.Mesta.Contains("Молодежибульвар,36") || A.Mesta.Contains("----")) && A.Mesta.Contains("Заправка") == false && RealViezd == 0)
                                {
                                    AC.Mesta.Add("На стоянке");
                                    Go = false;
                                }
                                else
                                {
                                    if (RealViezd == 0)
                                    {
                                        RealViezd = A.Date.Hour;
                                    }
                                    if (RealEndd > 0)
                                    {
                                        RealEndd = 0;
                                    }

                                        AC.Mesta.AddRange(A.Mesta.Split(';'));
                                        AC.AllActions.AddRange(A.Mesta.Split(';'));
                                    
                                }
                            }

                        }
                        else
                        {
                            //считаем что он на базе пока не засечен первый выезд
                            if (RealViezd == 0)
                            {
                                AC.Mesta = new List<string>();
                                AC.Mesta.Add("На стоянке");
                                // Go = false;
                            }
                        }
                    }
                    //если пошли следующие сутки
                    if (A.Date.Day != AS.Date.Day)
                    {
                        Go = false;
                    }
                    //A.NoSvaz = A.NoSvaz.Replace(" ", "");
                    if (A.NoSvaz != "" && A.NoSvaz != null && Go)
                    {
                        //AC.NoSvaz = new List<string>();
                        AC.NoSvaz.AddRange(A.NoSvaz.Split(';'));
                        AC.NoSvaz.RemoveAt(AC.NoSvaz.Count - 1);
                        nocounter++;
                    }
                    if (Go)
                    {
                        AC.ALLKm.Add(A.KM);
                        AC.ALLDut.Add(A.DUT);
                        zap += A.Zapravleno;
                        KMAS += A.KM;
                        DUT += A.DUT;
                        AC.TimeDut.Add(A.Date.Hour);


                        // ViewBag.Mesta = A.Mesta;//места, посещённые автомобилем
                        counter++;
                    }
                }
            }
            if (KMAS > 0)
            {
                AC.SredniiRashodDay = DUT / KMAS * 100;
            }
                //Заправлено факт это данные с заправки а заправлено зап это данные с ДУТ
                AC.ZapravlenoFact = AC.Zapravleno;
            AC.Zapravleno = zap; 
                //считаем средний расход за 100 наблюдений
            List<AS24> AllRashod = new List<AS24>();
            try
            {
                AllRashod = db.AS24.Where(x => x.AvtoId == AC.AvtoId && x.DUT > 5 && x.KM > 5).OrderByDescending(x=>x.Date).Take(100).ToList();
            }
            catch { }
            if (AllRashod.Count > 0)
            {
                decimal summDut = 0;
                decimal summKm = 0;
                decimal max = 0;
                for (int i=0;i<AllRashod.Count;i++)
                {
                    if (max< AllRashod[i].DUT/ AllRashod[i].KM*100) { max = AllRashod[i].DUT / AllRashod[i].KM * 100; }
                    summDut += AllRashod[i].DUT;
                    summKm += AllRashod[i].KM;
                    AC.AllRashod.Add(AllRashod[i].DUT.ToString() + ";" + AllRashod[i].KM.ToString() + ";" + AllRashod[i].Date.ToString());
                }
                AC.SredniiRashod = (summDut / summKm )*100;
                AC.MaxRashod = max;
            }
            try
            {
                DateTime D = DateTime.Today.AddDays(-1);
                AC.SredniiRashodVchera = db.AS24.Where(x => x.Date >= D && x.Date <= DateTime.Today&&x.DUT>0&&x.KM>0).Sum(x => x.DUT)/ db.AS24.Where(x => x.Date >= D && x.Date <= DateTime.Today && x.DUT > 0 && x.KM > 0).Sum(x => x.KM)*100;

                          }
            catch { }


            AC.NoNabludenii = nocounter;
                AC.Nabludenii=counter;
                AC.RealGo = RealViezd;
                AC.RealEnd = RealEndd;
                AC.KMAS = KMAS;
                AC.DUT = DUT;
            AC.MarkaAvto = AC.Avto.Marka.Name;
            AC.TypeAvto = AC.Avto.Type.Type;

            
            return AC;
        }

        
       
     
        
      public ActionResult Info(string selection)
        {
            string[] S = selection.Split(';');
            int id = Convert.ToInt32(S[0]);
            DateTime date = DateTime.Now;
            ASControl AS = db.ASControls.Where(x => x.Id == id).Include(x => x.Avto).Include(x=>x.Avto.Marka).Include(x => x.Avto.Type).First();//находим запись по айдишнику
            ASControlView ASV = new ASControlView();

            ASV = RaschetAvto(AS);
            List<SelectListItem> HourSnyatia = new List<SelectListItem>();
            try
            {
                HourSnyatia = db.AS24.Where(x => x.Date >= ASV.Date).Select(x => x.Date.Hour).Distinct().OrderBy(x => x).Select(a => new SelectListItem { Value = a.ToString(), Text = a.ToString() + ":00" }).ToList();
            }
            catch { }
            ViewBag.Date = AS.Date;
            ViewBag.HourSnyatia = HourSnyatia;
            return View(ASV);
        }

        public ActionResult Old(string selection)
        {
            List<ASControlView> ASV = new List<ASControlView>();
            DateTime date = new DateTime();

            if (selection!=null)
            {
                string[] S = selection.Split('/');
                
               date = new DateTime(Convert.ToInt16(S[0]), Convert.ToInt16(S[1]), Convert.ToInt16(S[2]));
             
            }
            else
            {
                date = DateTime.Today.AddDays(-1);
            }
            List<ASControl> AS = db.ASControls.Where(x => x.Date.Year == date.Year&&x.Date.Month==date.Month&&x.Date.Day==date.Day).Include(x => x.Avto).Include(x => x.Avto.Marka).Include(x => x.Avto.Type).ToList();//находим запись по айдишнику
            for (int i = 0; i < AS.Count; i++)
            {
                ASV.Add(RaschetAvto(AS[i]));
            }
            List<SelectListItem> HourSnyatia = new List<SelectListItem>();
            try
            {
                HourSnyatia = db.AS24.Where(x => x.Date >= date).Select(x => x.Date.Hour).Distinct().OrderBy(x => x).Select(a => new SelectListItem { Value = a.ToString(), Text = a.ToString() + ":00" }).ToList();
            }
            catch { }
            //заполняем месяца
            



            ViewBag.Date = date;
            ViewBag.HourSnyatia = HourSnyatia;
            return View(ASV);
        }

        // GET: ASControls
        public ActionResult Mechanic()
        {
            DateTime Date = DateTime.Now;
            ViewBag.Date = Date;

            //берем автомобили вышедшие в рейс
            List<int> ASId = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).Select(y => y.Avto.Id).ToList();
            List<Avtomobil> Avtos = new List<Avtomobil>();
            foreach (int i in ASId)
            {
                Avtos.Add(db.Avtomobils.Where(x => x.Id == i).First());
            }

            //берем все записи контрола за этот день и пробиваем по базе АС24
            List<ASControl> ASC = new List<ASControl>();
            try
            {
                ASC = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).Include(x => x.Avto).Include(x => x.Avto.Type).Include(x => x.Voditel).ToList();
            }
            catch (Exception e)
            { }
            //берем все записи ас24 за день
            List<AS24> AS24db = new List<AS24>();
            List<SelectListItem> HourSnyatia = new List<SelectListItem>();
            try
            {

                AS24db = db.AS24.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).ToList();
                HourSnyatia = AS24db.Select(x => x.Date.Hour).Distinct().OrderBy(x => x).Select(a => new SelectListItem { Value = a.ToString(), Text = a.ToString() + ":00" + "(час:мин)", }).ToList();

            }
            catch { }

            ViewBag.HourSnyatia = HourSnyatia;

            //пробиваем каждую запись и суммируем километраж и ДУТ
            List<int> Nabludenii = new List<int>();
            foreach (ASControl AC in ASC)
            {
                int counter = 0;
                decimal KMAS = 0;
                decimal DUT = 0;
                List<AS24> A24 = new List<AS24>();
                try
                {//берем все записи с данной тачкой
                    A24 = AS24db.Where(x => x.AvtoId == AC.AvtoId && x.Date.Hour >= AC.Date.Hour).OrderBy(y => y.Date).ToList();
                }
                catch { }
                ViewBag.NoSvaz = "";
                //ViewBag.Mesta = "";
                AC.Mesta = new List<string>();
                AC.Mesta.Add("Движений нет.");
                AC.NoSvaz = new List<string>();
                //AC.NoSvaz.Add("Потерь нет.");
                foreach (AS24 A in A24)
                {
                    if (A.Mesta != "" && A.Mesta != null)
                    {
                        AC.Mesta = new List<string>();
                        AC.Mesta.AddRange(A.Mesta.Split(';'));

                    }
                    //A.NoSvaz = A.NoSvaz.Replace(" ", "");
                    if (A.NoSvaz != "" && A.NoSvaz != null)
                    {
                        //AC.NoSvaz = new List<string>();
                        AC.NoSvaz.AddRange(A.NoSvaz.Split(';'));
                        AC.NoSvaz.RemoveAt(AC.NoSvaz.Count - 1);

                    }

                    KMAS += A.KM;
                    DUT += A.DUT;


                    // ViewBag.Mesta = A.Mesta;//места, посещённые автомобилем
                    counter++;
                }
                Nabludenii.Add(counter);
                AC.KMAS = KMAS;
                AC.DUT = DUT;

            }

            //Берем все не закрытые записи за предыдущие дни. Километраж и ДУТ автоматически взят из автоскана при ночном обновлении.
            List<ASControl> ASCOld = new List<ASControl>();
            try
            {
                ASCOld = db.ASControls.Where(x => x.DateClose < x.Date && x.Date.Day != Date.Day).Include(x => x.Avto).Include(x => x.Avto.Type).OrderByDescending(x => x.Date).ToList();
            }
            catch { }



            ViewBag.Counter = ASC.Count();//сохраняем количество записей за текущий день.
            ASC.AddRange(ASCOld);//добавляем в конец списка все не закрытые записи
            ViewBag.Nabludenii = Nabludenii;//массив с числом наблюдений по часам
            ViewBag.Avto = db.Avtomobils.OrderBy(x => x.Number).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Number, }).ToList();
            ViewBag.Voditel = db.Voditels.OrderBy(x => x.Name).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name, }).ToList();

            HttpCookie cookieReq = Request.Cookies["Mechanic"];

            // Проверить, удалось ли обнаружить cookie-набор с таким именем.
            // Это хорошая мера предосторожности, потому что         
            // пользователь мог отключить поддержку cookie-наборов,         
            // в случае чего cookie-набор не существует        
            int MechId=1;
            if (cookieReq != null)
            {
                try
                {
                    MechId = Convert.ToInt32(cookieReq["MechId"]);
                }
                catch
                {

                }
            }
            List<SelectListItem> M = new List<SelectListItem>();
            SelectListItem TecMech = db.Mechanics.Where(x=>x.Id==MechId).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).First();
            ViewBag.TecMech = TecMech.Text;
            M.AddRange(db.Mechanics.OrderBy(x => x.Id).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList());
            M.RemoveAt(MechId-1);
            M.Insert(0, TecMech);
            ViewBag.Mechanics = M;
            ViewBag.Zakazchik = db.Zakazchiks.OrderBy(x => x.Id).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList();
            return View(ASC);
        }

        public ActionResult SelectMech(string selection)
        {
            // Создать объект cookie-набора
            HttpCookie cookie = new HttpCookie("Mechanic");
            string[] S = selection.Split(';');
            // Установить значения в нем
            cookie["MechId"] = S[0];
            cookie.Expires = DateTime.Now.AddMonths(1);

            // Добавить куки в ответ
            Response.Cookies.Add(cookie);
            
            return Json("Mechanic");
        }

        // GET: ASControls
        public ActionResult Dispetcher()
        {
            DateTime Date = DateTime.Now;
            ViewBag.Date = Date;

            //берем автомобили вышедшие в рейс
            List<int> ASId = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).Select(y => y.Avto.Id).ToList();
            List<Avtomobil> Avtos = new List<Avtomobil>();
            foreach (int i in ASId)
            {
                Avtos.Add(db.Avtomobils.Where(x => x.Id == i).First());
            }

            //берем все записи контрола за этот день и пробиваем по базе АС24
            List<ASControl> ASC = new List<ASControl>();
            try
            {
                ASC = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day&&x.DateClose.Day!=x.Date.Day).Include(x => x.Avto).Include(x => x.Avto.Type).Include(x => x.Voditel).ToList();
            }
            catch (Exception e)
            { }
            //берем все записи ас24 за день
            List<AS24> AS24db = new List<AS24>();
            List<SelectListItem> HourSnyatia = new List<SelectListItem>();
            try
            {

                AS24db = db.AS24.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).ToList();
                HourSnyatia = AS24db.Select(x => x.Date.Hour).Distinct().OrderBy(x=>x).Select(a => new SelectListItem { Value = a.ToString(), Text = a.ToString() + ":00" + "(час:мин)", }).ToList();

            }
            catch { }

            ViewBag.HourSnyatia = HourSnyatia;

            //пробиваем каждую запись и суммируем километраж и ДУТ
            List<int> Nabludenii = new List<int>();
            foreach (ASControl AC in ASC)
            {
                int counter = 0;
                decimal KMAS = 0;
                decimal DUT = 0;
                List<AS24> A24 = new List<AS24>();
                try
                {//берем все записи с данной тачкой
                    A24 = AS24db.Where(x => x.AvtoId == AC.AvtoId && x.Date.Hour >= AC.Date.Hour).OrderBy(y => y.Date).ToList();
                }
                catch { }
                ViewBag.NoSvaz = "";
                //ViewBag.Mesta = "";
                AC.Mesta = new List<string>();
                AC.Mesta.Add("Движений нет.");
                AC.NoSvaz = new List<string>();
                //AC.NoSvaz.Add("Потерь нет.");
                foreach (AS24 A in A24)
                {

                    if (A.Mesta != "" && A.Mesta != null)
                    {
                        AC.Mesta = new List<string>();
                        AC.Mesta.AddRange(A.Mesta.Split(';'));

                    }
                    //A.NoSvaz = A.NoSvaz.Replace(" ", "");
                    if (A.NoSvaz != "" && A.NoSvaz != null)
                    {
                        //AC.NoSvaz = new List<string>();
                        AC.NoSvaz.AddRange(A.NoSvaz.Split(';'));
                        AC.NoSvaz.RemoveAt(AC.NoSvaz.Count - 1);

                    }

                    KMAS += A.KM;
                    DUT += A.DUT;


                    // ViewBag.Mesta = A.Mesta;//места, посещённые автомобилем
                    counter++;
                }
                Nabludenii.Add(counter);
                AC.KMAS = KMAS;
                AC.DUT = DUT;

            }

            //Берем все не закрытые записи за предыдущие дни. Километраж и ДУТ автоматически взят из автоскана при ночном обновлении.
           // List<ASControl> ASCOld = new List<ASControl>();
          //  try
          //  {
          //      ASCOld = db.ASControls.Where(x => x.DateClose < x.Date && x.Date.Day != Date.Day).Include(x => x.Avto).Include(x => x.Avto.Type).OrderByDescending(x => x.Date).ToList();
          //  }
          //  catch { }



            ViewBag.Counter = ASC.Count();//сохраняем количество записей за текущий день.
            //ASC.AddRange(ASCOld);//добавляем в конец списка все не закрытые записи
            ViewBag.Nabludenii = Nabludenii;//массив с числом наблюдений по часам
            ViewBag.Avto = db.Avtomobils.OrderBy(x => x.Number).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Number, }).ToList();
            ViewBag.Voditel = db.Voditels.OrderBy(x => x.Name).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name, }).ToList();
            ViewBag.Zakazchik = db.Zakazchiks.OrderBy(x => x.Id).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name, }).ToList();
            return View(ASC);
        }


        // GET: ASControls
        public ActionResult AvtoWarnings()
        {
            DateTime Date = DateTime.Now;
            ViewBag.Date = Date;

            //берем автомобили вышедшие в рейс
            List<int> ASId = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).Select(y => y.Avto.Id).ToList();
            List<Avtomobil> Avtos = new List<Avtomobil>();
            foreach (int i in ASId)
            {
                Avtos.Add(db.Avtomobils.Where(x => x.Id == i).First());
            }

            //берем все записи контрола за этот день и пробиваем по базе АС24
            List<ASControl> ASC = new List<ASControl>();
            try
            {
                ASC = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).Include(x => x.Avto).Include(x => x.Avto.Type).Include(x => x.Voditel).ToList();
            }
            catch (Exception e)
            { }
            //берем все записи ас24 за день
            List<AS24> AS24db = new List<AS24>();
            try
            {

                AS24db = db.AS24.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).ToList();

            }
            catch { }
            //пробиваем каждую запись и суммируем километраж и ДУТ
            List<int> Nabludenii = new List<int>();
            foreach (ASControl AC in ASC)
            {
                int counter = 0;
                decimal KMAS = 0;
                decimal DUT = 0;
                List<AS24> A24 = new List<AS24>();
                try
                {//берем все записи с данной тачкой
                    A24 = AS24db.Where(x => x.AvtoId == AC.AvtoId && x.Date.Hour >= AC.Date.Hour).OrderBy(y => y.Date).ToList();
                }
                catch { }
                ViewBag.NoSvaz = "";
                //ViewBag.Mesta = "";
                AC.Mesta = new List<string>();
                AC.Mesta.Add("Движений нет.");
                AC.NoSvaz = new List<string>();
                //AC.NoSvaz.Add("Потерь нет.");
                foreach (AS24 A in A24)
                {
                 

                    if (A.Mesta != "" && A.Mesta != null)
                    {
                        AC.Mesta = new List<string>();
                        AC.Mesta.AddRange(A.Mesta.Split(';'));

                    }
                    //A.NoSvaz = A.NoSvaz.Replace(" ", "");
                    if (A.NoSvaz != "" && A.NoSvaz != null)
                    {
                        //AC.NoSvaz = new List<string>();
                        AC.NoSvaz.AddRange(A.NoSvaz.Split(';'));
                        AC.NoSvaz.RemoveAt(AC.NoSvaz.Count - 1);

                    }


                    KMAS += A.KM;
                    DUT += A.DUT;


                    // ViewBag.Mesta = A.Mesta;//места, посещённые автомобилем
                    counter++;
                }
                Nabludenii.Add(counter);
                AC.KMAS = KMAS;
                AC.DUT = DUT;

            }

            //Берем все не закрытые записи за предыдущие дни. Километраж и ДУТ автоматически взят из автоскана при ночном обновлении.
            List<ASControl> ASCOld = new List<ASControl>();
            try
            {
                ASCOld = db.ASControls.Where(x => x.DateClose < x.Date && x.Date.Day != Date.Day&&x.Warning==true&&x.Podtvergdeno==false).Include(x => x.Avto).Include(x => x.Avto.Type).OrderByDescending(x => x.Date).ToList();
            }
            catch { }
            List<ASControl> ASCPodtvergdenie = new List<ASControl>();
            try
            {
                ASCPodtvergdenie = db.ASControls.Where(x => x.Podtvergdeno == true).Include(x => x.Avto).Include(x => x.Avto.Type).OrderByDescending(x => x.Date).ToList();
            }
            catch { }

            ViewBag.Counter = ASC.Count();//сохраняем количество записей за текущий день.
            ASC.AddRange(ASCOld);//добавляем в конец списка все не закрытые записи
            ASC.AddRange(ASCPodtvergdenie);//добавляем в конец списка все не закрытые записи
            ViewBag.Nabludenii = Nabludenii;//массив с числом наблюдений по часам
            ViewBag.Avto = db.Avtomobils.OrderBy(x => x.Number).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Number, }).ToList();
            ViewBag.Voditel = db.Voditels.OrderBy(x => x.Name).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name, }).ToList();
            return View(ASC);
        }



        // GET: ASControls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ASControl aSControl = db.ASControls.Find(id);
            if (aSControl == null)
            {
                return HttpNotFound();
            }
            return View(aSControl);
        }

        // GET: ASControls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ASControls/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AvtoId,Name,Date,Go,Primech,KMAS,KM,DUT,Start,End,Zapravleno,Sliv")] ASControl aSControl)
        {
            if (ModelState.IsValid)
            {
                db.ASControls.Add(aSControl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aSControl);
        }
        //добавление нового автомобиля скриптом на индексе
        [HttpPost]
        public ActionResult AddAvto(string selection)
        {
            string Data = "";
            string[] S = selection.Split(';');
            int AvtoId = Convert.ToInt32(S[0]);
            int VoditelId = 1;
            //Convert.ToInt32(S[2]);
            int ZakazchikId = 1;
            //Convert.ToInt32(S[3]);
            bool Pricep = Convert.ToBoolean(S[4]);
            //int A = 0;
            DateTime D = DateTime.Now;
        
        ASControl ASC = new ASControl();
        Avtomobil Avto = db.Avtomobils.Where(a => a.Id == AvtoId).First();
        Voditel Vod = db.Voditels.Where(a => a.Id == VoditelId).First();
        Zakazchik Zak = db.Zakazchiks.Where(a => a.Id == ZakazchikId).First();
            ASControl A = null;
            try
            {
                A = db.ASControls.Where(x => x.AvtoId == AvtoId&&x.Date.Year==D.Year&&x.Date.Month==D.Month&&x.Date.Day==D.Day).First();
            }
            catch
            {

            }
            if (A !=null)
            {
                //контроль УАТ имеет приоритет при создании выездов. Даже если выезд открыт время выезда заменяется на время реального выезда 
                if (User.Identity.Name.Contains("КонтрольУАТ"))
                {
                    if (!A.Kontrol)
                    {
                        A.Date = DateTime.Now;
                        A.Kontrol = true;
                        try
                        {
                            db.Entry(A).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch
                        {
                            Data = "Проблема с изменением времени выезда. Попробуйте еще раз.";
                        }
                    }
                    else
                    {
                        Data = "Вы уже отправили данный автомобиль. Именно вы, а не диспетчер! Ищите его в списке ниже.";
                        return Json(Data);
                    }
                }
                else
                {
                    Data = "Автомобиль с таким номером уже выехал!";
                    return Json(Data);
                }
            }
            else
            {




                ASC.Go = true;
                ASC.KM = 0;
                ASC.Primech = "";//S[1];
                ASC.VoditelId = VoditelId;
                ASC.Sliv = 0;
                ASC.Start = 0;
                ASC.Zapravleno = 0;
                ASC.KMAS = 0;
                ASC.AvtoId = AvtoId;
                ASC.Date = DateTime.Now;
                ASC.DateClose = new DateTime(2001, 1, 1, 0, 0, 0);
                ASC.Podtvergdeno = false;
                ASC.ZakazchikId = ZakazchikId;
                ASC.Pricep = Pricep;
                if (User.Identity.Name.Contains("КонтрольУАТ"))
                {
                    A.Kontrol = true;
                }




                    try
                {
                    db.ASControls.Add(ASC);
                    db.SaveChanges();
                    Data = "";
                }
                catch (Exception e)
                {
                    Data = "Проблема с добавлением автомобиля в базу данных. Попробуйте позже.";
                }
            }
            return Json(Data);
        }


        [HttpPost]
        public ActionResult CloseAvto(string selection)
        {
            string[] S = selection.Split(';');
            int Id = Convert.ToInt32(S[0]);
            //если адекватно написаны километры то сохраняем иначе 0
            int KM = 0;
            try
            {
                KM = Convert.ToInt16(S[1]);
            }
            catch
            {

            }
            
            
             ASControl ASC = db.ASControls.Where(a => a.Id == Id).First();
            ASC.KM = KM;
            ASC.Primech = S[2];
            ASC.DateClose = DateTime.Now;
            string Data = "";
            int HourClose = ASC.DateClose.Hour;
            if (ASC.DateClose.Day > ASC.Date.Day||ASC.DateClose.Month>=ASC.Date.Month) { HourClose=23; }//если закрывают на следующий день то ставим время 23:00 считаем как закрыто с опозданием

            //Ищем потери связи за весь период на AS24
            List<AS24> db24 = new List<AS24>(); 
            try
            {
               db24 =  db.AS24.Where(a => a.AvtoId == a.Id && a.Date.Year == ASC.Date.Year && a.Date.Month == ASC.Date.Month && a.Date.Day == ASC.Date.Day && a.Date.Hour >= ASC.Date.Hour && a.Date.Hour <= HourClose).ToList();
            }
            catch
            {

            }
            int NoSvazMin = 0;
            decimal koef = 0;
            
            for (int i =0;i<db24.Count;i++)
            {

                    if (db24[i].NoSvaz != "")
                    {
                        string[] SS = db24[i].NoSvaz.Split('@');//получаем дату и длительность
                        string[] SSS = SS[1].Split(':');//бьём длительность на часы минуты секунды
                        int ind = SS[0].IndexOf(":") - 2;
                        NoSvazMin += Convert.ToInt32(SSS[1]);//берем минуты так как макс диапазон 10 минут
                    }
                    else
                    {

                    }

            }
            if (db24.Count != 0)
            {
                koef = Convert.ToDecimal(NoSvazMin) / (60 * db24.Count);//проверяем общую длительность потерь связи. Если она больше 25 процентов отправляем на подтверждение 
            }
                if (koef >0.25m)
            {
                ASC.Podtvergdeno = true;//при истине отправляем на проверку
                ASC.Primech += " Больше 25% потери связи.";
                //ASC.Warning = true;
            }
            else
            {
                ASC.Podtvergdeno = false;
            }


            //если не вбит пробег то возврат обратно

           // if (KM == 0) { Data = "Чтобы закрыть выезд, введите пробег автомобиля (записанный водителем в путёвке) в соответствующее поле. Пробег должен быть больше нуля!"; return Json(Data); }
            try
            {

                    db.Entry(ASC).State = EntityState.Modified;
                    db.SaveChanges();
                    Data = "";

            }
            catch (Exception e)
            {
                Data = "Неудача";
            }

            return Json(Data);
        }

        [HttpPost]
        public ActionResult PodtverditAvto(string selection)
        {
            string[] S = selection.Split(';');
            int Id = Convert.ToInt32(S[0]);
            //если адекватно написаны километры то сохраняем иначе 0
            int KM = 0;
            try
            {
                KM = Convert.ToInt16(S[1]);
            }
            catch
            {

            }


            ASControl ASC = db.ASControls.Where(a => a.Id == Id).First();
            ASC.KM = KM;
            ASC.Primech = S[2];
            ASC.Podtvergdeno = false; 
            string Data = "";
            //если не вбит пробег то возврат обратно

           // if (KM == 0) { Data = "Чтобы закрыть выезд, введите пробег автомобиля (записанный водителем в путёвке) в соответствующее поле. Пробег должен быть больше нуля!"; return Json(Data); }
            try
            {

                db.Entry(ASC).State = EntityState.Modified;
                db.SaveChanges();
                Data = "";

            }
            catch (Exception e)
            {
                Data = "Неудача";
            }

            return Json(Data);
        }


        // GET: ASControls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ASControl aSControl = db.ASControls.Find(id);
            if (aSControl == null)
            {
                return HttpNotFound();
            }
            return View(aSControl);
        }

        // POST: ASControls/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AvtoId,Name,Date,Go,Primech,KMAS,KM,DUT,Start,End,Zapravleno,Sliv")] ASControl aSControl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aSControl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aSControl);
        }

        // GET: ASControls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ASControl aSControl = db.ASControls.Find(id);
            if (aSControl == null)
            {
                return HttpNotFound();
            }
            return View(aSControl);
        }

        // POST: ASControls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ASControl aSControl = db.ASControls.Find(id);
            db.ASControls.Remove(aSControl);
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
