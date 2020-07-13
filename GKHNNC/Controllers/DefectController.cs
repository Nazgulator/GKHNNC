using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
using static System.IO.File;
using Opredelenie;
using System.Collections;
using Microsoft.AspNet.SignalR;
using System.Reflection;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections;

using System.IO;

namespace GKHNNC.Controllers
{
    public class DefectController : Controller
    {
        private WorkContext db = new WorkContext();
        // GET: Defect
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Upload()
        {
            return View();
        }
        public ActionResult DefectEdit(string selection,string del,string addd,string addw,int xd = 0,int xw =0)
        {
            List<Element> Elements = db.Elements.ToList();



            List<SelectListItem> Result = new List<SelectListItem>();
            foreach (Element El in Elements)
            {
                SelectListItem R = new SelectListItem();
                R.Text = El.Name+ "("+El.ElementType+")";
                R.Value = El.Id.ToString();
                Result.Add(R);
            }
           
            

            int id = Elements[0].Id;
            try
            {
                if (selection != null)
                {
                    id = Convert.ToInt32(selection);
                }
            }
            catch { }
            if (addd != null)
            { Defect d = new Defect();
                d.Def = addd;
                d.ElementId = id;

                db.Defects.Add(d);
                db.SaveChanges();

            }
            if (addw != null)
            {
                DefWork d = new DefWork();
                d.Work = addw;
                d.ElementId = id;

                db.DefWorks.Add(d);
                db.SaveChanges();

            }
            DefectEdit DE = new DefectEdit();
            Element E = null;
            try
            {
                 E = db.Elements.Where(x => x.Id == id).First();
            }
            catch { }
            if (E != null)
            {
                List<Defect> D = new List<Defect>();
                try
                {
                    D = db.Defects.Where(x => x.ElementId == E.Id).Distinct().ToList();

                }
                catch { }
                
                List<DefWork> W = new List<DefWork>();
                try
                {
                    W = db.DefWorks.Where(x => x.ElementId == E.Id).Distinct().ToList();
                }
                catch { }
                //если есть что почистить
              
                if (del != null)
                {
                    del = del.Replace(" ", "");
                    if (del != "")
                    {
                        List<Defect> DD = new List<Defect>();
                        try
                        {
                            DD = D.Where(x => x.Def.Contains(del)).ToList();
                            foreach (Defect c in DD)
                            {
                                D.Remove(c);
                            }
                            db.Defects.RemoveRange(DD);
                            db.SaveChanges();

                        }
                        catch { }
                      /*  List<DefWork> DW = new List<DefWork>();
                        try
                        {
                            DW = W.Where(x => x.Work.Contains(del)).ToList();
                            foreach (DefWork c in DW)
                            { 
                                W.Remove(c);
                            }
                            db.DefWorks.RemoveRange(DW);
                            db.SaveChanges();

                        }
                        catch { }
                        */
                    }

                }
                if (xd!=0)
                {

                    try
                    {

                        Defect DD = db.Defects.Where(x => x.Id == xd).First();
                        D.Remove(DD);
                        db.Defects.Remove(DD);
                        db.SaveChanges();
                    }
                    catch(Exception e) { }
                }
                if (xw != 0)
                {

                    try
                    {

                        DefWork DD = db.DefWorks.Where(x => x.Id == xw).First();
                        W.Remove(DD);
                        db.DefWorks.Remove(DD);
                        db.SaveChanges();
                    }
                    catch (Exception e) { }
                }
                //удаляем дубликаты навсегда
                List<Defect> NoD = new List<Defect>();
                List<DefWork> NoW = new List<DefWork>();
                //удаляем дубликаты дефектов
                foreach (Defect DD in D)
                {
                    List<Defect> N = new List<Defect>();
                    try
                    {
                        N = NoD.Where(x => x.Def.Replace(" ", "").ToLower().Equals(DD.Def.Replace(" ", "").ToLower())).ToList();
                        if (N.Count > 0)
                        {
                            try
                            {//если уже есть такой в последовательности то удаляем
                                db.Defects.Remove(DD);

                                db.SaveChanges();
                            }
                            catch { }
                        }
                        else
                        {
                            NoD.Add(DD);
                        }

                    }
                    catch
                    {
                        //если нет дубликата то добавляем в не дублированные
                        NoD.Add(DD);
                    }
                   
                 
                }
                //удаляем дубликаты работ
                foreach (DefWork DD in W)
                {
                    List<DefWork> N = new List<DefWork>();
                    try
                    {
                        N = NoW.Where(x => x.Work.Replace(" ", "").ToLower().Equals(DD.Work.Replace(" ", "").ToLower())).ToList();
                        if (N.Count > 0)
                        {
                            try
                            {//если уже есть такой в последовательности то удаляем
                                db.DefWorks.Remove(DD);

                                db.SaveChanges();
                            }
                            catch { }
                        }
                        else
                        {
                            NoW.Add(DD);
                        }

                    }
                    catch
                    {
                        //если нет дубликата то добавляем в не дублированные
                        NoW.Add(DD);
                    }


                }
                D = NoD.OrderBy(x=>x.Def).ToList();
                W = NoW.OrderBy(x => x.Work).ToList();
                DE.Element = E;
                DE.DefWork = W;
                DE.Defect = D;
            }
            //  int ind = Convert.ToInt16(Result.Where(x => x.Value.Equals(id.ToString())).Select(x=>x.Value).First());
            //   SelectListItem S = Result[ind];
            //   Result.Remove(S);
            //  Result.Insert(0, S);
            ViewBag.Name = id;
            ViewBag.Result = Result;
            return View(DE);
        }

