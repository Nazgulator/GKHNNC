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

        // GET: Poligons
        public ActionResult Index(string Number, int Poisk = 0, int PoiskKontr = 0, int result = 0, int Vesi = 0, int Avtosort = 0,string TekDate="")
        {
            DateTime Date = DateTime.Now;
            if (TekDate!="")
            {
                Date = Convert.ToDateTime(TekDate);
                //добавляем куки с осмотром
                HttpCookie cookie = new HttpCookie("Poligon");
                cookie["Date"] = Date.ToString();
                // Добавить куки в ответ
                Response.Cookies.Add(cookie);
            }
            else
            {
                 HttpCookie cookieReq = Request.Cookies["Poligon"];
                 if (cookieReq != null)
                 {
                     Date = Convert.ToDateTime(cookieReq["Date"]);
                 }
               
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
            try
            {
                DateTime D = Date;
                if (Poisk != 0)
                {
                    Poligons = db.Poligons.Where(x => x.Date.Year == D.Year && x.Date.Month == D.Month && x.Date.Day == D.Day && x.AvtomobilId == Poisk).Include(z => z.Avtomobil).Include(z => z.Avtomobil.Marka).Include(z => z.Avtomobil.Type).Include(x => x.Marka).Include(x => x.Type).Include(x => x.Avtomobil.KontrAgent).ToList();
                    ViewBag.Poisk = db.Avtomobils.Where(x => x.Id == Poisk).Select(x => x.Number).First();
                    ViewBag.PoiskId = Poisk;

                }
                else
                {

                    Poligons = db.Poligons.Where(x => x.Date.Year == D.Year && x.Date.Month == D.Month && x.Date.Day == D.Day).Include(z => z.Avtomobil).Include(z => z.Avtomobil.Marka).Include(z => z.Avtomobil.Type).Include(x => x.Marka).Include(x => x.Type).Include(x => x.Avtomobil.KontrAgent).ToList();
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
            catch (Exception e)
            {

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
                Poligons = Poligons.OrderBy(x=>x.AvtomobilId).ThenBy(x => x.Date).ThenByDescending(x=>x.Id).ToList();
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
        public ActionResult Create([Bind(Include = "Number,MassIn,MassOut,Description,Marka,TypeId,KontrAgentId")] Poligon poligon)
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
                A = db.Avtomobils.Where(x => x.Number.Replace(" ", "").Equals(poligon.Number)).Include(x => x.Marka).Include(x => x.Type).First();
                poligon.AvtomobilId = A.Id;
                poligon.MarkaId = A.Marka.Id;
                poligon.TypeId = A.Type.Id;
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
