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
    public class MusorsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Musors
        public ActionResult Index()
        {
            return View(db.Musors.ToList());
        }

        // GET: Musors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Musor musor = db.Musors.Find(id);
            if (musor == null)
            {
                return HttpNotFound();
            }
            return View(musor);
        }
        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }
        public ActionResult PoiskMusor(DateTime date)
        {
            //ищем все данные за этот месяц, если они есть выводим предупреждение что уже есть данные и они удалятся если сюда грузить UEV
            int dbMusor = db.Musors.Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month).Count();
            return Json(dbMusor);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, HttpPostedFileBase upload2, DateTime Date)
        {
            int progress = 0;
            double pro100 = 0;
            int procount = 0;
            if (upload != null)
            {
                HttpCookie cookie = new HttpCookie("My localhost cookie");

                //найдем старые данные за этот месяц и заменим их не щадя
                List<Musor> dbMusor = new List<Musor>();
                try { dbMusor = db.Musors.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList(); }
                catch { }
                pro100 = dbMusor.Count;
                foreach (Musor S in dbMusor)
                {
                    try
                    {
                        db.Musors.Remove(S);
                        db.SaveChanges();
                        procount++;
                        progress = Convert.ToInt16(procount / pro100 * 100);
                        if (procount > pro100) { procount = Convert.ToInt32(pro100); }
                        ProgressHub.SendMessage("Удаляем старые данные...", progress);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }


                }

                Excel.Application ApExcel = new Excel.Application();
                Excel.Workbooks WB = null;
                WB = ApExcel.Workbooks;
                Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);


                for (int h = 1; h <= 31; h++)
                {
                    ApExcel.Worksheets.Add(Type.Missing);//Добавляем лист
                    Excel.Worksheet WS = WbExcel.Sheets[1];
                    WS.Name = h.ToString() + ".01.2019";
                    Excel.Range range;//рэндж
                    ApExcel.Visible = false;//невидимо
                    ApExcel.ScreenUpdating = false;//и не обновляемо

                    //call this method inside your working action
                    ProgressHub.SendMessage("Инициализация и подготовка...", 0);

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
                    string[] Names2 = new string[] { "Государственный регистрационный знак", "Итого принято ТКО за смену, кг", "Итого принято ТКО за смену, куб.м." };
                    string Error2 = "";
                    List<List<string>> excel2 = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName2), Names2, out Error2,h.ToString());
                    //Загружаем файл наших выездов
                    string[] Names = new string[] { "Дата", "Автомобиль", "Улица", "Номер дома", "Объем ТКО за смену, м3" };
                    string Error = "";
                    List<List<string>> excel = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName), Names, out Error,h.ToString());
                    List<Musor> Result = new List<Musor>();
                    if (excel.Count < 1 || excel2.Count < 1)
                    {
                        //если нифига не загрузилось то 
                        ViewBag.Error = Error + Error2;
                        ViewBag.Names = Names;
                        return View("NotUpload");
                    }
                    else
                    {
                        pro100 = excel.Count;

                        List<Musor> Mus = new List<Musor>();
                        List<string> NeMus = new List<string>();



                        //для каждой строки в экселе "Дата", "Автомобиль", "Улица", "Номер дома", "Объем ТКО за смену, м3"
                        Musor Old = new Musor();
                        string Name = "";
                        foreach (List<string> L in excel)
                        {
                            try
                            {

                                if (L[1] != null && L[1] != "" && L[1] != "0")
                                {
                                    Musor MusorKA = new Musor();

                                    MusorKA.Date = Convert.ToDateTime(L[0]);
                                    MusorKA.Name = L[1].ToUpper().Replace(" ", "").Replace("154", "").Replace("54", "").Replace("ГАЗ", "").Replace("САЗ", "").Replace("МАЗ", "").Replace("ВАЗ", "").Replace("ЗИЛ", "").Replace("МТЗ", "").Replace("-", "");
                                    MusorKA.AvtoID = 0;
                                    try
                                    {
                                        MusorKA.AvtoID= db.Avtomobils.Where(x => MusorKA.Name.Equals(x.Number)).Select(x => x.Id).First();
                                    }
                                    catch
                                    {

                                    }

                                    MusorKA.KgOut = 0;
                                    MusorKA.Mesta = (L[2] + L[3]).Replace(" ", "");
                                    MusorKA.ObiemIn = Convert.ToDecimal(L[4]);
                                    Mus.Add(MusorKA);
                                    Old = MusorKA;
                                }
                                else
                                {
                                    if (L[2] != "0" &&L[2]!=null&&L[2]!="")
                                    {
                                        Musor MusorKA = new Musor();

                                        MusorKA.Date = Old.Date;
                                        MusorKA.Name = Old.Name;
                                        MusorKA.AvtoID = Old.AvtoID;
                                        MusorKA.KgOut = 0;
                                        MusorKA.Mesta = (L[2] + L[3]).Replace(" ", "");
                                        MusorKA.ObiemIn = Convert.ToDecimal(L[4]);
                                        Mus.Add(MusorKA);
                                    }
                                    else
                                    {
                                        NeMus.Add(L[0] + ";" + L[1] + ";" + L[2] + ";" + L[3] + ";" + L[4] + ";");
                                        continue;
                                    }

                                    
                                }
                                // db.Musors.Add(MusorKA);
                                // db.SaveChanges();
                            }
                            catch
                            {
                                if (L[0] != "" && L[0] != null)
                                {
                                    Name = L[0].ToUpper().Replace(" ", "").Replace("154", "").Replace("54", "").Replace("ГАЗ", "").Replace("САЗ", "").Replace("МАЗ", "").Replace("ВАЗ", "").Replace("ЗИЛ", "").Replace("МАРШРУТ", "").Replace("МТЗ", "").Replace("-", "");
                                    if (Name.Contains("№"))
                                        for (int i = 0; i < 10; i++)
                                        {
                                            Name = Name.Replace("№" + i.ToString(), "");
                                        }
                                    Old.AvtoID = 0;
                                    try
                                    {
                                        Old.AvtoID = db.Avtomobils.Where(x => Name.Equals(x.Number)).Select(x => x.Id).First();
                                        Old.Name = Name;
                                    }
                                    catch
                                    {
                                        NeMus.Add(L[0] + ";" + L[1] + ";" + L[2] + ";" + L[3] + ";" + L[4] + ";");
                                        continue;
                                    }
                                }
                                else
                                {
                                    NeMus.Add(L[0] + ";" + L[1] + ";" + L[2] + ";" + L[3] + ";" + L[4] + ";");
                                    continue;
                                }
                            }
                            procount++;
                            progress = Convert.ToInt16(50 + procount / pro100 * 50);
                            ProgressHub.SendMessage("Обрабатываем файл УЭВ...", progress);
                            if (procount > pro100) { procount = Convert.ToInt32(pro100); }

                        }
                        List<string> Nam = Mus.Select(x => x.Name).Distinct().ToList();

                        foreach (string N in Nam)
                        {
                            if (N != "")
                            {
                                List<Musor> summa = Mus.Where(x => x.Name.Equals(N)).ToList();
                                string Mest = "";
                                decimal sum = 0;
                                for (int i = 0; i < summa.Count; i++)
                                {
                                    Mest += summa[i].Mesta + "\r\n";
                                    sum += summa[i].ObiemIn;
                                }
                                Musor M = new Musor();
                                M.Date = summa[0].Date;
                                M.AvtoID = summa[0].AvtoID;
                                M.Mesta = Mest;
                                M.Name = N;
                                M.ObiemIn = sum;
                                M.ObiemOut = 0;
                                M.KgOut = 0;
                                //Теперь ищем объёмы с полигона "Государственный регистрационный знак", "Итого принято ТКО за смену, кг", "Итого принято ТКО за смену, куб.м."
                                foreach (List<string> P in excel2)
                                {
                                    if (P[0].ToUpper().Replace(" ", "").Contains(N))
                                    {
                                        M.ObiemOut = Convert.ToDecimal(P[2]);
                                        M.KgOut = Convert.ToDecimal(P[1]);
                                        break;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                //Ищем водителя и пробег в ездках
                                Ezdka E = new Ezdka();
                                try
                                {
                                    E = db.Ezdkas.Where(x => x.Date.Year == M.Date.Year && x.Date.Month == M.Date.Month && x.Date.Day == M.Date.Day && x.AvtoId == M.AvtoID).First();
                                }
                                catch
                                {

                                }
                                if (E.Voditel == null) { E.Voditel = ""; }
                                if (E.Probeg == null) { E.Probeg = 0; }
                                M.Driver = E.Voditel;
                                M.Probeg = E.Probeg;

                                Result.Add(M);
                            }
                        }


                    }

                    //Выводим отчет

                    int stroka = 1;
                    WS.Cells[stroka, 1] = "Приложение №3 ";
                    range = WS.get_Range("A" + stroka.ToString(), "P" + stroka.ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    stroka++;
                    WS.Cells[stroka, 1] = "к договору на оказание услуг по транспортированию твердых коммунальных отходов №______от " + Result[0].Date.ToString("dd/MM/yyyy");
                    range = WS.get_Range("A" + stroka.ToString(), "P" + stroka.ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    stroka++;
                    WS.Cells[stroka, 1] = "Выезд на маршрут";
                    range = WS.get_Range("A" + stroka.ToString(), "C" + stroka.ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    WS.Cells[stroka, 4] = "Загрузка ТКО";
                    range = WS.get_Range("D" + stroka.ToString(), "G" + stroka.ToString());
                    Opr.RangeMerge(ApExcel, range, true, true, 13, 70);
                    WS.Cells[stroka, 8] = "Транспортирование ТКО";
                    range = WS.get_Range("H" + stroka.ToString(), "J" + stroka.ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    WS.Cells[stroka, 11] = "Выгрузка ТКО";
                    range = WS.get_Range("K" + stroka.ToString(), "N" + stroka.ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    WS.Cells[stroka, 15] = "Примечание";
                    range = WS.get_Range("O" + stroka.ToString(), "O" + stroka.ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    stroka++;
                    WS.Cells[stroka, 1] = "Дата";
                    range = WS.get_Range("A" + stroka.ToString(), "A" + stroka.ToString());
                    Opr.RangeMerge(ApExcel, range, true, true, 13, 20);
                    WS.Cells[stroka, 2] = "Номер рейса";
                    range = WS.get_Range("B" + stroka.ToString(), "B" + (stroka + 1).ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    WS.Cells[stroka, 3] = "ФИО водителя";
                    range = WS.get_Range("C" + stroka.ToString(), "C" + (stroka + 1).ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    WS.Cells[stroka, 4] = "Место накопления(адрес загрузки мусоровоза)";
                    range = WS.get_Range("D" + stroka.ToString(), "D" + (stroka + 1).ToString());
                    Opr.RangeMerge(ApExcel, range, true, true, 13, 60);
                    WS.Cells[stroka, 5] = "Объем / тип контейнера";
                    range = WS.get_Range("E" + stroka.ToString(), "E" + (stroka + 1).ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    WS.Cells[stroka, 6] = "Время загрузки";
                    range = WS.get_Range("F" + stroka.ToString(), "F" + (stroka + 1).ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    WS.Cells[stroka, 7] = "Объем загрузки";
                    WS.Cells[stroka, 8] = "Показания одометра, км";
                    range = WS.get_Range("H" + stroka.ToString(), "I" + stroka.ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    WS.Cells[stroka, 10] = "Показания одометра Плечо доставки, км";
                    WS.Cells[stroka, 11] = "Место(адрес) выгрузки";
                    range = WS.get_Range("K" + stroka.ToString(), "K" + (stroka + 1).ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    WS.Cells[stroka, 12] = "Дата / время выгрузки";
                    range = WS.get_Range("L" + stroka.ToString(), "L" + (stroka + 1).ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    WS.Cells[stroka, 13] = "Объем и / или масса выгрузки";
                    range = WS.get_Range("O" + stroka.ToString(), "P" + stroka.ToString());
                    Opr.RangeMerge(ApExcel, range, true, true);
                    stroka++;
                    WS.Cells[stroka, 1] = "Время";
                    range = WS.get_Range("A" + stroka.ToString(), "A" + stroka.ToString());
                    Opr.RangeMerge(ApExcel, range, true, true, 13, 20);
                    WS.Cells[stroka, 7] = "Объем, м³";
                    WS.Cells[stroka, 8] = "Начало маршрута";
                    WS.Cells[stroka, 9] = "Конец маршрута";
                    WS.Cells[stroka, 10] = "(от последнего места загрузки до места выгрузки)";
                    WS.Cells[stroka, 13] = "Объем, м³";
                    WS.Cells[stroka, 14] = "Масса, кг";
                    range = WS.get_Range("A1", "O15");
                    range.Font.Size = 13;
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.WrapText = true;
                    range.Font.Bold = true;
                    range.ColumnWidth = 15;
                    range = WS.get_Range("A1", "A1");
                    range.ColumnWidth = 20;
                    range = WS.get_Range("B1", "B1");
                    range.ColumnWidth = 6;
                    range = WS.get_Range("C1", "C1");
                    range.ColumnWidth = 20;
                    range = WS.get_Range("D1", "D1");
                    range.ColumnWidth = 60;
                    range = WS.get_Range("E1", "E1");
                    range.ColumnWidth = 10;
                    range = WS.get_Range("G1", "G1");
                    range.ColumnWidth = 10;
                    range = WS.get_Range("J1", "J1");
                    range.ColumnWidth = 10;
                    range = WS.get_Range("M1", "O1");
                    range.ColumnWidth = 10;
                    stroka++;
                    for (int i = 0; i < Result.Count(); i++)
                    {
                        WS.Cells[stroka, 1] = Result[i].Date;
                        WS.Cells[stroka, 2] = (i+1).ToString();
                        WS.Cells[stroka, 3] = Result[i].Driver;
                        WS.Cells[stroka, 4] = Result[i].Mesta;
                        WS.Cells[stroka, 5] = "0,75/M";
                        WS.Cells[stroka, 7] = Result[i].ObiemIn;
                        WS.Cells[stroka, 10] = Result[i].Probeg;
                        WS.Cells[stroka, 11] = "Полигон ТБО";
                        WS.Cells[stroka, 13] = Result[i].ObiemOut;
                        WS.Cells[stroka, 14] = Result[i].KgOut;
                        WS.Cells[stroka, 15] = Result[i].Name;
                        range = WS.get_Range("A"+stroka, "A"+stroka);
                        range.RowHeight = 50;
                        range.Font.Size = 12;
                        range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        range.WrapText = true;

                        stroka++;
                    }
                }
              

                string patch = Server.MapPath("~/Content/OtchetECO.xlsx"); //@"C:\inetpub\Otchets\ASP" + "X" + Year.Remove(0, 2) + "X" + Month + ".xlsx";//Server.MapPath("~\\ASP" +"X"+ Year.Remove(0,2) +"X"+ Month + ".xlsx");
                string filename = "OtchetECO.xlsx";
                string path2 = Url.Content("~/Content/OtchetECO.xlsx");

                // RedirectToAction("DownloadPS", new {path,filename});
                string dat = path2; //+ filename;
                string contentType = "application/vnd.ms-excel";
                //

               



               
                // Сохранение файла Excel.
                try
                {
                    Opr.EstLiFile(patch);
                    WbExcel.SaveCopyAs(patch);//сохраняем в папку
                }
                catch
                {

                }
                //WbExcel.PrintOutEx(1, 1, 1, true, null, null, null, null, null);//печать сразу после сохранения
                ApExcel.Visible = false;//невидимо
                ApExcel.ScreenUpdating = false;//и не обновляемо
                                               // Закрытие книги.
                WbExcel.Close(false, "", Type.Missing);
                // Закрытие приложения Excel.


                ApExcel.Quit();
               
               
                Marshal.FinalReleaseComObject(WbExcel);
                Marshal.FinalReleaseComObject(WB);



                GC.Collect();
                Marshal.FinalReleaseComObject(ApExcel);
                GC.WaitForPendingFinalizers();







                ViewBag.date = Date;
                    ViewBag.file = filename;
                ViewBag.dat = dat;
               // return File(path2, contentType, filename);
              //  return Json(dat);
                return View("UploadComplete");
                }
            
            return RedirectToAction("Upload");
        }


     


        public ActionResult UploadComplete()
        {

            return View();
        }
        // GET: Musors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Musors/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Date,AvtoID,Mesta,ObiemIn,ObiemOut,KgOut")] Musor musor)
        {
            if (ModelState.IsValid)
            {
                db.Musors.Add(musor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(musor);
        }

        // GET: Musors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Musor musor = db.Musors.Find(id);
            if (musor == null)
            {
                return HttpNotFound();
            }
            return View(musor);
        }

        // POST: Musors/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Date,AvtoID,Mesta,ObiemIn,ObiemOut,KgOut")] Musor musor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(musor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(musor);
        }

        // GET: Musors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Musor musor = db.Musors.Find(id);
            if (musor == null)
            {
                return HttpNotFound();
            }
            return View(musor);
        }

        // POST: Musors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Musor musor = db.Musors.Find(id);
            db.Musors.Remove(musor);
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
