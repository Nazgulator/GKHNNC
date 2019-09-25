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
             List<int> ASId = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).Select(y=>y.Avto.Id).ToList();
              List<Avtomobil> Avtos = new List<Avtomobil>();
              foreach (int i in ASId)
              {
                  Avtos.Add(db.Avtomobils.Where(x => x.Id == i).First());
              }

            //берем все записи контрола за этот день и пробиваем по базе АС24
            List<ASControl> ASC = new List<ASControl>();
            try
            {
                ASC = db.ASControls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).Include(x => x.Avto).Include(x=>x.Avto.Type).Include(x=>x.Voditel).ToList();
            }
            catch(Exception e)
            {            }
            //берем все записи ас24 за день
            List<AS24> AS24db = new List<AS24>();
            List<SelectListItem> HourSnyatia = new List<SelectListItem>();
            try
            {
               
               AS24db = db.AS24.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).ToList();
               HourSnyatia = AS24db.Select(x=>x.Date.Hour).Distinct().Select(a => new SelectListItem { Value = a.ToString(), Text = a.ToString()+":00"+"(час:мин)", }).ToList();

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
                    A24 = AS24db.Where(x => x.AvtoId == AC.AvtoId && x.Date.Hour >= AC.Date.Hour).OrderBy(y=>y.Date).ToList();
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
                    
                    if (A.Mesta!=""&&A.Mesta!=null)
                    {
                        AC.Mesta = new List<string>();
                        AC.Mesta.AddRange(A.Mesta.Split(';'));
                        
                    }
                    //A.NoSvaz = A.NoSvaz.Replace(" ", "");
                    if (A.NoSvaz != "" && A.NoSvaz != null)
                    {
                        //AC.NoSvaz = new List<string>();
                        AC.NoSvaz.AddRange(A.NoSvaz.Split(';'));
                        AC.NoSvaz.RemoveAt(AC.NoSvaz.Count-1);

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
                ASCOld = db.ASControls.Where(x => x.DateClose<x.Date&&x.Date.Day!=Date.Day).Include(x => x.Avto).Include(x=>x.Avto.Type).OrderByDescending(x=>x.Date).ToList();
            }
            catch { }


            
            ViewBag.Counter = ASC.Count();//сохраняем количество записей за текущий день.
            ASC.AddRange(ASCOld);//добавляем в конец списка все не закрытые записи
            ViewBag.Nabludenii = Nabludenii;//массив с числом наблюдений по часам
            ViewBag.Avto = db.Avtomobils.OrderBy(x => x.Number).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Number, }).ToList();
            ViewBag.Voditel = db.Voditels.OrderBy(x => x.Name).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name, }).ToList();
            ViewBag.Zakazchik = db.Zakazchiks.OrderBy(x => x.Id).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name, }).ToList();
            return View(ASC);
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
                HourSnyatia = AS24db.Select(x => x.Date.Hour).Distinct().Select(a => new SelectListItem { Value = a.ToString(), Text = a.ToString() + ":00" + "(час:мин)", }).ToList();

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
                HourSnyatia = AS24db.Select(x => x.Date.Hour).Distinct().Select(a => new SelectListItem { Value = a.ToString(), Text = a.ToString() + ":00" + "(час:мин)", }).ToList();

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
                ASCOld = db.ASControls.Where(x => (x.DateClose < x.Date && x.Date.Day != Date.Day&&x.Warning==true)||x.Podtvergdeno==true).Include(x => x.Avto).Include(x => x.Avto.Type).OrderByDescending(x => x.Date).ToList();
            }
            catch { }



            ViewBag.Counter = ASC.Count();//сохраняем количество записей за текущий день.
            ASC.AddRange(ASCOld);//добавляем в конец списка все не закрытые записи
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
            string[] S = selection.Split(';');
            int AvtoId = Convert.ToInt32(S[0]);
            int VoditelId = Convert.ToInt32(S[2]);
            int ZakazchikId = Convert.ToInt32(S[3]);
            bool Pricep = Convert.ToBoolean(S[4]);
            

            ASControl ASC = new ASControl();
            Avtomobil Avto = db.Avtomobils.Where(a => a.Id == AvtoId).First();
            Voditel Vod = db.Voditels.Where(a => a.Id == VoditelId).First();
            Zakazchik Zak = db.Zakazchiks.Where(a => a.Id == ZakazchikId).First();


            ASC.Go = true;
            ASC.KM = 0;
            ASC.Primech = S[1];
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



            string Data = "";

            try
            {
                db.ASControls.Add(ASC);
                db.SaveChanges();
                Data = "Успешно добавлена";
            }
            catch (Exception e)
            {
                Data = "Неудача";
            }

            return Json(Data);
        }


        [HttpPost]
        public ActionResult CloseAvto(string selection)
        {
            string[] S = selection.Split(';');
            int Id = Convert.ToInt32(S[0]);
            //если адекватно написаны километры то сохраняем иначе 0
            decimal KM = 0;
            try
            {
                KM = Convert.ToDecimal(S[1]);
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

            if (KM == 0) { Data = "Чтобы закрыть выезд, введите пробег автомобиля (записанный водителем в путёвке) в соответствующее поле. Пробег должен быть больше нуля!"; return Json(Data); }
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
