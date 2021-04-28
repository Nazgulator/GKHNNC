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

namespace GKHNNC.Controllers
{
    public class AdresController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Adres
       
        public ActionResult Index()
        {
            return View(db.Adres.ToList());
        }

        // GET: Adres/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adres adres = db.Adres.Find(id);
            if (adres == null)
            {
                return HttpNotFound();
            }
            return View(adres);
        }

        // GET: Adres/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Adres/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Ulica,Dom,GEU,UEV,OBSD,Ploshad,ActivePloshad")] Adres adres)
        {
            if (ModelState.IsValid)
            {
                adres.Adress = adres.Ulica.Replace(" ", "")+adres.Dom.Replace(" ","");
                adres.IP = "";
                db.Adres.Add(adres);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(adres);
        }

        // GET: Adres/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adres adres = db.Adres.Find(id);
            if (adres == null)
            {
                return HttpNotFound();
            }
            return View(adres);
        }

        [HttpGet]
        public ActionResult Upload()
        {

            return View();
        }

       
        public ActionResult APUpload()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ActivePloshadUpload(HttpPostedFileBase upload)
        {
            int progress = 0;
            double pro100 = 0;
            int procount = 0;
            if (upload != null)
            {
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


                                                // 0       1         2      3                      4         5            6                       7                8                      9                                 10                                            11                      12               13             14                       15                      16
                string[] Names = new string[] { "№п.п.", "Адрес", "Дом", "Итогожилая+нежилая", "Этажей", "Подъездов", "Количествоквартир", "Количестволифтов", "Количествопроживающих", "Общаяплощадьквартир", "Нежилаяплощадьквартир,собственниковпобазеОРС", "Площадьподвала", "Площадьлестничныхклеток", "Площадькровли", "Площадьмусорокамер", "Площадьземельногоучастка", "Подрядчик" };
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
                    pro100 = excel.Count;
                    procount = 0;
                    List<Adres> Adresdb = db.Adres.ToList();
                    foreach (List<string> e in excel)
                    {
                        string E = e[1].Replace("ул.", "").ToUpper().Replace(" ", "").Replace("ПРОСПЕКТ","").Replace("ПРОЕЗД", "").Replace("БУЛЬВАР","").Replace(".","");
                        string D = e[2].Replace(" ", "");
                        E = E + D;
                        Adres A = null;
                             
                        //чистим от пр ул и т.д.
                        try
                        {
                          A= Adresdb.Where(x => x.Adress.Equals(E)).First();
                            try
                            {
                                A.ActivePloshad = Convert.ToDecimal(e[3]);
                                A.Etagi = Convert.ToInt16(e[4]);
                                A.Podezds = Convert.ToInt16(e[5]);
                                A.Kvartirs = Convert.ToInt16(e[6]);
                                A.Lifts = Convert.ToInt16(e[7]);
                                A.Peoples = Convert.ToInt16(e[8]);
                              
                                A.PloshadGilaya = Convert.ToDecimal(e[9]);
                                A.PloshadNegilaya = Convert.ToDecimal(e[10]);
                                A.PloshadPodval = Convert.ToDecimal(e[11]);
                                A.PloshadLestnica = Convert.ToDecimal(e[12]);
                                A.PloshadKrovlya = Convert.ToDecimal(e[13]);
                                A.PloshadMusor = Convert.ToDecimal(e[14]);
                                A.PloshadZemlya = Convert.ToDecimal(e[15]);
                                A.IP = e[16];

                            }
                            catch (Exception exx)
                            {

                            }
                           
                            db.Entry(A).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {

                        }
                      
                        procount++;
                        progress = Convert.ToInt16(50 + procount / pro100 * 50);
                        ProgressHub.SendMessage("Обрабатываем файл, подождите чуток ...", progress);
                        if (procount > pro100) { procount = Convert.ToInt32(pro100); }

                    }


                }

            }
            return View("UploadComplete");
        }


        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            int progress = 0;
            double pro100 = 0;
            int procount = 0;
            if (upload != null)
            {
              




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



                string[] Names = new string[] { "Adres", "Code" };
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
                    pro100 = excel.Count;
                    procount = 0;
                    List<Adres> Adresdb = db.Adres.ToList();
                    foreach (List<string> e in excel)
                    {
                        string E = e[0].Replace(" ", "");
                        foreach (Adres A in Adresdb)
                        {
                            if (E.Equals(A.Adress.Replace(" ","")))
                            {//модифицируем записи в ДБ
                                A.UEV = Convert.ToInt16(e[1]);
                                db.Entry(A).State = EntityState.Modified;
                                db.SaveChanges();
                                break;
                            }

                        }
                        procount++;
                        progress = Convert.ToInt16(50 + procount / pro100 * 50);
                        ProgressHub.SendMessage("Обрабатываем файл, подождите чуток ...", progress);
                        if (procount > pro100) { procount = Convert.ToInt32(pro100); }

                    }


                }

            }
            return View("UploadComplete");
        }



                    //не используется
        public ActionResult Save(int? id, [Bind(Include = "UEV")] string UEV)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adres adres = db.Adres.Find(id);
            adres.UEV = Convert.ToInt32(UEV);
          //  adres.IP = "";
            db.Entry(adres).State = EntityState.Modified;
            db.SaveChanges();
           
            if (adres == null)
            {
                return HttpNotFound();
            }

            return Json("Index");
        }


        // POST: Adres/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Ulica,Dom,GEU,UEV,OBSD,Ploshad,ActivePloshad")] Adres adres)
        {
            if (ModelState.IsValid)
            {
                adres.Adress = adres.Ulica.Replace(" ", "") + adres.Dom.Replace(" ", "");
                adres.IP = "";
                db.Entry(adres).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(adres);
        }

        // GET: Adres/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adres adres = db.Adres.Find(id);
            if (adres == null)
            {
                return HttpNotFound();
            }
            return View(adres);
        }

        // POST: Adres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Adres adres = db.Adres.Find(id);
            db.Adres.Remove(adres);
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
