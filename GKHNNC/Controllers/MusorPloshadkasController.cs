using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GKHNNC.DAL;
using GKHNNC.Models;

namespace GKHNNC.Controllers
{
    public class MusorPloshadkasController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: MusorPloshadkas
        public ActionResult Index(string StreetId = "")
        {
            HttpCookie cookieReq = Request.Cookies["Errors"];
            if (cookieReq != null)
            {
                ViewBag.Errors = cookieReq["Errors"];
            }
                var StreetsSL = new SelectList(db.AllStreets, "Id", "Name");
            List<SelectListItem> SSL = new List<SelectListItem>();
            SSL.Add(new SelectListItem { Text = "Все", Value = "0" });
            SSL.AddRange(StreetsSL);
            ViewBag.Streets = SSL;
            string[] ULs = new string[] { "МКД", "ЮЛ","ИЖД" };
            ViewBag.UL = ULs;
            List<MusorPloshadka> musorPloshadkas = db.MusorPloshadkas.Include(x=>x.Type).ToList();
            ViewBag.ContainersTypes = new SelectList(db.ContainersTypes,"Id","Name");
            


            foreach (MusorPloshadka M in musorPloshadkas)
            {
                string[] Streets = M.StreetId.Split(';');
                M.VseUlici = new List<AllStreet>();
                M.AllStreets = new List<string>();
                foreach (string S in Streets)
                {
                    try
                    {
                        int N = Convert.ToInt32(S);
                        AllStreet Name = db.AllStreets.Where(x => x.Id == N).First();
                        M.VseUlici.Add(Name);
                        M.AllStreets.Add(Name.Name);
                    }
                    catch (Exception e)
                    {

                    }
                }
                //загружаем Объёмы
                string[] Obiems = M.Obiem.Split(';');
                for (int i = 0; i < 7; i++)
                {
                    if (i <= Obiems.Length - 1)
                    {
                        M.Obiem7[i] = Convert.ToDecimal(Obiems[i]);
                    }
                    else
                    {
                        M.Obiem7[i] = 0;
                    }
                }
                string[] Kontainers = M.Kontainers.Split(';');
                for (int i = 0; i < 7; i++)
                {
                    if (i <= Kontainers.Length - 1)
                    {
                        M.Kontainers7[i] = Convert.ToInt32(Kontainers[i]);
                    }
                    else
                    {
                        M.Kontainers7[i] = 0;
                    }
                }

            }
            List<MusorPloshadka> MP = new List<MusorPloshadka>();
            if (StreetId.Equals(""))
            {
                ViewBag.StreetId = "";
                MP = musorPloshadkas;
            }
            else
            {
                int SID = 0;
                try
                {
                    SID = Convert.ToInt32(StreetId);
                }
                catch
                {
                    SID = db.AllStreets.Where(x => x.Name.Replace(" ", "").Equals(StreetId)).Select(x => x.Id).First();
                }
               
                ViewBag.StreetId = SID;
                ViewBag.Name = db.AllStreets.Where(x => x.Id==SID).Select(x => x.Name).First();

                // MP =  musorPloshadkas.Where(x => x.VseUlici.Where(y => y.Id == StreetId).First() != null).ToList();
                foreach (MusorPloshadka P in musorPloshadkas)
                {
                    foreach (AllStreet S in P.VseUlici)
                    {
                        if (S.Id == SID)
                        {
                            MP.Add(P);
                            break;
                        }
                    }
                }

            }
            return View(MP);
        }

        public void LoadStreets( ref MusorPloshadka P)
        {

            string[] S = P.StreetId.Split(';');
                foreach (string s in S)
                {
                int id = Convert.ToInt32(s);
                   try
                {
                    P.AllStreets.Add(db.AllStreets.Where(x=>x.Id==id).Select(x=>x.Name).First());
                }
                catch
                {

                }
                }
            
        }

