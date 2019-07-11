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
using System.Collections;
using Microsoft.AspNet.SignalR;
using Opredelenie;
using System.Web;
using System.Reflection;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections;

using System.IO;

using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using System.Threading;
using GKHNNC.Models;
using Microsoft.AspNet.Identity;
using System.Web.Helpers;
using Opredelenie;
using System.Diagnostics;



namespace GKHNNC.Controllers
{
    public class HomeController : Controller
    {
        private WorkContext db = new WorkContext();
        public ActionResult Index()
        {
            return View();
        }
        /*
        public ActionResult ProverkaVodaMonth(int Month)
        {
            return View();
        }
        */

        public ActionResult ExportToExcelJquery(List<List<string>> selection)
        {
            List<List<string>> Table = selection;
            DateTime Date = DateTime.Now;
            string Avto = Table[0][0] + Table[0][2];

            string Path = Server.MapPath("~/Content/Voda.xlsx");
            string Path2 = Url.Content("~/Content/Voda.xlsx");
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо

            for (int i = 0; i < Table.Count; i++)
            {
                int mer = 0;
                int max = Table[0].Count;
                int tek = Table[i].Count;
                int from = 1;
                for (int j = 0; j < Table[i].Count; j++)
                {
                    string TT = Table[i][j].Replace("  ", "").Replace("\n", "");
                    WS.Cells[i + 1, j + 1] = TT;

                    if (Table[i][j].Equals(""))
                    {
                        mer++;
                        if (from == 1) { from = i + 1; }
                    }

                }
                //если пустые ячейки в строке занимают более половины то объединяем их

                string F = Opr.OpredelenieBukvi(tek);
                string T = Opr.OpredelenieBukvi(max);
                range = WS.get_Range(F + (i + 1).ToString(), T + (i + 1).ToString());
                range.Merge();
                range.EntireRow.AutoFit();
                range.WrapText = true;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            }

            for (int i = 1; i < Table[0].Count; i++)
            {
                string F = Opr.OpredelenieBukvi(i);

                range = WS.get_Range(F + (1).ToString(), F + (1).ToString());
                range.EntireColumn.AutoFit();
            }

            // Сохранение файла Excel.
            WbExcel.SaveCopyAs(Path);//сохраняем в папку

            ApExcel.Visible = true;//невидимо
            ApExcel.ScreenUpdating = true;//и не обновляемо
                                          // Закрытие книги.
            WbExcel.Close(false, "", Type.Missing);
            // Закрытие приложения Excel.

            ApExcel.Quit();

            Marshal.FinalReleaseComObject(WbExcel);
            Marshal.FinalReleaseComObject(WB);
            Marshal.FinalReleaseComObject(ApExcel);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return Json(Path2);
        }


