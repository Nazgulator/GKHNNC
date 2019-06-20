using GKHNNC.DAL;
using GKHNNC.Models;
using Opredelenie;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using Opredelenie;

namespace GKHNNC.Controllers
{
    public class EzdkasController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Ezdkas
        public ActionResult Index()
        {
            var ezdkas = db.Ezdkas.Include(e => e.Avto).OrderBy(c=>c.Avto.Id);
            return View(ezdkas.ToList());
        }

        public static void ExportToExcel(List<List<string>> Table, DateTime Date, string Path)
        {
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо

            for (int i=0; i< Table.Count;i++)
            {
                for (int j=0;j<Table[i].Count;j++)
                {
                    WS.Cells[i+1, j+1] = Table[i][j];
                 

                }

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
        }
        public ActionResult SverkaAvtoscan()
        {
            List<AutoScansSverka> ASSdb = new List<AutoScansSverka>();

            List<int> ASAvto = new List<int>();
            ASAvto = db.Avtomobils.Where(v => v.Glonass==true).Select(c=>c.Id).Distinct().ToList();
            ProgressHub.SendMessage("Инициализация и подготовка...", 0);
            int progress = 0;
            for (int i = 1; i <= 31; i++)
            {
                progress =  Convert.ToInt16(Convert.ToDecimal(i) / 31 * 100);
                ProgressHub.SendMessage("Загружено...", progress);
                //пишем каждую запись в лист
                //проверка по часам
                // for (int j = 0; j < 25;j++) {
                foreach (int Avto in ASAvto)
                    {
                        decimal DUTS = 0;
                        decimal KM = 0;
                        decimal MaxSpeed = 0;
                        decimal Sliv = 0;
                        decimal Zapravleno = 0;

                        //сумма по дуту
                        try
                        {
                            List<AutoScansSverka> ASS = db.AutoScansSverkas.Where(x => x.DateSnyatia.Day == i && (x.DateSnyatia.Hour == 20) && x.AvtoId == Avto).Distinct().ToList();

                            AutoScansSverka A = new AutoScansSverka();
                        A.Name = ASS[0].Name;
                        A.DateSnyatia = ASS[0].DateSnyatia;
                        A.AvtoId = Avto;
                        foreach (AutoScansSverka AS in ASS)
                            {
                            A.DUT += AS.DUT;
                            A.KM += AS.KM;
                            A.MaxSpeed += AS.MaxSpeed;
                            A.Sliv += AS.Sliv;
                            A.Zapravleno += AS.Zapravleno;
                        }

                            
                            ASSdb.Add(A);
                        }
                        catch
                        {

                        }
                    }
                //}
            }
            ASSdb = ASSdb.OrderBy(b => b.AvtoId).ToList();
            return View(ASSdb);
        }

        public ActionResult ExportToExcelJquery(List<List<string>> selection)
        {
            List<List<string>> Table = selection;
            DateTime Date=DateTime.Now;
            string Avto = Table[0][0] +Table[0][2];
           
            string Path = Server.MapPath("~/Content/Avtootchet"+Avto+".xlsx");
            string Path2 = Url.Content("~/Content/Avtootchet"+Avto+".xlsx");
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
                    string TT = Table[i][j].Replace("  ", "").Replace("\n","");
                    WS.Cells[i + 1, j + 1] = TT;

                    if (Table[i][j].Equals("")) { mer++;
                        if (from == 1) { from = i + 1; }
                    }

                }
                //если пустые ячейки в строке занимают более половины то объединяем их
                
                    string F = Opr.OpredelenieBukvi(tek);
                    string T = Opr.OpredelenieBukvi(max);
                    range = WS.get_Range(F + (i+1).ToString(), T + (i+1).ToString());
                    range.Merge();
                    range.EntireRow.AutoFit();
                    range.WrapText = true;
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            }

            for (int i=1;i<Table[0].Count;i++)
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