        // GET: MusorPloshadkas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MusorPloshadka musorPloshadka = db.MusorPloshadkas.Find(id);
            if (musorPloshadka == null)
            {
                return HttpNotFound();
            }
            return View(musorPloshadka);
        }

        // GET: MusorPloshadkas/Create
        public ActionResult Create()
        {
            ViewBag.StreetId = new SelectList(db.AllStreets, "Id", "Name");
            return View();
        }

        // POST: MusorPloshadkas/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public JsonResult ObiemFact(int Id = 0, int Value = 0)
        {
            string Data = "";
            MarshrutsALL M = null;
            try
            {
                M = db.MarshrutsAlls.Where(x => x.Id == Id).First();
                M.ObiemFact = Value;
                M.Modify = true;
                db.Entry(M).State = EntityState.Modified;
                db.SaveChanges();
                Data = M.ObiemFact.ToString();
            }
            catch (Exception e)
            {

            }


            return Json(Data);
        }


        [HttpPost]
        public ActionResult AddMarshrutActive([Bind(Include = "MarshrutId")] MarshrutsActive MB)
        {
            try
            {
                MarshrutsALL M = null;
                int Day = (int)DateTime.Now.DayOfWeek - 1;
                try
                {
                    M = db.MarshrutsAlls.Where(x => x.Day == Day && x.MarshrutId == MB.MarshrutId).First();
                    M.Date = DateTime.Now;
                    M.Type = "A";


                }
                catch (Exception e)
                {
                    TempData["Error"] = "Не найден базовый маршрут! Добавьте сначала базовый маршрут на текущий день недели.";
                }

                if (M != null)
                {
                    //проверка есть ли уже такой маршрут сегодня
                    try
                    {
                      //  MarshrutsALL Z = db.MarshrutsAlls.Where(x => x.Date.Year == M.Date.Year && x.Date.Month == M.Date.Month && x.Date.Day == M.Date.Day && x.MarshrutId == M.MarshrutId).First();
                      //  TempData["Error"] = "Такой маршрут уже запущен!";

                    }
                    catch
                    {
                        //если нет такого маршрута то ок
                      
                    }
                    //с 20 марта принято решение о создании любого количеств адубликатов активного маршрута
                    db.MarshrutsAlls.Add(M);
                    db.SaveChanges();
                    List<MarshrutsALL> MA = new List<MarshrutsALL>();
                    MA.Add(M);
                    RefreshObiems(MA);
                   
                    for (int i=0;i<MA.Count();i++)
                    {
                        MA[i] = LoadMusorPloshadkaInMarshrut(MA[i]);
                        for (int j=0; j<MA[i].MusorPloshadkas7.Count;j++)
                        {
                            MusorPloshadkaActive MPA = new MusorPloshadkaActive();
                            MPA.MarshrutId = M.Id;
                            MPA.PloshadkaId = MA[i].MusorPloshadkas7[j].Id;
                            MPA.ObiemFact = MA[i].MusorPloshadkas7[j].Obiem7[M.Day];
                            MPA.KontainersFact = MA[i].MusorPloshadkas7[j].Kontainers7[M.Day];
                            try
                            {
                                db.MusorPloshadkaActives.Add(MPA);
                                db.SaveChanges();
                            }
                            catch(Exception e)
                            {

                            }
                        }
                    }

                }
                else
                {

                    return RedirectToAction("MarshrutsBaseIndex");
                }
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("MarshrutsBaseIndex");
        }

        [HttpPost]
        public ActionResult AddMarshrutBase([Bind(Include = "MarshrutId,Day")] MarshrutsBase MB)
        {
            try
            {
                MarshrutsALL M = null;
                try
                {
                    M = db.MarshrutsAlls.Where(x => x.Day == MB.Day && x.MarshrutId == MB.MarshrutId&&x.Type.Equals("B")).First();
                    TempData["Error"] = "Такой маршрут уже запущен!";
                }
                catch
                {

                }
                if (M == null)
                {
                    MB.Type = "B";
                    MB.Date = new DateTime(2000, 1, 1);
                    db.MarshrutsAlls.Add(MB);
                    db.SaveChanges();
                }
                else
                { return RedirectToAction("MarshrutsBaseIndex"); }
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("MarshrutsBaseIndex");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStreet([Bind(Include = "StreetId,Id")] MusorPloshadka musorPloshadka)
        {
            string data = "";
            MusorPloshadka MP = new MusorPloshadka();
            try
            {
                MP = db.MusorPloshadkas.Where(x => x.Id == musorPloshadka.Id).First();

                MP.StreetId += ";" + musorPloshadka.StreetId;
                db.Entry(MP).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult MarshrutIndex()
        {
            List<Marshrut> M = db.Marshruts.ToList();
            return View(M);

        }
        public decimal[] LoadMusorPloshadkaObiems7(MusorPloshadka MP)
        {
            decimal[] M = new decimal[7];
            string[] S = MP.Obiem.Split(';');
            for (int i=0;i<S.Length;i++)
            {
                decimal N = Convert.ToDecimal(S[i]);
                M[i] = N;
              
            }
            return M;
        }
        public int[] LoadMusorPloshadkaKontainers7(MusorPloshadka MP)
        {
            int[] M = new int[7];
            string[] S = MP.Kontainers.Split(';');
            for (int i = 0; i < S.Length; i++)
            {
                int N = Convert.ToInt32(S[i]);
                M[i] = N;

            }
            return M;
        }

        public void LoadMusorPloshadkas(ref List<MarshrutsALL> MB)
        {//грузим мусорные площадки из маршрута
            foreach (MarshrutsALL M in MB)
            {
                M.MusorPloshadkas7 = new List<MusorPloshadka>();
                try
                {
                    string[] S = M.MusorPloshadkas.Split(';');


                    for (int i = 0; i < S.Length; i++)
                    {
                        int N = Convert.ToInt32(S[i]);
                        try
                        {
                            M.MusorPloshadkas7.Add(db.MusorPloshadkas.Where(y => y.Id == N).Include(x=>x.Type).First());
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                catch
                {

                }

                //подгружаем данные мусорных площадок
                List<MusorPloshadka> MP7 = new List<MusorPloshadka>();
                MP7 = M.MusorPloshadkas7;
                LoadPloshadkasNames(ref MP7);
                M.MusorPloshadkas7 = MP7;
            }
        }

        public void LoadMusorObiems(ref List<MusorPloshadka> MB)
        {//грузим мусорные площадки из маршрута
            foreach (MusorPloshadka M in MB)
            {
               // M.Obiem7 = new decimal[7];
                M.Obiem7 =LoadMusorPloshadkaObiems7(M);
                //подгружаем данные мусорных площадок
            }
        }

        public void LoadAvtomobils(ref List<MarshrutsALL> MB)
        {//грузим мусорные площадки из маршрута
            foreach (MarshrutsALL M in MB)
            {
                M.Avtomobils7 = new List<Avtomobil>();
                try
                {
                    string[] S = M.Avtomobils.Split(';');


                    for (int i = 0; i < S.Length; i++)
                    {
                        int N = Convert.ToInt32(S[i]);
                        try
                        {
                            M.Avtomobils7.Add(db.Avtomobils.Where(x => x.Id == N).Include(x => x.Marka).Include(x => x.Type).First());
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                catch
                {

                }
            }
        }
        public void LoadPloshadkasNames(ref List<MusorPloshadka> MP)
        {
            foreach (MusorPloshadka M in MP)
            {
                string[] S = new string[7] { "0", "0", "0", "0", "0", "0", "0" };
                string[] SS = M.Obiem.Split(';');
                string[] SSS = M.Kontainers.Split(';');
                for (int i = 0; i < 7; i++)
                {
                    M.Obiem7[i] = Convert.ToDecimal(SS[i]);
                    M.Kontainers7[i] = Convert.ToInt32(SSS[i]);
                }
                string STREETS = "";
                string[] Streets = M.StreetId.Split(';');
                foreach (string ST in Streets)
                {
                    int NS = Convert.ToInt32(ST);
                    try
                    {
                        STREETS += db.AllStreets.Where(x => x.Id == NS).Select(x => x.Name).First() + ",";
                    }
                    catch
                    {

                    }
                }
                STREETS = STREETS.Remove(STREETS.Length - 1, 1);
                M.TimeName = STREETS + " " + M.Name;
            }

        }

        //Заполнение дней недели
        public List<SelectListItem> ZapolnitDays()
        {
            List<SelectListItem> Days = new List<SelectListItem>();
            for (int i = 0; i < 7; i++)
            {
                string d = "";
                switch (i)

                {
                    case 0:
                        d = "Понедельник";
                        break;
                    case 1:
                        d = "Вторник";
                        break;
                    case 2:
                        d = "Среда";
                        break;
                    case 3:
                        d = "Четверг";
                        break;
                    case 4:
                        d = "Пятница";
                        break;
                    case 5:
                        d = "Суббота";
                        break;
                    case 6:
                        d = "Воскресенье";
                        break;
                    default:
                        d = "Понедельник";
                        break;
                }
                SelectListItem I = new SelectListItem();
                I.Text = d;
                I.Value = i.ToString();
                Days.Add(I);
            }
            return Days;
        }

        [HttpGet]
        public ActionResult MarshrutsBaseIndex(string Date = "", int day = 8, int MarshrutId = 0, int Id = 0, string Type = "Base")
        {

            DateTime DATE = DateTime.Now;
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }

            HttpCookie cookieReq = Request.Cookies["MarshrutBase"];
            if (cookieReq != null)
            {
                int dayOld = Convert.ToInt32(cookieReq["Day"]);
                int marshOld = Convert.ToInt32(cookieReq["MarshrutId"]);
                string TypeOld = cookieReq["Type"];
                string DateOld = cookieReq["Date"];
                HttpCookie cookie = new HttpCookie("MarshrutBase");
                if (day != 8 && day != dayOld && day != 8)
                {
                    cookie["Day"] = day.ToString();
                    if (day < 0)
                    {
                        cookie["Day"] = "8";
                        day = 8;
                    }
                }
                else
                {
                    cookie["Day"] = dayOld.ToString();
                    day = dayOld;
                }
                if (MarshrutId != 0 && marshOld != MarshrutId && MarshrutId > 0)
                {
                    cookie["MarshrutId"] = MarshrutId.ToString();

                }
                else
                {
                    if (MarshrutId < 0)
                    {
                        cookie["MarshrutId"] = "0";
                        MarshrutId = 0;
                    }
                    else
                    {
                        cookie["MarshrutId"] = marshOld.ToString();
                        MarshrutId = marshOld;
                    }
                }
                if (Type.Equals("Base") && TypeOld.Equals("Base") == false)
                {
                    Type = TypeOld;
                }
                if (Type.Equals("ToBase"))
                {
                    Type = "Base";

                }
                cookie["Type"] = Type;

                try
                {
                    DATE = Convert.ToDateTime(Date);

                    cookie["Date"] = DATE.ToString();
                }
                catch
                {
                    Date = DateOld;
                    DATE = Convert.ToDateTime(DateOld);
                    cookie["Date"] = Date;
                }
                Response.Cookies.Add(cookie);

            }
            else
            {
                if (cookieReq == null)
                {
                    HttpCookie cookie = new HttpCookie("MarshrutBase");

                    cookie["Day"] = day.ToString();
                    cookie["Type"] = Type.ToString();

                    cookie["MarshrutId"] = MarshrutId.ToString();
                    Date = DateTime.Now.ToString();
                    cookie["Date"] = Date;
                    // Добавить куки в ответ
                    Response.Cookies.Add(cookie);
                }
            }
            ViewBag.Date = DATE;
            SelectListItem F = new SelectListItem();
            F.Text = "Все";
            F.Value = "-1";
            List<SelectListItem> AllM = new List<SelectListItem>();
            AllM.Add(F);
            AllM.AddRange(new SelectList(db.Marshruts, "Id", "Name"));
            ViewBag.Marshruts = AllM;

            List<SelectListItem> Days = ZapolnitDays();
            ViewBag.Day = day;
            ViewBag.Marshrut = MarshrutId;
            string MN = "Все";
            try
            {
                MN = db.Marshruts.Where(x => x.Id == MarshrutId).Select(x => x.Name).First();
            }
            catch
            {

            }
            ViewBag.MarshrutName = MN;
            SelectListItem All = new SelectListItem();
            All.Text = "Все";
            All.Value = "-1";
            Days.Insert(0, All);
            ViewBag.Days = Days;
            ViewBag.Avtomobils = new SelectList(db.Avtomobils.Where(x => x.GKHNNC == true).OrderBy(x => x.Number), "Id", "Number");
            List<MusorPloshadka> AllMP = db.MusorPloshadkas.Include(x=>x.Type).ToList();
            LoadPloshadkasNames(ref AllMP);
            List<SelectListItem> AllSL = new List<SelectListItem>();
            foreach (MusorPloshadka MP in AllMP)
            {
                SelectListItem SL = new SelectListItem();
                SL.Text = MP.TimeName;
                SL.Value = MP.Id.ToString();
                AllSL.Add(SL);





            }
            ViewBag.Ploshadkas = AllSL;

            List<MarshrutsALL> MB = new List<MarshrutsALL>();
            if (day > 7)
            {
                MB = db.MarshrutsAlls.OrderBy(x => x.MarshrutId).Include(x => x.Marshrut).ToList();
            }
            else
            {
                MB = db.MarshrutsAlls.Where(x => x.Day == day).Include(x => x.Marshrut).OrderBy(x => x.MarshrutId).ToList();
            }
            if (MarshrutId != 0)
            {
                MB = MB.Where(x => x.MarshrutId == MarshrutId).ToList();
            }
            //грузим мусорные площадки из маршрута
            if (Id != 0)
            {
                MB = MB.Where(x => x.Id == Id).ToList();
            }



            ViewBag.Type = Type;
            if (Type.Equals("Base"))
            {
                MB = MB.Where(x => x.Type.Equals("B")).ToList();


            }
            else
            {
                MB = MB.Where(x => x.Type.Equals("A")).ToList();
                ViewBag.Type = "Active";
                //сортируем по датам только активные маршруты
                MB = MB.Where(x => x.Date.Year == DATE.Year && x.Date.Month == DATE.Month && x.Date.Day == DATE.Day).ToList();
                // LoadMusorPloshadkas(ref MB);
                //  LoadAvtomobils(ref MB);


            }
            LoadMusorPloshadkas(ref MB);
            LoadAvtomobils(ref MB);
            LoadMusorPloshadkaActive(ref MB);
            //обновляем нулевые объёмы факт только у активных 
            RefreshObiems(MB);


            return View(MB);

        }
        [HttpPost]
        public ActionResult RefreshObiemsActivePloshadka(int ActivePloshadkaId = 0, decimal ActivePloshadkaObiemFact = 0, int ActivePloshadkaKontainersFact = 0)
        {
            if (ActivePloshadkaId != 0)
            {
                try
                {
                    MusorPloshadkaActive MPA = db.MusorPloshadkaActives.Where(x => x.Id == ActivePloshadkaId).First();
                    MPA.ObiemFact = ActivePloshadkaObiemFact;
                    MPA.KontainersFact = ActivePloshadkaKontainersFact;
                    db.Entry(MPA).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch(Exception e) { }
            }
            return RedirectToAction("MarshrutsBaseIndex");
        }
        public void LoadMusorPloshadkaActive(ref List<MarshrutsALL> Marshrut)
        {
            foreach (MarshrutsALL MA in Marshrut)
            {
                int MarshId = MA.Id;
                foreach (MusorPloshadka MP in MA.MusorPloshadkas7)
                    try
                    {
                        int ID = MP.Id;
                        MP.MPA = db.MusorPloshadkaActives.Where(x => x.MarshrutId == MarshId && x.PloshadkaId == ID).First();

                    }
                    catch (Exception e)
                    {

                    }
            }

        }

        public void RefreshObiems(List<MarshrutsALL> MB)
        {
            //суммируем объемы и если объём нулевой, то сохраняем его
            decimal Summ = 0;

            foreach (MarshrutsALL MA in MB)
            {
                //Считаем только для активных и не измененных пользователем или равных нулю
                if (MA.Type.Equals("A")&&(!MA.Modify||MA.ObiemFact==0) )
                {
                    //если запись обнулена то снимаем человеческий фактор
                    if (MA.ObiemFact == 0 && MA.Modify) { MA.Modify = false; }
                    foreach (MusorPloshadka MP in MA.MusorPloshadkas7)
                    {
                        Summ += MP.Obiem7[MA.Day];
                    }
                    if (MA.ObiemFact != Summ)
                    {
                        MA.ObiemFact = Math.Round(Summ, 2);
                        db.Entry(MA).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            }
        }

        public JsonResult DeleteMarshrut(int Id)
        {
            string Data = "";
            MarshrutsALL M = db.MarshrutsAlls.Where(x => x.Id == Id).First();
            try
            {
                db.MarshrutsAlls.Remove(M);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Data = "Нельзя удалить";
            }

            return Json(Data);
        }

        public JsonResult SearchStreet(string term)
        {
            if (term != null)
            {

                term = term.ToUpper().Replace(" ", "");
            }
            List<string> Num = new List<string>();
            try
            {
                Num = db.AllStreets.Where(x => x.Name.Contains(term)).Select(x => x.Name.Replace(" ", "")).ToList();
            }
            catch
            {
                Num.Add("Нет такой площадки");
            }
            return Json(Num, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchIdStreet(string term)
        {
            if (term != null)
            {

                term = term.ToUpper().Replace(" ", "");
            }
            List<SelectListItem> Num = new List<SelectListItem>();
            try
            {
                Num = new SelectList( db.AllStreets.Where(x => x.Name.Contains(term)),"Id","Name").ToList();
            }
            catch
            {
              
            }
            return Json(Num, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectStreetFromBase(string number)
        {
            if (number != null)
            {

                number = number.ToUpper().Replace(" ", "");
            }
            string Num = "";
            try
            {
                //ид;номер;картинка 
                Num = db.AllStreets.Where(x => x.Name.Replace(" ", "").Contains(number)).Select(x => x.Name).First();
            }
            catch
            {

            }
            List<string> RealMP = new List<string>();
            if (Num.Equals("") == false)
            {

                List<MusorPloshadka> MP = new List<MusorPloshadka>();
                try
                {
                    MP = db.MusorPloshadkas.Include(x=>x.Type).ToList();

                    LoadPloshadkasNames(ref MP);//грузим временное имя
     
                                                //   LoadMusorObiems(ref MP);//грузим объёмы
                    RealMP = MP.Where(X => X.TimeName.Contains(Num)).Select(x => x.Id + ";" + x.TimeName + ";"+x.Type.Ico+";" + x.Obiem+";"+x.Kontainers).ToList();
                }
                catch (Exception e)
                {

                }
            }
            return Json(RealMP, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchNumber(string term)
        {
            if (term != null)
            {

                term = term.ToUpper().Replace(" ", "");
            }
            List<string> Num = new List<string>();
            try
            {
                Num = db.Avtomobils.Where(x => x.Number.Contains(term) && x.GKHNNC).Select(x => x.Number.Replace(" ", "")).ToList();
            }
            catch
            {
                Num.Add("Добавить новый автомобиль");
            }
            return Json(Num, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectAvtoFromBase(string number)
        {
            if (number != null)
            {

                number = number.ToUpper().Replace(" ", "");
            }
            string Num = "";
            try
            {
                //ид;номер;картинка 
                Num = db.Avtomobils.Where(x => x.Number.Contains(number)).Select(x => x.Id + ";" + x.Number.Replace(" ", "") + ";" + x.Type.Ico + ".png").First();
            }
            catch
            {

            }
            return Json(Num, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ExportMarshrutsToday(string date)
        {
            
            if (date != null)
            {
                DateTime Date = Convert.ToDateTime(date);
                
                List<string> Shapka = new List<string>();
                List<string> Stolbci = new List<string>();
                List<List<string>> Stroki = new List<List<string>>();

                Shapka.Add("Отчет по преревозке отходов за "+Date.ToString("dd.MM.yyyy"));
                List<MarshrutsALL> Marshruts = db.MarshrutsAlls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day && x.Type.Equals("A")).ToList();
                List<Poligon> Poligons = db.Poligons.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).ToList();
                //подгружаем данные
                LoadAvtomobils(ref Marshruts);
                LoadMusorPloshadkas(ref Marshruts);
                int number = 1;
                Stolbci.Add("№ п/п");
                Stolbci.Add("Дата");
                Stolbci.Add("Автомобиль");
                Stolbci.Add("Район");
                Stolbci.Add("Населенный пункт");
                Stolbci.Add("Адрес площадки");
                Stolbci.Add("ИНН");
                Stolbci.Add("obj_id");
                Stolbci.Add("Категория потребителя");
                Stolbci.Add("Наименование ЮЛ");
                Stolbci.Add("Тип контейнеров");
                Stolbci.Add("Кол-во контейнеров (шт.)");
                Stolbci.Add("Объём контейнера (м3)");
                Stolbci.Add("Кол-во вывезенных контейнеров (шт.)");
                Stolbci.Add("Объем ТКО за смену(м3)");
                Stolbci.Add("Объект размещения отходов");
                Stolbci.Add("Примечание");

                foreach (MarshrutsALL M in Marshruts)
                {

                    int Obiem = Convert.ToInt32(M.ObiemFact);
                    foreach (Avtomobil A in M.Avtomobils7)
                    {
                        int MassMusor = 0;
                        int Containers = 0;
                        foreach (MusorPloshadka MP in M.MusorPloshadkas7)
                        {
                            List<string> Stroka = new List<string>();

                            Stroka.Add(number.ToString());//номер по порядку
                            Stroka.Add(Date.ToString("dd.MM.yy"));//дата
                            Stroka.Add(A.Number);//номер тачки
                            Stroka.Add("Советский");
                            Stroka.Add("Новосибирск");
                            Stroka.Add(MP.TimeName);//Имя площадки
                            Stroka.Add(MP.IDPloshadki);//ИНН это ИД площадки
                            Stroka.Add("");//Id пустой везде неясно зачем он нужен
                            Stroka.Add(MP.UL);//Тип мкд или ЮЛ
                            Stroka.Add(MP.NameUL);//имя ЮЛ
                            Stroka.Add(MP.Type.Name);//ТИП контейнеров
                            Stroka.Add(MP.Kontainers7[Convert.ToInt16(Date.DayOfWeek) - 1].ToString());//количество контейнеров
                            Stroka.Add(MP.ObiemContainera.ToString());//объём контейнеров пока не задан
                            Stroka.Add("");//количество вывезенных контейнеров пока не задано
                            Stroka.Add(MP.Obiem7[Convert.ToInt16(Date.DayOfWeek) - 1].ToString());//объём факт
                            Stroka.Add("ФГУП 'ЖКХ ННЦ'");//количество вывезенных контейнеров пока не задано
                            Stroka.Add(MP.Type.Primech);//количество вывезенных контейнеров пока не задано
                            Stroki.Add(Stroka);
                            Containers += MP.Kontainers7[Convert.ToInt16(Date.DayOfWeek) - 1];
                            number++;
                        }
                        MassMusor = Convert.ToInt32(Poligons.Where(x => x.AvtomobilId == A.Id).Sum(x => x.MassMusor));
                        List<string> Str = new List<string>();
                        Str.Add("ИТОГО");//номер по порядку
                        Str.Add(Date.ToString("dd.MM.yy"));//дата
                        Str.Add(A.Number);//номер тачки
                        Str.Add("Масса мусора");
                        Str.Add(MassMusor.ToString());
                        Str.Add("");//Имя площадки
                        Str.Add("");//ИНН это ИД площадки
                        Str.Add("");//Id пустой везде неясно зачем он нужен
                        Str.Add("");//Тип мкд или ЮЛ
                        Str.Add("Всего контейнеров");//имя ЮЛ
                        Str.Add(Containers.ToString());//количество контейнеров
                        Str.Add("");//объём контейнеров пока не задан
                        Str.Add("");//количество вывезенных контейнеров пока не задано
                        Str.Add(Obiem.ToString());//объём факт
                        Str.Add("Итого объём за смену");//объём факт
                        Stroki.Add(Str);

                    }



                }

                string Path2 = Url.Content("~/Files/Transport/TransportOtchet" + Date.ToString("dd.MM.yyyy") + ".xlsx");
                string path = Server.MapPath("~/Files/Transport/TransportOtchet" + Date.ToString("dd.MM.yyyy") + ".xlsx");

                 ExcelExportMonth.StandartExport(Stolbci, Stroki, Shapka,path);
                string file = Path2;
                string filename = "TransportOtchet" + Date.ToString("dd.MM.yyyy") + ".xlsx";
                string contentType = "application/vnd.ms-excel";
                //патч,тип файла,новое имя файла
                // return File(file, contentType, filename);//отправка файла пользователю (сохранение, скачать файл)
                //return File(Path2, ".xlsx");
                return Json(Path2);
            }
            return Json("Ошибочка вышла!",".txt");
        }


        public JsonResult ExportShortMarshrutsToday(string date)
        {

            if (date != null)
            {
                DateTime Date = Convert.ToDateTime(date);

                List<string> Shapka = new List<string>();
                List<string> Stolbci = new List<string>();
                List<List<string>> Stroki = new List<List<string>>();

                Shapka.Add("АКТ ПРИЕМА-СДАЧИ ТКО НА ОРО ");
                Shapka.Add("ФГУП 'ЖКХ ННЦ'");
                Shapka.Add("На ОРО Полигон ТКО");
                Shapka.Add(Date.ToString("dd.MM.yyyy"));
                List<MarshrutsALL> Marshruts = db.MarshrutsAlls.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day && x.Type.Equals("A")).ToList();
                List<Poligon> Poligons = db.Poligons.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day == Date.Day).ToList();
                //подгружаем данные
                LoadAvtomobils(ref Marshruts);
                LoadMusorPloshadkas(ref Marshruts);
                int number = 1;
                Stolbci.Add("№ п/п");
                Stolbci.Add("Дата прибытия мусоровоза на ОРО");
                Stolbci.Add("Марка мусоровоза");
                Stolbci.Add("гос. номер");
                Stolbci.Add("Объем ТКО (м3)");
                Stolbci.Add("Масса ТКО (Тн)");

                decimal AllObiem = 0;
                int AllMassa = 0;
                List<string> Str = new List<string>();
                foreach (MarshrutsALL M in Marshruts)
                {
                    
                    
                    foreach (Avtomobil A in M.Avtomobils7)
                    {
                        int MassMusor = 0;
                       
                        
                        MassMusor = Convert.ToInt32(Poligons.Where(x => x.AvtomobilId == A.Id).Sum(x => x.MassMusor));
                        Str = new List<string>();
                        Str.Add(number.ToString());//номер по порядку
                        Str.Add(Date.ToString("dd.MM.yy"));//дата
                        Str.Add(A.Type.Type);//номер тачки
                        Str.Add(A.Number);
                        Str.Add(M.ObiemFact.ToString());
                        Str.Add(M.MassaFact.ToString());
                        Stroki.Add(Str);
                        
                        number++;

                    }

                    AllObiem += M.ObiemFact;
                    AllMassa += M.MassaFact;


                }
              
                Str = new List<string>();
                Str.Add("");//номер по порядку
                Str.Add("ИТОГО");//дата
                Str.Add("");//номер тачки
                Str.Add("");
                Str.Add(AllObiem.ToString());
                Str.Add(AllMassa.ToString());
                Stroki.Add(Str);
                Str = new List<string>();
               // Str.Add("СДАЛ: _____________директор ФГУП 'ЖКХ ННЦ' Михеев В.П.");
                string Path2 = Url.Content("~/Files/Transport/TransportOtchetShort" + Date.ToString("dd.MM.yyyy") + ".xlsx");
                string path = Server.MapPath("~/Files/Transport/TransportOtchetShort" + Date.ToString("dd.MM.yyyy") + ".xlsx");

                ExcelExportMonth.StandartExport(Stolbci, Stroki, Shapka, path);
                string file = Path2;
                string filename = "TransportOtchetShort" + Date.ToString("dd.MM.yyyy") + ".xlsx";
                string contentType = "application/vnd.ms-excel";
                //патч,тип файла,новое имя файла
                // return File(file, contentType, filename);//отправка файла пользователю (сохранение, скачать файл)
                //return File(Path2, ".xlsx");
                return Json(Path2);
            }
            return Json("Ошибочка вышла!", ".txt");
        }


        public void ObnovitMassuVMarshrutah(MarshrutsALL MA)
        {
            //обновляем суммарную массу на авто в активных маршрутах
            List<Poligon> poligon = new List<Poligon>();
            try
            {
                //берем только активные маршруты на сегодня
                poligon = db.Poligons.Where(x => x.Date.Year == MA.Date.Year && x.Date.Month == MA.Date.Month && x.Date.Day == MA.Date.Day).ToList();
                string[] S = MA.Avtomobils.Split(';');
                MA.MassaFact = 0;
                foreach (Poligon p in poligon)
                {
                   
                    foreach (string s in S)
                    {
                        try
                        {
                            int Id = Convert.ToInt32(s);
                            //если автомобиль есть в активных маршрутах на сегодня то обновляем его массу
                            if (p.AvtomobilId == Id)
                            {
                                MA.MassaFact += Convert.ToInt32(p.MassMusor);
                              
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                db.Entry(MA).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
        }


        [HttpPost]
        public JsonResult AddAvto(int AvtoId, int Id = 0)
        {
            Avtomobil A = new Avtomobil();
            MarshrutsALL MB = null;
            try
            {
                MB = db.MarshrutsAlls.Where(x => x.Id == Id).First();
                A = db.Avtomobils.Where(x => x.Id == AvtoId).First();
                if (MB.Avtomobils != null)
                {
                    string[] S = MB.Avtomobils.Split(';');
                    foreach (string SS in S)
                    {

                        if (SS.Replace(" ", "").Equals(AvtoId.ToString()))
                        {
                            return Json(null);
                        }
                    }
                }
                if (MB.Avtomobils == null) { MB.Avtomobils = A.Id.ToString(); }
                else
                {
                    if (MB.Type.Equals("A"))
                    {
                        MB.Avtomobils = A.Id.ToString();
                    }
                    else
                    {
                        MB.Avtomobils += ";"+ A.Id.ToString();
                    }
                }
                db.Entry(MB).State = EntityState.Modified;
                db.SaveChanges();
                if (MB.Type.Equals("A")) { ObnovitMassuVMarshrutah(MB); }
                MB = db.MarshrutsAlls.Where(x => x.Id ==Id).First();
            }
            catch { }
            int I = Id;
            return Json(MB.MassaFact);
        }
        [HttpPost]
        public JsonResult DeleteAvto(int AvtoId, int MarshrutId = 0)
        {
            Avtomobil A = new Avtomobil();
            MarshrutsALL MB = null;
            try
            {
                MB = db.MarshrutsAlls.Where(x => x.Id == MarshrutId).First();
                A = db.Avtomobils.Where(x => x.Id == AvtoId).First();
                string[] S = MB.Avtomobils.Split(';');
                string NewAvtos = "";
                foreach (string SS in S)
                {
                    if (SS.Replace(" ", "").Equals(AvtoId.ToString()) == false)
                    {
                        NewAvtos += SS + ";";
                    }
                }
                NewAvtos = NewAvtos.Remove(NewAvtos.Length - 1, 1);//удаляем ; в конце
                MB.Avtomobils = NewAvtos;
                db.Entry(MB).State = EntityState.Modified;
                db.SaveChanges();
                if (MB.Type.Equals("A")) { ObnovitMassuVMarshrutah(MB); }
                MB = db.MarshrutsAlls.Where(x => x.Id == MarshrutId).First();
                return Json(MB.MassaFact);
            }
            catch
            {
                return Json(null);
            }
            //int I = MarshrutId;
          
        }


        [HttpPost]
        public ActionResult AddPloshadka(int PloshadkaId, int Id = 0)
        {
            MusorPloshadka A = new MusorPloshadka();
            MarshrutsALL MB;
            try
            {
                MB = db.MarshrutsAlls.Where(x => x.Id == Id).First();
                A = db.MusorPloshadkas.Where(x => x.Id == PloshadkaId).First();
                if (MB.MusorPloshadkas != null)
                {
                    string[] S = MB.MusorPloshadkas.Split(';');
                    foreach (string SS in S)
                    {
                        if (SS.Replace(" ", "").Equals(PloshadkaId.ToString()))
                        {
                            return RedirectToAction("MarshrutsBaseIndex");
                        }
                    }
                }
                if (MB.MusorPloshadkas == null) { MB.MusorPloshadkas = A.Id.ToString(); }
                else
                {
                    MB.MusorPloshadkas += ";" + A.Id;
                }
                db.Entry(MB).State = EntityState.Modified;
                db.SaveChanges();
                MB = LoadMusorPloshadkaInMarshrut(MB);
                
               

            }
            catch { }
            int I = Id;
            return RedirectToAction("MarshrutsBaseIndex", Id = I);
        }
        public MarshrutsALL LoadMusorPloshadkaInMarshrut(MarshrutsALL M)
        {
            string[] S = M.MusorPloshadkas.Split(';');
            List<MusorPloshadka> MP = new List<MusorPloshadka>();
            foreach (string s in S)
            {
                int Id = Convert.ToInt32(s);
                try
                {
                    MP.Add(db.MusorPloshadkas.Where(x => x.Id == Id).First());
                }
                catch
                {

                }
            }
            LoadPloshadkasNames(ref MP);
            LoadMusorObiems(ref MP);
            M.MusorPloshadkas7 = MP;
            List<MarshrutsALL> LM = new List<MarshrutsALL>();
            LM.Add(M);
            RefreshObiems(LM);
            return M;

        }


        [HttpPost]
        public ActionResult AddToAll(int PloshadkaId = 0, int AvtoId = 0, string Marshruts = "")
        {
            List<MarshrutsALL> MB = new List<MarshrutsALL>();
            if (Marshruts != "")
            {
                
                string[] S = Marshruts.Split(';');
                foreach (string SS in S)
                {
                    int Id = Convert.ToInt32(SS);
                    MB.Add(db.MarshrutsAlls.Where(x => x.Id == Id).First());

                }
            }
            if (PloshadkaId != 0)
            {
                MusorPloshadka A = new MusorPloshadka();
                A = db.MusorPloshadkas.Where(x => x.Id == PloshadkaId).Include(x=>x.Type).First();
                foreach (MarshrutsALL M in MB)
                {
                    try
                    {
                        if (M.MusorPloshadkas == null) { M.MusorPloshadkas = ""; }
                        string[] S = M.MusorPloshadkas.Split(';');
                        bool go = true;
                        foreach (string SS in S)
                        {


                            if (SS.Replace(" ", "").Equals(PloshadkaId.ToString()))
                            {
                                go = false;
                                break;
                            }

                        }
                        if (go)
                        {
                            A.Obiem7 = LoadMusorPloshadkaObiems7(A);
                            A.Kontainers7 = LoadMusorPloshadkaKontainers7(A);
                            if (M.MusorPloshadkas.Length > 1)//в имени более 1 символа
                            {
                                M.MusorPloshadkas += ";" + A.Id;
                            }
                            else
                            {
                                M.MusorPloshadkas =  A.Id.ToString();
                            }
                            db.Entry(M).State = EntityState.Modified;
                            db.SaveChanges();
                            //Добавляем активную площадку только если маршрут активный
                            if (M.Type.Equals("A"))
                            {
                                MusorPloshadkaActive MPA = new MusorPloshadkaActive();
                                MPA.KontainersFact = A.Kontainers7[M.Day];
                                MPA.MarshrutId = M.Id;
                                MPA.PloshadkaId = A.Id;
                                MPA.ObiemFact = A.Obiem7[M.Day];
                                db.MusorPloshadkaActives.Add(MPA);
                                db.SaveChanges();
                            }

                        }
                        LoadMusorPloshadkaInMarshrut(M);
                    }
                    catch
                    {

                    }
                }

            }
            if (AvtoId != 0)
            {
                Avtomobil A = new Avtomobil();

                A = db.Avtomobils.Where(x => x.Id == AvtoId).First();

                foreach (MarshrutsALL M in MB)
                {
                    if (M.Avtomobils == null) { M.Avtomobils = ""; }
                    string[] S = M.Avtomobils.Split(';');
                    try
                    {
                        bool go = true;
                        foreach (string SS in S)
                        {

                            if (SS.Replace(" ", "").Equals(AvtoId.ToString()))
                            {
                                go = false;
                                break;
                            }
                        }
                        if (go)
                        {
                            if (S.Length > 0)
                            {
                                M.Avtomobils += ";" + A.Id;
                            }
                            else
                            {
                                M.Avtomobils = A.Id.ToString();
                            }
                            db.Entry(M).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    catch
                    {

                    }
                    
                }
            }
            return Json("ОК");
        }
        
            
          /*  try
            {
                MB = db.MarshrutsBases.Where(x => x.Id == Id).First();
                A = db.MusorPloshadkas.Where(x => x.Id == PloshadkaId).First();
                string[] S = MB.MusorPloshadkas.Split(';');
                foreach (string SS in S)
                {
                    if (SS.Replace(" ", "").Equals(PloshadkaId.ToString()))
                    {
                        return RedirectToAction("MarshrutsBaseIndex");
                    }
                }
                MB.MusorPloshadkas += ";" + A.Id;
                db.Entry(MB).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch { }
            int I = Id;*/
          
        



        [HttpPost]
        public ActionResult ADDMP(int PloshadkaId, int Id = 0)
        {
            MusorPloshadka A = new MusorPloshadka();
            MarshrutsALL MB ;
            decimal obiemfact = 0;
            decimal kontainersfact = 0;
            decimal Obiems = 0;
            int mpa = 0;
            try
            {
                MB = db.MarshrutsAlls.Where(x => x.Id == Id).First();
                A = db.MusorPloshadkas.Where(x => x.Id == PloshadkaId).Include(x=>x.Type).First();
                if (MB.MusorPloshadkas != null)
                {
                    string[] S = MB.MusorPloshadkas.Split(';');
                    foreach (string SS in S)
                    {
                        if (SS.Replace(" ", "").Equals(PloshadkaId.ToString()))
                        {
                            return Json("D");
                        }
                    }
                }
                A.Kontainers7 = LoadMusorPloshadkaKontainers7(A);
                A.Obiem7 = LoadMusorPloshadkaObiems7(A);
                if (MB.MusorPloshadkas == null) { MB.MusorPloshadkas = A.Id.ToString(); }
                else
                {
                    MB.MusorPloshadkas += ";" + A.Id;
                }


                MB.Modify = false;//Если добавили площадку то пересчитываем объёмы, отменяем ручной ввод данных
                db.Entry(MB).State = EntityState.Modified;
                db.SaveChanges();
                MB = LoadMusorPloshadkaInMarshrut(MB);


                
                //Добавляем активную площадку только если маршрут активный
                if (MB.Type.Equals("A"))
                {
                    MusorPloshadkaActive MPA = new MusorPloshadkaActive();
                    MPA.KontainersFact = A.Kontainers7[MB.Day];
                    MPA.MarshrutId = MB.Id;
                    MPA.PloshadkaId = A.Id;
                    MPA.ObiemFact = A.Obiem7[MB.Day];
                    db.MusorPloshadkaActives.Add(MPA);
                    db.SaveChanges();
                    obiemfact = MPA.ObiemFact;
                    kontainersfact = MPA.KontainersFact;

                    Obiems = db.MusorPloshadkaActives.Where(x => x.MarshrutId == MB.Id).Sum(x => x.ObiemFact);
                    mpa = MPA.Id;

                }
                else
                {
                    Obiems = MB.MusorPloshadkas7.Sum(x => x.Obiem7[MB.Day]);
                }
             //   decimal Obiems2 = 0;
             //   foreach (MusorPloshadka m in MB.MusorPloshadkas7)
             //   {
                   
             //   }

            }
            catch(Exception e)
            {

            }
            int I = Id;
            return Json(Obiems+";"+mpa+";"+obiemfact+";"+kontainersfact);
        }

        [HttpPost]
        public ActionResult DeletePloshadka(int PloshadkaId, int Id = 0)
        {
            MusorPloshadka A = new MusorPloshadka();
            MarshrutsALL MB;
            decimal Obiems = 0;
            try
            {
                MB = db.MarshrutsAlls.Where(x => x.Id == Id).First();
                A = db.MusorPloshadkas.Where(x => x.Id == PloshadkaId).First();
                string[] S = MB.MusorPloshadkas.Split(';');
                string NewPloshadkas = "";
                foreach (string SS in S)
                {
                    if (SS.Replace(" ", "").Equals(PloshadkaId.ToString())==false)
                    {
                        NewPloshadkas += SS+";";
                    }
                }
                NewPloshadkas = NewPloshadkas.Remove(NewPloshadkas.Length - 1, 1);//удаляем ; в конце
                MB.MusorPloshadkas = NewPloshadkas;
                MB.Modify = false;//Если удалили площадку то пересчитываем объёмы, отменяем ручной ввод данных
                db.Entry(MB).State = EntityState.Modified;
                db.SaveChanges();
                MB = LoadMusorPloshadkaInMarshrut(MB);
                //Удаляем активную площадку только если маршрут активный
                if (MB.Type.Equals("A"))
                {
                    try
                    {
                        MusorPloshadkaActive MPA = db.MusorPloshadkaActives.Where(x => x.MarshrutId == MB.Id && x.PloshadkaId == PloshadkaId).First();
                        db.MusorPloshadkaActives.Remove(MPA);
                        db.SaveChanges();
                        Obiems = db.MusorPloshadkaActives.Where(x => x.MarshrutId == MB.Id).Sum(x => x.ObiemFact);
                    }
                    catch (Exception e)
                    {

                    }
                }

            }
            catch { }
            int I = Id;
            
            return Json(Obiems);
        }


        [HttpPost]
        public JsonResult IDPloshadki(int Id=0, string V="" )
        {
            string Data = "";
            try
            {
                MusorPloshadka M = db.MusorPloshadkas.Where(x => x.Id == Id).First();
                M.IDPloshadki = V;
                db.Entry(M).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return Json(Data);
        }
        [HttpPost]
        public JsonResult NameUL(int Id = 0, string V = "")
        {
            string Data = "";
            try
            {
                MusorPloshadka M = db.MusorPloshadkas.Where(x => x.Id == Id).First();
                M.NameUL = V;
                db.Entry(M).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return Json(Data);
        }
        [HttpPost]
        public JsonResult UL(int Id = 0, string V = "")
        {
            string Data = "";
            try
            {
                MusorPloshadka M = db.MusorPloshadkas.Where(x => x.Id == Id).First();
                M.UL = V;
                db.Entry(M).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return Json(Data);
        }


        [HttpPost]
        public ActionResult ReKontainer(decimal Value = 0, int Day = 0, int Id = 0)
        {//день, размер и ид площадки
            string Data = "";
            MusorPloshadka MP = new MusorPloshadka();
            string[] S = new string[7] { "0", "0", "0", "0", "0", "0", "0" };
            try
            {
                MP = db.MusorPloshadkas.Where(x => x.Id == Id).Include(x=>x.Type).First();
                string[] SS = MP.Kontainers.Split(';');
                for (int i = 0; i < SS.Length; i++)
                {
                    S[i] = SS[i];
                }
                S[Day] = Value.ToString();
                string result = "";
                for (int i = 0; i < 7; i++)
                {
                    result += S[i] + ";";
                }
                MP.Kontainers = result.Remove(result.Length - 1, 1);
              

               
                db.Entry(MP).State = EntityState.Modified;
                db.SaveChanges();
                if (MP.TypeId > 1)
                {
                    RefreshObiems(MP);
                }
                MP = db.MusorPloshadkas.Where(x => x.Id == Id).First();
                Data = MP.Obiem;
            }
            catch
            {

            }
            return Json(Data);
        }

     

        [HttpPost]
        public ActionResult ReObiem(decimal Value = 0, int Day=0, int Id = 0)
        {//день, размер и ид площадки
            string Data = "";
            MusorPloshadka MP = new MusorPloshadka();
            string[] S = new string[7] { "0", "0", "0", "0", "0", "0", "0" };
            try
            {
                MP = db.MusorPloshadkas.Where(x => x.Id == Id).First();
                string[] SS = MP.Obiem.Split(';');
                for (int i = 0; i < SS.Length; i++)
                {
                    S[i] = SS[i];
                }
                S[Day] = Value.ToString();
                string result = "";
                for (int i=0;i<7;i++)
                {
                    result += S[i]+";";
                }
                MP.Obiem = result.Remove(result.Length - 1, 1);
                db.Entry(MP).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return Json(Data);
        }

        [HttpPost]
        public ActionResult ImportMusorPloshadka(int Day, HttpPostedFileBase file)
        {
            string errors = "";
            string fileName = System.IO.Path.GetFileName(file.FileName);
            // сохраняем файл в папку Files в проекте
            if (Directory.Exists(Server.MapPath("~/Files/")) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files/"));

            }
            file.SaveAs(Server.MapPath("~/Files/" + fileName));
            //обрабатываем файл после загрузки
            string[] Names = new string[] { "УЛИЦА", "НОМЕРДОМА", "ОБЪЕМКОНТЕЙНЕРАМ3", "ГРАФИКВЫВОЗА","Кол-во контейнеров шт.","Объем ТКО за смену, м3","IDПлощадки"};
            string Error = "";
            List<List<string>> excel = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName), Names, out Error);
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
                List<MusorPloshadka> LMP = new List<MusorPloshadka>();
                LMP = db.MusorPloshadkas.ToList();
                foreach (List<string> E in excel)
                {
                    MusorPloshadka MP = new MusorPloshadka();
                   
                    //Грузим улицы из базы 
                    string[] Streets = E[0].Split(',');
                    List<AllStreet> StreetsExcel = new List<AllStreet>();
                    foreach (string ss in Streets)
                    {
                        try
                        {
                            AllStreet AS = db.AllStreets.Where(x => x.Name.Replace(" ", "").Equals(ss.Replace(" ", ""))).First();
                            StreetsExcel.Add(AS);
                        }
                        catch
                        {

                        }
                    }

                    for (int i=0;i< LMP.Count;i++)
                    {
                        //загружаем все улицы
                        string[] S = LMP[i].StreetId.Split(';');
                        foreach (string s in S)
                        {
                          
                            try
                            {
                                LMP[i].VseUlici = new List<AllStreet>();
                                int id = Convert.ToInt32(s);
                                LMP[i].VseUlici.Add(db.AllStreets.Where(x => x.Id == id).First());
                            }
                            catch { }
                        } 
                    }
                    if (StreetsExcel.Count > 0) {
                        //ищем улицу совпавшую с файлом
                        string NomerDoma = E[1].Replace(" ","").Replace(",","").Replace(".","").Replace("-","");
                        List<MusorPloshadka> Chastichno = LMP.Where(x => x.Name.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("-", "").Equals(NomerDoma)).ToList();
                        List<MusorPloshadka> Polnostu = new List<MusorPloshadka>();
                        foreach (MusorPloshadka AS in Chastichno)
                        {
                            int go = StreetsExcel.Count;
                            foreach (AllStreet ES in StreetsExcel)
                            {
                                try
                                {
                                    

                                    try
                                    {
                                        if (AS.VseUlici.Contains(ES)) { go--; break; }
                                    }
                                    catch
                                    {

                                    }

                                }
                                catch
                                {

                                }
                            }
                            if (go==0)
                            {
                                Polnostu.Add(AS);
                            }
                        }
                        if (Polnostu.Count>1)
                        {
                            MusorPloshadka Test = null;
                            try
                            {
                                Test = Polnostu.Where(x => x.ObiemContainera == Convert.ToDecimal(E[2])).First();
                            }
                            catch
                            {

                            }
                            try
                            {
                                Test = Polnostu.First();
                            }
                            catch
                            {

                            }
                            if (Test != null)
                            {
                                Polnostu = new List<MusorPloshadka>();
                                Polnostu.Add(Test);
                            }


                        }
                        if (Polnostu.Count==1)
                        {
                            MP = Polnostu[0];
                            MP.GrafikVivoza = E[3];
                            MP.ObiemContainera = Convert.ToDecimal(E[2]);
                            MP.IDNew = E[5].Replace(" ", "");
                            if (MP.ObiemContainera>0)
                            {

                               

                            }
                            string[] kontainers = MP.Kontainers.Split(';');

                            kontainers[Day] = "0";
                            try
                            {
                                kontainers[Day] = Convert.ToInt32(E[4]).ToString();

                            }catch
                            {

                            }
                            string[] Obiems = MP.Obiem.Split(';');
                            Obiems[Day] = E[5];
                            string ob = "";
                            string kont = "";
                            for (int i=0;i<7;i++)
                            {
                                kont += kontainers[i]+";";
                                ob += Obiems[i] + ";";
                            }
                            kont = kont.Remove(kont.Length - 1, 1);
                            ob = ob.Remove(ob.Length - 1, 1);
                            MP.Kontainers = kont;
                            MP.Obiem = ob;
                            try
                            {
                                db.Entry(MP).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            catch
                            {

                            }
                            
                            
                            
                            
                        }
                        else
                        {
                            string s = "Ошибка загрузки";
                            if (Polnostu.Count > 1) { s = "У площадки "+Polnostu.Count.ToString()+" варианта, не могу определиться"; }
                            if (Polnostu.Count == 0) { s = "У площадки нет совпадений, были варианты: ";
                                for (int i=0;i<Chastichno.Count;i++)
                                {
                                    s += Chastichno[i].VseUlici[0].Name+" "+Chastichno[i].Name + ";";
                                }
                            }
                            errors += s +E[0]+" "+E[1]+";";
                        }

                    }
                }

            }
            errors = errors.Remove(errors.Length - 1, 1);
            HttpCookie cookie = new HttpCookie("Errors");

            cookie["Errors"] = errors;
            // Добавить куки в ответ
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult SaveTypePloshadki(int Id = 0, int Val = 0)
        {
            MusorPloshadka MP = db.MusorPloshadkas.Where(x => x.Id == Id).First();
            MP.TypeId = Val;
            db.Entry(MP).State = EntityState.Modified;
            db.SaveChanges();
            MP = db.MusorPloshadkas.Where(x => x.Id == Id).Include(x=>x.Type).First();

            return Json(MP.Type.Ico);
        }
        [HttpPost]
        public JsonResult SaveNamePloshadki(int Id = 0, string Val = "")
        {
            MusorPloshadka MP = db.MusorPloshadkas.Where(x => x.Id == Id).First();
            MP.Name = Val;
            db.Entry(MP).State = EntityState.Modified;
            db.SaveChanges();
           

            return Json("");
        }
        public string SgatObiems (MusorPloshadka MP)
        {
            string M = "";
            foreach(decimal O in MP.Obiem7)
            {
                M += Math.Round(O, 2).ToString()+";";
            }
            M = M.Remove(M.Length - 1, 1);
            return M;
        }
        public void RefreshObiems (MusorPloshadka MP)
        {

            MP.Obiem7 = LoadMusorPloshadkaObiems7(MP);
            MP.Kontainers7 = LoadMusorPloshadkaKontainers7(MP);
            for (int i=0;i<7;i++)
            {
                MP.Obiem7[i] = MP.Kontainers7[i] * MP.ObiemContainera;
            }
            MP.Obiem = SgatObiems(MP);
            db.Entry(MP).State = EntityState.Modified;
            db.SaveChanges();
        }
        [HttpPost]
        public JsonResult SaveObiemContainera(int Id =0, decimal Val =0)
        {
            MusorPloshadka MP = db.MusorPloshadkas.Where(x => x.Id == Id).First();
            MP.ObiemContainera = Val;
            if (MP.TypeId > 1)//если не мешки то обновляем объёмы исходя из объёма контейнера
            {
                RefreshObiems(MP);
            }
            else
            {
               
            }
            db.Entry(MP).State = EntityState.Modified;
            db.SaveChanges();

            return Json (MP.Obiem);
        }

        [HttpPost]
        public ActionResult AddMusorPloshadka( string MKD = "", string Obiem = "0;0;0;0;0;0;0", string Kontainers = "0;0;0;0;0;0;0", string UL = "", string StreetId = "", int Id = 0, string Name = "",decimal KontainerObiem=0, int TypeId=1)
        {//день, размер и ид площадки
            string Data = "";
            MusorPloshadka MP = new MusorPloshadka();
            if (StreetId != null && StreetId != "" && StreetId != null&&Name.Equals("")==false)
            {
                MP.IDPloshadki = Id.ToString();
                if (UL.Equals("0")) { UL = ""; }
                MP.NameUL = UL;
                MP.ObiemContainera = KontainerObiem;
                MP.TypeId = TypeId;
                MP.Obiem = Obiem;
                MP.TKO = true;
                MP.Kontainers = Kontainers;
                MP.UL = MKD;
                MP.Name = Name;
                MP.StreetId = StreetId;
                try
                {

                    db.MusorPloshadkas.Add(MP);
                    db.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
            return Json(Data);
        }

        [HttpPost]
        public ActionResult DeleteStreet(int StreetId=0, int Id=0)
        {
            
            if (StreetId != 0 && Id != 0)
            {
                MusorPloshadka MP = new MusorPloshadka();
                try
                {
                    MP = db.MusorPloshadkas.Where(x => x.Id == Id).First();
                    string[] S = MP.StreetId.Split(';');
                    string newStreets = "";
                    foreach (string ss in S)
                    {
                        if (ss.Equals(StreetId.ToString()) == false)
                        {
                            newStreets += ss + ";";
                        }
                    }
                    newStreets = newStreets.Remove(newStreets.Length - 1, 1);
                    MP.StreetId = newStreets;
                    db.Entry(MP).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch
                {

                }
            }
            return RedirectToAction("Index",StreetId=0);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,StreetId,Obiem,IDPloshadki,NameUL,UL,TKO")] MusorPloshadka musorPloshadka)
        {
            if (ModelState.IsValid)
            {
                db.MusorPloshadkas.Add(musorPloshadka);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StreetId = new SelectList(db.AllStreets, "Id", "Name", musorPloshadka.StreetId);
            return View(musorPloshadka);
        }

        // GET: MusorPloshadkas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MusorPloshadka musorPloshadka = db.MusorPloshadkas.Find(id);
            if (musorPloshadka == null)
            {
                return HttpNotFound();
            }
            ViewBag.StreetId = new SelectList(db.AllStreets, "Id", "Name", musorPloshadka.StreetId);
            return View(musorPloshadka);
        }

        // POST: MusorPloshadkas/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,StreetId,Obiem,IDPloshadki,NameUL,UL,TKO")] MusorPloshadka musorPloshadka)
        {
            if (ModelState.IsValid)
            {
                db.Entry(musorPloshadka).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StreetId = new SelectList(db.AllStreets, "Id", "Name", musorPloshadka.StreetId);
            return View(musorPloshadka);
        }

        // GET: MusorPloshadkas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MusorPloshadka musorPloshadka = db.MusorPloshadkas.Find(id);
            if (musorPloshadka == null)
            {
                return HttpNotFound();
            }
            return View(musorPloshadka);
        }

        // POST: MusorPloshadkas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MusorPloshadka musorPloshadka = db.MusorPloshadkas.Find(id);
            db.MusorPloshadkas.Remove(musorPloshadka);
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