        public ActionResult VodaToExcel()
        {
            string Data="";


            return Json(Data);
        }
        public ActionResult VodaMonth(int Month)
        {
            
           int Year = DateTime.Now.Year;
            List<Adres> dbAdresa = db.Adres.ToList();//список всех адресов
            //Сервис айди 1 = отопление, 2 = ГВ, 3 = ГВ на общее имущество берем только гв и гв на общее и смотрим складывать ли их
            List<SVN> dbSVNs = db.SVNs.Where(a => a.Date.Year == DateTime.Now.Year && a.Date.Month == Month&&(a.ServiceId == 2||a.ServiceId==3)).Include(b=>b.Service).ToList();
            List<UEV> dbUEV = db.UEVs.Where(c => c.Date.Year == Year && c.Date.Month == Month).ToList();
            List<OPU> dbOPU = db.OPUs.Where(c => c.Date.Year == Year && c.Date.Month == Month).ToList();
            List<IPU> dbIPU = db.IPUs.Where(c => c.Date.Year == Year && c.Date.Month == Month).ToList();
            ViewBag.SVN = false;
            if (dbSVNs.Count > 0)
            {
                ViewBag.SVN = true;
            }
            ViewBag.UEV = false;
            if (dbUEV.Count > 0)
            {
                ViewBag.UEV = true;
            }
            ViewBag.OPU = false;
            if (dbOPU.Count > 0)
            {
                ViewBag.OPU = true;
            }
            ViewBag.IPU = false;
            if (dbIPU.Count > 0)
            {
                ViewBag.IPU = true;
            }
            List<ViewVoda> Result = new List<ViewVoda>();//пишем сюда результат
            List<ViewVoda> RedResult = new List<ViewVoda>();//пишем сюда результат
            List<ViewVoda> NullResult = new List<ViewVoda>();//пишем сюда результат

            //для каждого адреса ищем данные уэв и сумму данных SVN
            bool skladivat = false;
            List<TableService> TS = db.TableServices.Where(g => g.Id == 2 || g.Id == 3).ToList();//проверка складывать ли если числа в поле сумм равны то складываем
            if (TS[0].Summ == TS[1].Summ) { skladivat = true; }
            foreach(Adres A in dbAdresa)
            {
                ViewVoda V = new ViewVoda();
                decimal Plan = 0;
                decimal Fact = 0;
                SVN GVSVN = new SVN();
                SVN GVOSVN = new SVN();
                try
                {
                    GVSVN = dbSVNs.Where(d => d.AdresId == A.Id && d.ServiceId == 2).Single();//горячая вода
                }
                catch { }
                try
                {
                     GVOSVN = dbSVNs.Where(d => d.AdresId == A.Id && d.ServiceId == 3).Single();//горячая вода на общее имущество
                }
                catch { }
                    if (skladivat)//Если суммы равны то значит складываем ГВ общее и ГВ 
                {
                    Plan = GVSVN.Plan + GVOSVN.Plan;//Складываем плановые показатели 
                    Fact = GVSVN.Fact + GVOSVN.Fact;//Складываем фактические показатели

                }
                else
                {
                    Plan = GVSVN.Plan;//если не складывать то берем данные только из свн
                    Fact = GVSVN.Fact;
                }
                //Выставленные показания в рублях
                decimal GVUEV = 0;
                try
                {
                   GVUEV =  dbUEV.Where(e => e.AdresId == A.Id).Sum(f => f.HwVodaRub + f.HwEnergyRub);//ищем выставленную сумму в рублях по горячей воде в данном доме УЭВ
                   
                }
               
                catch
                {   }
                decimal GVIPU = 0;
                try
                {
                    GVIPU = dbIPU.Where(e => e.AdresId == A.Id).Sum(f => f.Normativ - f.Schetchik);//Суммируем ипу норматив - показания по счетчику на домах без ПУ
                }
                catch { }
                //ищем прибор учета и если он есть то выводим галку
                bool pu = false;
                try
                {
                    int Pribor = dbUEV.Where(e => e.AdresId == A.Id).Select(f => f.Pribor).First();
                    if (Pribor > 0) { pu = true; }
                }
                catch { }
                decimal GVUEVM3 = 0;
                try
                {
                    GVUEVM3 = dbUEV.Where(e => e.AdresId == A.Id).Sum(f => f.HwVodaM3);//ищем выставленную сумму в рублях по горячей воде в данном доме УЭВ
                }
                catch { }

                decimal RaznPlan = GVUEV -GVIPU - Plan;//Показания УЭВ - ИПУ - план
                decimal RaznFact = GVUEV -GVIPU - Fact;//Показания УЭВ - ИПУ - Факт

                //ищем в базе все опушки
                string Primech = "";
                decimal VFact = 0;
                try
                {
                    VFact = dbOPU.Where(h => h.AdresId == A.Id).Select(k => k.GWM3).First();
                    Primech = dbOPU.Where(h => h.AdresId == A.Id).Select(k => k.Primech).First();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }


                //сохраняем данные для вывода
                V.Primech = Primech;
                V.VFact = VFact;
                V.Fact = Fact;
                V.Plan = Plan;
                V.RaznFact = RaznFact;
                V.RaznPlan = RaznPlan;
                V.Uev = GVUEV;
                V.Adres = A.Adress;
                V.PU = pu;//прибор учета галкой
                V.IPU = GVIPU;
                V.GVUEVM3 = GVUEVM3;
                if (Plan + Fact + GVUEV == 0)
                {
                    NullResult.Add(V);
                }
                else
                {
                    if (GVUEVM3 > VFact+5 && pu&&Primech=="")//если разница в показаниях более 5 то пишем в красный список
                    {
                        V.Primech = Convert.ToString(Convert.ToInt32(VFact-GVUEVM3));
                        RedResult.Add(V);
                    }
                    else
                    {
                        if (pu&&Primech=="") { V.Primech = Convert.ToString(Convert.ToInt32(VFact - GVUEVM3)); };
                        Result.Add(V);// иначе в желтый список
                    }
                    

                }

            }


            
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            WS.Name = "ГорячаяВода"+Opr.MonthOpred(Month);
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо



            // Сохранение файла Excel.

            WbExcel.SaveCopyAs("C:\\inetpub\\Otchets\\" + "HotWater.xlsx");//сохраняем в папку

            ApExcel.Visible = true;//невидимо
            ApExcel.ScreenUpdating = true;//и не обновляемо
                                          // Закрытие книги.
            WbExcel.Close(false, "", Type.Missing);
            // Закрытие приложения Excel.

            ApExcel.Quit();

            Marshal.FinalReleaseComObject(WbExcel);
            Marshal.FinalReleaseComObject(WB);
            Marshal.FinalReleaseComObject(ApExcel);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            


            List<ViewVoda> MainResult = new List<ViewVoda>();//пишем сюда результат
            MainResult.AddRange(RedResult);
            MainResult.AddRange(Result);
            MainResult.AddRange(NullResult);
            ViewBag.Year = Year;
            ViewBag.Month = Opr.MonthOpred(Month);
            return View(MainResult);
        }
        public decimal RubliVObiem(decimal number, decimal tarif, decimal NDS) //преобразует рубли в объём 
        {
            //(10-10/1.18*0.18)/тариф       (сумма - ндс)/тариф
            decimal itog = (number - number / (1 + NDS) * NDS) / tarif;
            return itog;

        }
        public ActionResult OtchetMonth(int Month, int Year)
        {
            //грузим данные по аренде за месяц
            string OH = "";//Ошибка сохраняется сюда
            List<Arendator> Arendators = new List<Arendator>();
            List<SVN> SVNs = new List<SVN>();
            List<UEV> UEVs = new List<UEV>();
            try
            {
               Arendators = db.Arendators.Where(x => x.Date.Year == Year && x.Date.Month == Month).ToList();//выбираем всех арендаторов 
            }
            catch { OH += "Арендаторов в "+Opr.MonthToNorm(Opr.MonthOpred(Month))+" нет;"; }
            try
            {
                SVNs = db.SVNs.Where(x => x.Date.Year == Year && x.Date.Month == Month).ToList();//берем все записи СВН за дату
            }
            catch { OH += "Данных СВН в " + Opr.MonthToNorm(Opr.MonthOpred(Month)) + " нет;"; }
            try
            {
                UEVs = db.UEVs.Where(x => x.Date.Year == Year && x.Date.Month == Month).ToList();
            }
            catch { OH += "Данных УЭВ в " + Opr.MonthToNorm(Opr.MonthOpred(Month)) + " нет;"; }
            decimal HWPlan = 0;
            try
            {
                HWPlan = SVNs.Where(y => y.ServiceId == 2 || y.ServiceId == 3).Sum(x => x.Plan);//суммируем горячую воду если она по 2 или 3 сервису идет
            }
            catch { OH += "Горячая вода план за " + Opr.MonthToNorm(Opr.MonthOpred(Month)) + " не определена;"; }
            decimal HWFact = 0;
            try
            {
                HWFact = SVNs.Where(y => y.ServiceId == 2 || y.ServiceId == 3).Sum(x => x.Fact);
            }
            catch { OH += "Горячая вода факт за " + Opr.MonthToNorm(Opr.MonthOpred(Month)) + " не определена;"; }
            decimal HWGEU = 0;
            try
            {
                HWGEU = Arendators.Where(x => x.Name.Contains("ЖЭУ") && x.Name.Replace(" ", "").Contains("ЖЭУ3") == false).Sum(y => y.HotWater);//выбираем только жэу без ЖЭУ 3
            }
            catch { OH += "Горячая вода ЖЭУ за " + Opr.MonthToNorm(Opr.MonthOpred(Month)) + " не определена;"; }
            decimal HWArenda = 0;
            try
            {
                HWArenda = Arendators.Where(x => x.Name.Contains("ЖЭУ") == false).Sum(y => y.HotWater);//берем все без жэу
            }
            catch { OH += "Горячая вода Аренда за " + Opr.MonthToNorm(Opr.MonthOpred(Month)) + " не определена;"; }
            decimal HWUEV = 0;
            decimal HWUEVM3 = 0;
            decimal HWUEVGkal = 0;
            try
            {
                HWUEV = UEVs.Where(y=>y.KodUEV!=7225&&y.KodUEV!=29).Sum(x => x.HwVodaRub + x.HwEnergyRub);//Берем данные с УЭВ ГВруб + ГВэнергияруб
                HWUEVM3 = UEVs.Where(y => y.KodUEV != 7225 && y.KodUEV != 29).Sum(x => x.HwVodaM3);
                HWUEVGkal = UEVs.Where(y => y.KodUEV != 7225 && y.KodUEV != 29).Sum(x => x.HwEnergyGkal);//Берем данные с УЭВ ГВруб + ГВэнергияруб
            }
            catch { OH += "Горячая вода УЭВ за " + Opr.MonthToNorm(Opr.MonthOpred(Month)) + " не определена;"; }
            decimal HWNegilaya = 0;
            decimal HWNegilayaM3 = 0;
            decimal HWNegilayaGkal = 0;
            try
            {
                HWNegilaya = UEVs.Where(y => y.KodUEV == 7225 ).Sum(x => x.HwVodaRub + x.HwEnergyRub);//Нежилая в рублях
                HWNegilayaM3 = UEVs.Where(y => y.KodUEV == 7225).Sum(x => x.HwVodaM3);//Нежилая в объёмах
                HWNegilayaGkal = UEVs.Where(y => y.KodUEV == 7225).Sum(x => x.HwEnergyGkal);//Нежилая в гкал
            }
            catch { OH += "Горячая вода Нежилой части не найдена в данных УЭВ за " + Opr.MonthToNorm(Opr.MonthOpred(Month)) + ";"; }
            decimal HWIPU = 0;
            decimal HWIPUGkal = 0;
            decimal HWIPUM3 = 0;
            try
            {
                HWIPU = UEVs.Where(y => y.KodUEV == 29).Sum(x => x.HwVodaRub + x.HwEnergyRub);//Берем данные с УЭВ ГВруб + ГВэнергияруб
                HWIPUGkal = UEVs.Where(y => y.KodUEV == 29).Sum(x => x.HwEnergyGkal);//Берем данные с УЭВ ГВруб + ГВэнергияруб
                HWIPUM3 = UEVs.Where(y => y.KodUEV == 29).Sum(x => x.HwVodaM3);//Берем данные с УЭВ ГВруб + ГВэнергияруб
            }
            catch { OH += "Горячая вода ИПУ не найдена в данных УЭВ за " + Opr.MonthToNorm(Opr.MonthOpred(Month)) + ";"; }
            List<int> SoranEol = new List<int>();
            List<OBSD> OBSDs = new List<OBSD>();
            decimal HWSoEoKv = 0;//Соран ЭОЛ и КВАРСИС 
            try
            {
                //тут ищем нежилую по лицевым счетам
                SoranEol = db.Negilayas.Select(x=>x.CodeOBSD).ToList();
                OBSDs = db.OBSDs.Where(x => x.Date.Year == Year && x.Date.Month == Month&&(x.TableServiceId == 2|| x.TableServiceId == 3)).ToList();
            }
            catch { OH += "СоранЭолКварсис не найдена в данных ОБСД за " + Opr.MonthToNorm(Opr.MonthOpred(Month)) + ";"; }
            foreach (int i in SoranEol)
            {
                HWSoEoKv += OBSDs.Where(x => x.Licevoi == i).Sum(y => y.Nachislenie);
                
            }


            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;
            WS.Name = "Справка по ГВ'ЖКХННЦ' " + Opr.MonthOpred(Month);

                int podrazd = 1;
                int gvsM3 = 3;
                int gvsGK = 2;
                int summa = 4;
            int maxCount = 0;//общее количество строк
            int stroka2 = 2;//начало 2 строки

            WS.Cells[1, 1] = "Справка о распределении затрат по горячему водоснабжению по подразделениям ФГУП 'ЖКХ ННЦ ' за " + Opr.MonthOpred(Month);
                WS.Cells[2, podrazd] = "Подразделения";
                WS.Cells[2, gvsGK] = "ГВ гКал";
                WS.Cells[2, gvsM3] = "ГВ м3";
                WS.Cells[2, summa] = "Сумма";

                decimal NDS = 20;//ндс сейчас 20 подгрузить из таблицы за последнюю дату
                decimal TarifHW = 102.08M;//тариф на теплую воду вынести в глобальные
                decimal TeplotaVKube = 0.062M;//вынести в глобальные переменные
                int punktHW = 3;
                decimal tep = 0.98M;
                decimal TepVK = TeplotaVKube * tep;//конвертатор в тепло
                decimal NegilayaRub = HWNegilaya * TarifHW;
                decimal GVRubToM3 = HWUEV / TarifHW;
                decimal GVRubToGkal = GVRubToM3 * TeplotaVKube * tep;
                decimal GEUGVRubToM3 = HWGEU / TarifHW;
                decimal GEUGVRubToGkal = GEUGVRubToM3 * TeplotaVKube * tep;

                //ЗДЕСЬ
                WS.Cells[punktHW, podrazd] = "1.Собственные нужды (ЖЭУ)";
                WS.Cells[punktHW, gvsGK] = Math.Round(GEUGVRubToGkal, 2);
                WS.Cells[punktHW, gvsM3] = Math.Round(GEUGVRubToM3, 2);
                // WS.Cells[2, summa + 1] = "ОБСД";
                WS.Cells[punktHW, summa] = Math.Round(HWGEU, 2);

                range = WS.get_Range("A" + Convert.ToString(punktHW), "D" + Convert.ToString(punktHW));
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Columns.Interior.Color = System.Drawing.Color.FromArgb(255, 255, 255, 240);

                punktHW++;
                decimal IPU = HWIPU;
                decimal ElesinaArenda = HWArenda + HWNegilaya;//Аренда еслесиной + нежилая часть
                decimal KvarsisSoranEol = HWSoEoKv;
                decimal GeuHWBezGeu3 = HWGEU;
                decimal GVSodergObshImush = HWFact;//горячая вода на общее это ГВФАКТ
                WS.Cells[punktHW, podrazd] = "2.Жилищный фонд";

                decimal Obiem = HWUEVM3 + HWIPUM3 - GEUGVRubToM3 - GVRubToM3 - RubliVObiem(KvarsisSoranEol + HWFact, TarifHW,NDS);
                decimal Obiem2 = HWUEVGkal + HWIPUGkal - GEUGVRubToGkal - GVRubToGkal - RubliVObiem(KvarsisSoranEol + HWFact, TarifHW,NDS) * TeplotaVKube *tep;
                //сибэко не участвует в распределении УЭВ Также ИПУ не суммируется  - summGvFact99
                WS.Cells[punktHW, gvsGK] = Math.Round(Obiem2, 2);
                WS.Cells[punktHW, gvsM3] = Math.Round(Obiem, 2);//(Convert.ToDecimal(IPU) /Tarif0)
                WS.Cells[punktHW, summa] = Math.Round(HWUEV + HWIPU - KvarsisSoranEol - GeuHWBezGeu3 - HWArenda - GVSodergObshImush, 2);//общая сумма по домам из таблицы Зиминой 1830 + сумма ИПУ (она минусовая)- ЭОЛСОРАНКВАРСИС - Жэу без Жэу3 - ЕлесинаАренда- ГВ на содерж
                WS.Cells[punktHW - 1, summa + 1] = "Сумма";
                WS.Cells[punktHW - 1, summa + 5] = "ИПУ";
                // WS.Cells[punktHW - 1, summa + 2].width = 30;
                WS.Cells[punktHW - 1, summa + 3] = "СОРАН,ЭОЛ,Кв.";
                // WS.Cells[punktHW - 1, summa + 3].width = 30;
                WS.Cells[punktHW - 1, summa + 4] = "ЖЭУ без ЖЕУ3";
                // WS.Cells[punktHW - 1, summa + 4].width = 30;
                WS.Cells[punktHW - 1, summa + 2] = "Аренда";
                // WS.Cells[punktHW - 1, summa + 5].width = 30;
                WS.Cells[punktHW - 1, summa + 6] = "ГВ СОИ УЭВ";
                // WS.Cells[punktHW - 1, summa + 7] = "ГВ СОИ С.ЭКО";
                // WS.Cells[punktHW - 1, summa + 6].width = 30;
                WS.Cells[punktHW, summa + 1] = HWUEV;
                WS.Cells[punktHW, summa + 5] = HWIPU;
                WS.Cells[punktHW, summa + 3] = KvarsisSoranEol;
                WS.Cells[punktHW, summa + 4] = GeuHWBezGeu3;
                WS.Cells[punktHW, summa + 2] = HWArenda;
                WS.Cells[punktHW, summa + 6] = GVSodergObshImush;
                // WS.Cells[punktHW, summa + 7] = summGvFact99;
                range = WS.get_Range("A" + Convert.ToString(punktHW), "J" + Convert.ToString(punktHW));
                range.Columns.Interior.Color = System.Drawing.Color.FromArgb(255, 255, 255, 200);
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                range = WS.get_Range("E" + Convert.ToString(punktHW - 1), "J" + Convert.ToString(punktHW - 1));
                range.Columns.Interior.Color = System.Drawing.Color.FromArgb(255, 255, 255, 200);
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                punktHW++;

                WS.Cells[punktHW, podrazd] = "3.Аренда";

                Obiem = GVRubToM3 + HWNegilayaM3 + RubliVObiem(KvarsisSoranEol, TarifHW,NDS);
                Obiem2 = GVRubToGkal + HWNegilayaGkal + RubliVObiem(KvarsisSoranEol, TarifHW,NDS) *TeplotaVKube *tep;
                WS.Cells[punktHW, gvsGK] = Math.Round(Obiem2, 2);//ГВС м3*конвертатор в тепло + нежилая часть гкал 
                WS.Cells[punktHW, gvsM3] = Math.Round(Obiem, 2);//Convert.ToDecimal(GVAll)+Convert.ToDecimal(NCHW[mes]), 2);//Суммируем Аренда Елесиной + Нежилая часть м3
                WS.Cells[punktHW, summa] = Math.Round(HWArenda + HWNegilaya + KvarsisSoranEol, 2);//Аренда+Нежилая+КварсисСоранЭол

                range = WS.get_Range("A" + Convert.ToString(punktHW), "G" + Convert.ToString(punktHW));
                range.Columns.Interior.Color = System.Drawing.Color.FromArgb(255, 200, 255, 255);
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                range = WS.get_Range("E" + Convert.ToString(punktHW + 1), "G" + Convert.ToString(punktHW + 1));
                range.Columns.Interior.Color = System.Drawing.Color.FromArgb(255, 200, 255, 255);
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                WS.Cells[punktHW, summa + 4] = "Тарифы:";
                // WS.Cells[punktHW - 1, summa + 3].width = 30;
                // WS.Cells[punktHW, summa + 4] = "Отопление";
                // WS.Cells[punktHW - 1, summa + 4].width = 30;
                WS.Cells[punktHW, summa + 5] = "ГВ на Общ.Им.";
                // WS.Cells[punktHW - 1, summa + 5].width = 30;
                // WS.Cells[punktHW, summa + 6] = "ХВ на Общ.Им.";
                WS.Cells[punktHW, summa + 6] = "Теплота в m3";

                punktHW++;
                //тут


                WS.Cells[punktHW, podrazd] = "4.На содерж. общего имущества УЭВ";
                Obiem = RubliVObiem(HWFact, TarifHW,NDS);
                Obiem2 = Obiem *TeplotaVKube * tep;
                WS.Cells[punktHW, summa] = Math.Round(HWFact, 2); //Сумма ГВ 
                WS.Cells[punktHW, gvsM3] = Math.Round(Obiem, 2); //ГВ М3
                WS.Cells[punktHW, gvsGK] = Math.Round(Obiem2, 3); //ГВ ГКал

                WS.Cells[punktHW - 1, summa + 1] = Math.Round(NegilayaRub, 2);
                WS.Cells[punktHW - 1, summa + 2] = Math.Round(HWArenda, 2);
                WS.Cells[punktHW - 1, summa + 3] = Math.Round(KvarsisSoranEol, 2);

                WS.Cells[punktHW, summa + 1] = "Нежилая часть";
                WS.Cells[punktHW, summa + 2] = "Аренда";
                WS.Cells[punktHW, summa + 3] = "СОРАН,ЭОЛ,КВ.";
                WS.Cells[punktHW, summa + 4] = "УЭВ";
                // WS.Cells[punktHW, summa + 4] = Tarif0;
                WS.Cells[punktHW, summa + 5] = TarifHW;
                //  WS.Cells[punktHW, summa + 6] = Tarif2;
                WS.Cells[punktHW, summa + 6] = TeplotaVKube;
               

                range = WS.get_Range("A" + Convert.ToString(punktHW), "D" + Convert.ToString(punktHW));
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Columns.Interior.Color = System.Drawing.Color.FromArgb(255, 240, 255, 255);

                range = WS.get_Range("H" + Convert.ToString(punktHW), "J" + Convert.ToString(punktHW));

                range.Columns.Interior.Color = System.Drawing.Color.FromArgb(255, 240, 255, 255);

                range = WS.get_Range("H" + Convert.ToString(punktHW - 1), "J" + Convert.ToString(punktHW + 1));//последняя чать квадрат
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                // WS.Cells[punktHW, summa + 1] = Math.Round(Convert.ToDecimal(OBSD[mes, 3]));
                range = WS.get_Range("E1", "K1");
                range.EntireColumn.ColumnWidth = 14;
                range.EntireColumn.Hidden = true;



                punktHW++;
                WS.Cells[punktHW, podrazd] = "5.Итог";
                WS.Cells[punktHW, summa].FormulaLocal = "=СУММ(D3:D6)";//Math.Round(summGvFact99+ summGvFact+ Convert.ToDouble(Convert.ToDecimal(GVAllRub) + NCRub));
                WS.Cells[punktHW, gvsM3].FormulaLocal = "=СУММ(C3:C6)"; //ГВ М3
                WS.Cells[punktHW, gvsGK].FormulaLocal = "=СУММ(B3:B6)"; //Сумму делим на тариф ГВ умножить на Конвертатор в тепло сибэко (0,064...) * 0.98
                range = WS.get_Range("A" + Convert.ToString(punktHW), "D" + Convert.ToString(punktHW));
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                range = WS.get_Range("A" + punktHW.ToString(), "D" + punktHW.ToString());
                range.Columns.Interior.Color = System.Drawing.Color.FromArgb(255, 255, 225, 200);
                range.Font.Bold = true;

               


                //ширина столбцов
                range = WS.Cells[maxCount + stroka2, podrazd];//столбец подразделение ширина
                range.ColumnWidth = 40;

                //range.NumberFormat = "@";

                range = WS.Cells[maxCount + stroka2, gvsM3];//столбец M3
                range.ColumnWidth = 12;
                range.NumberFormat = "@";

                range = WS.Cells[maxCount + stroka2, gvsGK];//столбец GK
                range.ColumnWidth = 12;
                range.NumberFormat = "0.00";

                range = WS.Cells[maxCount + stroka2, summa];//столбец сумма
                range.ColumnWidth = 12;
                range.NumberFormat = "0.00";



                range = WS.get_Range("A1", "D1");
                range.Merge(Type.Missing);

                range.Font.Bold = true;
                range.Font.Size = 13;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.RowHeight = 40;
                range.WrapText = true;//перенос по словам

                range = WS.get_Range("A2", "D2");
                range.Font.Bold = true;
                //Выделяем всю таблицу

                range = WS.get_Range("A1", "D" + (maxCount + stroka2).ToString());

                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            string path = Server.MapPath("~/Content/OtchetHW.xlsx");
            string filename = "OtchetHW.xlsx";
            WbExcel.SaveCopyAs(path);//сохраняем в папку

            ApExcel.Visible = true;//невидимо
            ApExcel.ScreenUpdating = true;//и не обновляемо
                                          // Закрытие книги.
            WbExcel.Close(false, "", Type.Missing);
            // Закрытие приложения Excel.
            ApExcel.Quit();

            Marshal.FinalReleaseComObject(WbExcel);
            Marshal.FinalReleaseComObject(WB);
            Marshal.FinalReleaseComObject(ApExcel);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string contentType = "application/vnd.ms-excel";
            return File(path, contentType, filename);//отправка файла пользователю (сохранение, скачать файл)

            
        }
        public ActionResult OtoplenieMonth(int Month,int Year)
        {
            if (Year == null)//если год не указан
            {
                Year = DateTime.Now.Year;
            }
            ProgressHub.SendMessage("Инициализация и подготовка...", 0);
            int progress = 0;

               
                List<Adres> dbAdresa = db.Adres.ToList();//список всех адресов
            List<List<string>> VV = new List<List<string>>();//Сюда сохраняем по месяцам все данные
            decimal[] SummFact = new decimal[dbAdresa.Count];
            decimal[] SummPlan = new decimal[dbAdresa.Count];
            decimal[] SummUEV = new decimal[dbAdresa.Count];
            List<string> Months = new List<string>();
            int tek = 0;
            
            for (int i = 1; i <= Month; i++)
            {
                tek++;
                progress = Convert.ToInt32(Convert.ToDecimal(i) / Month*100);
                ProgressHub.SendMessage("Загружено...", progress);
                Months.Add(Opr.MonthOpred(i));//Записываем месяц 
                                              //Сервис айди 1 = отопление, 2 = ГВ, 3 = ГВ на общее имущество берем только гв и гв на общее и смотрим складывать ли их
                List<SVN> dbSVNs = new List<SVN>();
                List<UEV> dbUEV = new List<UEV>();
                List<OPU> dbOPU = new List<OPU>();
                try
                {
                    dbSVNs = db.SVNs.Where(a => a.Date.Year == DateTime.Now.Year && a.Date.Month == i && (a.ServiceId == 1)).Include(b => b.Service).ToList();
                }
                catch { }
                try
                {
                   dbUEV = db.UEVs.Where(c => c.Date.Year == Year && c.Date.Month == i).ToList();
                } catch{ }
                try
                {
                   dbOPU = db.OPUs.Where(c => c.Date.Year == Year && c.Date.Month == i).ToList();
                }
                catch { }
                ViewBag.SVN = false;
                if (dbSVNs.Count > 0)
                {
                    ViewBag.SVN = true;
                }
                ViewBag.UEV = false;
                if (dbUEV.Count > 0)
                {
                    ViewBag.UEV = true;
                }
                ViewBag.OPU = false;
                if (dbOPU.Count > 0)
                {
                    ViewBag.OPU = true;
                }

                List<string> Result = new List<string>();//пишем сюда результат средний


                //для каждого адреса ищем данные уэв и сумму данных SVN
                int count = 0;
                
                
                foreach (Adres A in dbAdresa)
                {
                    

                    string V = "";
                    decimal Plan = 0;
                    decimal Fact = 0;
                    SVN OTOPSVN = new SVN();

                    try
                    {
                        OTOPSVN = dbSVNs.Where(d => d.AdresId == A.Id && d.ServiceId == 1).First();//горячая вода
                    }
                    catch { }
                    Plan = OTOPSVN.Plan;// берем данные только из свн
                    Fact = OTOPSVN.Fact;

                    //Выставленные показания в рублях
                    decimal OTOPUEV = 0;
                    try
                    {
                        OTOPUEV = dbUEV.Where(e => e.AdresId == A.Id).Sum(f => f.OtEnergyRub);//ищем выставленную сумму в рублях по отоплению в данном доме УЭВ

                    }
                    catch
                    { }
                    //ищем прибор учета и если он есть то выводим галку
                    bool pu = false;
                    try
                    {
                        int Pribor = dbUEV.Where(e => e.AdresId == A.Id).Select(f => f.Pribor).First();
                        if (Pribor > 0) { pu = true; }
                    }
                    catch { }


                    //сохраняем данные для вывода
                    //0 факт 1 план 2 уэв 3 пу
                    V += OTOPUEV.ToString() + ";";
                    V += Plan.ToString() + ";";
                    V += Fact.ToString()+";";
                    V += pu.ToString();
                    Result.Add(V);

                    //Сохраняем сумму
                    SummFact[count] += Fact;
                    SummPlan[count] += Plan;
                    SummUEV[count] += OTOPUEV;
                    count++;
                }
                VV.Add(new List<string>());
                VV[i-1].AddRange(Result);//Закидываем массив результатов за месяц

            }

            /*
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            WS.Name = "ГорячаяВода" + Opr.MonthOpred(Month);
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо



            // Сохранение файла Excel.

            WbExcel.SaveCopyAs("C:\\inetpub\\Otchets\\" + "HotWater.xlsx");//сохраняем в папку

            ApExcel.Visible = true;//невидимо
            ApExcel.ScreenUpdating = true;//и не обновляемо
                                          // Закрытие книги.
            WbExcel.Close(false, "", Type.Missing);
            // Закрытие приложения Excel.

            ApExcel.Quit();

            Marshal.FinalReleaseComObject(WbExcel);
            Marshal.FinalReleaseComObject(WB);
            Marshal.FinalReleaseComObject(ApExcel);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            */
            ViewBag.Adresa = dbAdresa.Select(x => x.Adress).ToList();
            ViewBag.Year = Year;
            ViewBag.Month = Opr.MonthOpred(Month);
            ViewBag.Months = Months;
            ViewBag.VV = VV;
            ViewBag.SummFact = SummFact;
            ViewBag.SummPlan = SummFact;
            ViewBag.SummUev = SummUEV;
            return View();
        }
        public ActionResult VODAMain()
        {
            return View();
        }
        public ActionResult VODAMenu ()
        {
            List<int> Years = new List<int>();
            for (int i = 2018; i <= DateTime.Now.Year; i++)
            {
                Years.Add(i);
            }
            ViewBag.Years = Years;
            return View();
        }
        public ActionResult VODAIndex(int year = 0)
        {
            if (year == 0) { year = DateTime.Now.Year; }
           
            ViewBag.Month = Opr.MonthZabit();
            int[] Go =new int[12];
            for (int i=1;i<13;i++)
            {
                
                int x =db.SVNs.Where(a => a.Date.Year == year && a.Date.Month == i).Count();
                if (x > 0) { Go[i - 1]++; }
                int y = db.UEVs.Where(a => a.Date.Year == year && a.Date.Month == i).Count();
                if (y > 0) { Go[i - 1]++; }
                int z = db.OPUs.Where(a => a.Date.Year == year && a.Date.Month == i).Count();
                if (z > 0) { Go[i - 1]++; }
                int d = db.IPUs.Where(a => a.Date.Year == year && a.Date.Month == i).Count();
                if (d > 0) { Go[i - 1]++; }
            }
            ViewBag.Go = Go;
            return View();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, DateTime Date)
        {
            if (upload != null)
            {
                HttpCookie cookie = new HttpCookie("My localhost cookie");

                // Установить значения в нем
                cookie["Download"] = "0";
                // Добавить куки в ответ
                Response.Cookies.Add(cookie);

                


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
                List<HouseToAkt> houses = ExcelUpload.IMPORT(Server.MapPath("~/Files/" + fileName));
              if (houses.Count < 1)
                {
                    
                    RedirectToAction("Warning");
                    
                  
                }
              else
                {
                    List<string> H = new List<string>();//дома списком
                    List<string> U = new List<string>();//услуги списком списком
                    List<bool> HTF = new List<bool>();//помечаем адреса, совпавшие с БД
                    List<int> HId = new List<int>();//помечаем адреса, совпавшие с БД
               
                    List<Adres> Adresa = db.Adres.ToList();// грузим все адреса из БД
                    List<Usluga> Usl = db.Usluga.ToList();// грузим все услуги из БД
                    int progress = 0;
                    decimal pro100 = houses.Count;
                    int procount = 0;
                    foreach (HouseToAkt ho in houses)
                    {
                        procount++;
                        progress = Convert.ToInt16(50+ procount / pro100 * 50);
                        if (progress > 100) { progress = 100; }
                        ProgressHub.SendMessage("Загрузка...", progress);
                        bool go = false;
                        int id = 0;
                        string Adr = "";
                        foreach (Adres A in Adresa)
                        {
                            
                            if (A.Adress.Replace(" ", "").Equals(ho.Adres))
                            {
                                Adr = A.Adress;
                                id = A.Id;
                                go = true;                       
                                break;
                            }
                        }
                        if (go)
                        {
                            H.Add(Adr);//если нашли адрес в БД то сохраним его в список (Он отформатирован верно)
                        }
                        else
                        {
                            H.Add(ho.Adres);// иначе сохраняем тот что в экселе
                        }
                        HTF.Add(go);
                        HId.Add(id);
                        ho.HId = id;
                    }
                    List<bool> UTF = new List<bool>();// помечаем услуги, совпавшие с БД
                    for (int d = 0; d < houses.Count; d++) {

                       
                        List<int> UId = new List<int>();
                        int Ucount = 0;
                        foreach (string us in houses[d].pokazateli)
                        {
                            
                            bool go = false;
                            int id = 0;
                            string PN = "";
                            foreach (Usluga P in Usl)
                            {
                              
                                if (P.Name.ToUpper().Replace(" ", "").Equals(us.ToUpper().Replace(" ", "")))
                                {
                                    PN = P.Name;
                                    id = P.Id;
                                    go = true;
                                    break;

                                }
                                else
                                {
                                    //если объединять все корректировки то этот блок работает
                                  //  if (us.Contains("Корректировка"))
                                  //  {
                                  //      PN = us;
                                  //      id = 17;//корректировки получают код 17
                                  //      go = true;
                                  //      break;
                                  //  }
                                }
                            }
                            if (go)
                            {
                                if (d == 0) { U.Add(PN); }//если нашли услугу в БД то сохраним его в список (Она отформатирована верно)
                            }
                            else
                            {
                                
                                if (d == 0) { U.Add(us); }// иначе сохраняем тот что в экселе
                            }
                            if (d == 0) { UTF.Add(go); }
                            UId.Add(id);

                            houses[d].UId.Add(id);
                            Ucount++;
                        }
                    }

                   

                    //Session["Act2House"] = houses;
                    SessionObjects.HouseToAktsSet(Session, houses);
                    ViewBag.file = fileName;
                    ViewBag.H = H;
                    ViewBag.U = U;
                    ViewBag.HTF = HTF;
                    ViewBag.HId = HId;
                    ViewBag.UTF = UTF;
                    
                    ViewBag.UId = houses[0].UId; 
                    ViewBag.Data = Date;//отправляем дату с загруженного файла
                    ViewBag.Houses = houses;
                    int c = houses.Count;
                    if (c < houses[0].pokazateli.Count) {
                        ViewBag.MaxCount = houses[0].pokazateli.Count;
                            }
                   else
                    {
                        ViewBag.MaxCount = c;
                    }
                    return View("UploadComplete");
                }
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult UploadComplete(DateTime Date)
        {
            //При подтверждении записываем в БД 
           
            if (Date != null)
            {

                var houses = SessionObjects.HouseToAktsGet(Session);

                int progress = 0;
                decimal pro100 = houses.Count;
                int procount = 0;
                ProgressHub.SendMessage("Ожидаем подтверждения...", progress);



                for (int j = 0; j < houses.Count; j++)
                {

                    procount++;
                    progress = Convert.ToInt16( procount / pro100 * 100);
                    if (progress > 100) { progress = 100; }
                    ProgressHub.SendMessage("Записываем в базу...", progress);

                    if (houses[j].HId != 0)//если адрес определен
                    {

                        //если определен адрес и дата не нулевая то чистим совпадающие области в БД
                        List<VipolnennieUslugi> homeDate = db.VipolnennieUslugis.Where(x => x.Date.Year.Equals(Date.Year) && x.Date.Month.Equals(Date.Month)).ToList();
                        for (int a=homeDate.Count-1;a>=0;a--)
                        {
                            if (homeDate[a].AdresId.Equals(houses[j].HId))//если в эту дату в базен есть такой адрес то удаляем услугу
                            {
                                db.VipolnennieUslugis.Remove(homeDate[a]);
                                db.SaveChanges();
                            }
                        }
                        for (int i = 0; i < houses[j].UId.Count; i++)
                        {
                            //Usluga U = new Usluga();
                            if (houses[j].UId[i] != 0)//если услуга определена
                            {
                                VipolnennieUslugi V = new VipolnennieUslugi();
                                V.UslugaId = houses[j].UId[i];//выполненная услуга ID
                                V.AdresId = houses[j].HId;
                                V.Date = Date;
                                V.StoimostNaM2 = houses[j].StoimostNaM2[i];
                                V.StoimostNaMonth = houses[j].StoimostNaMonth[i];
                               // V.Usluga
                                db.VipolnennieUslugis.Add(V);
                                
                                    db.SaveChanges();
                                
                               
                               
                               // db.SaveChanges();
                            }
                        }

                    }
                }
            }
           
            return View("UploadEnd");
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}