        public static void ExportToExcelAvtoMonth(List<List<string>> Table, DateTime Date, string Path)
        {
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо

            int progress = 0;
            for (int i = 0; i < Table.Count; i++)
            {
                progress = Convert.ToInt16(50+Convert.ToDecimal(i) / Table.Count * 50);
                ProgressHub.SendMessage("Загружено...", progress);
                for (int j = 0; j < Table[i].Count; j++)
                {
                    WS.Cells[i + 1, j + 1] = Table[i][j];
                    WS.Cells[i + 1, j + 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    if (i == 0) {
                        range = WS.Cells[i + 1, j + 1];
                        range.Font.Bold = true;
                        range.Font.Size = 10;
                        range.RowHeight = 70;//высота строки
                        range.WrapText = true;
                        
                    }
                    if (j==0)
                    {
                        range = WS.Cells[i + 1, j + 1];
                        range.Font.Bold = true;
                        range.Font.Size = 10;
                        range.ColumnWidth = 15;//ширина строки
                        range.WrapText = true;
                    }

                   
                }

            }

            // Сохранение файла Excel.
            
            WbExcel.SaveCopyAs(Path);//сохраняем в папку


                                          // Закрытие книги.
            WbExcel.Close(false, "", Type.Missing);
            // Закрытие приложения Excel.
            
            ApExcel.Quit();

            // WbExcel = null;
            //  WS = null;
            //  ApExcel = null;
            Marshal.FinalReleaseComObject(WbExcel);
            Marshal.FinalReleaseComObject(WB);
            Marshal.FinalReleaseComObject(ApExcel);
           
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


        public ActionResult IndexOtchet(string selection)
        { //Avtomobil;Month;Year
            bool AV = false;
            bool MO = false;
            bool GLONASS = true;
            int Avto = 9999;
            int Month = 1;
            int Year = DateTime.Now.Year;
            List<Ezdka> ezdkas = new List<Ezdka>();
            int ZimaLeto = 1;
           
            if (selection != null && selection != "")
            {
                string[] s = selection.Split(';');
                 Avto = Convert.ToInt16(s[0]);
                 Month = Convert.ToInt16(s[1]);
                 Year = Convert.ToInt16(s[2]);
                 GLONASS = Convert.ToBoolean(s[3]);
                HttpContext.Response.Cookies["Month"].Value = Opr.MonthOpred(Month);//Opr.MonthOpred(Convert.ToInt16(Month));
                HttpContext.Response.Cookies["Month"].Name = "Month";
                HttpContext.Response.Cookies["Month"].Expires = DateTime.Now.AddDays(1);

                //если зима то 2 иначе 1
                if (Month == 11 || Month == 12 || Month == 1 || Month == 2 || Month == 3)
                {
                    ZimaLeto = 2;
                }
                AV = true;
                MO = true;
                if (Avto == 9999){AV = false; }//авто 9999 это все авто
                if (Month==13)   {MO = false; }//Месяц 13 это весь год

                if (!AV&&MO)
                {
                     ezdkas = db.Ezdkas.Include(e => e.Avto).Include(t=>t.Avto.Marka).Where(f => f.Date.Year == Year && f.Date.Month == Month).OrderBy(c => c.Date).ToList();
                  
                }
                if (AV&&MO)
                {
                    ezdkas = db.Ezdkas.Include(e => e.Avto).Include(t => t.Avto.Marka).Where(f => f.Date.Year == Year && f.Date.Month == Month&&f.AvtoId==Avto).OrderBy(c => c.AvtoId).ToList();
                   
                }
                if (AV&&!MO)
                {
                    ezdkas = db.Ezdkas.Include(e => e.Avto).Include(t => t.Avto.Marka).Where(f => f.Date.Year == Year&& f.AvtoId == Avto).OrderBy(c => c.AvtoId).ToList();
                   
                }
                if (!AV && !MO)
                {
                    //если ничего не выбрано
                    ezdkas = db.Ezdkas.Include(e => e.Avto).Include(t => t.Avto.Marka).Where(f => f.Date.Year == Year).OrderBy(c => c.Date).ToList();
                  
                }
               
            }
            else
            {
                if (Request.Cookies["Month"] != null)
                {
                    Month = Opr.MonthObratno(Request.Cookies["Month"].Value);
                }

                //если ничего не выбрано
                if (Month == 11 || Month == 12 || Month == 1 || Month == 2 || Month == 3)
                {
                    ZimaLeto = 2;
                }

                ezdkas = db.Ezdkas.Include(e => e.Avto).Include(t => t.Avto.Marka).Where(f => f.Date.Year == Year && f.Date.Month == Month).OrderBy(c => c.AvtoId).ToList();
                
            }

            //если выбран глонас то выводим только глонассовские машины
            if (GLONASS)
            {
                //суммарные суммы
                int ProbegSS = 0;
                decimal NightUsadkaS = 0;
                decimal SlivkiS = 0;
                decimal ProbegGloSS = 0;
                decimal RashodSS = 0;
                decimal RashodGloSS = 0;
                decimal RashodDutSS = 0;
                decimal ZapravkiPoGloSS = 0;
                decimal Norma = 0;
                int EzdkiSS = 0;
                decimal EzdkiNormsSS = 0;
                int RaznicaVProbegahS = 0;
                int RaznicaDutZapravkaS = 0;
                int AZSLitSS = 0;
                int AZSSumSS = 0;
                decimal OstatokSS = 0;
                decimal OstatokGloSS = 0;
                int TochnostSS = 0;
                List<List<string>> ExportExcelMain = new List<List<string>>();
                ezdkas = ezdkas.Where(x => x.Avto.Glonass == true).ToList();

                //если выбраны все авто то делаем список на каждую тачку
              
                    //автоотчет содержит данные через ; Номер Marka пробегОДО пробегГЛО Разница РасходПоОдо расходПоГло расходпоДУТ заправки РазницаДутиЗАП суммаЗапр
                    List<string> AvtoOtchet = new List<string>();
                    List<string> VseAvtoOtchets = new List<string>();
                ezdkas.OrderBy(m => m.Date);
                List<int> EAvtoNumbers = ezdkas.Select(v => v.Avto.Id).Distinct().ToList();
                //Берем месячную выписку по всем записям автоскана для ускорения поиска по ДБ
                List<AutoScan> ASMonthdb = db.AutoScans.Where(n => n.Date.Year == Year && n.Date.Month == Month).ToList();
                //Берем месячную выписку по всем записям заправок для ускорения поиска по ДБ
                List<Zapravka> Zdb = db.Zapravkas.Where(n => n.Date.Year == Year && n.Date.Month == Month).ToList();

                List<decimal> ASSliv = new List<decimal>();//сливы
                List<decimal> ASProbeg = new List<decimal>();//суммарный пробег авто в автоскане за весь месяц (не привязан к ездкам)
                List<decimal> ASDut = new List<decimal>();//суммарный пробег авто в автоскане за весь месяц (не привязан к ездкам)
                List<decimal> ASRashod = new List<decimal>();//расчет потраченых литров по норме (не учтенных в ездках)
                List<int> AZSLiters = new List<int>();//расчет заправленных литров (не учтенных в ездках)
                List<decimal> AZSSumma = new List<decimal>();//расчет потраченной суммы в рублях (не учтенных в ездках)
                List<int> ASCounter = new List<int>();//считаем сколько записей по факту и сколько по ездкам 
                List<int> AZSCounter = new List<int>();//считаем сколько записей по факту и сколько по ездкам 
                List<string> zagolovki = new List<string>();
                zagolovki.Add("Наименование");//1
                zagolovki.Add("Пробег по одометру ");//2Пробег по одометру
                zagolovki.Add("Пробег по Глонасс ");//3Пробег по Глонасс
                zagolovki.Add("Разница в пробегах ОДО/Глонасс(%)");//4Разница в пробегах ОДО/Глонасс%
                zagolovki.Add("Количество ездок ");//5Количество ездок
                zagolovki.Add("Расход на ездки (л.)");//6Потрачено по норме на ездки (л.)
                zagolovki.Add("Расчётный расход по одометру + ездки (л.)"); //7Расчётный расход (по одометру л.)
                zagolovki.Add("Расчётный расход по Глонасс + ездки (л.)"); //8Расчётный расход (по Глонасс л.)
                zagolovki.Add("Фактический расход по ДУТ Глонасс (л.)"); //9Фактический расход (по ДУТ Глонасс л.)
                zagolovki.Add("Ночная усадка Глонасс (л.)"); //9Фактический расход (по ДУТ Глонасс л.)
                zagolovki.Add("Сливы Глонасс (л.)"); //9Фактический расход (по ДУТ Глонасс л.)
                zagolovki.Add("Объём заправки АЗС(л.)"); //10Объём заправки(л.)
                zagolovki.Add("Остаток(л.)"); //11Остаток(л.)
                zagolovki.Add("Разница между ДУТ и заправками(%)");//12Разница между ДУТ и заправками(%)
                zagolovki.Add("Сумма заправки(р.)");//13Сумма заправки(р.)
                zagolovki.Add("Точность");//14Точность
                ExportExcelMain.Add(zagolovki);

               int progress = 0;

                int cc = 0;
                List<List<string>> XX = new List<List<string>>();                                //Для каждой машины в списке машин
                foreach (int I in EAvtoNumbers)
                    {
                    cc++;
                    progress = Convert.ToInt16(Convert.ToDecimal(cc) / EAvtoNumbers.Count * 50);
                    ProgressHub.SendMessage("Загружено...", progress);
                    List<string> ExportExcel = new List<string>();//для экспорта в эксель
                                                                  //Забили айдишник авто
                    Avtomobil A = db.Avtomobils.Where(c => c.Id == I).Include(v => v.Marka).Include(n=>n.Type).Single();
                        string AVTONUMBER = A.Number.ToUpper().Replace(" ", "").Replace("-","");//номер найденного авто
                        int AVTOID = A.Id;
                        //Ищем все ездки с данной машиной
                        List<Ezdka> Edb = ezdkas.Where(b => b.AvtoId == I).OrderBy(m => m.Date).ToList();
                    //считаем сумму пробега по автоскану для данной машины
                    //по номеру было но уже по ИД быстрее   List<AutoScan> ASAvto = ASMonthdb.Where(y => y.Name.ToUpper().Contains(AVTONUMBER)).ToList();
                    List<AutoScan> ASAvto = ASMonthdb.Where(y => y.AvtoId==AVTOID).ToList();
                    decimal OstatokVBake = 0;
                    decimal OstatokGloVBake = 0;
                    decimal ZapravkiPoGloS = 0;
                    //ищем норму расхода из модели авто
                    decimal Ras = 0;//норма расхода
                    if (ZimaLeto == 1) { Ras = A.Marka.SNorm; }
                    else { Ras = A.Marka.WNorm; }

                    //Суммируем дут и пробег и расход по автоскану за месяц
                    
                    decimal probegAS = 0;//пробег за весь месяц только по АС
                    decimal dutAS = 0;
                    decimal rashodAS = 0;
                    bool KMMOTO = A.Marka.KmMoto;// у тачкикилометраж или моточасы?
                    int AScou = 0;
                    ASCounter.Add(AScou);
                    List<int> ASEzdilDateDay = new List<int>();
                    
                    foreach (AutoScan ASA in ASAvto)
                    {
                        ZapravkiPoGloS += ASA.Zapravleno;//сумма всех заправок у авто за месяц
                        decimal ASAdd = 0;
                        if (KMMOTO) { ASAdd = ASA.KM; }
                        else { ASAdd = ASA.MotoHours.Hour; }

                        probegAS += ASAdd;
                        if (ASAdd+ASA.DUT > 0) {
                            AScou++;
                            ASEzdilDateDay.Add(ASA.Date.Day);
                        }
                        dutAS += ASA.DUT;   
                    }
                    ASEzdilDateDay.Sort();
                    //
                    if (KMMOTO) { rashodAS = Ras / 100 * probegAS; }
                    else { rashodAS = Ras * probegAS; }
                    ASDut.Add(dutAS);
                    ASProbeg.Add(probegAS);
                    ASRashod.Add(rashodAS);

                    //считаем сумму по заправке у тачки за месяц (без учета ездок)
                    List<Zapravka> ZAMonth = Zdb.Where(e => e.AvtoNumber.Replace(" ", "").Contains(AVTONUMBER)).ToList();//список всех заправок у этого авто за месяц
                    decimal summAZS = 0;
                    int litersAZS = 0;
                    int AZScou = ZAMonth.Count();//всего заправок в месяц
                    AZSCounter.Add(AZScou);
                    List<int> AZSDateDay = new List<int>();
                    foreach (Zapravka Zap in ZAMonth)
                    {
                        summAZS += Zap.Summa;
                        litersAZS += Zap.Liters;
                        AZSDateDay.Add(Zap.Date.Day);

                    }
                    AZSDateDay.Sort();
                    AZSLiters.Add(litersAZS);
                    AZSSumma.Add(summAZS);

                        int ProbegS = 0;
                        decimal ProbegGloS = 0;
                        decimal RashodS = 0;
                        decimal RashodGloS = 0;
                        decimal RashodDutS = 0;
                        int     EzdkiNumberS = 0;
                        decimal EzdkiToplivoS = 0;
                        int RaznicaVProbegah = 0;
                        int RaznicaDutZapravka = 0;
                        int AZSLitS = 0;
                        int AZSSumS = 0;
                        int counter = 0;
                        string KMMH = "Км.";
                        string sss = "";
                    List<int> Days = new List<int>();
                    List<int> DaysAZS = new List<int>();
                    int day = 0;
                    int co = 0;//счетчик ездок
                    foreach (Ezdka J in Edb)
                        {
                        co++;
                        int     EzdNumb = 0;
                        decimal Norm = 0;
                        bool    EzdkaPricep = false;
                        decimal EzdToplivo = 0;
                        decimal P = 0;
                        decimal PG = 0;
                        decimal R = 0;
                        decimal RG = 0;
                        decimal RD = 0;
                        decimal RVP = 0;
                        decimal RVZ = 0;
                        decimal ZapravkiPoGlo = 0;
                        int AZSL = 0;
                        int AZSS = 0;
                        
                        counter++;
                        
                        AutoScan AS = new AutoScan();
                        //ищем запись с этой тачкой в ДБ автоскана (если уже есть такой день значит 2 ездки за день а в автоскане 1 запись за день. Не берем повторно!)
                        if (Days.Contains(J.Date.Day) == false)
                        {
                           
                            try
                            {
                                //из найденного авто ищем записи по дню
                                AS = ASAvto.Where(n => n.Date.Day == J.Date.Day).Single();
                                Days.Add(J.Date.Day);

                            }
                            catch
                            {

                            }
                        }
                        else
                        {

                        }
                        //ищем запись с этой тачкой в ДБ АЗС
                        Zapravka Z = new Zapravka();
                        //ищем запись с этой тачкой в ДБ заправок (если уже есть такой день значит 2 ездки за день а в автоскане 1 запись за день. Не берем повторно!)
                        if (DaysAZS.Contains(J.Date.Day) == false)
                        {
                            try
                            {
                                Z = ZAMonth.Where(n => n.Date.Day == J.Date.Day).Single();
                            }
                            catch { }
                            AZSL = Z.Liters;
                            AZSS = Z.Summa;
                            DaysAZS.Add(J.Date.Day);

                        }
                        else
                        {

                        }

                        decimal ZapravkaGlo = AS.Zapravleno;
                        if (AS.AvtoId==52)
                        {

                        }

                            P = J.Probeg;
                            RD = Convert.ToInt32(AS.DUT);
                            ProbegS += J.Probeg;//суммируем пробег

                        if (AS.Sliv > 1)
                        {
                            RashodDutS += AS.Start+AS.Zapravleno-AS.End-AS.Sliv;
                        }
                        else
                        {
                            RashodDutS += AS.DUT;//расход по ДУТ
                        }
                            AZSLitS += AZSL;
                            AZSSumS += AZSS;
                            if (KMMOTO)
                            {
                            KMMH = "Км.";
                                R =Ras / 100 * J.Probeg;
                                PG = AS.KM;
                                RG = Ras / 100 * AS.KM;
                                RashodS += Ras / 100 * J.Probeg;
                                ProbegGloS += AS.KM;
                                RashodGloS += Ras / 100 * AS.KM;

                            }
                            else
                            { //если считаем по моточасам 
                            KMMH = "Мч.";
                                R = Convert.ToInt16(Ras * J.Probeg);
                                PG = AS.MotoHours.Hour+ Convert.ToDecimal(AS.MotoHours.Minute)/60;
                                RG = Convert.ToInt16(Ras * AS.MotoHours.Hour+Ras/60*AS.MotoHours.Minute);
                                RashodS += Ras * J.Probeg;
                                ProbegGloS += AS.MotoHours.Hour+Convert.ToDecimal(AS.MotoHours.Minute)/60;
                                RashodGloS += Ras * AS.MotoHours.Hour+Ras/60*AS.MotoHours.Minute;//расход за час + минут
                            }
                        
                        if (P != 0&&PG!=0)
                        {
                            if (PG < P)
                            {
                                RVP = Convert.ToInt16(Math.Abs(100 - PG / P * 100));
                            }
                            else
                            {
                                RVP = Convert.ToInt16(Math.Abs(100 - P / PG * 100));
                            }
                        }
                        if (RD != 0&&AZSL!=0)
                        {
                            if (AZSL < RD)
                            {
                                RVZ = Convert.ToInt16(Math.Abs(100 - Convert.ToDecimal(AZSL) / RD * 100));
                            }
                            else
                            {
                                RVZ = Convert.ToInt16(Math.Abs(100 - Convert.ToDecimal(RD) /AZSL * 100));
                            }
                        }

                        //считаем остаток в баке по показаниям водителей
                        OstatokVBake -= R;
                        OstatokVBake += AZSL;
                        OstatokGloVBake -= RG;
                        OstatokGloVBake += AZSL;
                        int T = 0;
                        int TT = 0;
                        if (RVP > 15) { T = 1; }
                        if (RVZ > 20) { TT = 1; }
                        EzdNumb = Convert.ToInt16(J.Ezdki);
                        Norm = A.Marka.NormaEzdka;
                        EzdkaPricep = A.Marka.EzdkaPricep;
                        
                        if (EzdkaPricep)//если ездка то
                        {
                        
                                EzdToplivo = Norm * EzdNumb;
                      
                        }
                        else //если прицеп то
                        {
                            if (KMMOTO)
                            {
                               
                                EzdToplivo = Ras / 100 * Norm;
                                
                            }
                            else
                            {
                                EzdToplivo = Ras * Norm;
                            }
                        }
                        EzdkiToplivoS += EzdToplivo;
                        EzdkiNumberS += EzdNumb;
                        OstatokVBake -= EzdToplivo;
                        OstatokGloVBake -= EzdToplivo;
                        decimal SlivGlo = AS.Sliv;
                        decimal NightGlo = 0;
                        if (Days.Count>1)
                        {
                            decimal End = ASAvto.Where(n => n.Date.Day == Days[Days.Count-2]).Select(f=>f.End).Single();
                            NightGlo = End - AS.Start;
                        }
                        //                  0Пробег                                   1ПробегГло                                2разницаПробеги                       3расходОДО                             4расходГЛО                               5расходДУТ                           6Заправка             7РазницаДутАЗС             8СуммаЗапр                 9День                     10Водитель  11НесоответствиеПробегов 12НесоответствиеЗаправок 13Кол-воЕздок                      14Кол-воТоплива                          15ОстатокВБаке                          16ОстатокГлоВБаке                                   17ЗаправкаГло                 18СливГло                19УсадкаГло
                        string ss = Math.Round(P, 1).ToString() + KMMH + ";" + Math.Round(PG, 1).ToString() + KMMH + ";" + Math.Round(RVP, 2).ToString() + "%;" + Math.Round(R, 2).ToString() + "л.;" + Math.Round(RG, 2).ToString() + "л.;" + Math.Round(RD, 2).ToString() + "л.;" + AZSL.ToString() + "л.;" + RVZ.ToString() + "%;" + AZSS.ToString() + "р.;" + J.Date.Day.ToString() + ";" + J.Voditel + ";" + T.ToString() + ";" + TT.ToString() + ";" + EzdNumb.ToString() + ";" + Math.Round(EzdToplivo).ToString() + "л.;" + Math.Round(OstatokVBake).ToString() + "л.;" + Math.Round(OstatokGloVBake).ToString() + "л.;" + ZapravkaGlo.ToString() + "л.;"+SlivGlo.ToString()+"л.;"+NightGlo.ToString()+"л." ;
                        sss += ss + "!";
                    }
                    VseAvtoOtchets.Add(sss);
                    //а теперь считаем пропорции для каждой тачки
                    if (ProbegS != 0&&ProbegGloS!=0)
                    {
                        if (ProbegS > ProbegGloS)
                        {
                            RaznicaVProbegah = Convert.ToInt16(Math.Abs(100 - ProbegGloS / ProbegS * 100));
                        }
                        else
                        {
                            RaznicaVProbegah = Convert.ToInt16(Math.Abs(100 - ProbegS/ ProbegGloS * 100));
                        }
                    }
                    if (RashodDutS != 0&& AZSLitS!=0)
                    {
                        if (RashodDutS > AZSLitS)
                        {
                            RaznicaDutZapravka = Convert.ToInt16(Math.Abs(100 - AZSLitS / RashodDutS * 100));
                        }
                        else
                        {
                            RaznicaDutZapravka = Convert.ToInt16(Math.Abs(100 -  RashodDutS/ AZSLitS* 100));
                        }
                    }
                    //сравниваем пробеги( если погрешность более 5% то подсвечиваем значит авто выезжало без спроса)
                    if (ProbegGloS!=0&&probegAS!=0&&ProbegGloS/probegAS > 0.95m) { probegAS = ProbegGloS; }
                    if (RashodGloS!=0&&rashodAS!=0&&RashodGloS / rashodAS > 0.95m) { rashodAS = RashodGloS; }
                    if (RashodDutS!=0&&dutAS!=0&&RashodDutS / dutAS > 0.95m) { dutAS = RashodDutS; }
                    decimal RashodAndEzdka = RashodS + EzdkiToplivoS;
                    decimal RashodGloAndEzdka = RashodGloS + EzdkiToplivoS;
                    AScou = DateTime.DaysInMonth(Year,Month)-AScou;
                    int GloTochnost = 0;
                    // if (AScou != 0) { GloTochnost = Convert.ToInt16(Math.Abs(100 - Convert.ToDecimal(AScou) / DateTime.DaysInMonth(Year, Month) * 100)); }
                    List<int> EzdkaDateDay = new List<int>();
                    foreach (Ezdka E in Edb)
                    {
                        EzdkaDateDay.Add(E.Date.Day);
                    }
                    EzdkaDateDay.Sort();//дни в которые были ездки

                    string NetEzdokVDni = "";
                    string NetEzdokLiters = "";

                    string ASNetEzdokVDni = "";
                    string ASNetEzdokKM = "";
                    string ASNetEzdokDUT = "";
                    string ASNetEzdokVDniDUT = "";

                    List<int> NetEzdok = AZSDateDay.Except(EzdkaDateDay).ToList();//дни в которые нет ездок но есть заправки
                    List<int> ASNetEzdok = ASEzdilDateDay.Except(EzdkaDateDay).ToList();//дни в которые нет ездок но есть пробег по Глонасс
                    List<int> ASToch = ASNetEzdok.ToList();
                    
                    //ищем дни в которых нет ездки но есть пробег по глонасс
                    if (ASNetEzdok.Count > 0)
                    {
                        foreach (int F in ASNetEzdok)
                        {
                            AutoScan AS = ASAvto.Where(d => d.Date.Day == F).First();
                             bool KMOT = false;

                                KMOT = db.Avtomobils.Where(g => g.Number.Equals(AS.Name)).Include(p => p.Marka.KmMoto).Select(f => f.Marka.KmMoto).First();
                            int prob = 0;
                            if (KMOT)
                            {
                                if (AS.KM > 0)
                                {

                                    ProbegGloS += AS.KM;

                                    if (AS.KM > 1)
                                    {
                                        ASNetEzdokVDni += F.ToString() + "|";
                                        ASNetEzdokKM += Math.Round(AS.KM, 2).ToString() + "|";
                                        prob++;
                                    }

                                }
                            }
                            else
                            {
                                if (AS.MotoHours.Hour > 0)
                                {
                                    ProbegGloS += AS.MotoHours.Hour+Convert.ToDecimal(AS.MotoHours.Minute)/60;
                                    ASNetEzdokVDni += F.ToString() + "|";
                                    ASNetEzdokKM += AS.MotoHours.Hour.ToString() + "|";
                                    prob++;
                                }

                            }
                            if (AS.DUT > 0)
                            {
                               
                                RashodDutS += AS.DUT;
                                if (AS.DUT > 1)
                                {
                                    ASNetEzdokVDniDUT += F.ToString() + "|";
                                    ASNetEzdokDUT += Math.Round(AS.DUT, 2).ToString() + "|";
                                    prob++;
                                }

                            }
                            if (prob==0)
                            {
                                ASToch.Remove(F);
                            }
                        }
                        //удаляем последний символ |
                        if (ASNetEzdokVDni.Length > 0)
                        {
                            ASNetEzdokVDni = ASNetEzdokVDni.Remove(ASNetEzdokVDni.Length - 1);
                            ASNetEzdokKM = ASNetEzdokKM.Remove(ASNetEzdokKM.Length - 1);
                        }
                        if (ASNetEzdokVDniDUT.Length > 0)
                        {
                            ASNetEzdokVDniDUT = ASNetEzdokVDniDUT.Remove(ASNetEzdokVDniDUT.Length - 1);
                            ASNetEzdokDUT = ASNetEzdokDUT.Remove(ASNetEzdokDUT.Length - 1);
                        }
                    }

                   

                    //расчет ночной погрешности дут
                    List<AutoScan> AS2 = ASAvto.OrderBy(d=>d.Date).ToList();
                    decimal DUTMOD = 0;
                    decimal start = 0;
                    decimal end = 0;
                    string ASSlivki = "";
                    string ASSlivkiDays = "";
                    decimal NightSumm = 0;

                    decimal SlivkiSumm = 0;
                    if (AS2.Count > 0) { SlivkiSumm = AS2[0].Sliv; };
                    for (int a=1; a<AS2.Count;a++)
                    {
                        start = AS2[a].Start;
                        end = AS2[a - 1].End;
                        SlivkiSumm += AS2[a].Sliv;
                        NightSumm += end - start;
                        if (AS2[a - 1].Sliv == 0)
                        {
                            start = AS2[a].Start;
                            end = AS2[a - 1].End;
                            DUTMOD += end - start;
                        }
                        else
                        {
                            ASSlivki += AS2[a - 1].Sliv.ToString()+"|";
                            ASSlivkiDays += AS2[a - 1].Date.Day.ToString() + "|";
                           

                        }
                    }
                    if (ASSlivki.Length > 0)
                    {
                        ASSlivki = ASSlivki.Remove(ASSlivki.Length - 1);
                        ASSlivkiDays = ASSlivkiDays.Remove(ASSlivkiDays.Length - 1);
                    }
                    //Дутмод если нужно
                    //RashodDutS += DUTMOD;
                   // dutAS += DUTMOD;
                    string tochnostDni = "";
                    foreach (int i in ASToch)
                    {
                        tochnostDni += i.ToString() + "|";
                    }
                    if (tochnostDni.Length > 0)
                    {
                        tochnostDni = tochnostDni.Remove(tochnostDni.Length - 1);
                    }

                    
                    if (ASToch.Count != 0) { GloTochnost = Convert.ToInt16(Math.Abs(100 - Convert.ToDecimal(ASToch.Count) / ASEzdilDateDay.Count * 100)); }
                    else {GloTochnost=100; }
                    if (TochnostSS == 0) { TochnostSS = GloTochnost;}
                    else
                    {
                        TochnostSS = (TochnostSS + GloTochnost) / 2;
                    }
                    //1ОДО расход, 2ДУТ расход 3Глонасс расход
                    int Naimenshee = 1;
                    if (RashodAndEzdka>RashodGloAndEzdka)
                    {
                        Naimenshee = 3;
                        if (GloTochnost > 90)
                        {
                            if(RashodDutS< RashodGloAndEzdka)
                            {
                                Naimenshee = 2;
                            }
                        }
                    }
                    else
                    {
                        if (GloTochnost > 90)
                        {
                            if (RashodAndEzdka > RashodDutS)
                            {
                                Naimenshee = 2;
                            }
                        }
                    }
                    //1Наименование
                    //2Пробег по одометру
                    //3Пробег по Глонасс
                    //4Разница в пробегах ОДО/Глонасс%
                    //5Количество ездок
                    //6Потрачено по норме на ездки (л.)
                    //7Расчётный расход (по одометру л.)
                    //8Расчётный расход (по Глонасс л.)
                    //9Фактический расход (по ДУТ Глонасс л.)
                    //10Объём заправки(л.)
                    //11Остаток(л.)
                    //12Разница между ДУТ и заправками(%)
                    //13Сумма заправки(р.)
                    //14Точность
                    ExportExcel.Add(AVTONUMBER+"|"+ A.Marka.Name);//1Наименование
                    ExportExcel.Add(Convert.ToInt32(ProbegS).ToString());//2Пробег по одометру
                    ExportExcel.Add(Math.Round(ProbegGloS,1).ToString());//3Пробег по Глонасс
                    ExportExcel.Add(RaznicaVProbegah.ToString());//4Разница в пробегах ОДО/Глонасс%
                    ExportExcel.Add(EzdkiNumberS.ToString());//5Количество ездок
                    ExportExcel.Add(Math.Round(EzdkiToplivoS).ToString());//6Потрачено по норме на ездки (л.)
                    ExportExcel.Add(Math.Round(RashodAndEzdka,3).ToString()); //7Расчётный расход (по одометру л.)
                    ExportExcel.Add(Math.Round(RashodGloAndEzdka,3).ToString()); //8Расчётный расход (по Глонасс л.)
                    ExportExcel.Add(Math.Round(RashodDutS,3).ToString()); //9Фактический расход (по ДУТ Глонасс л.)
                    ExportExcel.Add(Math.Round(NightSumm, 3).ToString()); //Усадка
                    ExportExcel.Add(Math.Round(SlivkiSumm, 3).ToString()); //Сливы
                    ExportExcel.Add(AZSLitS.ToString()); //10Объём заправки(л.)
                    ExportExcel.Add(Math.Round(OstatokVBake).ToString()); //11Остаток(л.)
                    ExportExcel.Add(RaznicaDutZapravka.ToString());//12Разница между ДУТ и заправками(%)
                    ExportExcel.Add(AZSSumS.ToString());//13Сумма заправки(р.)
                    ExportExcel.Add(GloTochnost.ToString());//14Точность


                   
                    //ищем дни в которых нет ездки но есть заправка
                    if (NetEzdok.Count > 0)
                    {
                        foreach (int F in NetEzdok) {
                            Zapravka NE = ZAMonth.Where(d => d.Date.Day == F).First();
                            NetEzdokVDni += F.ToString() + "|";
                            NetEzdokLiters+= NE.Liters + "|"; 
                        }
                        //удаляем последний символ |
                        NetEzdokVDni = NetEzdokVDni.Remove(NetEzdokVDni.Length - 1);
                        NetEzdokLiters = NetEzdokLiters.Remove(NetEzdokLiters.Length - 1);
                    }
                    decimal Nedoliv = AZSLitS - ZapravkiPoGloS;
                    XX.Add(ExportExcel);
                    //сохраняем все через запятую 0Номер 1Марка           2Кол-воЕздок               3Пробег                                                 4ПробегГло                                   5разницаПробеги                                6расходОДО                                   7расходГЛО                                          8расходДУТ                             9Заправка                        10РазницаДутАЗС                  11СуммаЗапр              12ТипАвто             13ДутФакт                                   14пробегФакт                                      15РасходФакт                                  16АЗСлитрФакт                                    17АЗССуммФакт                   18РеалДней            19ГлоДней              20АЗСДней               21ЕздкиКолво                     22ЕздкиТопливо                            23Расход и ездка                          24Расход+ездка гло                            25ОстатокВБакеОДО                       26ТочностьГлонасс                27 Наименьшее              28Норма на ездку                     30Остатоквбаке ГЛО                  31Нет ездок в дни с разделителем| 32 Нет ездок литры|33 Нет ездок по Глонасс|34 Нет ездок по глонасс Пробег|35 Нет ездок по в дни ДУТ|36 Нет ездок ДУТ|37 ТочностьДни 38Модификатор ДУТ 39Сливки дни 40 Сливки объём           41СуммаСливок           42СуммаУсадкиНочь             43ЗаправкиПоГлонасс      44Недолив
                    string s = AVTONUMBER + ";" + A.Marka.Name + ";" + counter.ToString() + ";" + Convert.ToInt32(ProbegS).ToString()+KMMH + ";" + Math.Round(ProbegGloS,1).ToString() +KMMH+ ";" + RaznicaVProbegah.ToString() + "%;" + Convert.ToInt32(RashodS).ToString() + "л.;" + Convert.ToInt32(RashodGloS).ToString() + "л.;" + Convert.ToInt32(RashodDutS).ToString() + "л.;" + AZSLitS.ToString() + "л.;" + RaznicaDutZapravka.ToString() + "%;" + AZSSumS.ToString()+"р.;" + A.Type.Type+";"+Convert.ToInt32(dutAS).ToString()+"л.;"+ Convert.ToInt32(probegAS).ToString() + KMMH + ";"+ Convert.ToInt32(rashodAS).ToString()+"л.;"+ Convert.ToInt32(litersAZS).ToString() + "л.;"+ Convert.ToInt32(summAZS).ToString()+"р.;"+counter.ToString()+";"+AScou.ToString()+";"+AZScou.ToString()+";"+EzdkiNumberS.ToString()+";"+ Math.Round(EzdkiToplivoS).ToString()+";"+ Math.Round(RashodAndEzdka).ToString()+";"+ Math.Round(RashodGloAndEzdka).ToString()+";"+Math.Round(OstatokVBake).ToString()+"л.;"+";"+GloTochnost.ToString()+"%;"+Naimenshee.ToString()+";"+A.Marka.NormaEzdka.ToString()+";"+ Math.Round(OstatokGloVBake).ToString() + "л.;"+NetEzdokVDni+";"+NetEzdokLiters+";"+ASNetEzdokVDni+";"+ASNetEzdokKM + ";" + ASNetEzdokVDniDUT + ";" + ASNetEzdokDUT+";"+tochnostDni+";"+Math.Round(DUTMOD,2).ToString()+";"+ASSlivkiDays.ToString()+";"+ ASSlivki.ToString()+";"+SlivkiSumm.ToString()+";"+NightSumm.ToString()+";"+ZapravkiPoGloS.ToString()+";"+Nedoliv.ToString();
           
                    AvtoOtchet.Add(s);
                    NightUsadkaS += NightSumm;
                    SlivkiS += SlivkiSumm;
                    OstatokSS += OstatokVBake;
                    OstatokGloSS += OstatokGloVBake;
                    RashodDutSS += RashodDutS;
                    AZSSumSS += AZSSumS;
                    AZSLitSS += AZSLitS;
                    RashodSS += RashodAndEzdka;
                    RashodGloSS += RashodGloAndEzdka;
                    ProbegGloSS += ProbegGloS;
                    ProbegSS += ProbegS;
                    EzdkiNormsSS += EzdkiToplivoS;
                    EzdkiSS += EzdkiNumberS;
                    }
                zagolovki = new List<string>();
                zagolovki.Add("Итого");//1
                zagolovki.Add(ProbegGloSS.ToString());//2Пробег по одометру
                zagolovki.Add(ProbegSS.ToString());//3Пробег по Глонасс
                int RaznPro = 0;
                int RaznDUTAZS = 0;
                if (ProbegGloSS * ProbegSS == 0) { RaznPro = 100; }
                else
                {
                    if (ProbegGloSS < ProbegSS) { RaznPro = Convert.ToInt32(100 - ProbegGloSS / ProbegSS * 100); }
                    else
                    {
                        RaznPro = Convert.ToInt32(100 - ProbegSS / ProbegGloSS * 100);
                    }
                }
                if (RashodDutSS * AZSLitSS == 0) { RaznDUTAZS = 100; }
                else
                {
                    if (RashodDutSS < AZSLitSS) { RaznPro = Convert.ToInt32(100 - RashodDutSS / AZSLitSS * 100); }
                    else
                    {
                        RaznPro = Convert.ToInt32(100 - AZSLitSS / RashodDutSS * 100);
                    }
                }
                zagolovki.Add(RaznPro.ToString());//4Разница в пробегах ОДО/Глонасс%
                zagolovki.Add(EzdkiSS.ToString());//5Количество ездок
                zagolovki.Add(Math.Round(EzdkiNormsSS).ToString());//6Потрачено по норме на ездки (л.)
                zagolovki.Add(Math.Round(RashodSS,3).ToString()); //7Расчётный расход (по одометру л.)
                zagolovki.Add(Math.Round(RashodGloSS,3).ToString()); //8Расчётный расход (по Глонасс л.)
                zagolovki.Add(Math.Round(RashodDutSS,3).ToString()); //9Фактический расход (по ДУТ Глонасс л.)
                zagolovki.Add(Math.Round(NightUsadkaS, 3).ToString()); //Ночная усадка
                zagolovki.Add(Math.Round(SlivkiS, 3).ToString()); //Сливы
                zagolovki.Add(AZSLitSS.ToString()); //10Объём заправки(л.)
                zagolovki.Add(Convert.ToInt32(OstatokSS).ToString()); //11Остаток(л.)
                zagolovki.Add(RaznDUTAZS.ToString());//12Разница между ДУТ и заправками(%)
                zagolovki.Add(AZSSumSS.ToString());//13Сумма заправки(р.)
                zagolovki.Add(TochnostSS.ToString());//14Точность
                ExportExcelMain.Add(zagolovki);
                foreach(List<string> X in XX)
                {
                    ExportExcelMain.Add(X);
                }
                    ViewBag.TochnostSS = TochnostSS;
                    ViewBag.OstatokGloSS = Convert.ToInt32(OstatokGloSS);
                    ViewBag.OstatokSS = Convert.ToInt32(OstatokSS);
                    ViewBag.EzdkiSS = EzdkiSS;
                    ViewBag.EzdkiNormSS = Math.Round(EzdkiNormsSS);
                    ViewBag.VseAvtoOtchets = VseAvtoOtchets;
                    ViewBag.AvtoOtchet = AvtoOtchet;
                    ViewBag.RashodDutS = Math.Round(RashodDutSS,3);
                    ViewBag.AZSSummaS = AZSSumSS;
                    ViewBag.AZSLitersS = AZSLitSS;
                    ViewBag.RashodS = Math.Round(RashodSS,3);
                    ViewBag.RashodGlonassS = Math.Round(RashodGloSS,3);
                    ViewBag.ProbegGlonasS = Math.Round(ProbegGloSS,3);//сумма пробега из расчета ездок Глонасс
                ViewBag.ProbegS = ProbegSS;//сумма пробега из расчета ездок
                    ViewBag.Month = Opr.MonthOpred(Month);
                    ViewBag.Year = Year;
                    ViewBag.ASDut = ASDut;//сумма дут по автоскану
                    ViewBag.ASProbeg = ASProbeg;//сумма пробега по автоскану
                    ViewBag.ASRashod = ASRashod;
                    ViewBag.AZSLiters = AZSLiters;
                    ViewBag.AZSSumma = AZSSumma;
                ViewBag.SlivkiS = SlivkiS;
                ViewBag.NightUsadkaS = NightUsadkaS;
                    DateTime D = new DateTime(Year, Month,1);
           
                string filename = Server.MapPath("~/Content/Avtootchet" + Opr.MonthOpred(Month) + ".xlsx");
                ExportToExcelAvtoMonth(ExportExcelMain, D, filename);
                string path = Url.Content("~/Content/Avtootchet" + Opr.MonthOpred(Month) + ".xlsx");
                ViewBag.Path = path;
                return View();
            }
            else
            {
                //определяем норму расхода
                List<int> Probeg = new List<int>();
                List<decimal> Rashod = new List<decimal>();
                List<decimal> RashodGlonass = new List<decimal>();
                List<bool> KmMh = new List<bool>();
                List<bool> Glonass = new List<bool>();
                List<int> ProbegGlonass = new List<int>();
                List<decimal> RashodDut = new List<decimal>();
                List<int> AZSSummaAll = new List<int>();
                List<int> AZSLitersAll = new List<int>();
                List<int> RaznicaAll = new List<int>();
                //лето 1 зима 2
                foreach (Ezdka E in ezdkas)
                {

                    decimal Ras = 0;
                    if (ZimaLeto == 1) { Ras = E.Avto.Marka.SNorm; }
                    else { Ras = E.Avto.Marka.WNorm; }
                    decimal RasRas = 0;
                    decimal RasDut = 0;
                    decimal RasGlo = 0;
                    decimal ProGlo = 0;
                    int AZSLiter = 0;
                    int AZSSumma = 0;
                    decimal Raznica = 0;
                    int Prob = 0;
                    bool GLO = false;

                    AutoScan AS = new AutoScan();
                    try
                    {
                        string AN = E.Avto.Number.Replace(" ", "").Replace("-", "");
                        List<AutoScan> ASdb = db.AutoScans.Where(g => g.Date.Year == E.Date.Year && g.Date.Month == E.Date.Month && g.Date.Day == E.Date.Day).ToList();
                        foreach (AutoScan S in ASdb)
                        {
                            if (S.Name.Replace(" ", "").ToUpper().Equals(AN))
                            {
                                AS = S;
                                GLO = true;

                                break;
                            }

                        }

                    }
                    catch
                    {
                        GLO = false;

                    }
                    //если считаем по километрам
                    if (E.Avto.Marka.KmMoto)
                    {
                        RasRas = Ras / 100 * E.Probeg;
                        ProGlo = AS.KM;
                        Prob = E.Probeg;
                        if (GLO)
                        {
                            RasGlo = Ras / 100 * AS.KM;
                            RasDut = AS.DUT;
                            if (RasGlo != 0)
                            {
                                decimal R = RasRas / RasGlo * 100;
                                Raznica = Math.Abs(100 - R);
                            }
                        }




                    }
                    else
                    { //если считаем по моточасам 
                        RasRas = Ras * E.Probeg;
                        ProGlo = AS.MotoHours.Hour;
                        Prob = E.Probeg;
                        if (GLO)
                        {
                            RasGlo = Ras * AS.MotoHours.Hour + Ras / 60 * AS.MotoHours.Minute;
                            RasDut = AS.DUT;
                            if (RasGlo != 0)
                            {
                                decimal R = RasRas / RasGlo * 100;
                                Raznica = Math.Abs(100 - R);
                            }
                        }


                    }

                    try
                    {
                        Zapravka Z = db.Zapravkas.Where(c => c.Date.Year == E.Date.Year && c.Date.Month == E.Date.Month && c.Date.Day == E.Date.Day && c.AvtoNumber.Equals(E.Avto.Number)).First();
                        AZSLiter = Z.Liters;
                        AZSSumma = Z.Summa;
                        AZSLitersAll.Add(AZSLiter);
                        AZSSummaAll.Add(AZSSumma);
                    }
                    catch
                    {
                        AZSLitersAll.Add(0);
                        AZSSummaAll.Add(0);
                    }
                    RashodDut.Add(RasDut);
                    KmMh.Add(E.Avto.Marka.KmMoto);
                    Rashod.Add(RasRas);
                    RashodGlonass.Add(RasGlo);
                    ProbegGlonass.Add(Convert.ToInt32(ProGlo));
                    Glonass.Add(GLO);
                    Probeg.Add(Prob);
                    RaznicaAll.Add(Convert.ToInt32(Raznica));
                    //ищем авто по глонасу
                }

                ViewBag.ASdut = 
                ViewBag.RashodDut = RashodDut;
                ViewBag.AZSLiters = AZSLitersAll;
                ViewBag.AZSSumma = AZSSummaAll;
                ViewBag.Rashod = Rashod;
                ViewBag.Raznica = RaznicaAll;
                ViewBag.RashodGlonass = RashodGlonass;
                ViewBag.KmMh = KmMh;
                ViewBag.Glonass = Glonass;
                ViewBag.ProbegGlonass = ProbegGlonass;
                ViewBag.PG = ProbegGlonass;

                decimal RashodDutSumm = 0;
                int AZSSummaSumm = 0;
                int AZSLitersSumm = 0;
                decimal RashodSumm = 0;
                decimal RashodGlonassSumm = 0;
                int ProbegSumma = 0;

                int ProbegGlonassSumm = 0;

                for (int i = 0; i < RashodDut.Count; i++)
                {
                    RashodDutSumm += RashodDut[i];
                    AZSSummaSumm += AZSSummaAll[i];
                    AZSLitersSumm += AZSLitersAll[i];
                    RashodSumm += Rashod[i];
                    RashodGlonassSumm += RashodGlonass[i];
                    ProbegGlonassSumm += ProbegGlonass[i];
                    ProbegSumma += Probeg[i];
                }
                
                ViewBag.RashodDutS = RashodDutSumm;
                ViewBag.AZSSummaS = AZSSummaSumm;
                ViewBag.AZSLitersS = AZSLitersSumm;
                ViewBag.RashodS = RashodSumm;
                ViewBag.RashodGlonassS = RashodGlonassSumm;
                ViewBag.ProbegGlonasS = ProbegGlonassSumm;
                ViewBag.ProbegS = ProbegSumma;
                ViewBag.Month = Month;
                ViewBag.Year = Year;
            }
            return View(ezdkas);
        }
        public ActionResult IndexMain()
        {
           
            return View();
        }
        public ActionResult IndexMenu()
        {

            List<Avtomobil> Avtodb = db.Avtomobils.OrderBy(g=>g.Number).ToList();
            List<SelectListItem> ASL = new List<SelectListItem>();
            foreach(Avtomobil A in Avtodb)
            {
                SelectListItem SLI = new SelectListItem();
                SLI.Text = A.Number;
                SLI.Value = A.Id.ToString();
                ASL.Add(SLI);

            }
            
            
            

            SelectListItem SL = new SelectListItem();
            SL.Text = "Все авто";
            SL.Value = "9999";
            ASL.Insert(0, SL);
            ViewBag.Avtomobils = ASL;

            //Создаем список месяцев
            List<SelectListItem> MonthList = new List<SelectListItem>();
          //  SelectListItem SLM = new SelectListItem();
         //   SLM.Text = "Все месяцы";
         //   SLM.Value = "13";
          //  MonthList.Add(SLM);

            for (int i=1;i<13;i++)
            {
                SelectListItem SLI = new SelectListItem();
                SLI.Text = Opr.MonthOpred(i);
                SLI.Value = i.ToString();
                MonthList.Add(SLI);

            }

            SelectListItem M = new SelectListItem();
            //если в куки что-то есть
            if (HttpContext.Request.Cookies["Month"] != null)
            {
                M.Text = HttpContext.Request.Cookies["Month"].Value;
                M.Value = Opr.MonthObratno(HttpContext.Request.Cookies["Month"].Value).ToString();//Opr.MonthObratno(M.Text).ToString();
                MonthList.RemoveAt(Opr.MonthObratno(M.Text) - 1);
                MonthList.Insert(0, M);
            }

            ViewBag.Month = MonthList;
            //ищем год
            List<SelectListItem> Years = new List<SelectListItem>();
           
            
           
            for (int i = DateTime.Now.Year; i >= 2018; i--)
            {
                SelectListItem Y = new SelectListItem();
                Y.Text = i.ToString();
                Y.Value = i.ToString();
                Years.Add(Y); 
            }
            ViewBag.Year = Years;
            return View();
        }

            // GET: Ezdkas/Details/5
            public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ezdka ezdka = db.Ezdkas.Find(id);
            if (ezdka == null)
            {
                return HttpNotFound();
            }
            return View(ezdka);
        }

        // GET: Ezdkas/Create
        public ActionResult Create()
        {
            ViewBag.AvtoId = new SelectList(db.Avtomobils, "Id", "Number");
            return View();
        }

        // POST: Ezdkas/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ArhNumb,AvtoId,Date,Probeg,Ezdki,Time,Voditel")] Ezdka ezdka)
        {
            if (ModelState.IsValid)
            {
                db.Ezdkas.Add(ezdka);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AvtoId = new SelectList(db.Avtomobils, "Id", "Number", ezdka.AvtoId);
            return View(ezdka);
        }

        // GET: Ezdkas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ezdka ezdka = db.Ezdkas.Find(id);
            if (ezdka == null)
            {
                return HttpNotFound();
            }
            ViewBag.AvtoId = new SelectList(db.Avtomobils, "Id", "Number", ezdka.AvtoId);
            return View(ezdka);
        }

        // POST: Ezdkas/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ArhNumb,AvtoId,Date,Probeg,Ezdki,Time,Voditel")] Ezdka ezdka)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ezdka).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AvtoId = new SelectList(db.Avtomobils, "Id", "Number", ezdka.AvtoId);
            return View(ezdka);
        }

        // GET: Ezdkas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ezdka ezdka = db.Ezdkas.Find(id);
            if (ezdka == null)
            {
                return HttpNotFound();
            }
            return View(ezdka);
        }

        // POST: Ezdkas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ezdka ezdka = db.Ezdkas.Find(id);
            db.Ezdkas.Remove(ezdka);
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
