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
    public class DOMFundamentsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: DOMFundaments
        public ActionResult Index()
        {
            var dOMFundaments = db.DOMFundaments.Include(d => d.Adres).Include(d => d.Material).Include(d => d.Type);
            return View(dOMFundaments.ToList());
        }

        // GET: DOMFundaments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMFundament dOMFundament = db.DOMFundaments.Find(id);
            if (dOMFundament == null)
            {
                return HttpNotFound();
            }
            return View(dOMFundament);
        }

        // GET: DOMFundaments/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres.OrderBy(x=>x.Adress), "Id", "Adress");
            ViewBag.MaterialId = new SelectList(db.FundamentMaterials, "Id", "Material");
            ViewBag.TypeId = new SelectList(db.FundamentTypes, "Id", "Type");
            return View();
        }

        // POST: DOMFundaments/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Ploshad,MaterialId,TypeId,AdresId,Date")] DOMFundament dOMFundament)
        {
            if (ModelState.IsValid)
            {
                db.DOMFundaments.Add(dOMFundament);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMFundament.AdresId);
            ViewBag.MaterialId = new SelectList(db.FundamentMaterials, "Id", "Material", dOMFundament.MaterialId);
            ViewBag.TypeId = new SelectList(db.FundamentTypes, "Id", "Type", dOMFundament.TypeId);
            return View(dOMFundament);
        }

        // GET: DOMFundaments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMFundament dOMFundament = db.DOMFundaments.Find(id);
            if (dOMFundament == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMFundament.AdresId);
            ViewBag.MaterialId = new SelectList(db.FundamentMaterials, "Id", "Material", dOMFundament.MaterialId);
            ViewBag.TypeId = new SelectList(db.FundamentTypes, "Id", "Type", dOMFundament.TypeId);
            return View(dOMFundament);
        }

        [HttpGet]
        public ActionResult Upload()
        {

            return View();
        }
        public ActionResult NotUpload()
        {
            return View();
        }
        public void otklik (int max,ref int tek,string message)
        {
           tek++;
            int progress = Convert.ToInt16(tek / max * 100);
            if (tek > max) { tek = Convert.ToInt32(max); }
            ProgressHub.SendMessage(message, progress);
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, DateTime Date)
        {
            int progress = 0;
            double pro100 = 0;
            int procount = 0;
            if (upload != null)
            {
                HttpCookie cookie = new HttpCookie("My localhost cookie");

                //найдем старые данные за этот месяц и заменим их не щадя
                List<DOMFundament> dbFundaments = db.DOMFundaments.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList();
               
                int tek = 0;
                foreach (DOMFundament S in dbFundaments)
                {
                    try
                    {
                        db.DOMFundaments.Remove(S);
                        db.SaveChanges();
                        otklik(dbFundaments.Count, ref tek, "удаляем старые данные фундамента...");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                List<DOMRoof> dbRoofs = db.DOMRoofs.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList();
                
                tek = 0;
                foreach (DOMRoof S in dbRoofs)
                {
                    try
                    {
                        db.DOMRoofs.Remove(S);
                        db.SaveChanges();
                        otklik(dbRoofs.Count, ref tek, "удаляем старые данные крыш...");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }


                // Установить значения в нем
                cookie["Download"] = "0";
                // Добавить куки в ответ
                Response.Cookies.Add(cookie);




                //call this method inside your working action
                ProgressHub.SendMessage("Инициализация и подготовка...", 0);

                // получаем имя файла
                string fileName = Path.GetFileName(upload.FileName);
                // сохраняем файл в папку Files в проекте
                if (Directory.Exists(Server.MapPath("~/Files/")) == false)
                {
                    Directory.CreateDirectory(Server.MapPath("~/Files/"));

                }
                upload.SaveAs(Server.MapPath("~/Files/" + fileName));
                //обрабатываем файл после загрузки


                                                 //0адрес        1площадь_отмостки     2материал_фундамента    3тип_фундамента        4Кап_Ремонт_кровли        5Утеплитель          6ФормаКрыши      7КапРемонтНесущейЧасти    8ВидНесущейЧасти         9 ТипКровли
                string[] Names = new string[] { "HOME_ADDRESS", "MKDSPECIFIED_14581", "MKDSPECIFIED_15016_1", "MKDSPECIFIED_13516_1","MKDSPECIFIED_20083","MKDSPECIFIED_15246_1","MKDSPECIFIED_12179_1","MKDSPECIFIED_20078","MKDSPECIFIED_20152_1","MKDSPECIFIED_12185_1"


 };
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
                    DOMFundament Fundament = new DOMFundament();
                    DOMRoof Roof = new DOMRoof();
                    List<Adres> Adresa = db.Adres.ToList();// грузим все адреса из БД
                    List<FundamentMaterial> FundamentMat = db.FundamentMaterials.ToList();
                    List<FundamentType> FundamentType = db.FundamentTypes.ToList();
                    List<RoofType> RT = db.RoofTypes.ToList();
                    List<RoofForm> RF = db.RoofForms.ToList();
                    List<RoofVid> RV = db.RoofVids.ToList();
                    List<RoofUteplenie> RU = db.RoofUteplenies.ToList();
                    List<string> save = new List<string>();
                    List<string> errors = new List<string>();
                    List<string> saveR = new List<string>();
                    List<string> errorsR = new List<string>();


                    //для каждой строки в экселе
                    foreach (List<string> L in excel)
                    {
                       
                        //ищем первые три запятые и вырезаем строку ищем улицу
                        int zap = 0;
                        for (int i= (L[0].Length-1); i>0;i--)
                        {
                            if(L[0][i].Equals(','))
                            {
                                zap++;
                                if (zap==2)
                                {
                                    L[0] = L[0].Remove(0, i).Replace("пр-кт", "").Replace("Бульвар", "").Replace("проезд", "").Replace("ул","").Replace("д.","").Replace("б-р","").Replace(",", "").Replace(" ","").ToUpper();
                                    break;
                                }
                            }
                        }
                        //сверяем улицу 
                        bool go = false;
                        foreach (Adres A in Adresa)
                        {
                            if (A.Adress.Equals(L[0]))
                            {
                                //если улица совпала то сохраняем айдишник
                                Fundament.AdresId = A.Id;
                                Roof.AdresId = A.Id;
                                go = true;
                                break;
                            }
                        }
                        //если нашли адрес то сохраняем все остальные данные
                      
                        if (go)
                        {
                            Fundament.Date = Date;
                            //ищем материал 
                            Fundament.MaterialId = 1;//если не найдет
                            Fundament.Ploshad = Convert.ToDecimal(L[1]);

                            foreach (FundamentMaterial FM in FundamentMat)
                            {
                                if (FM.Material.Replace(" ", "").Equals(L[2].Replace(" ", "")))
                                {
                                    Fundament.MaterialId = FM.Id;
                                    break;
                                }
                            }
                            //ищем тип фундамента
                            Fundament.TypeId = 1;//если не найдет
                            foreach (FundamentType FT in FundamentType)
                            {
                                if (FT.Type.Replace(" ", "").Equals(L[3].Replace(" ", "")))
                                {
                                    Fundament.TypeId = FT.Id;
                                    break;
                                }
                            }
                            
                            //сохраняем фундамент
                            if (Fundament.TypeId != 1 && Fundament.MaterialId != 1)
                            {
                                try
                                {
                                    db.DOMFundaments.Add(Fundament);
                                    db.SaveChanges();
                                    save.Add(L[0]);
                                }
                                catch (Exception e)
                                {
                                    
                                    errors.Add(L[0]+ "(ошибка сохранения)");
                                }
                            }
                            else
                            {
                               
                                errors.Add(L[0]+ "(нулевые данные)");
                            }


                            //теперь ищем крыши
                            Roof.Date = Date;
                          

                            Roof.Ploshad = 0;
                            Roof.YearKrovlya = Convert.ToInt16(L[4]);
                            Roof.Year = Convert.ToInt16(L[7]);
                            //Тип крыши
                            Roof.TypeId = 1;//если не найдет
                            foreach (RoofType R in RT)
                            {
                                if (R.Type.Replace(" ", "").Equals(L[9].Replace(" ", "")))
                                {
                                    Roof.TypeId = R.Id;
                                    break;
                                }
                            }
                            //Вид крыши
                            Roof.VidId = 1;//если не найдет
                            foreach (RoofVid R in RV)
                            {
                                if (R.Vid.Replace(" ", "").Equals(L[8].Replace(" ", "")))
                                {
                                    Roof.VidId = R.Id;
                                    break;
                                }
                            }
                            //Форма крыши
                            Roof.FormId = 1;//если не найдет
                            foreach (RoofForm R in RF)
                            {
                                if (R.Form.Replace(" ", "").Equals(L[6].Replace(" ", "")))
                                {
                                    Roof.FormId = R.Id;
                                    break;
                                }
                            }
                            //Утепление крыши
                            Roof.UteplenieId = 1;//если не найдет
                            foreach (RoofUteplenie R in RU)
                            {
                                if (R.Uteplenie.Replace(" ", "").Equals(L[5].Replace(" ", "")))
                                {
                                    Roof.UteplenieId = R.Id;
                                    break;
                                }
                            }

                            //если данные по крышам не нулевые то сохраняем
                            if (Roof.UteplenieId + Roof.TypeId + Roof.VidId + Roof.FormId >4)
                            {
                                try
                                {
                                    db.DOMRoofs.Add(Roof);
                                    db.SaveChanges();
                                    saveR.Add(L[0]);
                                }
                                catch (Exception e )
                                {
                                    db.DOMRoofs.Remove(Roof);
                                    errorsR.Add(L[0] + "(ошибка сохранения)");
                                }
                            }
                            else
                            {
                                errorsR.Add(L[0] + "(нулевые данные)");
                            }



                        }
                        else
                        {
                            errors.Add(L[0]);
                        }
                        //теперь сохраняем крыши если конечно нашли адрес
                     
                        
                           
                       
                      


                        
                        procount++;
                        progress = Convert.ToInt16(50 + procount / pro100 * 50);
                        ProgressHub.SendMessage("Обрабатываем файл ГИС ЖКХ...", progress);
                        if (procount > pro100) { procount = Convert.ToInt32(pro100); }

                    }
                   
                    ViewBag.Save = save;
                    ViewBag.Errors = errors;
                    ViewBag.SaveR = saveR;
                    ViewBag.ErrorsR = errorsR;



                    ViewBag.date = Date;
                    ViewBag.file = fileName;



                    return View("UploadComplete");
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult UploadComplete()
        {

            return View();
        }



        // POST: DOMFundaments/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Ploshad,MaterialId,TypeId,AdresId,Date")] DOMFundament dOMFundament)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dOMFundament).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMFundament.AdresId);
            ViewBag.MaterialId = new SelectList(db.FundamentMaterials, "Id", "Material", dOMFundament.MaterialId);
            ViewBag.TypeId = new SelectList(db.FundamentTypes, "Id", "Type", dOMFundament.TypeId);
            return View(dOMFundament);
        }

        // GET: DOMFundaments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMFundament dOMFundament = db.DOMFundaments.Find(id);
            if (dOMFundament == null)
            {
                return HttpNotFound();
            }
            return View(dOMFundament);
        }

        // POST: DOMFundaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DOMFundament dOMFundament = db.DOMFundaments.Find(id);
            db.DOMFundaments.Remove(dOMFundament);
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
