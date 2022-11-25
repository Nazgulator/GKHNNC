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
    public class PoligonsController : Controller
    {
        private WorkContext db = new WorkContext();
        private AutomarshallContext adb = new AutomarshallContext();

        // GET: Poligons
        public ActionResult Index(string Number, int Poisk = 0, int PoiskKontr = 0, int result = 0, int Vesi = 0, int Avtosort = 0,string TekDate="", string Date2 = "")
        {
            DateTime Date = DateTime.Now;
            DateTime DateTo = DateTime.Now;

            if (Date2!="")
            {
                //   if (Date2.Contains("/"))
                //    {
                //       string[] S = Date2.Split('/');
                //         DateTo = new DateTime(Convert.ToInt16(S[0]), Convert.ToInt16(S[1]), Convert.ToInt16(S[1]));
                //    }

                DateTo = Convert.ToDateTime(Date2);
                ViewBag.Date2 = DateTo;
            }
          
                if (TekDate!="")
            {
                Date = Convert.ToDateTime(TekDate);
              //  if (TekDate.Contains("/"))
              //  {
            //        string[] S = Date2.Split('/');
           //         Date = new DateTime(Convert.ToInt16(S[0]), Convert.ToInt16(S[1]), Convert.ToInt16(S[1]));
           //     }
                 
                //добавляем куки с осмотром
                HttpCookie cookie = new HttpCookie("Poligon");
                cookie["Date"] = Date.ToString();
                // Добавить куки в ответ
                Response.Cookies.Add(cookie);
            }
            else
            {
             //    HttpCookie cookieReq = Request.Cookies["Poligon"];
            //     if (cookieReq != null)
            //     {
           //          Date = Convert.ToDateTime(cookieReq["Date"]);
          //       }
               
            }
            if (Date.Day == DateTime.Now.Day && Date.Month == DateTime.Now.Month && Date.Year == DateTime.Now.Year) { Date = DateTime.Now; }
            if (Number != null)
            {
                //если выбрана машина 
                ViewBag.Number = Number;
                //и у нее уже были заезды, то забьем массу выезда сразу
                try
                {
                    int idA = db.Avtomobils.Where(x => x.Number.Equals(Number)).Select(x => x.Id).First();
                    ViewBag.MassOut = db.Poligons.Where(x => x.AvtomobilId == idA&& x.Date.Day == Date.Day && x.Date.Month == Date.Month && x.Date.Year == Date.Year).Select(x=>x.MassOut).First();
                   
                }
                catch
                {

                }

            }
            else
            {
                ViewBag.Number = null;
            }
            List<string> A = new List<string>();

            try
            {
                A = db.Avtomobils.OrderBy(x => x.Number).Select(x => x.Number).ToList();
            }
            catch (Exception e)
            {

            }
            ViewBag.AllKontrAgents = new SelectList(db.KontrAgents, "Id", "Name");
            if (User.Identity.Name.Contains("Администратор")||User.IsInRole("Администратор"))
            {
                ViewBag.KontrAgents = new SelectList(db.KontrAgents, "Id", "Name");
               
            }
            else
            {//исключаем экологию из списка если не администратор
                ViewBag.KontrAgents = new SelectList(db.KontrAgents.Where(x=>x.Id!=2), "Id", "Name");
            }
            ViewBag.TypeAvtos = new SelectList(db.TypeAvtos, "Id", "Type");
            ViewBag.MarkaAvtomobils = new SelectList(db.MarkaAvtomobils.OrderBy(x => x.Name), "Id", "Name");
            if (Number != null)
            {
                Avtomobil Avto = new Avtomobil();
                try
                {
                    Avto = db.Avtomobils.Where(x => x.Number.Replace(" ", "").Equals(Number)).Include(x => x.Type).Include(x => x.KontrAgent).First();
                    ViewBag.Number = Avto.Number.ToUpper().Replace(" ", "");
                    ViewBag.TypeAvtos = new SelectList(db.TypeAvtos, "Id", "Type");
                    ViewBag.MarkaAvtomobils = new SelectList(db.MarkaAvtomobils.OrderBy(x => x.Name), "Id", "Name");
                    ViewBag.AvtoType = Avto.Type.Type;
                    ViewBag.AvtoImage = Avto.Type.Ico;
                    ViewBag.AvtoKontragent = Avto.KontrAgent.Name;
                    ViewBag.AvtoKontragentId = Avto.KontrAgent.Id;
                    ViewBag.Avto = Avto;
            
                }
                catch(Exception e)
                {
                    ViewBag.Avto = null;
                    ViewBag.Number = null;
                    ViewBag.AvtoType = db.TypeAvtos.Where(x => x.Id == 16).Select(x => x.Type).First();
                    ViewBag.AvtoImage = db.TypeAvtos.Where(x => x.Id == 16).Select(x => x.Ico).First();
                }
            }
            else
            {
                ViewBag.Avto = null;
                ViewBag.AvtoType = db.TypeAvtos.Where(x => x.Id == 16).Select(x => x.Type).First();
                ViewBag.AvtoKontragent = "Нет";
                ViewBag.AvtoKontragentId = 1;
                ViewBag.AvtoImage = db.TypeAvtos.Where(x => x.Id == 16).Select(x => x.Ico).First();
            }

            List<Poligon> Poligons = new List<Poligon>();
            ViewBag.Date = Date;//сохраняем дату для передачи
      


            string NumberPoisk = "";
            try
            {
                DateTime D = Date;
                if (Poisk != 0)
                {
                    if (Date2 == "")
                    {
                        Poligons = db.Poligons.Where(x => x.Date.Year == D.Year && x.Date.Month == D.Month && x.Date.Day == D.Day && x.AvtomobilId == Poisk).Include(z => z.Avtomobil).Include(z => z.Avtomobil.Marka).Include(z => z.Avtomobil.Type).Include(x => x.Marka).Include(x => x.Type).Include(x => x.Avtomobil.KontrAgent).ToList();
                    }
                    else
                    {
                        Poligons = db.Poligons.Where(x => x.Date>=D&&x.Date<=DateTo && x.AvtomobilId == Poisk).Include(z => z.Avtomobil).Include(z => z.Avtomobil.Marka).Include(z => z.Avtomobil.Type).Include(x => x.Marka).Include(x => x.Type).Include(x => x.Avtomobil.KontrAgent).ToList();

                    }

                    NumberPoisk = db.Avtomobils.Where(x => x.Id == Poisk).Select(x => x.Number).First();
                    ViewBag.Poisk = NumberPoisk;
                    ViewBag.PoiskId = Poisk;

                }
                else
                {
                    if (Date2 == "")
                    {
                        Poligons = db.Poligons.Where(x => x.Date.Year == D.Year && x.Date.Month == D.Month && x.Date.Day == D.Day).Include(z => z.Avtomobil).Include(z => z.Avtomobil.Marka).Include(z => z.Avtomobil.Type).Include(x => x.Marka).Include(x => x.Type).Include(x => x.Avtomobil.KontrAgent).ToList();
                    }
                    else
                    {
                        Poligons = db.Poligons.Where(x => x.Date>=D&& x.Date<=DateTo).Include(z => z.Avtomobil).Include(z => z.Avtomobil.Marka).Include(z => z.Avtomobil.Type).Include(x => x.Marka).Include(x => x.Type).Include(x => x.Avtomobil.KontrAgent).ToList();

                    }

                    ViewBag.Poisk = "0";
                    ViewBag.PoiskId = 0;


                }
                //-2 это экология. Ищем все кроме экологии.
                if (PoiskKontr == -2)
                {
                    Poligons = Poligons.Where(x => x.KontrAgentId != 2).ToList();
                    ViewBag.PoiskKontr = "ТАЛОНЫ";
                    ViewBag.PoiskKontrId = -2;
                }
                else
                {
                    if (PoiskKontr != 0)
                    {
                        Poligons = Poligons.Where(x => x.KontrAgentId == PoiskKontr).ToList();
                        ViewBag.PoiskKontr = db.KontrAgents.Where(x => x.Id == PoiskKontr).Select(x => x.Name).First();
                        ViewBag.PoiskKontrId = PoiskKontr;
                    }
                    else
                    {
                        ViewBag.PoiskKontr = "0";
                        ViewBag.PoiskKontrId = 0;
                    }
                }

       

            }
            catch (Exception Ex)
            {
            }

            Dictionary<char, char> DicVer = new Dictionary<char, char>()
                {
                    {'A','А'},
                    {'E','Е'},
                    {'Y','У'},
                    {'P','Р'},
                    {'M','М'},
                    {'K','К'},
                    {'Z','З'},
                    {'X','Х'},
                    {'C','С'},
                    {'O','О'},
                    {'L','Л'},
                    {'B','В'},
                    {'H','Н'},
                    {'T','Т'}
                };






            // List<Poligon> P2 = Poligons;
            //сверяем камеру с людьми
           

            if (User.IsInRole("Администратор")|| User.Identity.Name.Equals("НачальникПолигона")||User.Identity.Name.Equals("Полигон"))
            {
                List<AutomarshallView> AW = new List<AutomarshallView>();
              
                try
                {
                    if (User.IsInRole("Администратор") || User.Identity.Name.Equals("НачальникПолигона"))
                    {
                        if (Date2 == "")
                        {
                            AW = adb.AutomarshallViews.Where(x => x.TimeStamp.Year == Date.Year && x.TimeStamp.Month == Date.Month && x.TimeStamp.Day == Date.Day).ToList();
                        }
                        else
                        {
                            AW = adb.AutomarshallViews.Where(x => x.TimeStamp>=Date&&x.TimeStamp<=DateTo).ToList();
                        }
                    }
                    else
                    {
                        AW = adb.AutomarshallViews.Where(x => x.TimeStamp.Year == Date.Year && x.TimeStamp.Month == Date.Month && x.TimeStamp.Day == Date.Day&&x.TimeStamp.Hour>=Date.Hour-8).ToList();
                    }

                }
                catch (Exception e)
                {
                    ViewBag.ERROR = e.Message;
                   // return Json(e.Message);
                }
                if (Poisk != 0)
                {
                    try
                    {
                        AW = AW.Where(x => x.Plate.Contains(NumberPoisk)).ToList();
}
                    catch
                    {

                    }
                }
                if (PoiskKontr != 0)
                {
                    AW = new List<AutomarshallView>();
                }

                    List<Avtomobil> Avtomobils = db.Avtomobils.Include(x => x.Marka).Include(x => x.KontrAgent).Include(x => x.Type).ToList();
                TypeAvto Typ = db.TypeAvtos.Where(x => x.Id == 16).First();
                foreach (AutomarshallView V in AW)
                {
                    Avtomobil Avto = new Avtomobil();
                    foreach (char ch in V.Plate)
                    {
                        V.Plate = V.Plate.Replace(ch, DicVer.ContainsKey(ch) ? DicVer[ch] : ch);
                    }
                    V.Plate = V.Plate.Replace(" ", "").Replace("#","");
                    string NumRe = V.Plate.Remove(0, V.Plate.Length - 3);
                    int count = 0;
                    string N54 = "";
                    foreach (char s in NumRe)
                    {


                        if (Char.IsNumber(s))
                        {
                            N54 += s;
                            count++;
                        }
                       else
                        {
                            N54 = "";
                            count = 0;
                        }
                    }


                    if (count > 0)
                    {
                        if (N54.Contains("154") == false && N54.Contains("15"))
                        {
                            V.Plate = V.Plate.Replace("15", "154");
                        }
                        else
                        {
                           
                            if (count < 3&&N54.Equals("54")==false)
                            {
                                V.Plate = V.Plate.Remove(V.Plate.Length - count, count);
                                try
                                {
                                    
                                    V.Plate = Avtomobils.Where(x => x.Number.Contains(V.Plate)).Where(x => x.Number.Equals(V.Plate)).Select(x=>x.Number).First();

                                  
                                }
                                catch
                                {
                                    V.Plate += "154";
                                }
                             //   V.Plate = V.Plate.Remove(0, V.Plate.Length - 3);
                              //  V.Plate += "154";
                            }
                        }
                    }
                    Poligon P = new Poligon();
                    try
                    {
                        P = Poligons.Where(x => x.Number.Equals(V.Plate)&&x.KontrAgentName.Equals("Камера")==false&& V.TimeStamp.AddHours(7) <= x.Date.AddMinutes(40) && V.TimeStamp.AddHours(7) >= x.Date.AddMinutes(-40)).First();
                        P.CameraFix = true;
                        var base64 = Convert.ToBase64String(V.Picture);
                        P.Picture = String.Format("data:image/jpg;base64,{0}", base64);
                        var plate64 = Convert.ToBase64String(V.PlateShot);
                        P.PlateShot = String.Format("data:image/jpg;base64,{0}", plate64);
                        P.IdCam = V.LogId;
                    }
                    catch (Exception e)
                    {
                    
                        try
                        {
                            Avto = Avtomobils.Where(x => x.Number.Equals(V.Plate)).First();
                            P.Avtomobil = Avto;
                            P.User = "Камера";
                            P.VibralRab = false;
                            P.Number = V.Plate;
                            P.MassIn = Math.Round(Avto.ObiemBunkera * Avto.KoefficientSgatiya * 165.1M, 2); ;
                            P.MassOut = 0;
                            P.MassMusor = P.MassIn;
                            P.KontrAgentId = 103;
                            P.Date = V.TimeStamp.AddHours(7);
                            P.KontrAgentName = "Камера";
                            P.Type = Avto.Type;
                           
                        }
                        catch
                        {
                            Avto = Avtomobils.Where(x => x.Id == 3).First();
                            P.AvtomobilId = Avto.Id;
                            P.Avtomobil = Avto;
                            P.User = "Камера";
                            P.VibralRab = false;
                            P.Number = V.Plate;
                            P.MassIn = 0;
                            P.MassOut = 0;
                            P.MassMusor = 0.0m;
                            P.KontrAgentId = 103;
                            P.Avtomobil.GKHNNC = false;
                            P.TypeId = 16;
                            P.Date = V.TimeStamp.AddHours(7);
                            P.KontrAgentName = "Камера";
                            P.Type = Typ;







                        }
                        var base64 = Convert.ToBase64String(V.Picture);
                        P.Picture = String.Format("data:image/jpg;base64,{0}", base64);
                        var plate64 = Convert.ToBase64String(V.PlateShot);
                        P.PlateShot = String.Format("data:image/jpg;base64,{0}", plate64);
                        P.IdCam = V.LogId;
                        P.Description = "В пределах нормы.";
                        Poligons.Add(P);

                    }
                }

            }

            if (Vesi!=0)
            {
                //Только с целыми весами
                if(Vesi==1)
                {
                    Poligons = Poligons.Where(x => x.MassOut != 0).ToList();
                }
                //Только со сломанными весами
                if (Vesi == 2)
                {
                    Poligons = Poligons.Where(x => x.MassOut == 0).ToList();
                }
            }
            ViewBag.Vesi = Vesi;

            if (Avtosort!=0)
            {
                Poligons = Poligons.OrderBy(x=>x.Number).ThenBy(x => x.Date).ThenByDescending(x=>x.Id).ToList();
                foreach (Poligon P in Poligons)
                {
                    try
                    {
                        P.PoligonIn = db.AutoScans.Where(x => x.AvtoId == P.AvtomobilId && x.Date.Year == P.Date.Year && x.Date.Month == P.Date.Month && x.Date.Day == P.Date.Day).Select(x => x.Poligon).First();
                    }
                    catch { }
                }
            }
                
            else
            {
                Poligons = Poligons.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).ToList();
            }
            ViewBag.Avtosort = Avtosort;
            if (result!=0)
            {
                /*List<Avtomobil> Avtomobils = Poligons.Select(x => x.Avtomobil).Distinct().ToList();
                List<Poligon> NewPoligons = new List<Poligon>();
                foreach (Avtomobil Avto in Avtomobils)
                {
                    Poligon P = new Poligon();
                    P.AvtomobilId = Avto.Id;
                    P.Avtomobil = Avto;
                    P.Description = "";
                    P.MarkaId = Avto.MarkaId;
                    P.Marka = Avto.Marka;
                    P.TypeId = Avto.TypeId;
                    P.Type = Avto.Type;
                    P.Number = Avto.Number;
                    P.MassMusor = Poligons.Where(x => x.AvtomobilId == Avto.Id).Sum(y => y.MassMusor);
                    P.MassIn = Poligons.Where(x => x.AvtomobilId == Avto.Id).Sum(y => y.MassIn);
                    P.MassOut = Poligons.Where(x => x.AvtomobilId == Avto.Id).Sum(y => y.MassOut);
                    P.Description = Poligons.Where(x => x.AvtomobilId == Avto.Id).ToList().Count.ToString();
                    P.KontrAgentName = Avto.KontrAgent.Name;
                    P.KontrAgentId = Avto.KontrAgent.Id;
                    NewPoligons.Add(P);
                }

                Poligons = NewPoligons;
                */

                Poligons = Poligons.Where(x => x.Avtomobil.GKHNNC).ToList();


            }

            //Сверяем с белым листом
            List<WhiteList> White = new List<WhiteList>();
            try
            {
                White = db.WhiteLists.ToList();
            }
            catch
            {

            }
            ViewBag.White = White;
            // List<string> WL = White.OrderBy(x => x.Id).Select(x => x.Nomer.Replace(" ","").Remove(4).Remove(0,1)).ToList(); //сравнение только цифр
            List<string> WL = White.OrderBy(x => x.Id).Select(x => x.Nomer.Replace(" ", "")).ToList();
            foreach (Poligon p in Poligons)
            {
                
                string Nimb = p.Number.Replace(" ", "").Remove(5);
               
                   // int y = Convert.ToInt16(Nimb[0]);
                 //   if (!char.IsDigit(Nimb[0]))

               //     {
                    //   Nimb = Nimb.Remove(0, 1); //сравнение только цифр
              //  }




                p.White = false;
                try
                {
                    var X = White.Where(x => x.Nomer.Contains(Nimb)).First();// WL.Where(x => x.Equals(Nimb)).First();
                    p.White = true;
                    p.MassIn = Math.Round(X.Obiem  * 165.1M, 2); ;
                    p.MassOut = 0;
                    p.MassMusor =p.MassIn;
                    p.Avtomobil.ObiemBunkera = X.Obiem;
                    p.Avtomobil.KontrAgent.Name = X.Kontragent;
                    p.KontrAgentName = X.Kontragent;
                    p.Avtomobil.Marka.Name = X.Marka;
                    p.Tint = "Поиск в белом листе как "+Nimb;
                   // p.MassMusor = X.Obiem;
                }
                catch
                {
                    p.White = false;
                    p.Tint = "Не найдена в белом листе как " + Nimb;
                }
            }




            ViewBag.Resultat = result;
            ViewBag.Summ = Poligons.Sum(x => x.MassMusor);
            return View(Poligons);
        }



        public ActionResult Resultat(string Date)
        {
            DateTime D = DateTime.Now;
            if (Date != null)
            {
                ViewBag.Date = Date;
            }
            else
            {
                ViewBag.Date = D;
            }

            List<Avtomobil> A = new List<Avtomobil>();
            List<Poligon> Result = new List<Poligon>();
            try
            {

                List<Poligon> dbPoligons = db.Poligons.Where(x => x.Date.Year == D.Year && x.Date.Month == D.Month && x.Date.Day == D.Day).Include(x => x.Avtomobil).Include(x => x.Avtomobil.Marka).Include(x => x.Avtomobil.Type).ToList();
                A = dbPoligons.Where(x => x.Date.Year == D.Year && x.Date.Month == D.Month && x.Date.Day == D.Day).Select(x => x.Avtomobil).Distinct().ToList();
                foreach (Avtomobil Avto in A)

                { List<Poligon> TekPol = dbPoligons.Where(x => x.AvtomobilId == Avto.Id).ToList();
                    Poligon P = new Poligon();
                    P.MassIn = TekPol.Where(x => x.AvtomobilId == Avto.Id).Sum(x => x.MassIn);
                    P.MassOut = TekPol.Sum(x => x.MassOut);
                    P.MassMusor = TekPol.Sum(x => x.MassMusor);
                    P.AvtomobilId = Avto.Id;
                    P.Avtomobil = Avto;
                    P.Date = DateTime.Now;
                    P.Description = TekPol.Count().ToString();
                    P.TypeId = Avto.TypeId;
                    P.MarkaId = Avto.MarkaId;
                    P.Number = Avto.Number;
                    Result.Add(P);
                }
            }
            catch (Exception e)
            {

            }

            return View(Result);
        }

        public ActionResult DeleteZaezd(int id)
        {
            Poligon P = new Poligon();
            Poligon PTF = new Poligon(); 
            try
            {
                P = db.Poligons.Where(x => x.Id == id).First();
                PTF = P;
                db.Poligons.Remove(P);
                db.SaveChanges();
                ObnovitMassuVMarshrutah(PTF);

            }
            catch
            {

            }
            ViewBag.TypeAvtos = new SelectList(db.TypeAvtos, "Id", "Type");
            ViewBag.MarkaAvtomobils = new SelectList(db.MarkaAvtomobils.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.AvtoType = db.TypeAvtos.Where(x => x.Id == 16).Select(x => x.Type).First();
            ViewBag.AvtoImage = db.TypeAvtos.Where(x => x.Id == 16).Select(x => x.Ico).First();
            return RedirectToAction("Index");

        }

        public ActionResult DeleteCamera(int id)
        {
            VehicleRegistrationLog P = new VehicleRegistrationLog();
    
            try
            {
                P = adb.VehicleRegistrationLog.Where(x => x.Id == id).First();
                P.IsDeleted = true;
                adb.Entry(P).State = EntityState.Modified;
                adb.SaveChanges();

            }
            catch
            {

            }
          //  ViewBag.TypeAvtos = new SelectList(db.TypeAvtos, "Id", "Type");
          //  ViewBag.MarkaAvtomobils = new SelectList(db.MarkaAvtomobils.OrderBy(x => x.Name), "Id", "Name");
           // ViewBag.AvtoType = db.TypeAvtos.Where(x => x.Id == 16).Select(x => x.Type).First();
          //  ViewBag.AvtoImage = db.TypeAvtos.Where(x => x.Id == 16).Select(x => x.Ico).First();
            return RedirectToAction("Index");

        }

        public JsonResult MassOutEdit(decimal massOut, int id)
        {
            Poligon P = new Poligon();
            try
            {
                P = db.Poligons.Where(x => x.Id == id).First();
                P.MassOut = massOut;

                P.MassMusor = P.MassIn - P.MassOut;
                db.Entry(P).State = EntityState.Modified;
                db.SaveChanges();
                ObnovitMassuVMarshrutah(P);
                return Json("Ok");
            }
            catch
            {
                return Json("Error");
            }

        }

        // GET: Poligons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Poligon poligon = db.Poligons.Find(id);
            if (poligon == null)
            {
                return HttpNotFound();
            }
            return View(poligon);
        }

        // GET: Poligons/Create
        public ActionResult Create()
        {
            return View();
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
                Num = db.Avtomobils.Where(x => x.Number.Contains(term)).Select(x => x.Number.Replace(" ", "")).ToList();
            }
            catch
            {
                Num.Add("Добавить новый автомобиль");
            }
            return Json(Num, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchCompleteNumber(string term)
        {
            if (term != null)
            {
                term = term.ToLower();
                foreach (char ch in term)
                {
                    if ((int)ch >= 97 && (int)ch <= 122)
                    {

                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                }
                term = term.ToUpper().Replace(" ", "");
            }
            string Num = "";
            try
            {
                Num = db.Avtomobils.Where(x => x.Number.StartsWith(term)).Select(x => x.Number.Replace(" ", "")).First().Replace(" ", "");

            }
            catch
            {

            }
            return Json(Num, JsonRequestBehavior.AllowGet);
        }


        //Ищем номер и обновляем страницу
        public ActionResult NumberAndRefresh(string term)
        {
            if (term != null)
            {
                term = term.ToUpper().Replace(" ", "");

                List<string> Num = new List<string>();
                Avtomobil A = new Avtomobil();
                try
                {
                    //пробуем найти автомобиль по номеру
                    //Num = db.Avtomobils.Where(x => x.Number.StartsWith(term)).Select(x => x.Number).ToList();

                    A = db.Avtomobils.Where(x => x.Number.Replace(" ", "").Equals(term)).Include(x => x.Type).First();
                    ViewBag.Number = A.Number.ToUpper().Replace(" ", "");
                    ViewBag.TypeAvtos = new SelectList(db.TypeAvtos, "Id", "Type");
                    ViewBag.MarkaAvtomobils = new SelectList(db.MarkaAvtomobils.OrderBy(x => x.Name), "Id", "Name");
                    ViewBag.AvtoType = A.Type.Type;
                    ViewBag.AvtoImage = A.Type.Ico;
                    return RedirectToAction("Index", new { Number = A.Number.ToUpper().Replace(" ", "") });
                    return View("Index", db.Poligons.Include(z => z.Avtomobil).Include(x => x.Marka).Include(x => x.Type).ToList());
                }
                catch
                {
                    ViewBag.Number = term;
                    ViewBag.TypeAvtos = new SelectList(db.TypeAvtos, "Id", "Type");
                    ViewBag.MarkaAvtomobils = new SelectList(db.MarkaAvtomobils.OrderBy(x => x.Name), "Id", "Name");
                    ViewBag.AvtoType = db.TypeAvtos.Where(x => x.Id == 16).Select(x => x.Type).First();
                    ViewBag.AvtoImage = db.TypeAvtos.Where(x => x.Id == 16).Select(x => x.Ico).First();
                    term = term.ToUpper().Replace(" ", "");


                }
            }
            return RedirectToAction("Index", new { Number = term });
            return View("Index", db.Poligons.Include(z => z.Avtomobil).Include(x => x.Marka).Include(x => x.Type).ToList());
        }


        // POST: Poligons/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Number,MassIn,MassOut,Description,Marka,TypeId,KontrAgentId,VibralRab")] Poligon poligon)
        {
            poligon.Date = DateTime.Now;
            poligon.AvtomobilId = 0;
            poligon.Number = poligon.Number.Replace(" ", "").ToUpper();
            poligon.User = User.Identity.Name;
            
             HttpCookie cookieReq = Request.Cookies["Poligon"];
             if (cookieReq != null)
             {
                poligon.Date = Convert.ToDateTime(cookieReq["Date"]);
        
             }
             if (poligon.Date.Day == DateTime.Now.Day && poligon.Date.Month == DateTime.Now.Month && poligon.Date.Year == DateTime.Now.Year) { poligon.Date = DateTime.Now; }

            Avtomobil A = new Avtomobil();
            try
            {
                A = db.Avtomobils.Where(x => x.Number.Replace(" ", "").Equals(poligon.Number)).Include(x => x.Marka).Include(x => x.Type).Include(x=>x.KontrAgent).First();
                poligon.AvtomobilId = A.Id;
                poligon.MarkaId = A.Marka.Id;
                poligon.TypeId = A.Type.Id;
                if (poligon.VibralRab == false)
                {
                    if (A.KontrAgent.Id != poligon.KontrAgentId) { poligon.KontrAgentId = A.KontrAgent.Id; }
            }
                poligon.KontrAgentName = db.KontrAgents.Where(x => x.Id == poligon.KontrAgentId).Select(x=>x.Name).First();
            }
            catch (Exception e)
            {
                A.Number = poligon.Number;
                A.Glonass = false;
                A.Garage = 0;
                A.GKHNNC = false;
                A.KoefficientSgatiya = 1;
                A.ObiemBunkera = 0;
                A.TypeId = poligon.TypeId;

                return RedirectToAction("Index", new { Number = poligon.Number });

                //если не нашли по номеру в бд то отправляем создать новый
            }

            if (poligon.Description == null) {
                poligon.Description = "В пределах нормы.";
                if (poligon.MassIn == 0)
                {
                    poligon.MassIn = Math.Round(A.ObiemBunkera * A.KoefficientSgatiya * 165.1M, 2);
                    poligon.MassOut = 0;
                }

            }
            else
            {
                poligon.MassIn = 0;
                poligon.MassOut = 0;

            }



            try
            {
                //сохраняем изменения

                poligon.MassMusor = poligon.MassIn - poligon.MassOut;
                db.Poligons.Add(poligon);
                db.SaveChanges();
                //обновляем суммарную массу на авто в активных маршрутах
                ObnovitMassuVMarshrutah(poligon);

            }
            catch (Exception e)
            {

            }
          

            ViewBag.Number = poligon.Number;
            ViewBag.TypeAvtos = new SelectList(db.TypeAvtos, "Id", "Type");
            ViewBag.MarkaAvtomobils = new SelectList(db.MarkaAvtomobils.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.Numbers = A;
            ViewBag.AvtoType = db.TypeAvtos.Where(x => x.Id == 16).Select(x => x.Type).First();
            ViewBag.AvtoImage = db.TypeAvtos.Where(x => x.Id == 16).Select(x => x.Ico).First();
            return RedirectToAction("Index", new { number = poligon.Number });


        }

         public void ObnovitMassuVMarshrutah(Poligon poligon)
        {
            //обновляем суммарную массу на авто в активных маршрутах
            List<MarshrutsALL> MA = new List<MarshrutsALL>();
            try
            {
                //берем только активные маршруты на сегодня
               MA = db.MarshrutsAlls.Where(x => x.Date.Year == poligon.Date.Year && x.Date.Month == poligon.Date.Month && x.Date.Day == poligon.Date.Day && x.Type.Equals("A")).ToList();
                foreach (MarshrutsALL M in MA)
                {
                    string[] S = M.Avtomobils.Split(';');
                    decimal MassMusor  = 0;
                    foreach (string s in S)
                    {
                        try
                        {
                            int Id = Convert.ToInt32(s);
                            MassMusor += db.Poligons.Where(x => x.Date.Year == poligon.Date.Year && x.Date.Month == poligon.Date.Month && x.Date.Day == poligon.Date.Day && x.AvtomobilId == Id).Sum(x => x.MassMusor);

                        }
                        catch
                        {

                        }
                    }
                    //сохраняем суммарную массу всех автомобилей
                    M.MassaFact = Convert.ToInt32(MassMusor);
                    db.Entry(M).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            catch
            {

            }
        }


        public void PoligonZamena(Avtomobil Avto, Avtomobil NewAvto)
            {
            List<Poligon> Polig = db.Poligons.Where(x => x.AvtomobilId == Avto.Id).ToList();
            foreach (Poligon P in Polig)
            {
                P.Number = NewAvto.Number;
                P.TypeId = NewAvto.TypeId;
                P.MarkaId = NewAvto.MarkaId;
                P.AvtomobilId = NewAvto.Id;
                try
                {
                    db.Entry(P).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
        }
        [HttpPost]
        public ActionResult EditAvto([Bind(Include = "Id,Number,TypeId,MarkaId,ObiemBunkera,KoefficientSgatiya,KontrAgentId,NePuskat")] Avtomobil A)
        {
            A.Date = DateTime.Now.Year;

            A.Number = A.Number.ToUpper().Replace(" ", "");
         
            Avtomobil Avto = new Avtomobil();
            Avtomobil NewAvto = new Avtomobil();
            //грузим авто по айдишнику
            try
            {
                Avto = db.Avtomobils.Where(x => x.Id == A.Id).First();
            }
            catch{ }
            if (A.Number != ""&& A.Number.Length>=6)//если номер адекватен
            {
                try
                {
                    NewAvto = db.Avtomobils.Where(x => x.Number.Replace(" ", "").Equals(A.Number)).First();
                    if (NewAvto == Avto)//если номер не поменяли то улучшаем этот автомобиль
                    {
                        try
                        {

                            Avtomobil StarAvto = Avto;
                            Avto.TypeId = A.TypeId;
                            Avto.Type = db.TypeAvtos.Where(x => x.Id == Avto.TypeId).First();
                            Avto.ObiemBunkera = A.ObiemBunkera;
                            Avto.MarkaId = A.MarkaId;
                            Avto.Marka = db.MarkaAvtomobils.Where(x => x.Id == Avto.MarkaId).First();
                            Avto.KoefficientSgatiya = A.KoefficientSgatiya;
                            Avto.KontrAgentId = A.KontrAgentId;
                            Avto.NePuskat = A.NePuskat;
                            db.Entry(Avto).State = EntityState.Modified;
                            db.SaveChanges();
                            PoligonZamena(StarAvto, Avto);//чтоб картинка в полигонах заменилась
                        }
                        catch (Exception e)
                        {

                        }
                       
                    }
                    else
                    {
                        //если номер поменяли проверим есть ли номер в базе
                        //если номер не поменяли то улучшаем этот автомобиль

                        try
                        {
                        if (User.Identity.Name.Contains("Администратор"))
                        {
                            //Если номер поменяли то тупо меняем все полигональные записи и машину удаляем (соединяем с другой) без учета полей на изменение
                            PoligonZamena(Avto,NewAvto);
                            db.Avtomobils.Remove(Avto);
                            db.SaveChanges();
                            Avto = NewAvto;
                        }
                            else
                            {
                                return Json("У вас нет прав на изменение базы данных автомобилей. Обратитесь к администратору для замены автомобиля или его номера.");
                            }
                        }
                        catch (Exception e) { }
                        
                    }

                }
                catch
                {
                    //если такого номера нет, значит поправили номер и просто его заменяем
                    Avtomobil StarAvto = Avto;
                    Avto.Number = A.Number;
                    Avto.TypeId = A.TypeId;
                    Avto.ObiemBunkera = A.ObiemBunkera;
                    Avto.MarkaId = A.MarkaId;
                    Avto.KoefficientSgatiya = A.KoefficientSgatiya;
                    Avto.KontrAgentId = A.KontrAgentId;
                    db.Entry(Avto).State = EntityState.Modified;
                    db.SaveChanges();
                    PoligonZamena(StarAvto, Avto);
                }
            }
            return RedirectToAction("Index", new { number = Avto.Number });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAvto ([Bind(Include = "Number,TypeId,MarkaId,ObiemBunkera,KoefficientSgatiya,KontrAgentId")] Avtomobil A)
        {
            A.Date = DateTime.Now.Year;
                A.Glonass = false;
                A.Garage = 0;
                A.GKHNNC = false;
            A.Number = A.Number.ToUpper().Replace(" ", "");
            try
            {
                A = db.Avtomobils.Where(x => x.Number.Replace(" ", "").Equals(A.Number)).Include(x=>x.Type).First();
                ViewBag.Number = A.Number;
                ViewBag.TypeAvtos = new SelectList(db.TypeAvtos, "Id", "Type");
                ViewBag.MarkaAvtomobils = new SelectList(db.MarkaAvtomobils.OrderBy(x => x.Name), "Id", "Name");
                ViewBag.Numbers = A;
                ViewBag.AvtoType = A.Type.Type;
                ViewBag.AvtoImage = A.Type.Ico;
                return RedirectToAction("Index", new { number = A.Number });
            }
            catch (Exception e)
            {
                //если не нашли по номеру в бд то сохраняем как новый

                try
                {
                    db.Avtomobils.Add(A);
                    db.SaveChanges();
                    ViewBag.Number = A.Number;
                    ViewBag.TypeAvtos = new SelectList(db.TypeAvtos, "Id", "Type");
                    ViewBag.MarkaAvtomobils = new SelectList(db.MarkaAvtomobils.OrderBy(x => x.Name), "Id", "Name");
                    ViewBag.Numbers = A;
                    
                    ViewBag.AvtoType = db.TypeAvtos.Where(x=>x.Id == A.TypeId).Select(x=>x.Type).First();
                    ViewBag.AvtoImage = db.MarkaAvtomobils.Where(x=>x.Id == A.MarkaId).Select(x=>x.Name).First();
                    return RedirectToAction("Index",new { number = A.Number });
                }
                catch (Exception f)
                {
                    return Json(f.Message);
                }


            }
              
                
           
                

            

            return View("Index", new { number = A.Number });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CameraToPoligon([Bind(Include = "Number,TypeId,MarkaId,ObiemBunkera,Id,Description,KontrAgentId")] CameraToPoligon C)
        {
            string numb = C.Number.ToUpper().Replace(" ", "");
            Dictionary<char, char> DicVer = new Dictionary<char, char>()
                {
                    {'A','А'},
                    {'E','Е'},
                    {'Y','У'},
                    {'P','Р'},
                    {'M','М'},
                    {'K','К'},
                    {'Z','З'},
                    {'X','Х'},
                    {'C','С'},
                    {'O','О'},
                    {'L','Л'},
                    {'B','В'},
                    {'H','Н'},
                    {'T','Т'}
                };
            //Заменяем английские символы в номере на русские
            foreach (char ch in numb)
            {
                numb =numb.Replace(ch, DicVer.ContainsKey(ch) ? DicVer[ch] : ch);
            }


            DateTime Date = DateTime.Now;
            //Разбираемся с камерой
            try
            {
                VehicleRegistrationLog L = adb.VehicleRegistrationLog.Where(x => x.Id == C.Id).First();
                Date = L.TimeStamp.AddHours(7);
                L.Plate = numb;
                adb.Entry(L).State = EntityState.Modified;
                adb.SaveChanges();
                    }
            catch(Exception e)
            {

            }


            //Разбираемся с автомобилем
            Avtomobil A = null;
            try
            {
                A = db.Avtomobils.Where(x => x.Number.Equals(numb)).Include(x=>x.KontrAgent).First();
            }
            catch
            {

            }
            if (A == null)
            {
                try
                {
                    A = new Avtomobil();
                    A.Date = Date.Year;
                    A.Glonass = false;
                    A.Garage = 0;
                    A.GKHNNC = false;
                    A.Number = numb;
                    A.TypeId = C.TypeId;
                    A.MarkaId = C.MarkaId;
                    A.KontrAgentId = 104;
                    A.KontrAgent = db.KontrAgents.Where(x => x.Id == 104).First();
                    A.KoefficientSgatiya = 0;
                    A.ObiemBunkera = 0;
                    db.Avtomobils.Add(A);
                    db.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }

            //Разбираемся с полигоном
            if (A != null)
            {
                try
                {
                    Poligon P = new Poligon();
                    P.AvtomobilId = A.Id;
                   
                    if (C.KontrAgentId==104)
                    {
                        P.KontrAgentId = A.KontrAgentId.Value;
                     
                    }
                    else
                    {
                        P.KontrAgentId = C.KontrAgentId;
                    }
                  
                   
                    P.KontrAgentName = db.KontrAgents.Where(x=>x.Id == P.KontrAgentId).Select(x=>x.Name).First();
                    P.MarkaId = A.MarkaId;
                    
                    if (P.KontrAgentId != 105)
                    {
                        P.MassMusor = Math.Round(A.ObiemBunkera * A.KoefficientSgatiya * 165.1M, 2);
                        P.MassIn = Math.Round(A.ObiemBunkera * A.KoefficientSgatiya * 165.1M, 2); 
                    }
                    else
                    {
                        P.MassMusor = 0;
                        P.MassIn = 0;

                    }
                    P.MassOut = 0;
                    P.Number = numb;
                    P.TypeId = A.TypeId;
                    P.Date = Date;
                    P.User = "Создано пользователем "+User.Identity.Name+" на основании камеры "+ C.Id;
                    P.VibralRab = false;
                    P.CameraFix = true;
                    P.Description = C.Description;
                    db.Poligons.Add(P);
                    db.SaveChanges();
               
                }
                catch (Exception e)
                {

                }
            }

            return RedirectToAction("Index");
         
        }


        // GET: Poligons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Poligon poligon = db.Poligons.Find(id);
            if (poligon == null)
            {
                return HttpNotFound();
            }
            return View(poligon);
        }

        // POST: Poligons/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Number,Date,MassIn,MassOut,Description,AvtomobilId")] Poligon poligon)
        {
            if (ModelState.IsValid)
            {
                db.Entry(poligon).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(poligon);
        }

        // GET: Poligons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Poligon poligon = db.Poligons.Find(id);
            if (poligon == null)
            {
                return HttpNotFound();
            }
            return View(poligon);
        }

        // POST: Poligons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Poligon poligon = db.Poligons.Find(id);
            db.Poligons.Remove(poligon);
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