        [HttpPost]
        public ActionResult ElementSelect(string selection)
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(selection);
            }
            catch { }
            DefectEdit DE = new DefectEdit();
            Element E = db.Elements.Where(x => x.Id == id).First();
            List<Defect> D = db.Defects.Where(x => x.ElementId == E.Id).Distinct().ToList();
            List<DefWork> W = db.DefWorks.Where(x => x.ElementId == E.Id).Distinct().ToList();
            

            return View(DE);
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, HttpPostedFileBase upload2)
        {
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            List<Element> Defects = new List<Element>();
            // получаем имя файла
            string fileName = Path.GetFileName(upload.FileName);
            string fileName2 = Path.GetFileName(upload2.FileName);
            // сохраняем файл в папку Files в проекте
            if (Directory.Exists(Server.MapPath("~/Files/")) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files/"));

            }
            upload.SaveAs(Server.MapPath("~/Files/" + fileName));
            upload2.SaveAs(Server.MapPath("~/Files/" + fileName2));

            //обрабатываем файл после загрузки
            //Загружаем файл ТБО
            string[] Names = new string[] { "Id" ,"Element"  };
            string Error = "";
            List<List<string>> excel = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName), Names, out Error);
            //Загружаем файл наших выездов
            string[] Names2 = new string[] { "ElementId","Description","Location","DeffDesc" };
            string Error2 = "";
            List<List<string>> excel2 = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName2), Names2, out Error2);
            List<Musor> Result = new List<Musor>();
            int elId = 0;
            if (excel.Count < 1 || excel2.Count < 1)
            {
                //если нифига не загрузилось то 
                ViewBag.Error = Error + Error2;
                ViewBag.Names = Names;
                return View("NotUpload");
            }
            else
            {
                try
                {
                    List<Defect> Def = db.Defects.ToList();
                    if (Def.Count > 0)
                    {
                        foreach (Defect D in Def)
                        {
                            db.Defects.Remove(D);
                            db.SaveChanges();
                        }
                    }
                }
                catch { }
                try
                {
                    List<DefDescription> Def = db.DefDescriptions.ToList();
                    if (Def.Count > 0)
                    {
                        foreach (DefDescription D in Def)
                        {
                            db.DefDescriptions.Remove(D);
                            db.SaveChanges();
                        }
                    }
                }
                catch { }
                try
                {
                    List<DefWork> DefWorks = db.DefWorks.ToList();
                    if (DefWorks.Count > 0)
                    {
                        foreach (DefWork D in DefWorks)
                        {
                            db.DefWorks.Remove(D);
                            db.SaveChanges();
                        }
                    }
                }
                catch { }

                try
                {
                    List<Element> elem = db.Elements.ToList();
                    if (elem.Count > 0)
                    {
                        foreach (Element D in elem)
                        {
                            db.Elements.Remove(D);
                            db.SaveChanges();
                        }
                    }
                }
                catch { }

                for (int h = 1; h < excel.Count; h++)
                {
                    if (excel[h][1] != "NULL" && excel[h][1] != null && excel[h][1] != "")
                    {
                        Element E = new Element();
                        E.Name= excel[h][1];
                        E.ElementId = Convert.ToInt32(excel[h][0]);
                        try
                        {
                            db.Elements.Add(E);
                            db.SaveChanges();
                        }
                        catch
                        {
                            db.Elements.Remove(E);
                        }

                        
                    }
                }
              
                decimal pro100 = Convert.ToDecimal(excel2.Count);
                int progress = 0;
                int procount = 0;
                /*
                Excel.Range range;//рэндж
                ApExcel.Visible = false;//невидимо
                ApExcel.ScreenUpdating = false;//и не обновляемо

                //call this method inside your working action
                ProgressHub.SendMessage("Инициализация и подготовка...", 0);
                ApExcel.Visible = false;//невидимо
                ApExcel.ScreenUpdating = false;//и не обновляемо
                */

                    /*ApExcel.Worksheets.Add(Type.Missing);//Добавляем лист
                    Excel.Worksheet WS = WbExcel.Sheets[1];
                    try
                    {
                        WS.Name = Defects[h].Element;
                    }
                    catch
                    {
                        WS.Name = Defects[h].Element+h.ToString();
                        
                    }
                    */



                    foreach (List<string> E in excel2)
                    {
                    if (E[0] != null && E[0] != "")
                    {
                        int id = 0;
                        string E1 = E[1].ToLower().Replace(",", "").Replace(".", "").Replace("-", "");
                        string E3 = E[3].ToLower().Replace(",", "").Replace(".", "").Replace("-", "");
                        string E2 = E[2].ToLower().Replace(",", "").Replace(".", "").Replace("-", "");
                        Defect DEFECT = new Defect();
                        bool go = true;
                        try
                        {
                            id = Convert.ToInt32(E[0]);
                            elId = db.Elements.Where(x => x.ElementId == id).Select(z => z.Id).First();
                            
                        }
                        catch
                        {
                            go = false;
                        }
                        if (go)
                        {

                            List<Defect> D = new List<Defect>();



                            //если нет совпадений то ok
                            if (E1 != "" && E1 != "0" && E1 != "null")
                            {
                                DEFECT.Def = E1;
                                DEFECT.ElementId = elId;
                               
                                try
                                {
                                    db.Defects.Add(DEFECT);
                                    db.SaveChanges();
                                }
                                catch
                                {
                                    // db.DefDescriptions.Remove(DESC);
                                   // db.Defects.Remove(DEFECT);
                                }
                            }




                            //Работа
                            DefWork WORK = new DefWork();


                            //если нет совпадений то ok
                            if (E2 != "" && E2 != "0" && E2 != "null")
                            {
                                WORK.Work = E2;
                                WORK.ElementId = elId;
                                
                                try
                                {
                                    db.DefWorks.Add(WORK);
                                    db.SaveChanges();
                                }
                                catch
                                {
                                    // db.DefDescriptions.Remove(DESC);
                                    //db.DefWorks.Remove(WORK);

                                }


                            }
                            if (E3 != "" && E3 != "0" && E3 != "null")
                            {
                                WORK.Work = E3;
                                WORK.ElementId = elId;
                                
                                try
                                {
                                    db.DefWorks.Add(WORK);
                                    db.SaveChanges();
                                }
                                catch
                                {
                                    // db.DefDescriptions.Remove(DESC);
                                   // db.DefWorks.Remove(WORK);

                                }

                            }





                            //если нет совпадений то ok


                          
                        }
                        
                    }
                    procount++;
                    progress = Convert.ToInt16(50 + procount / pro100 * 50);
                    ProgressHub.SendMessage("Обрабатываем файл ...", progress);
                    if (procount > pro100) { procount = Convert.ToInt32(pro100); }
                }
                    
                    

                   



                }
                
            
            string patch = Server.MapPath("~/Content/OtchetDefects.xlsx"); //@"C:\inetpub\Otchets\ASP" + "X" + Year.Remove(0, 2) + "X" + Month + ".xlsx";//Server.MapPath("~\\ASP" +"X"+ Year.Remove(0,2) +"X"+ Month + ".xlsx");
            string filename = "OtchetDefects.xlsx";
            string path2 = Url.Content("~/Content/OtchetDefects.xlsx");

            // RedirectToAction("DownloadPS", new {path,filename});
            string dat = path2; //+ filename;
            string contentType = "application/vnd.ms-excel";
            //
            ViewBag.file = filename;
            ViewBag.dat = dat;





            // Сохранение файла Excel.
            try
            {
                Opr.EstLiFile(patch);
                WbExcel.SaveCopyAs(patch);//сохраняем в папку
            }
            catch
            {

            }
            ApExcel.Visible = true;//видимо
            ApExcel.ScreenUpdating = true;//и  обновляемо
            ApExcel.Quit();


            Marshal.FinalReleaseComObject(WbExcel);
            Marshal.FinalReleaseComObject(WB);



            GC.Collect();
            Marshal.FinalReleaseComObject(ApExcel);
            GC.WaitForPendingFinalizers();
            return View("UploadComplete");
        }
        }
    }