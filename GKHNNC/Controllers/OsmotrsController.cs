﻿using System;
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
    public class OsmotrsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Osmotrs
        public ActionResult Index()
        {
            var osmotrs = db.Osmotrs.Include(o => o.Adres).Include(o => o.DOMCW).Include(o => o.DOMElectro).Include(o => o.DOMFasad).Include(o => o.DOMFundament).Include(o => o.DOMHW).Include(o => o.DOMOtoplenie).Include(o => o.DOMRoof).Include(o => o.DOMRoom).Include(o => o.DOMVodootvod);
            return View(osmotrs.ToList());
        }
      
        public ActionResult SaveElement(int Id=0, string Photo1="", string Photo2="")
        {
            string Data = "";
            if (Photo1 != "" && Photo2 != "")
            {
                try
                {
                    ActiveElement A = db.ActiveElements.Where(x => x.Id == Id).First();
                    A.Photo1 = Path.GetFileName(Photo1);
                    A.Photo2 = Path.GetFileName(Photo2);
                    
                    db.Entry(A).State = EntityState.Modified;
                    db.SaveChanges();
                    Data = A.Photo1 + ";" + A.Photo2+";"+A.Sostoyanie;

                }
                catch(Exception e) { Data = "Ошибка; сохранения изменений!"; }
            }
            return Json(Data);

        }
        // GET: Osmotrs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Osmotr osmotr = db.Osmotrs.Find(id);
            if (osmotr == null)
            {
                return HttpNotFound();
            }
            return View(osmotr);
        }
        public ActionResult SelectActiveDefect(DateTime Date,int ElementTypeId = 1 , int AdresId = 13)
        {
            List<SelectListItem> Elements = new SelectList(db.Elements.Where(x=>x.ElementTypeId==ElementTypeId), "Id", "Name").ToList();

            List<SelectListItem> Adress = new SelectList(db.Adres.Where(x=>x.Id==AdresId), "Id", "Adress").ToList();

            ViewBag.AdresId = Adress;
            ViewBag.ElementId = Elements;
            return View();
        }
        [HttpPost]
        public JsonResult Zvezda(string data = "")
        {
            string[] S = data.Replace(@"\", "").Replace(":", "").Replace(" ", "").Replace(",", "").Split('"');
            int ElementId = Convert.ToInt32(S[3].Replace("SostoyanieElement", ""));
            int Sostoyanie = Convert.ToInt32(S[6]);
            ActiveDefect A = new ActiveDefect();
            PoluchitCookie(ref A);
            ActiveElement AE = null;
            try
            {
                AE = db.ActiveElements.Where(x => x.OsmotrId == A.OsmotrId && x.ElementId == ElementId).First();//получаем активный элемент по ид и осмотру
                AE.Sostoyanie = Sostoyanie;
                //сохраняем его
                db.Entry(AE).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch { }
            return Json(data);
        }
        [HttpPost]
        public JsonResult ZvezdaRed(string data = "")
        {
            string[] S = data.Replace(@"\", "").Replace(":", "").Replace(" ", "").Replace(",", "").Split('"');
            int DefectId = Convert.ToInt32(S[5].Replace("S", ""));
            int Sostoyanie = Convert.ToInt32(S[2]);
            ActiveDefect A = new ActiveDefect();
            PoluchitCookie(ref A);
            HttpCookie cookie = new HttpCookie(DefectId.ToString());
            // Установить значения в нем
            cookie["Sostoyanie"] = Sostoyanie.ToString();
            // Добавить куки в ответ
            Response.Cookies.Add(cookie);

            ActiveDefect AE = null;
           // try
            //{
            //    AE = db.ActiveDefects.Where(x => x.OsmotrId == A.OsmotrId && x.Id== DefectId).First();//получаем активный элемент по ид и осмотру
            //    AE.Sostoyanie = Sostoyanie;
                //сохраняем его
            //    db.Entry(AE).State = EntityState.Modified;
            //    db.SaveChanges();
           // }
           // catch { }
            return Json(data);
        }


        public ActionResult SelectActiveElement(int ElementId=1)
        {
            List<SelectListItem> Elements = new List<SelectListItem>();
            try
            {
                Elements = new SelectList(db.Elements.Where(x=>x.ElementTypeId==ElementId), "Id", "Name").ToList();
            }
            catch { }

            
            //SelectListItem S = new SelectListItem();
           // S.Value = ElementId.ToString();
            //S.Text = db.Elements.Where(x => x.Id == ElementId).First().Name;
           // Elements.Remove(S);
           // Elements.Insert(0, S);
            ViewBag.Elements = Elements;
           
            return View();
        }
        [HttpPost]
        public ActionResult AddActiveDefect ([Bind(Include = "AdresId,Sostoyanie,Opisanie,ElementId,DefectId,OsmotrId,Date")] ActiveDefect A)
        {
            ///DateTime date, int AdresId = 13,int ElementId = 1,int DefectId = 1,string Opisanie = ""
            string Data = "";
           
            try
            {
                db.ActiveDefects.Add(A);
                db.SaveChanges();
                Data = "Дефект успешно добавлен";
            }
            catch (Exception e) { Data = "Ошибка сохранения в БД"; }

            return Json(A);//RedirectToAction("ViewActiveDefect",A);
        }
        
        public void PoluchitCookie(ref ActiveDefect A)
        {
            
            HttpCookie cookieReq = Request.Cookies["Osmotr"];
            // Проверить, удалось ли обнаружить cookie-набор с таким именем.
            // Это хорошая мера предосторожности, потому что         
            // пользователь мог отключить поддержку cookie-наборов,         
            // в случае чего cookie-набор не существует        
            DateTime DateCook;
            if (cookieReq != null)
            {
                DateCook = Convert.ToDateTime(cookieReq["Date"]);
                A.Date = DateCook;
                A.OsmotrId = Convert.ToInt32(cookieReq["OsmotrId"]);
                A.AdresId = Convert.ToInt32(cookieReq["AdresId"]);
            }
        }
        [HttpPost]
        public ActionResult RemoveAD(int ADId = 0)
        {
            string Data = "";
                try
                {
                ActiveDefect A = db.ActiveDefects.Where(x => x.Id == ADId).First();
                    db.ActiveDefects.Remove(A);
                    db.SaveChanges();
                  
                }
                catch (Exception e) { Data = "Ошибка;удаления;из;БД"; }
            
            return Json(Data);//RedirectToAction("ViewActiveDefect",A);
        }
        [HttpPost]
        public ActionResult AddAD(int ElementId = 0, int Sostoyanie = 0, string Opisanie = "", int DefectId = 0, string Photo1 ="", string Photo2="", int Number=0)
        {
            ///DateTime date, int AdresId = 13,int ElementId = 1,int DefectId = 1,string Opisanie = ""
        string Data = "";
            ActiveDefect A = new ActiveDefect();
            PoluchitCookie(ref A);//получаем часть данных из куки
            if (ElementId > 0 && DefectId > 0)//если данные не нулевые
            {
                A.ElementId = ElementId;


                HttpCookie cookieReq = Request.Cookies[ElementId.ToString()];
                // Проверить, удалось ли обнаружить cookie-набор с таким именем.
                // Это хорошая мера предосторожности, потому что         
                // пользователь мог отключить поддержку cookie-наборов,         
                // в случае чего cookie-набор не существует        
                if (cookieReq != null)
                {
                    A.Sostoyanie = Convert.ToInt32(cookieReq["Sostoyanie"]);//состояние сохраняется событием в куки 
                }

                //A.Sostoyanie = Sostoyanie;
                A.Opisanie = Opisanie;
                A.DefectId = DefectId;
                A.Number = Number;
                A.Photo1 = Path.GetFileName(Photo1);
                A.Photo2 = Path.GetFileName(Photo2);
                try
                {
                    db.ActiveDefects.Add(A);
                    db.SaveChanges();
                    Data = db.Defects.Where(x=>x.Id==A.DefectId).Select(y=>y.Def).First() + ";" + A.Sostoyanie + ";" + A.Opisanie + ";" + A.DefectId+";"+A.Id+";"+A.Photo1+";"+A.Photo2+";"+A.OsmotrId;
                }
                catch (Exception e) { Data = "Ошибка;сохранения;в;БД"; }
            }
            return Json(Data);//RedirectToAction("ViewActiveDefect",A);
        }

        [HttpPost]
        public JsonResult Upload()
        {
            HttpCookie cookieReq = Request.Cookies["Osmotr"];
            int AdresId = 0;
            DateTime Date = new DateTime();
            int OsmotrId = 0;
           
            // Проверить, удалось ли обнаружить cookie-набор с таким именем.
            // Это хорошая мера предосторожности, потому что         
            // пользователь мог отключить поддержку cookie-наборов,         
            // в случае чего cookie-набор не существует        
            DateTime DateCook;
            if (cookieReq != null)
            {
                DateCook = Convert.ToDateTime(cookieReq["Date"]);
                Date = DateCook;
                OsmotrId = Convert.ToInt32(cookieReq["OsmotrId"]);
                AdresId = Convert.ToInt32(cookieReq["AdresId"]);
            }
            //проверяем директорию и создаем если её нет
            if (Directory.Exists(Server.MapPath("~/Files")) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files"));

            }
            if (Directory.Exists(Server.MapPath("~/Files/"+OsmotrId.ToString()))==false)  
            {
                Directory.CreateDirectory(Server.MapPath("~/Files/" + OsmotrId.ToString()));

            }


          
           
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    // получаем имя файла
                    string fileName = System.IO.Path.GetFileName(upload.FileName);
                   // string NumberFiles = new DirectoryInfo(Server.MapPath("~/Files/" + OsmotrId )).GetFiles().Length.ToString();
                   // string rash = fileName.Substring(fileName.LastIndexOf(".") + 1);

                // сохраняем файл в папку Files в проекте
                upload.SaveAs(Server.MapPath("~/Files/"+OsmotrId.ToString()+"/" + fileName));
                }
            }
            return Json("файл успешно загружен!");
        }


        public ActionResult SpisokActiveDefect(string D="", int ElementId = 1,  int OsmotrId = 1)
        {
            DateTime Date = DateTime.Now;
            ActiveElement AE = new ActiveElement();
            HttpCookie cookieReq = Request.Cookies["Osmotr"];
            int AdresId = 0;

            // Проверить, удалось ли обнаружить cookie-набор с таким именем.
            // Это хорошая мера предосторожности, потому что         
            // пользователь мог отключить поддержку cookie-наборов,         
            // в случае чего cookie-набор не существует        
            DateTime DateCook;
            if (cookieReq != null&&D=="")
            {
                DateCook = Convert.ToDateTime(cookieReq["Date"]);
                Date = DateCook;
                OsmotrId = Convert.ToInt32(cookieReq["OsmotrId"]);
                AdresId = Convert.ToInt32(cookieReq["AdresId"]);
            }
            else
            {
                if (D != "")
                {
                    try
                    {
                        Date = Convert.ToDateTime(D);
                    }
                    catch { }
                }
            }

            //на всякий случай вдруг реально дата нулевая
            if (Date != null)
            {


                try
                {
                    AE = db.ActiveElements.Where(x => x.ElementId == ElementId && x.AdresId == AdresId).Include(x => x.Element).Include(x => x.Defects).OrderByDescending(x => x.Date).First();
                }
                catch
                {
                    AE.ElementId = ElementId;
                    AE.Element = db.Elements.Where(x => x.Id == ElementId).First();
                    AE.OsmotrId = OsmotrId;
                    AE.AdresId = AdresId;
                    AE.Date = Date;
                    AE.Sostoyanie = 10;

                   
                    try
                    {

                        AE.ActiveDefects = db.ActiveDefects.Where(x => x.ElementId == ElementId && x.AdresId == AdresId && x.Date.Year == Date.Year && x.Date.Month == Date.Month && x.Date.Day >= Date.Day).OrderByDescending(x => x.Date).Include(x => x.Defect).ToList();
                    }
                    catch (Exception e)
                    {
                        AE.ActiveDefects = new List<ActiveDefect>();
                    }
                }
                List<string> ADS = new List<string>();
                foreach (ActiveDefect A in AE.ActiveDefects)
                {
                    //ИД;Состояние;Описание;ЭлементИД
                    ADS.Add(A.Id+";"+A.Sostoyanie+";"+A.Opisanie+";"+A.ElementId);
                }

            }
            return View(AE);
        }
        //служит для отображения списка активных дефектов и возможных дефектов модели
        public ActionResult ViewActiveDefect(int ElementId = 1,ActiveDefect A=null)
        {
            DateTime Date = DateTime.Now;
            int OsmotrId = 13;
            int AdresId = 1;
            HttpCookie cookieReq = Request.Cookies["Osmotr"];
            if (cookieReq != null)
            {
                Date = Convert.ToDateTime(cookieReq["Date"]);
                OsmotrId = Convert.ToInt32(cookieReq["OsmotrId"]);
                AdresId = Convert.ToInt32(cookieReq["AdresId"]); 
            }
            else
            {

            }
                string Data = "";
            ActiveElement AE = new ActiveElement();
            // if (A != null)
            //{
            //    Date = A.Date;
            //     ElementId = Convert.ToInt32(A.ElementId);
            //     AdresId = Convert.ToInt32(A.AdresId);
            //     OsmotrId = Convert.ToInt32(A.OsmotrId);

            // }
            if (ElementId != 1)
            {
                if (Date != null)
                {


                    try
                    {//для загрузки осмотра
                        AE = db.ActiveElements.Where(x => x.ElementId == ElementId && x.AdresId == AdresId).Include(x => x.Element).OrderByDescending(x => x.Date).First();
                    }
                    catch
                    {//если осмотр новый
                        AE.ElementId = ElementId;
                        AE.Element = db.Elements.Where(x => x.Id == ElementId).First();
                        AE.OsmotrId = OsmotrId;
                        AE.AdresId = AdresId;
                        AE.Date = Date;
                        AE.Sostoyanie = 10;
                        
                      
                        db.ActiveElements.Add(AE);
                        db.SaveChanges();
                    }

                    //для загруженных и новых
                    try
                    {
                        AE.Defects = db.Defects.Where(x => x.ElementId == ElementId).ToList();
                    }
                    catch (Exception e)
                    {
                        AE.Defects = new List<Defect>();
                    }
                    try
                    {

                        AE.ActiveDefects = db.ActiveDefects.Where(x => x.ElementId == ElementId && x.OsmotrId == AE.OsmotrId).OrderByDescending(x => x.Date).Include(x => x.Defect).ToList();
                    }
                    catch (Exception e)
                    {
                        AE.ActiveDefects = new List<ActiveDefect>();
                    }



                }
            }
            else
            {
                return null;
            }
            return PartialView ("ViewActiveDefect",AE);
        }

        // GET: Osmotrs/Create
        public ActionResult Create(DateTime date,int id = 0)
        {
            bool LoadOsmotr = false;
            string error = "";
            List<Element> Elements = db.Elements.ToList();
            ViewBag.FundamentMaterials = new SelectList(db.FundamentMaterials, "Id", "Material");
            ViewBag.FundamentTypes = new SelectList(db.FundamentTypes, "Id", "Type");
            List<string> Parts =db.DOMParts.OrderBy(y => y.Id).Select(x => x.Name).ToList();
            ViewBag.DOMParts = Parts;
            if (date == null)
            {
                date = DateTime.Now;
            }
           
            Osmotr Result = new Osmotr();
            //ищем по базе осмотры, если есть за текущий месяц на данном доме то продолжаем заполнять его.
            try
            {
                Result = db.Osmotrs.Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month && x.AdresId == id).OrderByDescending(x => x.Date).Include(x=>x.Adres).Include(x=>x.DOMCW).Include(x=>x.DOMElectro).Include(x=>x.DOMFasad).Include(x=>x.DOMFundament).Include(x=>x.DOMHW).Include(x=>x.DOMOtoplenie).Include(x=>x.DOMRoof).Include(x=>x.DOMRoom).Include(x=>x.DOMVodootvod).First();
                LoadOsmotr = true;//Удалось загрузить осмотр используем уже имеющиеся данные
                Result.Elements = db.ActiveElements.Where(x => x.OsmotrId == Result.Id).ToList();//берем все активные элементы и кидаем в список
                foreach (ActiveElement A in Result.Elements)
                {
                    A.ActiveDefects = db.ActiveDefects.Where(x => x.OsmotrId == Result.Id&&x.ElementId==A.ElementId).ToList();
                    A.Defects = db.Defects.Where(x => x.ElementId == A.ElementId).ToList();
                }
                //сохраняем осмотр

                    //добавляем куки с осмотром
                    HttpCookie cookie = new HttpCookie("Osmotr");
                    cookie["Date"] = Result.Date.ToString();
                    cookie["OsmotrId"] = Result.Id.ToString();
                    cookie["AdresId"] = Result.AdresId.ToString();
                    // Добавить куки в ответ
                    Response.Cookies.Add(cookie);



            }
            catch
            {

                if (id != 0)
                {
                    Result.AdresId = id;
                    Result.Adres = db.Adres.Where(x => x.Id == id).First();
                    Result.Date = date;
                    try
                    {//пробуем грузануть данные по дому
                        Result.DOMCW = db.DOMCWs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                        Result.DOMHW = db.DOMHWs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                        Result.DOMElectro = db.DOMElectroes.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                        Result.DOMFasad = db.DOMFasads.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                        Result.DOMFundament = db.DOMFundaments.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).Include(x => x.Material).Include(x => x.Type).First();

                        Result.DOMOtoplenie = db.DOMOtoplenies.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                        Result.DOMRoof = db.DOMRoofs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                        Result.DOMRoom = db.DOMRooms.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                        Result.DOMVodootvod = db.DOMVodootvods.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                    }
                    catch
                    {//если данных нет, значит проблема с загрузкой данных с ГИСЖКХ. Проверьте данные. 
                        error += "Нет данных дома из ГИСЖКХ! Проверьте данные или заполните с нуля. Созданы нулевые данные.";
                        Result.DOMCW = new DOMCW(); ;
                        Result.DOMHW = new DOMHW();
                        Result.DOMElectro = new DOMElectro();
                        Result.DOMFasad = new DOMFasad();
                        Result.DOMFundament = new DOMFundament();

                        Result.DOMOtoplenie =new DOMOtoplenie();
                        Result.DOMRoof = new DOMRoof();
                        Result.DOMRoom = new DOMRoom();
                        Result.DOMVodootvod = new DOMVodootvod();
                    }
                    Result.Sostoyanie = 10;
                    Result.Elements = new List<ActiveElement>();


                    //сохраняем осмотр
                    try
                    {
                        db.Osmotrs.Add(Result);
                        db.SaveChanges();

                        //добавляем куки с осмотром
                        HttpCookie cookie = new HttpCookie("Osmotr");
                        cookie["Date"] = date.ToString();
                        cookie["OsmotrId"] = Result.Id.ToString();
                        cookie["AdresId"] = id.ToString();
                        // Добавить куки в ответ
                        Response.Cookies.Add(cookie);

                    }
                    catch (Exception e) { ViewBag.Id = 0; }

                    try
                    {//поскольку дефекты фиксируются осмотрами то у всех должна быть одна дата даже на разные элементы
                        DateTime D = date;
                       // try
                       // {
                       //     db.ActiveDefects.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).Select(x => x.Date).First();
                       // }
                        //catch { }
                        foreach (Element E in Elements)
                        {
                            //ищем самый новый по дате и если такого нет то создаем пустой

                            ActiveElement AE = new ActiveElement();

                            try
                            {
                                AE = db.ActiveElements.Where(x => x.ElementId == E.Id && x.AdresId == id).OrderByDescending(x => x.Date).First();
                            }
                            catch
                            {
                                AE.ElementId = E.Id;
                                AE.Element = db.Elements.Where(x => x.Id == E.Id).First();
                                AE.OsmotrId = Result.Id;
                                AE.AdresId = id;
                                AE.Date = date;
                                AE.Sostoyanie = 10;

                                try
                                {
                                    AE.Defects = db.Defects.Where(x => x.ElementId == E.Id).ToList();
                                }
                                catch (Exception e)
                                {
                                    AE.Defects = new List<Defect>();
                                }
                                try
                                {

                                    AE.ActiveDefects = db.ActiveDefects.Where(x => x.ElementId == E.Id && x.AdresId == id && x.Date == D).OrderByDescending(x => x.Date).Include(x => x.Defect).ToList();
                                }
                                catch (Exception e)
                                {
                                    AE.ActiveDefects = new List<ActiveDefect>();
                                }
                            }
                            db.ActiveElements.Add(AE);
                            db.SaveChanges();
                            Result.Elements.Add(AE);

                        }
                    }
                    catch (Exception e) { }

                    try
                    {
                        Result.Defects = db.ActiveDefects.Where(x => x.AdresId == id).ToList();
                    }
                    catch
                    {

                    }
                }
                else
                {
                    error += " Не определен ИД дома!!! Не можем создать осмотр. ИД дома =" + id.ToString()+" Дата="+date.ToString() ;
                    return RedirectToAction("Error",error);
                }
               
            }

            ViewBag.Error = error;
            return View(Result);
        }
        public ActionResult Error (string error)
        {
            ViewBag.Error = error;
            return View(error);
        }
      // POST: Osmotrs/Create
      // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
      // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdresId,DOMFasadId,DOMFundamentId,DOMElectroId,DOMCWId,DOMHWId,DOMOtoplenieId,DOMRoofId,DOMRoomId,DOMVodootvodId,Sostoyanie,Date")] Osmotr osmotr)
        {
            if (ModelState.IsValid)
            {
                db.Osmotrs.Add(osmotr);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", osmotr.AdresId);
            ViewBag.DOMCWId = new SelectList(db.DOMCWs, "Id", "Id", osmotr.DOMCWId);
            ViewBag.DOMElectroId = new SelectList(db.DOMElectroes, "Id", "Id", osmotr.DOMElectroId);
            ViewBag.DOMFasadId = new SelectList(db.DOMFasads, "Id", "Id", osmotr.DOMFasadId);
            ViewBag.DOMFundamentId = new SelectList(db.DOMFundaments, "Id", "Id", osmotr.DOMFundamentId);
            ViewBag.DOMHWId = new SelectList(db.DOMCWs, "Id", "Id", osmotr.DOMHWId);
            ViewBag.DOMOtoplenieId = new SelectList(db.DOMOtoplenies, "Id", "Id", osmotr.DOMOtoplenieId);
            ViewBag.DOMRoofId = new SelectList(db.DOMRoofs, "Id", "Id", osmotr.DOMRoofId);
            ViewBag.DOMRoomId = new SelectList(db.DOMRoofs, "Id", "Id", osmotr.DOMRoomId);
            ViewBag.DOMVodootvodId = new SelectList(db.DOMRoofs, "Id", "Id", osmotr.DOMVodootvodId);
            return View(osmotr);
        }

        // GET: Osmotrs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Osmotr osmotr = db.Osmotrs.Find(id);
            if (osmotr == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", osmotr.AdresId);
            ViewBag.DOMCWId = new SelectList(db.DOMCWs, "Id", "Id", osmotr.DOMCWId);
            ViewBag.DOMElectroId = new SelectList(db.DOMElectroes, "Id", "Id", osmotr.DOMElectroId);
            ViewBag.DOMFasadId = new SelectList(db.DOMFasads, "Id", "Id", osmotr.DOMFasadId);
            ViewBag.DOMFundamentId = new SelectList(db.DOMFundaments, "Id", "Id", osmotr.DOMFundamentId);
            ViewBag.DOMHWId = new SelectList(db.DOMCWs, "Id", "Id", osmotr.DOMHWId);
            ViewBag.DOMOtoplenieId = new SelectList(db.DOMOtoplenies, "Id", "Id", osmotr.DOMOtoplenieId);
            ViewBag.DOMRoofId = new SelectList(db.DOMRoofs, "Id", "Id", osmotr.DOMRoofId);
            ViewBag.DOMRoomId = new SelectList(db.DOMRoofs, "Id", "Id", osmotr.DOMRoomId);
            ViewBag.DOMVodootvodId = new SelectList(db.DOMRoofs, "Id", "Id", osmotr.DOMVodootvodId);
            return View(osmotr);
        }

     
        // POST: Osmotrs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,DOMFasadId,DOMFundamentId,DOMElectroId,DOMCWId,DOMHWId,DOMOtoplenieId,DOMRoofId,DOMRoomId,DOMVodootvodId,Sostoyanie,Date")] Osmotr osmotr)
        {
            if (ModelState.IsValid)
            {
                db.Entry(osmotr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", osmotr.AdresId);
            ViewBag.DOMCWId = new SelectList(db.DOMCWs, "Id", "Id", osmotr.DOMCWId);
            ViewBag.DOMElectroId = new SelectList(db.DOMElectroes, "Id", "Id", osmotr.DOMElectroId);
            ViewBag.DOMFasadId = new SelectList(db.DOMFasads, "Id", "Id", osmotr.DOMFasadId);
            ViewBag.DOMFundamentId = new SelectList(db.DOMFundaments, "Id", "Id", osmotr.DOMFundamentId);
            ViewBag.DOMHWId = new SelectList(db.DOMCWs, "Id", "Id", osmotr.DOMHWId);
            ViewBag.DOMOtoplenieId = new SelectList(db.DOMOtoplenies, "Id", "Id", osmotr.DOMOtoplenieId);
            ViewBag.DOMRoofId = new SelectList(db.DOMRoofs, "Id", "Id", osmotr.DOMRoofId);
            ViewBag.DOMRoomId = new SelectList(db.DOMRoofs, "Id", "Id", osmotr.DOMRoomId);
            ViewBag.DOMVodootvodId = new SelectList(db.DOMRoofs, "Id", "Id", osmotr.DOMVodootvodId);
            return View(osmotr);
        }

        // GET: Osmotrs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Osmotr osmotr = db.Osmotrs.Find(id);
            if (osmotr == null)
            {
                return HttpNotFound();
            }
            return View(osmotr);
        }

        // POST: Osmotrs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Osmotr osmotr = db.Osmotrs.Find(id);
            db.Osmotrs.Remove(osmotr);
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