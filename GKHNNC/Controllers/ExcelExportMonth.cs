using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ExcelExportMonth
    {
        public int ver = 0;

       

        public static void EXPORT(List<CompleteWork> CompleteWorks,string Month, string GEU,string Year) {

            if (CompleteWorks.Count == 0)
            {
               
                return;
            } 
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо
            int from = 1;
           
            string GKH = GEU.Remove(0, 4); ;//Номер участка ЖКХ
            string month = Month;
            string year = Year;
            int num = 1;
            int adres = 2;
            int vid = 3;
            
            int kolvo = 4;
            int izmerenie = 5;
            int data = 6;
           

            WS.Cells[1, 1] = "ФЕДЕРАЛЬНОЕ ГОСУДАРСТВЕННОЕ УНИТАРНОЕ ПРЕДПРИЯТИЕ 'ЖИЛИЩНО-КОММУНАЛЬНОЕ ХОЗЯЙСТВО НОВОСИБИРСКОГО НАУЧНОГО ЦЕНТРА'";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 50;
            range.WrapText = true;

            from++;
            WS.Cells[2, 1] = "ЖИЛИЩНО - ЭКСПЛУАТАЦИОННЫЙ УЧАСТОК №"+ GKH;
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 25;
            range.WrapText = true;


            from++;
            WS.Cells[3, 1] = "Отчет";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 12;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;
            range.WrapText = true;


            from++;
            WS.Cells[4, 1] = "Отчет о выполненной работе по техническому обслуживанию конструктивных элементов и техническому обслуживанию внутридомового инженерного оборудования ЖЭУ № " + GKH+"ФГУП 'ЖКХ ННЦ' за "+ month+" " + year+".";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 11;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 50;//высота строки
            range.WrapText = true;
           

            int startStroka = 5;
            range = WS.Cells[startStroka, num];//столбец номер ширина
            range.ColumnWidth = 3;
            range.Value  = "N";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, adres];
            range.ColumnWidth = 32;
            range.Value = "Адрес";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, vid];
            range.ColumnWidth = 35;
            range.Value = "Вид работ";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, kolvo];
            range.ColumnWidth = 5;
            range.Value = "Кол.";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, izmerenie];
            range.ColumnWidth = 5;
            range.Value = "Ед.";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            //дату убираем в отчете за месяц. Суммируем записи.
            //range = WS.Cells[startStroka, data];
           // range.ColumnWidth = 7;
           // range.Value = "Дата";
           // range.Font.Bold = true;
           // range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            List<string> Homes = new List<string>();//запишем сюда все дома из массива

            foreach (CompleteWork CW in CompleteWorks)
            {
                if (Homes.Contains(CW.WorkAdress)==false)//если дома не содержат такого адреса то добавим его 
                {
                    Homes.Add(CW.WorkAdress);
                }
            }
            Homes.Sort();
            int c = 0;
         

            foreach (string H in Homes)
            {
                
                c++;
                
                WS.Cells[startStroka+1, adres] = Homes[c-1];
                WS.Cells[startStroka+1, num] = c;
               


                int f = startStroka;
                int[] counter = new int[3];
                List<CompleteWork>[] Sortirovka = new List<CompleteWork>[2];
                for (int d = 0; d < 2; d++)
                {
                    Sortirovka[d] = new List<CompleteWork>();
                }

                string[] g = new string[] { "ТО конструктивных элементов", "ТО внутридомового инженерного оборудования" };
                string[] g2 = new string[] { "Конструктивные элементы", "Внутридомовое инженерное оборудование" };
                foreach (CompleteWork CW in CompleteWorks)
                {

                    string adress1 = CW.WorkAdress.Replace(" ", "");
                    string adress2 = Homes[c - 1].Replace(" ", "");



                    if (adress1.Equals(adress2))//если в этом адресе несколько работ то сортируем по дате. Записываем в массив сортировки.
                    {
                        counter[0]++;
                        //startStroka++;


                        string group2 = CW.WorkGroup.Replace(" ", "");
                        if (group2.Equals(g[0].Replace(" ", "")))
                        {
                            Sortirovka[0].Add(CW);
                            counter[1]++;
                        }
                        if (group2.Equals(g[1].Replace(" ", "")))
                        {
                            counter[2]++;
                            Sortirovka[1].Add(CW);
                        }

                        //  WS.Cells[startStroka, num] = c;
                    }
                }
                int froms = 0;
                for (int l = 1; l < 3; l++)
                {
                    if (counter[l] > 0)
                    {
                        if (l == 1) { froms = startStroka + 1; }
                        startStroka++;
                        WS.Cells[startStroka, vid] = g2[l - 1];
                        range = WS.get_Range("C" + startStroka, "E" + startStroka);
                        range.Merge();
                        range.Font.Bold = true;
                    }
                    if (counter[l] == 1)
                    {
                        startStroka++;
                        int tos = startStroka;
                        WS.Cells[startStroka, vid] = Sortirovka[l - 1][0].WorkName;
                        WS.Cells[startStroka, kolvo] = Sortirovka[l - 1][0].WorkNumber;
                        WS.Cells[startStroka, izmerenie] = Sortirovka[l - 1][0].WorkIzmerenie.Replace(" ","");

                        range = WS.get_Range("B" + froms.ToString(), "B" + tos.ToString());
                        range.Merge();
                        range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                        range = WS.get_Range("A" + froms.ToString(), "A" + tos.ToString());
                        range.Merge();
                        range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                        // string MONN = Sortirovka[l - 1][0].WorkDate.Month.ToString();
                        // if (MONN.Length < 2)
                        //  {
                        //      MONN = "0" + MONN;
                        //  }
                        //  WS.Cells[startStroka, data] = MONN + "-" + Sortirovka[l - 1][0].WorkDate.Day;
                    }
                    else
                    {
                        if (counter[l] > 1)
                        {
                           
                            List<CompleteWork> SortSummaKolichestvo = new List<CompleteWork>();
                            for (int n = Sortirovka[l - 1].Count-1; n >=0 ; n--)
                            {
                                bool go = false;
                                foreach (CompleteWork W in SortSummaKolichestvo)
                                {
                                    if (Sortirovka[l - 1][n].WorkCode.Equals(W.WorkCode))
                                    {
                                        W.WorkNumber += Sortirovka[l - 1][n].WorkNumber;
                                        go = true;
                                    }
                                }
                                if (!go)
                                {
                                    SortSummaKolichestvo.Add(Sortirovka[l - 1][n]);
                                }

                            }
                            //если нужно сортировать по дате
                         /*   List<CompleteWork> Sort2 = new List<CompleteWork>();
                            for (int i = SortSummaKolichestvo.Count - 1; i > 0; i--)
                            {
                                CompleteWork x = Sortirovka[l - 1][i];
                                for (int j = SortSummaKolichestvo.Count - 2; j > -1; j--)
                                {
                                    if (x.WorkDate > SortSummaKolichestvo[j].WorkDate)
                                    {
                                        x = SortSummaKolichestvo[j];
                                    }
                                }
                                Sort2.Add(x);
                                SortSummaKolichestvo.Remove(x);

                            }
                            Sort2.Add(SortSummaKolichestvo[0]);
                            */
                            foreach (CompleteWork CW in SortSummaKolichestvo)
                            {
                                startStroka++;
                                WS.Cells[startStroka, vid] = CW.WorkName;
                                WS.Cells[startStroka, kolvo] = CW.WorkNumber;
                                WS.Cells[startStroka, izmerenie] = CW.WorkIzmerenie.Replace(" ", "");
                                //добавление даты работ
                              //  string MONN = CW.WorkDate.Month.ToString();
                              //  if (MONN.Length < 2)
                              //  {
                              //      MONN = "0" + MONN;
                              //  }
                              //  WS.Cells[startStroka, data] = MONN + "-" + CW.WorkDate.Day;
                            }
                            int tos = startStroka;
                            range = WS.get_Range("B" + froms.ToString(), "B" + tos.ToString());
                            range.Merge();
                            range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                            range = WS.get_Range("A" + froms.ToString(), "A" + tos.ToString());
                            range.Merge();
                            range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                        }

                    }
                }

                int t = startStroka;
                range = WS.get_Range("A" + f, "E" + t);
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;



            }
               
           

            // Сохранение файла Excel.
          
            WbExcel.SaveCopyAs("C:\\inetpub\\Otchets\\" + "OtchetMonth"+GEU+".xlsx");//сохраняем в папку
            
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

    }
    public class HouseToAkt
    {
        public string Adres = "";
        public List<string> pokazateli = new List<string>();
        public List<string> periodichnost = new List<string>();
        public List<decimal> StoimostNaM2 = new List<decimal>();
        public List<decimal> StoimostNaMonth = new List<decimal>();
        public int HId = 0;//ID текущего дома
        public List<int> UId = new List<int>();//ID услуг
    }
    //Загрузка стандартного файла эксель с поиском имен колонок в первых 30 на 30 клеток имена передаются массивом типа стринг.
    public class ExcelSVNUpload
    {
        //Универсальный метод загрузки требует          имя файла        наименования колонок  номер вкладки или наименование  ( массив номеров столбцов) по желанию
        public static List<List<string>> IMPORT(string FilePatch, string[] Names, out string Error, string Vkladka = "1", int[] X = null )
        {
            Error = "";
            List<List<string>> SVNKI = new List<List<string>>();
            //инициализация загруженного файла
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = null;
            try
            {
                WbExcel = WB.Open(FilePatch);
            }
            catch {
                //если вкладку не нашли
                // Закрытие книги.
               // WbExcel.Close(false, "", Type.Missing);
                // Закрытие приложения Excel.

                ApExcel.Quit();

                //Marshal.FinalReleaseComObject(WbExcel);
                Marshal.FinalReleaseComObject(WB);
                Marshal.FinalReleaseComObject(ApExcel);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //Удаление файла с сервера
                File.Delete(FilePatch);
                Error = "Не найдена вкладка " + Vkladka;
                return SVNKI;
               
            }
                int vk = 1;
            string vks = "";
            bool Vkl = false;
            Excel.Worksheet WS = null;
            try
            {//если указана цифра вкладки то открываем ее
                vk = Convert.ToInt16(Vkladka);
                WS = WbExcel.Sheets[vk];
                Vkl = true;
            }
            catch
            {//если цифры нет то имя вкладки открываем
                try
                {
                    vks = Vkladka;
                    WS = (Excel.Worksheet)WbExcel.Worksheets[vks];
                    Vkl = true;
                }
                catch
                {
                    Error = "Нет такой вкладки в файле!" +Vkladka;
                    return SVNKI;
                }
            }
            if (Vkl == false) { Error = "Не найдена вкладка "+Vkladka; }
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо
            int stNumber = WS.UsedRange.Columns.Count; 
            int startStroka = 1;
            //Создаем массив той же размерности
            int[] NamesI = new int[Names.Length];
            if (X != null)//если заданы столбцы числами то размерность будет Х
            {
                NamesI = new int[X.Length];
            }
            for(int N=0;N<NamesI.Length;N++)
            {
                NamesI[N] = -1;//не найденные значения -1
                Names[N] = Names[N].Replace(" ", "").ToUpper();//Приводим все заголовки к виду без пробелов и верхний регистр
               
            }
            int progress = 0;
            double pro100 = WS.UsedRange.Rows.Count;
            int procount = 0;

            int lastRow = WS.UsedRange.Rows.Count;
            int LastCol = WS.UsedRange.Columns.Count;
            //range = WS.get_Range("A1", Opr.OpredelenieBukvi(lastRow)+"10");
           
            if (LastCol < Names.Length) { LastCol = Names.Length + 2; }//если вдруг количество столбцов криво определилось 
        
            var EXX = new object[lastRow, LastCol];
            int rr = Convert.ToInt32(Convert.ToDecimal(lastRow) / 100);
           
            if (lastRow - rr * 100 < 0) { rr = rr - 1; }
            if (rr == 0) { rr = 1; }
            int rrr = lastRow - rr * 100;
            if (rrr < 0) { rrr = 0; }
            int lastrr = 0;
            int end = 100;
            if (lastRow <= 100) { end = lastRow; }
            
            for (int i = 1; i <= end; i++)//грузим из файла кусками объём файла \ 100
            {
                lastrr++;

                var EX = (object[,])WS.Range["A" + lastrr.ToString() + ":"+Opr.OpredelenieBukvi(LastCol) + (rr * i).ToString()].Value;
                for (int j = 0; j < rr; j++)
                {
                    for (int k = 0; k < LastCol; k++)
                    {
                       
                            //сохраняем все строки экселя как объекты так намного быстрее затем обработаем
                            EXX[lastrr - 1 + j, k] = EX[j + 1, k + 1];
                        
                    }
                }


                lastrr = rr * i;

               
            }
            var EX2 = (object[,])WS.Range["A" + (lastrr ).ToString() + ":" + Opr.OpredelenieBukvi(LastCol) + (lastrr + rrr).ToString()].Value;//догружаем остатки
            for (int j = 0; j < rrr; j++)
            {
                for (int k = 0; k < LastCol; k++)
                {
                    EXX[lastrr - 1 + j, k] = EX2[j + 1, k + 1];
                }
            }

            //Загрузили теперь идем проверять шапку если она не задана изначально цифрами

            if (X == null)
            {
                for (int i = 0; i < 50; i++)
                {

                    for (int j = 0; j < LastCol; j++)
                    {

                        string W = "";
                        try
                        {
                            W = EXX[i, j].ToString().Replace(" ", "").ToUpper();
                        }
                        catch
                        {

                        }
                        for (int n = 0; n < NamesI.Length; n++)
                        {
                            if (NamesI[n] < 0)
                            {
                                if (Names[n].Equals(W))
                                {//Заголовки уже в верхнем регистре и без пробелов поэтому и не приводим.
                                    startStroka = i;
                                    NamesI[n] = j;
                                    bool xx = true;
                                    foreach (int g in NamesI)
                                    {
                                        if (g < 0) { xx = false; break; }
                                    }

                                    if (xx) { goto ShapkaNaidena; }//если все заголовки нашли то выходим
                                    else
                                    {

                                        //если нашли соответствие то записали и вышли чтобы одинаковые наименования заголовков можно было далее назначить
                                        break;
                                    }
                                    //если вся шапка найдена то нафиг цикл идем к метке

                                }

                            }


                        }



                    }
                    //если нашли все имена то выходим из цикла

                }
            }

            //Если не все заголовки найдены то смотрим каких не хватает и выдаем сообщение
            Error = "Не найдены заголовки: ";
            
            for (int g=0;g < NamesI.Length;g++)
            {
                if (NamesI[g] < 0) { Error += Names[g]+"; "; }
                
            }

            ShapkaNaidena:
            //все ли найдены столбцы? Доп проверка много времени не займет.
            bool go = true;
            if (X == null)
            {
                foreach (int n in NamesI)
                {
                    if (n < 0)
                    {
                        go = false;
                    }
                }
            }
            else//если Х не нулевой то заполняем масив
            {
                for (int x=0;x<X.Length;x++)
                {
                    NamesI[x] = X[x]; 
                }
            }
            
            //если все столбцы найдены то пишем данные
            if (go)
            {


                int Konec = 0;
                int Schetchik = 0;
               
                while (Konec<=5&& Schetchik<lastRow-1) 
                {
                    Schetchik++;
                    if (startStroka >= lastRow-1) { break; }
                    if (EXX[startStroka, NamesI[0]] != null)
                    {
                        Konec = 0;
                        List<string> L = new List<string>();
                        foreach (int j in NamesI)
                        {
                            string E = "0";
                            try
                            {
                                E = EXX[startStroka, j].ToString();
                            }
                            catch
                            {

                            }
                            L.Add(E);
                        }
                        SVNKI.Add(L);
                        procount++;
                        progress = Convert.ToInt16(procount / pro100 * 50);
                        ProgressHub.SendMessage("Загружаем файл... ", progress);
                        if (procount > pro100) { procount = Convert.ToInt32(pro100); }
                        startStroka++;
                    }
                    else
                    {
                        procount++;
                        progress = Convert.ToInt16(procount / pro100 * 50);
                        ProgressHub.SendMessage("Загружаем файл... ", progress);
                        if (procount > pro100) { procount = Convert.ToInt32(pro100); }
                        startStroka++;
                        Konec++;
                    }
                }

            }
            else
            {
                Console.WriteLine("Ничего не считано! Ошибка в файле.");
                ProgressHub.SendMessage("Ничего не считано! Ошибка в файле. ", 100);
                
            }
                

            // Закрытие книги.
            WbExcel.Close(false, "", Type.Missing);
            // Закрытие приложения Excel.

            ApExcel.Quit();

            Marshal.FinalReleaseComObject(WbExcel);
            Marshal.FinalReleaseComObject(WB);
            Marshal.FinalReleaseComObject(ApExcel);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Удаление файла с сервера
            File.Delete(FilePatch);


            return SVNKI;
        }

        public static void EXPORT(List<CompleteWork> CompleteWorks, string Month, string GEU, string Year, string Adres)
        {

            if (CompleteWorks.Count == 0)
            {

                return;
            }
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо
            int from = 1;

            string GKH = GEU.Remove(0, 4); ;//Номер участка ЖКХ
            string month = Month;
            string year = Year;
            int num = 1;
            int adres = 2;
            int vid = 3;

            int kolvo = 4;
            int izmerenie = 5;
            int data = 6;


            WS.Cells[1, 1] = "Приложение к акту №_______";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 50;
            range.WrapText = true;

            /* from++;
             WS.Cells[2, 1] = "ЖИЛИЩНО - ЭКСПЛУАТАЦИОННЫЙ УЧАСТОК №" + GKH;
             range = WS.get_Range("A" + from, "E" + from);
             range.Merge(Type.Missing);
             range.Font.Bold = true;
             range.Font.Size = 13;
             range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.RowHeight = 25;
             range.WrapText = true;


             from++;
             WS.Cells[3, 1] = "";
             range = WS.get_Range("A" + from, "E" + from);
             range.Merge(Type.Missing);
             range.Font.Bold = true;
             range.Font.Size = 12;
             range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.RowHeight = 15;
             range.WrapText = true;
             */

            from++;
            WS.Cells[2, 1] = "приемки оказанных услуг и(или) выполненных работ по содержанию и текущему ремонту общего имущества в многоквартирном доме по адресу " + Adres + " за " + month + " " + year + ".";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 11;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 50;//высота строки
            range.WrapText = true;


            int startStroka = 3;
            range = WS.Cells[startStroka, num];//столбец номер ширина
            range.ColumnWidth = 3;
            range.Value = "N";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, adres];
            range.ColumnWidth = 30;
            range.Value = "Адрес";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, vid];
            range.ColumnWidth = 30;
            range.Value = "Вид работ";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, kolvo];
            range.ColumnWidth = 5;
            range.Value = "Кол.";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, izmerenie];
            range.ColumnWidth = 5;
            range.Value = "Ед.";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, data];
            range.ColumnWidth = 7;
            range.Value = "Дата";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            List<string> Homes = new List<string>();//запишем сюда все дома из массива
            foreach (CompleteWork CW in CompleteWorks)
            {
                if (Homes.Contains(CW.WorkAdress) == false)//если дома не содержат такого адреса то добавим его 
                {
                    Homes.Add(CW.WorkAdress);
                }
            }
            Homes.Sort();
            int c = 0;
            foreach (string H in Homes)
            {
                c++;

                WS.Cells[startStroka + 1, adres] = Homes[c - 1];
                WS.Cells[startStroka + 1, num] = c;
                int f = startStroka;
                int[] counter = new int[3];
                List<CompleteWork>[] Sortirovka = new List<CompleteWork>[2];
                for (int d = 0; d < 2; d++)
                {
                    Sortirovka[d] = new List<CompleteWork>();
                }

                string[] g = new string[] { "ТО конструктивных элементов", "ТО внутридомового инженерного оборудования" };
                string[] g2 = new string[] { "Конструктивные элементы", "Внутридомовое инженерное оборудование" };
                foreach (CompleteWork CW in CompleteWorks)
                {

                    string adress1 = CW.WorkAdress.Replace(" ", "");
                    string adress2 = Homes[c - 1].Replace(" ", "");



                    if (adress1.Equals(adress2))//если в этом адресе несколько работ то сортируем по дате. Записываем в массив сортировки.
                    {
                        counter[0]++;
                        //startStroka++;


                        string group2 = CW.WorkGroup.Replace(" ", "");
                        if (group2.Equals(g[0].Replace(" ", "")))
                        {
                            Sortirovka[0].Add(CW);
                            counter[1]++;
                        }
                        if (group2.Equals(g[1].Replace(" ", "")))
                        {
                            counter[2]++;
                            Sortirovka[1].Add(CW);
                        }

                        //  WS.Cells[startStroka, num] = c;
                    }
                }
                int froms = 0;
                for (int l = 1; l < 3; l++)
                {
                    if (l == 1) { froms = startStroka + 1; }
                    startStroka++;
                    WS.Cells[startStroka, vid] = g2[l - 1];
                    range = WS.get_Range("C" + startStroka, "F" + startStroka);
                    range.Merge();
                    range.Font.Bold = true;

                    if (counter[l] == 1)
                    {
                        startStroka++;
                        WS.Cells[startStroka, vid] = Sortirovka[l - 1][0].WorkName;
                        WS.Cells[startStroka, kolvo] = Sortirovka[l - 1][0].WorkNumber;
                        WS.Cells[startStroka, izmerenie] = Sortirovka[l - 1][0].WorkIzmerenie;
                        string MONN = Sortirovka[l - 1][0].WorkDate.Month.ToString();
                        if (MONN.Length < 2)
                        {
                            MONN = "0" + MONN;
                        }
                        WS.Cells[startStroka, data] = MONN + "-" + Sortirovka[l - 1][0].WorkDate.Day;
                    }
                    else
                    {
                        if (counter[l] > 1)
                        {
                            List<CompleteWork> Sort2 = new List<CompleteWork>();
                            for (int i = Sortirovka[l - 1].Count - 1; i > 0; i--)
                            {
                                CompleteWork x = Sortirovka[l - 1][i];
                                for (int j = Sortirovka[l - 1].Count - 2; j > -1; j--)
                                {
                                    if (x.WorkDate > Sortirovka[l - 1][j].WorkDate)
                                    {
                                        x = Sortirovka[l - 1][j];
                                    }
                                }
                                Sort2.Add(x);
                                Sortirovka[l - 1].Remove(x);

                            }
                            Sort2.Add(Sortirovka[l - 1][0]);

                            foreach (CompleteWork CW in Sort2)
                            {
                                startStroka++;
                                WS.Cells[startStroka, vid] = CW.WorkName;
                                WS.Cells[startStroka, kolvo] = CW.WorkNumber;
                                WS.Cells[startStroka, izmerenie] = CW.WorkIzmerenie;
                                string MONN = CW.WorkDate.Month.ToString();
                                if (MONN.Length < 2)
                                {
                                    MONN = "0" + MONN;
                                }
                                WS.Cells[startStroka, data] = MONN + "-" + CW.WorkDate.Day;
                            }
                            int tos = startStroka;
                            range = WS.get_Range("B" + froms.ToString(), "B" + tos.ToString());
                            range.Merge();
                            range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                            range = WS.get_Range("A" + froms.ToString(), "A" + tos.ToString());
                            range.Merge();
                            range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                        }

                    }
                }

                int t = startStroka;
                range = WS.get_Range("A" + f, "F" + t);
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;



            }



            // Сохранение файла Excel.

            WbExcel.SaveCopyAs("C:\\inetpub\\Otchets\\" + "OtchetMonth" + GEU + ".xlsx");//сохраняем в папку

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

    }

    public class ExcelUpload
    {
        //при загрузке файла на сервер используем данный метод
        public static List<HouseToAkt> IMPORT(string FilePatch)
        {
            //инициализация загруженного файла
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null ;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = WB.Open(FilePatch); 
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо
            int stNumber = 0;
            int startStroka = 1;
            int stPeriod = 0;
            int stName = 0;
            int stDom = 0;
            List<bool> Krasnoe = new List<bool>();
            List<string> Name = new List<string>();
            List<string> Period = new List<string>();
            List<string> Homes = new List<string>();
            //находим ячейку со знаком № и от нее захватываем столбцы
            List<HouseToAkt> Houses = new List<HouseToAkt>();

            for (int i = 1; i < 11; i++)
                {
                    for (int j = 1; j < 11; j++)
                    {
                        string W = WS.Cells[i, j].Text;
                        if (W.Replace(" ", "").Equals("№"))
                        {
                            startStroka = i + 2;
                            stNumber = j;
                            stName = j + 1;
                            stPeriod = j + 2;
                            stDom = j + 3;
                            goto LoopEnd;

                        }
                    }
                }
                LoopEnd:
                if (stName == 0)
            {
               
                return (Houses);
            }
               

            
            int k = startStroka;
            while (WS.Cells[k,stNumber].Interior.Color != ColorTranslator.ToOle(Color.White))
            {
                //Идем вниз по строкам и если первая ячейка не белая и в ней есть цифра то заносим ее в базу.
                string C = WS.Cells[k, stNumber].Text;
                try
                {
                    C = Convert.ToInt32(C).ToString();
                    if (WS.Cells[k, stNumber].Font.Color != ColorTranslator.ToOle(Color.Black))
                    {
                        Krasnoe.Add(true);
                    }
                    else
                    {
                        Krasnoe.Add(false);
                    }
                    string S1 = WS.Cells[k, stName].Text;
                    string S2 = WS.Cells[k, stPeriod].Text;
                   
                    Name.Add(S1);
                    Period.Add(S2);
                }
                catch
                {

                }

               k++;
            }
            int h = stDom;

            int progress = 0;
            double pro100 = ApExcel.WorksheetFunction.CountA(WS.Columns[1]); 
            int procount = 0;
               
                //грузим список домов из файла
                while (WS.Cells[startStroka - 2, h].Text != "")
            {
                procount++;
                progress = Convert.ToInt16(procount / pro100 * 50);
                ProgressHub.SendMessage("Загрузка...", progress);
                string s = WS.Cells[startStroka - 2, h].Text;
                s=s.Replace("пр.", "");
                s = s.Replace("проспект", "");
                s = s.Replace("просп.", "");
                s = s.Replace("проезд", "");
                s = s.Replace("Бульвар", "");
                s = s.Replace("Бульв.", "");
                // s = s.Replace("ТСЖ", ""); все что помечено ТСЖ мы не включаем в акт (Осадчук 17.01.19)
                s = ZachistkaStroki(s);
                HouseToAkt Hou = new HouseToAkt();
                Hou.Adres = s;
                Homes.Add(s);
                //добавим дом и сразу его заполним по стандарту
                for (int t=0; t<Name.Count; t++)
                {
                    Hou.periodichnost.Add(Period[t]);
                    Hou.pokazateli.Add(Name[t]);
                    
                    try
                    {
                        string ss = WS.Cells[startStroka + t, h].Text;
                        ss = ss.Replace(",", ".");
                        ss=ss.Replace(" ", "");
                        decimal dd = Convert.ToDecimal(ss);
                        
                        //если помечена красным то
                        if (!Krasnoe[t])
                        {
                            if (dd != 0) {
                                Hou.StoimostNaMonth.Add(dd/12);
                            }
                            else
                            {
                                Hou.StoimostNaMonth.Add(0);
                            }
                        }
                        else
                        {
                            Hou.StoimostNaMonth.Add(dd);
                        }
                        
                    }
                    catch
                    {
                        Hou.StoimostNaMonth.Add(0);
                    }

                    try
                    {
                        string ss = WS.Cells[startStroka + t, h + 1].Text;
                        ss = ss.Replace(" ", "");
                        ss = ss.Replace(",", ".");
                        Hou.StoimostNaM2.Add(Convert.ToDecimal(ss));
                    }
                    catch
                    {
                        Hou.StoimostNaM2.Add(0);
                    }
                }
                Houses.Add(Hou);

                h += 2;
            }
                // Закрытие Excel.

                // Закрытие книги.
                WbExcel.Close(false, "", Type.Missing);
            // Закрытие приложения Excel.

            ApExcel.Quit();

            Marshal.FinalReleaseComObject(WbExcel);
            Marshal.FinalReleaseComObject(WB);
            Marshal.FinalReleaseComObject(ApExcel);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Удаление файла с сервера
            File.Delete(FilePatch);

            return (Houses);
            
            //Сохранение в БД полученных данных
        }
        public static string ZachistkaStroki(string S)
        {
            
            S = S.Replace(",", "");
            S = S.Replace(" ", "");
            S = S.Replace(".", "");
            S = S.Replace("-", "");
            S = S.ToUpper();
            return(S);

        }

    }


    public class ExcelExportDom
    {
        public int ver = 0;
        public static void EXPORT(List<CompleteWork> CompleteWorks, string Month, string GEU, string Year, string Adres)
        {

            if (CompleteWorks.Count == 0)
            {

                return;
            }
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо
            int from = 1;

            string GKH = GEU.Remove(0, 4); ;//Номер участка ЖКХ
            string month = Month;
            string year = Year;
            int num = 1;
            int adres = 2;
            int vid = 3;

            int kolvo = 4;
            int izmerenie = 5;
            int data = 6;


            WS.Cells[1, 1] = "Приложение к акту №_______";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 50;
            range.WrapText = true;

           /* from++;
            WS.Cells[2, 1] = "ЖИЛИЩНО - ЭКСПЛУАТАЦИОННЫЙ УЧАСТОК №" + GKH;
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 25;
            range.WrapText = true;


            from++;
            WS.Cells[3, 1] = "";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 12;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;
            range.WrapText = true;
            */

            from++;
            WS.Cells[2, 1] = "приемки оказанных услуг и(или) выполненных работ по содержанию и текущему ремонту общего имущества в многоквартирном доме по адресу " + Adres + " за " + month + " " + year + ".";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 11;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 50;//высота строки
            range.WrapText = true;


            int startStroka = 3;
            range = WS.Cells[startStroka, num];//столбец номер ширина
            range.ColumnWidth = 3;
            range.Value = "N";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, adres];
            range.ColumnWidth = 30;
            range.Value = "Адрес";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, vid];
            range.ColumnWidth = 30;
            range.Value = "Вид работ";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, kolvo];
            range.ColumnWidth = 5;
            range.Value = "Кол.";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, izmerenie];
            range.ColumnWidth = 5;
            range.Value = "Ед.";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            range = WS.Cells[startStroka, data];
            range.ColumnWidth = 7;
            range.Value = "Дата";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            List<string> Homes = new List<string>();//запишем сюда все дома из массива
            foreach (CompleteWork CW in CompleteWorks)
            {
                if (Homes.Contains(CW.WorkAdress) == false)//если дома не содержат такого адреса то добавим его 
                {
                    Homes.Add(CW.WorkAdress);
                }
            }
            Homes.Sort();
            int c = 0;
            foreach (string H in Homes)
            {
                c++;

                WS.Cells[startStroka + 1, adres] = Homes[c - 1];
                WS.Cells[startStroka + 1, num] = c;
                int f = startStroka;
                int[] counter = new int[3];
                List<CompleteWork>[] Sortirovka = new List<CompleteWork>[2];
                for (int d = 0; d < 2; d++)
                {
                    Sortirovka[d] = new List<CompleteWork>();
                }
           
                string[] g = new string[] { "ТО конструктивных элементов", "ТО внутридомового инженерного оборудования" };
                string[] g2 = new string[] { "Конструктивные элементы", "Внутридомовое инженерное оборудование" };
                foreach (CompleteWork CW in CompleteWorks)
                {

                    string adress1 = CW.WorkAdress.Replace(" ", "");
                    string adress2 = Homes[c - 1].Replace(" ", "");
                    


                    if (adress1.Equals(adress2))//если в этом адресе несколько работ то сортируем по дате. Записываем в массив сортировки.
                    {
                        counter[0]++;
                        //startStroka++;
                     
                       
                        string group2 = CW.WorkGroup.Replace(" ","");
                        if (group2.Equals(g[0].Replace(" ","")))
                        {
                            Sortirovka[0].Add(CW);
                            counter[1]++;
                        }
                        if (group2.Equals(g[1].Replace(" ","")))
                        {
                            counter[2]++;
                            Sortirovka[1].Add(CW);
                        }

                        //  WS.Cells[startStroka, num] = c;
                    }
                }
                int froms = 0;
                for (int l = 1; l < 3; l++)
                {
                    if (l == 1) { froms = startStroka + 1; }
                    startStroka++;
                    WS.Cells[startStroka, vid] = g2[l - 1];
                    range = WS.get_Range("C" + startStroka, "F" + startStroka);
                    range.Merge();
                    range.Font.Bold = true;

                    if (counter[l] == 1)
                    {
                        startStroka++;
                        WS.Cells[startStroka, vid] = Sortirovka[l-1][0].WorkName;
                        WS.Cells[startStroka, kolvo] = Sortirovka[l - 1][0].WorkNumber;
                        WS.Cells[startStroka, izmerenie] = Sortirovka[l - 1][0].WorkIzmerenie;
                        string MONN = Sortirovka[l - 1][0].WorkDate.Month.ToString();
                        if (MONN.Length < 2)
                        {
                            MONN = "0" + MONN;
                        }
                        WS.Cells[startStroka, data] = MONN + "-" + Sortirovka[l - 1][0].WorkDate.Day;
                    }
                    else
                    {
                        if (counter[l] > 1)
                        {
                            List<CompleteWork> Sort2 = new List<CompleteWork>();
                            for (int i = Sortirovka[l - 1].Count - 1; i > 0; i--)
                            {
                                CompleteWork x = Sortirovka[l - 1][i];
                                for (int j = Sortirovka[l - 1].Count - 2; j > -1; j--)
                                {
                                    if (x.WorkDate > Sortirovka[l - 1][j].WorkDate)
                                    {
                                        x = Sortirovka[l - 1][j];
                                    }
                                }
                                Sort2.Add(x);
                                Sortirovka[l - 1].Remove(x);

                            }
                            Sort2.Add(Sortirovka[l - 1][0]);
                           
                            foreach (CompleteWork CW in Sort2)
                            {
                                startStroka++;
                                WS.Cells[startStroka, vid] = CW.WorkName;
                                WS.Cells[startStroka, kolvo] = CW.WorkNumber;
                                WS.Cells[startStroka, izmerenie] = CW.WorkIzmerenie;
                                string MONN = CW.WorkDate.Month.ToString();
                                if (MONN.Length < 2)
                                {
                                    MONN = "0" + MONN;
                                }
                                WS.Cells[startStroka, data] = MONN + "-" + CW.WorkDate.Day;
                            }
                            int tos = startStroka;
                            range = WS.get_Range("B" + froms.ToString(), "B" + tos.ToString());
                            range.Merge();
                            range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                            range = WS.get_Range("A" + froms.ToString(), "A" + tos.ToString());
                            range.Merge();
                            range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                        }

                    }
                }

                int t = startStroka;
                range = WS.get_Range("A" + f, "F" + t);
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;



            }



            // Сохранение файла Excel.

            WbExcel.SaveCopyAs("C:\\inetpub\\Otchets\\" + "OtchetMonth" + GEU + ".xlsx");//сохраняем в папку

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

    }



    public class ExcelExportDomVipolnennieUslugi
    {
        public int ver = 0;
        public static void EXPORT(List<VipolnennieUslugi> CompleteWorks, string Month, string GEU, string Year, string Adres, string Nachalnik, string Prikaz,string patch,string Summa)
        {

            if (CompleteWorks.Count == 0)
            {

                return;
            }
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо
            int from = 1;

            string GKH = GEU.Remove(0, 3); ;//Номер участка ЖКХ
            string month = Opr.MonthToNorm(Month);
            string year = Year;
            int periodichnost = 2;
            int izmerenie = 3;
            int stoimost = 4;
            int cena = 5;
            int naimenovanie = 1;


            
            WS.Cells[from, 3] = "УТВЕРЖДЕНО";
            range = WS.get_Range("C" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 3] = " приказом Министерства строительства и жилищно-коммунального хозяйства Российской Федерации от 26.10.2015 №761/пр";
            range = WS.get_Range("C" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 65;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 1] = "АКТ №_______";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;
            range.WrapText = true;

            /* from++;
             WS.Cells[2, 1] = "ЖИЛИЩНО - ЭКСПЛУАТАЦИОННЫЙ УЧАСТОК №" + GKH;
             range = WS.get_Range("A" + from, "E" + from);
             range.Merge(Type.Missing);
             range.Font.Bold = true;
             range.Font.Size = 13;
             range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.RowHeight = 25;
             range.WrapText = true;


             from++;
             WS.Cells[3, 1] = "";
             range = WS.get_Range("A" + from, "E" + from);
             range.Merge(Type.Missing);
             range.Font.Bold = true;
             range.Font.Size = 12;
             range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.RowHeight = 15;
             range.WrapText = true;
             */
            int a = 0;
             for (int y = 0; y < Adres.Length; y++)
            {
                if (Adres[y].Equals('-'))
                {
                    a = y;
                }
            }
            
            string Ad = Adres.Remove(a);
            string Res = Adres.Remove(0, a+1);
            Res = Res.Replace(" ", "");
            Ad = "ул. " + Ad;
            Res = " д. " + Res;
            Adres = Ad + Res;
            Adres = Adres.Replace("-", " ");

            from++;
            WS.Cells[from, 1] = "г.Новосибирск";
            range = WS.get_Range("A" + from, "A" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            WS.Cells[from, 3] = "20 "+month+" "+Year ;//подписываем дату
            range = WS.get_Range("C" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;



            from++;
            WS.Cells[from, 1] = "приемки оказанных услуг и(или) выполненных работ по содержанию и текущему ремонту общего имущества в многоквартирном доме по адресу " + Adres + " за " + month + " " + year + ".";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 30;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 1] = "Собственники помещений в многоквартирном доме, расположенном по адресу:";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 1] = Adres;
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 1] = "именуемые в дальнейшем 'Заказчик' в лице ______________________________ ";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 2] = "(указывается Ф.И.О. уполномоченного собственника помещения в многоквартирном доме либо председателя Совета многоквартирного дома)";
            range = WS.get_Range("B" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 6;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;


            from++;
            WS.Cells[from, 1] = "являющегося собственником квартиры №_____, находящейся в данном многоквартирном доме*, " +
                "с одной стороны, и Федеральное государственное унитарное предприятие 'Жилищно-коммунальное хозяйство Новосибирского научного центра' (ФГУП 'ЖКХ ННЦ')"+
"именуемое в дальнейшем “Исполнитель”,  в лице начальника ЖЭУ-" + GKH + " " + Nachalnik + ", действующего на основании доверенности №" + Prikaz;
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 65;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 1] = "с другой стороны, совместно именуемые “Стороны”, составили настоящий Акт о нижеследующем:";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 1] = "1. Исполнителем предъявлены к приемке следующие оказанные на основании договора управления многоквартирным домом услуги и(или)"+
"выполненные работы по содержанию и текущему ремонту общего имущества в многоквартирном доме, расположенном по адресу "+ Adres;
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 50;//высота строки
            range.WrapText = true;

            from++;
            range = WS.get_Range("A" + from, "E" + from);
            range.RowHeight = 7;

            int startStroka = from+1;
            range = WS.Cells[startStroka, naimenovanie];//столбец номер ширина
            range.ColumnWidth = 40;
            range.Font.Size = 10;
            range.Value = "Наименование вида работы";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 66;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;
      


            range = WS.Cells[startStroka, periodichnost];
            range.ColumnWidth = 14;
            range.Value = "Периодичность количественный показатель выполненной работы (оказаной услуги)";
            range.Font.Bold = true;
            range.Font.Size = 8;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 66;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;
          

            range = WS.Cells[startStroka, izmerenie];
            range.ColumnWidth = 5;
            range.Value = "Единица измерения работы (услуги)";
            range.Font.Bold = true;
            range.Font.Size = 8;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 66;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;
          

            range = WS.Cells[startStroka, stoimost];
            range.ColumnWidth = 10;
            range.Font.Size = 8;
            range.Value = "Стоимость/ сметная стоимость выполненной работы(оказанной услуги за единицу)";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 66;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;
       

            range = WS.Cells[startStroka, cena];
            range.ColumnWidth = 10;
            range.Font.Size = 8;
            range.Value = "Цена выполненной работы (услуги) в рублях";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 66;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;

            foreach (VipolnennieUslugi U in CompleteWorks)
            {                
                if (U.StoimostNaM2 + U.StoimostNaMonth != 0)
                {
                    startStroka++;
                    WS.Cells[startStroka, naimenovanie] = U.Usluga.Name;
                    WS.Cells[startStroka, periodichnost] = U.Usluga.Periodichnost.PeriodichnostName;
                    WS.Cells[startStroka, izmerenie] = "Кв.м.";
                    if ((U.Usluga.Name.Contains("ДЕРАТИЗАЦИЯ")) && (Convert.ToDouble(U.StoimostNaM2) < 0.01)) { WS.Cells[startStroka, stoimost] = 0.01; }
                    else { WS.Cells[startStroka, stoimost] = U.StoimostNaM2;}
                    WS.Cells[startStroka, cena] = Convert.ToInt32(U.StoimostNaMonth);
                    range = WS.get_Range("A" + startStroka, "E" + startStroka);
                    range.Font.Size = 8;
                    range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                }
            }

           

            startStroka++;
            from = startStroka;
            int t = Summa.IndexOf('.');
            Summa = Summa.Remove(t);
            WS.Cells[startStroka, 4] = "Итого " ;
            WS.Cells[startStroka, 5] =  Summa ;
            range = WS.get_Range("D" + from, "E" + from);
            //range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;

            from++;
            range = WS.get_Range("A" + from, "E" + from);
            range.RowHeight = 7;

            from = startStroka;
            from+=1;
            startStroka+=1;

           
            WS.Cells[startStroka, 1] = "*Основание (указывается решение общего собрания собственников помещений в многоквартирном доме либо доверенность, дата, номер) прилагается.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 9;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 25;//высота строки
            range.WrapText = true;


            from++;
            WS.Cells[startStroka, 1] = "2. Всего за "+Month +" " + year + "выполнено работ(оказано услуг) на общую сумму " + Summa + "рублей.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 25;//высота строки
            range.WrapText = true;

            startStroka++;
            WS.Cells[startStroka, 1] = "3. Работы(услуги) выполнены(оказаны) полностью, в установленные сроки, с надлежащим качеством.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 25;//высота строки
            range.WrapText = true;


            startStroka++;
            from++;
            WS.Cells[startStroka, 1] = "4. Претензий по выполнению условий Договора Стороны друг к другу не имеют." +
           " Настоящий Акт составлен в 2 - х экземплярах, имеющих одинаковую юридическую силу, по одному для каждой из Сторон.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 35;//высота строки
            range.WrapText = true;



            startStroka++;
            from++;
            WS.Cells[startStroka, 1] = "Подписи Сторон:";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;

            startStroka++;
            from++;
            WS.Cells[startStroka, 1] = "Исполнитель _________________________________________     ____________________";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 18;//высота строки
            range.WrapText = true;

            startStroka++;
            from++;
            WS.Cells[startStroka, 1] = "Заказчик ____________________________________________     ____________________";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 18;//высота строки
            range.WrapText = true;


            // Сохранение файла Excel.

            WbExcel.SaveCopyAs(patch);//сохраняем в папку

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
        public static void SFORMIROVATAKT(List<CompleteWork> CompleteWorks, List<VipolnennieUslugi> VipolnennieUslugi, string Month, string GEU, string Year, string Ulica,string Dom, string Nachalnik, string Prikaz, string patch, string Summa)
        {

            if (VipolnennieUslugi.Count == 0)
            {

                return;
            }


            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = WB.Add(Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо
            ApExcel.StandardFont = "TimesNewRoman";



            int from = 1;

            string GKH = GEU.Remove(0, 3); ;//Номер участка ЖКХ
            string month = Opr.MonthToNorm(Month);
            string year = Year;
            int periodichnost = 2;
            int izmerenie = 3;
            int stoimost = 4;
            int cena = 5;
            int naimenovanie = 1;



            WS.Cells[from, 3] = "УТВЕРЖДЕНО";
            range = WS.get_Range("C" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 3] = " приказом Министерства строительства и жилищно-коммунального хозяйства Российской Федерации от 26.10.2015 №761/пр";
            range = WS.get_Range("C" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 65;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "АКТ №_______";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            /* from++;
             WS.Cells[2, 1] = "ЖИЛИЩНО - ЭКСПЛУАТАЦИОННЫЙ УЧАСТОК №" + GKH;
             range = WS.get_Range("A" + from, "E" + from);
             range.Merge(Type.Missing);
             range.Font.Bold = true;
             range.Font.Size = 13;
             range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.RowHeight = 25;
             range.WrapText = true;


             from++;
             WS.Cells[3, 1] = "";
             range = WS.get_Range("A" + from, "E" + from);
             range.Merge(Type.Missing);
             range.Font.Bold = true;
             range.Font.Size = 12;
             range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.RowHeight = 15;
             range.WrapText = true;
             */
           

            string Ad = Ulica;
            
           
           
            if (Ad.Contains("МОЛОДЕЖИ")|| Ad.Contains("ЛЕОНАРДО")) { Ad = "БУЛЬВАР "+Ad; }
            if (Ad.Contains("МОРСКОЙ")|| Ad.Contains("СТРОИТЕЛЕЙ")) { Ad = Ad+ " ПРОСПЕКТ"; }
            if (Ad.Contains("ДЕТСКИЙ")|| Ad.Contains("ВЕСЕННИЙ")|| Ad.Contains("ЦВЕТНОЙ")) { Ad = Ad + " ПРОЕЗД"; }
            Ad = "ул. " +Ad;
            string Res = " д. " + Dom.Replace(" ","");
            string Adres = Ad + Res;



           


            from++;
            WS.Cells[from, 1] = "приемки оказанных услуг и(или) выполненных работ по содержанию и текущему ремонту общего имущества в многоквартирном доме по адресу " + Adres + " за " + Month + " " + year + " года.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 30;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 1] = "г.Новосибирск";
            range = WS.get_Range("A" + from, "A" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            int M = Opr.MonthObratno(Month);
            M++;
            if (M > 12) { M = 1; }
            string Mon = Opr.MonthToNorm(Opr.MonthOpred(M));
            WS.Cells[from, 3] = "22 " + Mon + " " + Year;
            range = WS.get_Range("C" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 1] = "Собственники помещений в многоквартирном доме, расположенном по адресу:";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 9;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 1] = Adres;
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 1] = "именуемые в дальнейшем 'Заказчик' в лице ______________________________ ";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 2] = "(указывается Ф.И.О. уполномоченного собственника помещения в многоквартирном доме либо председателя Совета многоквартирного дома)";
            range = WS.get_Range("B" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 6;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;


            from++;
            WS.Cells[from, 1] = "являющегося собственником квартиры №_____, находящейся в данном многоквартирном доме*, " +
                "с одной стороны, и Федеральное государственное унитарное предприятие 'Жилищно-коммунальное хозяйство Новосибирского научного центра' (ФГУП 'ЖКХ ННЦ')" +
"именуемое в дальнейшем “Исполнитель”,  в лице начальника ЖЭУ-" + GKH + " " + Nachalnik + ", действующего на основании доверенности №" + Prikaz;
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 65;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 1] = "с другой стороны, совместно именуемые “Стороны”, составили настоящий Акт о нижеследующем:";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;

            from++;
            WS.Cells[from, 1] = "1. Исполнителем предъявлены к приемке следующие оказанные на основании договора управления многоквартирным домом услуги и(или)" +
"выполненные работы по содержанию и текущему ремонту общего имущества в многоквартирном доме, расположенном по адресу " + Adres;
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 50;//высота строки
            range.WrapText = true;

            from++;
            range = WS.get_Range("A" + from, "E" + from);
            range.RowHeight = 7;

            int startStroka = from + 1;
            range = WS.Cells[startStroka, naimenovanie];//столбец номер ширина
            range.ColumnWidth = 37;
            range.Font.Size = 10;
            range.Value = "Наименование вида работы";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 66;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;



            range = WS.Cells[startStroka, periodichnost];
            range.ColumnWidth = 13;
            range.Value = "Периодичность количественный показатель выполненной работы (оказаной услуги)";
            range.Font.Bold = true;
            range.Font.Size = 8;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 66;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;


            range = WS.Cells[startStroka, izmerenie];
            range.ColumnWidth = 4;
            range.Value = "Ед. изм. раб. (усл.)";
            range.Font.Bold = true;
            range.Font.Size = 8;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 66;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;


            range = WS.Cells[startStroka, stoimost];
            range.ColumnWidth = 10;
            range.Font.Size = 8;
            range.Value = "Стоим./ сметн. стоимость выполненной работы(оказ. услуги за ед.)";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 66;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;


            range = WS.Cells[startStroka, cena];
            range.ColumnWidth = 9;
            range.Font.Size = 8;
            range.Value = "Цена выполненной работы (услуги) в рублях";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 66;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;

            foreach (VipolnennieUslugi U in VipolnennieUslugi)
            {
                if (U.StoimostNaM2 + U.StoimostNaMonth != 0)
                {
                    startStroka++;
                    WS.Cells[startStroka, naimenovanie] = U.Usluga.Name;
                    if (U.Usluga.Name.Replace(" ", "").Length>50|| U.Usluga.Periodichnost.PeriodichnostName.Replace(" ","").Length > 20)
                    {
                        WS.Cells[startStroka, naimenovanie].RowHeight = 25;
                        WS.Cells[startStroka, naimenovanie].WrapText = true;
                        WS.Cells[startStroka, periodichnost].WrapText = true;
                    }
                    WS.Cells[startStroka, periodichnost] = U.Usluga.Periodichnost.PeriodichnostName;
                    WS.Cells[startStroka, izmerenie] = "кв.м.";
                    if ((U.Usluga.Name.Contains("ДЕРАТИЗАЦИЯ")) && (Convert.ToDouble(U.StoimostNaM2) < 0.01)) { WS.Cells[startStroka, stoimost] = 0.01; }
                    else { WS.Cells[startStroka, stoimost] = U.StoimostNaM2; }
                    WS.Cells[startStroka, cena] = Convert.ToInt32(U.StoimostNaMonth);
                    range = WS.get_Range("A" + startStroka, "E" + startStroka);
                    range.Font.Size = 8;
                    range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                }
            }



            startStroka++;
            from = startStroka;
            int t = Summa.IndexOf('.');
            Summa = Summa.Remove(t);
            WS.Cells[startStroka, 4] = "Итого ";
            WS.Cells[startStroka, 5] = Summa;
            range = WS.get_Range("D" + from, "E" + from);
            //range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;

            from++;
            range = WS.get_Range("A" + from, "E" + from);
            range.RowHeight = 7;

            from = startStroka;
            from += 2;
            startStroka += 2;

           
            WS.Cells[startStroka, 1] = "*Основание (указывается решение общего собрания собственников помещений в многоквартирном доме либо доверенность, дата, номер) прилагается.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 7;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 30;//высота строки
            range.WrapText = true;

            from++;
            startStroka++;
            WS.Cells[startStroka, 1] = "2. Всего за " + Month + " " + year + " выполнено работ (оказано услуг) на общую сумму " + Summa + " рублей.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 25;//высота строки
            range.WrapText = true;

            from++;
            startStroka++;
            WS.Cells[startStroka, 1] = "3. Работы(услуги) выполнены(оказаны) полностью, в установленные сроки, с надлежащим качеством.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 16;//высота строки
            range.WrapText = true;


            startStroka++;
            from++;
            WS.Cells[startStroka, 1] = "4. Претензий по выполнению условий Договора Стороны друг к другу не имеют." +
           " Настоящий Акт составлен в 2 - х экземплярах, имеющих одинаковую юридическую силу, по одному для каждой из Сторон.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 35;//высота строки
            range.WrapText = true;



            startStroka++;
            from++;
            WS.Cells[startStroka, 1] = "Подписи Сторон:";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;

            startStroka++;
            from++;
            WS.Cells[startStroka, 1] = "Исполнитель _________________________________________     ____________________";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 18;//высота строки
            range.WrapText = true;

            startStroka++;
            from++;
            WS.Cells[startStroka, 1] = "Заказчик ____________________________________________     ____________________";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 18;//высота строки
            range.WrapText = true;

            //Формируем приложение к акту !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

         

                if (CompleteWorks.Count == 0)
                {
                CompleteWork CW = new CompleteWork();
                CW.WorkAdress = "Нет адреса";
                CW.WorkGroup = "Нет группы";
                CompleteWorks.Add(CW);

                }

            

            WS.Name = "Акт";
            ApExcel.Worksheets.Add(Type.Missing);//Добавляем лист
            WS = WbExcel.Sheets[1];
                WS.Name = "Приложение";
                ApExcel.Visible = false;//невидимо
                ApExcel.ScreenUpdating = false;//и не обновляемо
                from = 1;

                GKH = GEU.Remove(0, 4); ;//Номер участка ЖКХ
                month = Month;
                year = Year;
               // int num = 1;
               // int adres = 2;
                int vid = 1;

                int kolvo = 2;
                izmerenie = 3;
                int data = 4;


                WS.Cells[1, 1] = "Приложение к акту №_______";
                range = WS.get_Range("A" + from, "D" + from);
                range.Merge(Type.Missing);
                range.Font.Bold = true;
                range.Font.Size = 13;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.RowHeight = 50;
                range.WrapText = true;
                range.Font.Name = "TimesNewRoman";
            /* from++;
             WS.Cells[2, 1] = "ЖИЛИЩНО - ЭКСПЛУАТАЦИОННЫЙ УЧАСТОК №" + GKH;
             range = WS.get_Range("A" + from, "E" + from);
             range.Merge(Type.Missing);
             range.Font.Bold = true;
             range.Font.Size = 13;
             range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.RowHeight = 25;
             range.WrapText = true;


             from++;
             WS.Cells[3, 1] = "";
             range = WS.get_Range("A" + from, "E" + from);
             range.Merge(Type.Missing);
             range.Font.Bold = true;
             range.Font.Size = 12;
             range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
             range.RowHeight = 15;
             range.WrapText = true;
             */

            from++;
                WS.Cells[2, 1] = "приемки оказанных услуг и(или) выполненных работ по содержанию и текущему ремонту общего имущества в многоквартирном доме по адресу " + Adres + " за " + month + " " + year + ".";
                range = WS.get_Range("A" + from, "D" + from);
                range.Merge(Type.Missing);
                range.Font.Bold = true;
                range.Font.Size = 11;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.RowHeight = 50;//высота строки
                range.WrapText = true;
            from++;
            startStroka = from;
            /*
                startStroka = 3;
                range = WS.Cells[startStroka, num];//столбец номер ширина
                range.ColumnWidth = 2;
                range.Value = "N";
                range.Font.Bold = true;
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                range = WS.Cells[startStroka, adres];
                range.ColumnWidth = 25;
                range.Value = "Адрес";
                range.Font.Bold = true;
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
*/
                range = WS.Cells[startStroka, vid];
                range.ColumnWidth = 55;
                range.Value = "Вид работ";
                range.Font.Bold = true;
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                range = WS.Cells[startStroka, kolvo];
                range.ColumnWidth = 5;
                range.Value = "Кол.";
                range.Font.Bold = true;
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                range = WS.Cells[startStroka, izmerenie];
                range.ColumnWidth = 5;
                range.Value = "Ед.";
                range.Font.Bold = true;
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                range = WS.Cells[startStroka, data];
                range.ColumnWidth = 7;
                range.Value = "Дата";
                range.Font.Bold = true;
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                List<string> Homes = new List<string>();//запишем сюда все дома из массива

            WS.PageSetup.LeftMargin = 72;//1 дюйм 72 поинта
            WS.PageSetup.RightMargin = 30;//1 дюйм 72 поинта
            WS.PageSetup.TopMargin = 30;//1 дюйм 72 поинта
            WS.PageSetup.BottomMargin = 30;//1 дюйм 72 поинта
            WS.PageSetup.PaperSize = Excel.XlPaperSize.xlPaperA4;
               Homes.Add(CompleteWorks[0].WorkAdress);
               foreach (CompleteWork CW in CompleteWorks)
                {
                
                for (int i = 0; i < Homes.Count; i++)
                {
                    if (Homes[i].Replace(" ","").Equals(CW.WorkAdress.Replace(" ",""))==false)//если дома не содержат такого адреса то добавим его 
                    {
                        //тут добавляем адреса в дом
                        Homes.Add(CW.WorkAdress);
                    }
                }
                }
                Homes.Sort();
                int c = 0;
                foreach (string H in Homes)
                {
                    c++;
                //тут пишем в экселе адрес дома
                string adresdoma = Homes[c - 1];

                if (adresdoma.Contains("МОЛОДЕЖИ")) { adresdoma = "БУЛЬВАР " + adresdoma; }
                if (adresdoma.Contains("МОРСКОЙ")) { adresdoma = adresdoma + " ПРОСПЕКТ"; }
                if (adresdoma.Contains("ДЕТСКИЙ") || adresdoma.Contains("ВЕСЕННИЙ") || adresdoma.Contains("ЦВЕТНОЙ")) { adresdoma = adresdoma + " ПРОЕЗД"; }
              //  WS.Cells[startStroka + 1, adres] = adresdoma;
               //     WS.Cells[startStroka + 1, num] = c;
                 
                    int f = startStroka;
                    int[] counter = new int[3];
                    List<CompleteWork>[] Sortirovka = new List<CompleteWork>[2];
                    for (int d = 0; d < 2; d++)
                    {
                        Sortirovka[d] = new List<CompleteWork>();
                    }

                    string[] g = new string[] { "ТО конструктивных элементов", "ТО внутридомового инженерного оборудования" };
                    string[] g2 = new string[] { "Конструктивные элементы", "Внутридомовое инженерное оборудование" };
                    foreach (CompleteWork CW in CompleteWorks)
                    {

                        string adress1 = CW.WorkAdress.Replace(" ", "");
                        string adress2 = Homes[c - 1].Replace(" ", "");



                        if (adress1.Equals(adress2))//если в этом адресе несколько работ то сортируем по дате. Записываем в массив сортировки.
                        {
                            counter[0]++;
                            //startStroka++;


                            string group2 = CW.WorkGroup.Replace(" ", "");
                            if (group2.Equals(g[0].Replace(" ", "")))
                            {
                                Sortirovka[0].Add(CW);
                                counter[1]++;
                            }
                            if (group2.Equals(g[1].Replace(" ", "")))
                            {
                                counter[2]++;
                                Sortirovka[1].Add(CW);
                            }

                            //  WS.Cells[startStroka, num] = c;
                        }
                    }
                    int froms = 0;
                    startStroka = from ;
                    for (int l = 1; l < 3; l++)
                    {
                        if (l == 1) { froms = startStroka + 1; }
                        startStroka++;
                        WS.Cells[startStroka, vid] = g2[l - 1];
                        range = WS.get_Range("A" + startStroka, "D" + startStroka);
                        range.Merge();
                        range.Font.Bold = true;
                        range.RowHeight = 12;
                        range.WrapText = true;

                        if (counter[l] == 1)
                        {
                            startStroka++;
                            WS.Cells[startStroka, vid] = Sortirovka[l - 1][0].WorkName;
                           
                        WS.Cells[startStroka, kolvo] = Sortirovka[l - 1][0].WorkNumber;
                            WS.Cells[startStroka, izmerenie] = Sortirovka[l - 1][0].WorkIzmerenie;
                        
                            string MONN = Sortirovka[l - 1][0].WorkDate.Month.ToString();
                            if (MONN.Length < 2)
                            {
                                MONN = "0" + MONN;
                            }
                            WS.Cells[startStroka, data] = MONN + "-" + Sortirovka[l - 1][0].WorkDate.Day;
                        }
                        else
                        {
                            if (counter[l] > 1)
                            {
                                List<CompleteWork> Sort2 = new List<CompleteWork>();
                                for (int i = Sortirovka[l - 1].Count - 1; i > 0; i--)
                                {
                                    CompleteWork x = Sortirovka[l - 1][i];
                                    for (int j = Sortirovka[l - 1].Count - 2; j > -1; j--)
                                    {
                                        if (x.WorkDate > Sortirovka[l - 1][j].WorkDate)
                                        {
                                            x = Sortirovka[l - 1][j];
                                        }
                                    }
                                    Sort2.Add(x);
                                    Sortirovka[l - 1].Remove(x);

                                }
                                Sort2.Add(Sortirovka[l - 1][0]);

                                foreach (CompleteWork CW in Sort2)
                                {
                                    startStroka++;
                                    WS.Cells[startStroka, vid] = CW.WorkName;
                                if (CW.WorkName.Replace(" ","").Length > 40)
                                {
                                    WS.Cells[startStroka, vid].RowHeight = 27;
                                    WS.Cells[startStroka, vid].WrapText = true;
                                }
                                    WS.Cells[startStroka, kolvo] = CW.WorkNumber;
                                    WS.Cells[startStroka, izmerenie] = CW.WorkIzmerenie;
                                    string MONN = CW.WorkDate.Month.ToString();
                                    if (MONN.Length < 2)
                                    {
                                        MONN = "0" + MONN;
                                    }
                                    WS.Cells[startStroka, data] = MONN + "-" + CW.WorkDate.Day;
                                }
                                int tos = startStroka;
                              //  range = WS.get_Range("B" + froms.ToString(), "B" + tos.ToString());
                              //  range.Merge();
                             //   range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                             //   range = WS.get_Range("A" + froms.ToString(), "A" + tos.ToString());
                              //  range.Merge();
                              //  range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                            }

                        }
                    }

                    t = startStroka;
                    range = WS.get_Range("A" + f, "D" + t);
                    range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;



                }

            startStroka++;
            from = startStroka;

            startStroka++;
            from++;
            WS.Cells[startStroka, 1] = "Подписи Сторон:";
            range = WS.get_Range("A" + from, "D" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;

            startStroka++;
            from++;
            WS.Cells[startStroka, 1] = "Исполнитель _________________________________________     ____________________";
            range = WS.get_Range("A" + from, "D" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 18;//высота строки
            range.WrapText = true;

            startStroka++;
            from++;
            WS.Cells[startStroka, 1] = "Заказчик ____________________________________________     ____________________";
            range = WS.get_Range("A" + from, "D" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 18;//высота строки
            range.WrapText = true;




            // Сохранение файла Excel.
            try
            {
                if (File.Exists(patch)) { File.Delete(patch); }
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
            Marshal.FinalReleaseComObject(WS);
            Marshal.FinalReleaseComObject(range);
            Marshal.FinalReleaseComObject(WbExcel);
            Marshal.FinalReleaseComObject(WB);



            GC.Collect();
            Marshal.FinalReleaseComObject(ApExcel);
            GC.WaitForPendingFinalizers();
 

            CloseProcess();
        }
        public static void CloseProcess()
        {
            Process[] List;
            List = Process.GetProcessesByName("EXCEL");
            foreach (Process proc in List)
            {
                try
                {
                    if (DateTime.Now.Hour - proc.StartTime.Hour > 0 || DateTime.Now.Minute - proc.StartTime.Minute > 2)
                    {
                        proc.Kill();
                    }
                }
                catch { }
            }
        }
    }
}