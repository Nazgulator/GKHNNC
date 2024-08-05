using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using GKHNNC.DAL;
using GKHNNC.Models;
using Opredelenie;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Web.UI;
using DocumentFormat.OpenXml.Spreadsheet;



namespace GKHNNC.Controllers
{
    public class ExcelExportMonth
    {
        public int ver = 0;

       

        public static void EXPORT(List<CompleteWork> CompleteWorks,string Month, string GEU,string Year,string EU) {

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
           

            WS.Cells[1, 1] = "ФЕДЕРАЛЬНОЕ БЮДЖЕТНОЕ ГОСУДАРСТВЕННОЕ УЧРЕЖДЕНИЕ 'ЖИЛИЩНО-КОММУНАЛЬНОЕ УПРАВЛЕНИЕ НОВОСИБИРСКОГО НАУЧНОГО ЦЕНТРА'";
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
            WS.Cells[4, 1] = "Отчет о выполненной работе по техническому обслуживанию конструктивных элементов и техническому обслуживанию внутридомового инженерного оборудования ЭУ № " + EU+"ФГБУ 'ЖКУ ННЦ' за "+ month+" " + year+".";
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



        public static void StandartExport(List<string> Stolbci, List<List<string>> Stroki, List<string> Shapka, string path)
        {

            
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = ApExcel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо
            int stroka = 1;
            //шапка
            for (int i=0;i<Shapka.Count;i++)
            {
                WS.Cells[stroka, 1] = Shapka[i];
                range = WS.get_Range("A" + stroka.ToString(), Opr.OpredelenieBukvi(Stolbci.Count) + stroka.ToString());
                range.Merge(Type.Missing);
                range.Font.Bold = true;
                range.Font.Size = 15;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.RowHeight = 25;
                range.WrapText = true;
                stroka++;
            }
            //заголовки
            for (int i=0;i<Stolbci.Count;i++)
            {
                WS.Cells[stroka, i + 1] = Stolbci[i];
            }
            range = WS.get_Range("A" + stroka.ToString(), Opr.OpredelenieBukvi(Stolbci.Count) + stroka.ToString());
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;
            range.WrapText = true;
            int strokaOld = stroka;
            stroka++;
         
          

            //строки

            for (int j = 0; j < Stroki.Count; j++)
            {
                for (int i = 0; i < Stroki[j].Count; i++)
                {
                    WS.Cells[stroka, i + 1] = Stroki[j][i];
                }
                stroka++;
            }

            range = WS.get_Range("A" + strokaOld.ToString(), Opr.OpredelenieBukvi(Stolbci.Count) + (stroka).ToString());
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;
            range.Columns.AutoFit();
            range.WrapText = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;



            // Сохранение файла Excel.

            WbExcel.SaveCopyAs(path);//сохраняем в папку

            ApExcel.Visible = true;//видимо
            ApExcel.ScreenUpdating = true;//обновляемо
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
        public static List<List<string>> IMPORT(string FilePatch, string[] Names, out string Error, string Vkladka = "1", int[] X = null,bool delete=true )
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
                        string EXL = "";
                        try
                        {
                            EXL = EXX[i, j].ToString();
                        }
                        catch { }
                        if (EXL.Equals("") == false)
                        {
                            string W = "";
                            try
                            {
                                W = EXL.ToString().Replace(" ", "").ToUpper();
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

                                        if (xx) {
                                            goto ShapkaNaidena; }//если все заголовки нашли то выходим
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
            if (delete) { File.Delete(FilePatch); }


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
            while (WS.Cells[k,stNumber].Interior.Color != ColorTranslator.ToOle(System.Drawing.Color.White))
            {
                //Идем вниз по строкам и если первая ячейка не белая и в ней есть цифра то заносим ее в базу.
                string C = WS.Cells[k, stNumber].Text;
                try
                {
                    C = Convert.ToInt32(C).ToString();
                    if (WS.Cells[k, stNumber].Font.Color != ColorTranslator.ToOle(System.Drawing.Color.Black))
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
        static private WorkContext db = new WorkContext();
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
                "с одной стороны, и Федеральное государственное бюджетное учреждение 'Академия комфорта' (ФГБУ 'Академия комфорта')" +
"именуемое в дальнейшем “Исполнитель”,  в лице начальника ЭУ-" + GKH + " " + Nachalnik + ", действующего на основании доверенности №" + Prikaz;
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

        //Акт осеннего и весеннего осмотра
        public static void ActOsmotra(List<ActiveElement> ActiveElements, Osmotr O, List<DOMPart> DP,int GEU,string path)
        {

            if (ActiveElements.Count == 0)
            {

                return;
            }

            PrintConstant Dir = db.PrintConstants.Where(x => x.Poisk.Equals("Директор")).First();
            PrintConstant MainEngineer = db.PrintConstants.Where(x => x.Poisk.Equals("Главный инженер")).First();
            PrintConstant ZamOEGF = db.PrintConstants.Where(x => x.Poisk.Equals("Заместитель директора ОЭЖФ")).First();
            PrintConstant PredstavitelUO = db.PrintConstants.Where(x => x.Poisk.Equals("ПредставительУО")).First();
            PrintConstant Predsedatel = db.PrintConstants.Where(x => x.Poisk.Equals("Председатель")).First();


            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = WB.Add(Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо
            ApExcel.StandardFont = "TimesNewRoman";
            WS.Name = "1. Акт";


            int from = 1;

           
            string month = Opr.MonthToNorm(Opr.MonthOpred(O.Date.Month));
            string year = O.Date.Year.ToString();
            int periodichnost = 2;
            int izmerenie = 3;
            int stoimost = 4;
            int cena = 5;
            int naimenovanie = 1;



            WS.Cells[from, 3] = "УТВЕРЖДАЮ: ";
            range = WS.get_Range("C" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            WS.Cells[from, 1] = "СОГЛАСОВАНО: ";
            range = WS.get_Range("A" + from, "A" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 3] = ZamOEGF.Dolgnost +"  _______________("+ZamOEGF.Name+")";
            range = WS.get_Range("C" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 40;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            WS.Cells[from, 1] = MainEngineer.Dolgnost +"  __________("+MainEngineer.Name+")";
            range = WS.get_Range("A" + from, "B" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 40;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 3] = " ___________________ "+O.Date.Year;
            range = WS.get_Range("C" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            WS.Cells[from, 1] = " ___________________ " + O.Date.Year;
            range = WS.get_Range("A" + from, "A" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

           
            from++;
            WS.Cells[from, 2] = "Акт весеннего осмотра №" + O.Id+" от "+"15.05."+(O.Date.Year);//ToString("dd.MM.yy")
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            string Ad = O.Adres.Ulica;
            bool AddUl = false;
            if (Ad.Contains("МОЛОДЕЖИ") || Ad.Contains("ЛЕОНАРДО")) { Ad = "БУЛЬВАР " + Ad; AddUl = true; }
            if (Ad.Contains("МОРСКОЙ") || Ad.Contains("СТРОИТЕЛЕЙ")) { Ad = Ad + " ПРОСПЕКТ"; AddUl = true; }
            if (Ad.Contains("ДЕТСКИЙ") || Ad.Contains("ВЕСЕННИЙ") || Ad.Contains("ЦВЕТНОЙ")) { Ad = Ad + " ПРОЕЗД"; AddUl = true; }
            if (!AddUl)
            {
                Ad = "ул."+ Ad;
            }
            
            string Res = " д. " + O.Adres.Dom.Replace(" ", "");
            string Adres = Ad+ Res;

            from++;
            WS.Cells[from, 2] = "по обследованию многоквартирного дома ";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] =  Adres;
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

          

            from++;
            range = WS.get_Range("A" + from, "E" + from);
            range.RowHeight = 7;

            int startStroka = from + 1;
            range = WS.Cells[startStroka, naimenovanie];//столбец номер ширина
            range.ColumnWidth = 20;
            range.Font.Size = 8;
            range.Value = "Конструктивный элемент";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;



            range = WS.Cells[startStroka, periodichnost];
            range.ColumnWidth = 13;
            range.Value = "Материал";
            range.Font.Bold = true;
            range.Font.Size = 8;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;


            range = WS.Cells[startStroka, izmerenie];
            range.ColumnWidth = 6;
            range.Value = "Ед. изм.";
            range.Font.Bold = true;
            range.Font.Size = 8;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;


            range = WS.Cells[startStroka, stoimost];
            range.ColumnWidth = 5.5;
            range.Font.Size = 8;
            range.Value = "Кол-во";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;


            range = WS.Cells[startStroka, cena];

           
            range.Font.Size = 8;
            range.Value = "Состояние";
            range.Font.Bold = true;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;
            

            foreach (DOMPart P in DP)
            {
                
                List<ActiveElement> AE = ActiveElements.Where(x=>x.Element.ElementTypeId==P.Id).ToList();
                if (AE.Where(x => x.Est).Count() > 0) //Если нет элементов то не выводим в экселе данный раздел
                {
                    startStroka++;
                    WS.Cells[startStroka, naimenovanie] = P.Name;
                    range = WS.get_Range("A" + startStroka, "E" + startStroka);
                    range.Font.Size = 12;
                    range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    range.Merge();

                    foreach (ActiveElement A in AE)
                    {
                        if (A.Est != false)
                        {
                            startStroka++;

                            WS.Cells[startStroka, naimenovanie] = A.Element.Name;
                            if (A.Element.Name.Replace(" ", "").Length > 50)
                            {
                                WS.Cells[startStroka, naimenovanie].RowHeight = 25;
                                WS.Cells[startStroka, naimenovanie].WrapText = true;
                                WS.Cells[startStroka, periodichnost].WrapText = true;
                            }
                            WS.Cells[startStroka, periodichnost] = A.Material.Name;
                            WS.Cells[startStroka, izmerenie] = A.Izmerenie.Name;
                            WS.Cells[startStroka, stoimost] = A.Kolichestvo;

                            WS.Cells[startStroka, cena] = sostOpred(A.Sostoyanie);

                            range = WS.get_Range("A" + startStroka, "E" + startStroka);
                            range.Font.Size = 8;
                            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            //теперь ищем дефекты 
                            List<ActiveDefect> AD = new List<ActiveDefect>();
                            try
                            {
                                AD = db.ActiveDefects.Where(x => x.OsmotrId == O.Id && x.ElementId == A.ElementId).Include(x => x.Defect).Include(x => x.Element).ToList();
                            }
                            catch { }
                            if (AD.Count > 0)
                            {
                                foreach (ActiveDefect D in AD)
                                {
                                    startStroka++;
                                    WS.Cells[startStroka, 1] = "      " + D.Defect.Def;
                                    WS.Cells[startStroka, 2] = "Дефект";
                                    WS.Cells[startStroka, 3] = D.Opisanie;
                                    range = WS.get_Range("C" + startStroka, "E" + startStroka);
                                    range.Merge();

                                    range = WS.get_Range("A" + startStroka, "E" + startStroka);
                                    range.Font.Size = 7;
                                    range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    range.Interior.Color = System.Drawing.Color.LightGray;

                                }
                            }


                        }
                    }
                }
            }

            //Определяем жэу
            GEU G = new GEU();
            string eu = "ЭУ1";
            try
            {
                G = db.GEUs.Where(x => x.Name.Contains(GEU.ToString())).First();
            }
            catch
            {
                G = db.GEUs.First();
            }

            try
            {
                eu = "ЭУ" + G.EU;//db.EU.Where(x => x.Id == G.EU).First();
            }
            catch
            {

            }


         

            startStroka++;
            startStroka++;
            WS.Cells[startStroka, 1] = "В ходе осмотра выявлено:";
            range = WS.Cells[startStroka, periodichnost];
            range.ColumnWidth = 20;

            range.Font.Bold = true;
            range.Font.Size = 8;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;

            startStroka++;
            range = WS.get_Range("A" + (startStroka), "E" + (startStroka));
            //  range.ColumnWidth = 13;
            WS.Cells[startStroka, 1] = "Выводы комиссии:";
            range.Merge();

            startStroka++;
            range = WS.get_Range("A" + (startStroka), "E" + (startStroka));
            //range.RowHeight = 30;
            range.WrapText = true;
            WS.Cells[startStroka, 1] = O.Vivods;
            range.RowHeight = 1;
            // range.EntireRow.AutoFit();
            try
            {
                range.RowHeight = (Math.Truncate(Convert.ToDecimal(O.Vivods.Length) / 60) + 1) * 18 + 1;
            }
            catch
            {

            }
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Merge();

            startStroka++;
            range = WS.get_Range("A" + (startStroka), "E" + (startStroka));
           // range.ColumnWidth = 20;
            WS.Cells[startStroka, 1] = "___________________________________________________________________________";
           
            range.Font.Bold = true;
            range.Font.Size = 8;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;
            range.Merge();

            startStroka++;
            range = WS.get_Range("A" + (startStroka), "E" + (startStroka));
    
            WS.Cells[startStroka, 1] = "___________________________________________________________________________";
            range.Font.Bold = true;
            range.Font.Size = 8;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;
            range.Merge();

            startStroka++;
            range = WS.get_Range("A" + (startStroka), "E" + (startStroka));

            WS.Cells[startStroka, 1] = "___________________________________________________________________________";
            range.Font.Bold = true;
            range.Font.Size = 8;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;
            range.Merge();

            startStroka++;
            range = WS.get_Range("A" + (startStroka), "E" + (startStroka));
             WS.Cells[startStroka, 1] = "___________________________________________________________________________";
            range.Font.Bold = true;
            range.Font.Size = 8;
    
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;
            range.Merge();

            startStroka++;
            range = WS.get_Range("A" + (startStroka), "E" + (startStroka));
            WS.Cells[startStroka, 1] = "___________________________________________________________________________";
            range.Font.Bold = true;
            range.Font.Size = 8;
    
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.HorizontalAlignment = Excel.Constants.xlLeft;
            range.Merge();

            startStroka++;
            from = startStroka;
            WS.Cells[startStroka, 1] = "Члены коммиссии:";
            range = WS.get_Range("A" + from, "A" + from);
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.RowHeight = 25;//высота строки
            startStroka++;
            WS.Cells[startStroka, 5] = "1._______________("+G.DirectorIP+")"+G.DirectorDolgnost;//+ eu
          //  WS.Cells[startStroka+1, 5] = "2._______________(___________) "+PredstavitelUO.Dolgnost;
            WS.Cells[startStroka+2, 5] = "2._______________(___________) "+Predsedatel.Dolgnost;
            WS.Cells[startStroka+3, 5] = "3._______________("+G.IngenerOEGF+") " + G.IngenerOEGFDolgnost;
            for (int i = 0; i < 4; i++)
            {
                range = WS.get_Range("A" + (startStroka + i), "E" + (startStroka + i));
                //range.Merge(Type.Missing);
                range.Font.Size = 10;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.Merge();
                range.RowHeight = 25;//высота строки
            }
            WS.Cells[startStroka, 5].ColumnWidth = 18;





            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!222222222222222222222222222222222222222!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Делаем 2 лист
            //  string[] Podpisi = new string[7] { "Пальцев Е.К.", "", "", "", "Полянских Э.В.", "Ширяев С.В.", "Сухов Е.А." };
            // ApExcel.Worksheets.Add();
            ApExcel.Worksheets.Add(Type.Missing);//Добавляем лист
            WS = WbExcel.Sheets[1];
            WS.Name = "2. Работы по ДТР";

            startStroka = 1;
            WS.Cells[startStroka, 5] = " Утверждаю:";
            range = WS.get_Range("E" + startStroka, "G" + startStroka);
            //  Opr.RangeMerge(ApExcel, range, true, true, 10, 15);
            range.Merge();
            range.Font.Size = 10;
            range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            startStroka++;
            WS.Cells[startStroka, 5] = Dir.Dolgnost;
            range = WS.get_Range("E" + startStroka, "G" + startStroka);
            //  Opr.RangeMerge(ApExcel, range, true, true, 13, 15);
            range.Merge();
            range.Font.Size = 10;
            WS.Cells[startStroka, 5].ColumnWidth  = 34;
            range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            startStroka++;
            WS.Cells[startStroka, 5] = " ____________________"+Dir.Name;
            range = WS.get_Range("E" + startStroka, "G" + startStroka);
            //  Opr.RangeMerge(ApExcel, range, true, true, 13, 15);
            range.Merge();
            range.Font.Size = 10;
            range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            startStroka++;
            WS.Cells[startStroka, 5] = " '___'________________" + (O.Date.Year) + "г.";
            range = WS.get_Range("E" + startStroka, "G" + startStroka);
            //  Opr.RangeMerge(ApExcel, range, true, true, 13, 15);
            range.Merge();
            range.Font.Size = 10;
            range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            startStroka++;

            // range = WS.get_Range("A" + 1, "H" + startStroka);
            // range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;

            WS.Cells[startStroka, 1] = "3. Работы по дополнительному текущему ремонту, определение их стоимости и размера платы за дополнительный текущий ремонт на " + O.Date.Year + "-" + (O.Date.Year + 1).ToString() + " год по адресу " + Adres;//O.Adres.Ulica + " " + O.Adres.Dom;
            range = WS.get_Range("A" + startStroka, "G" + startStroka);
            Opr.RangeMerge(ApExcel, range, true, true, 13, 50);


            startStroka++;

            range = WS.get_Range("A" + startStroka, "G" + startStroka);
            WS.Cells[startStroka, 1] = "№ п/п"; WS.Cells[startStroka, 1].ColumnWidth = 5;
            WS.Cells[startStroka, 2] = "Виды работ"; WS.Cells[startStroka, 2].ColumnWidth = 50;
            WS.Cells[startStroka, 3] = "Ед. изм."; WS.Cells[startStroka, 3].ColumnWidth = 6.25;
            WS.Cells[startStroka, 4] = "Объёмы работ"; WS.Cells[startStroka, 3].ColumnWidth = 8.5;
            WS.Cells[startStroka, 5] = "Стоимость работ, включая организационно-контрольные работы УО, руб."; WS.Cells[startStroka, 5].ColumnWidth = 22.5;
            //   WS.Cells[startStroka, 6] = "Вознаграждение УК за выполнение работ по доп. текущему ремонту"; WS.Cells[startStroka, 6].ColumnWidth = 32;
            WS.Cells[startStroka, 6] = "Стоимость работ на кв.м  в месяц, руб."; WS.Cells[startStroka, 6].ColumnWidth = 15;
            WS.Cells[startStroka, 7] = "Срок выполнения ***"; WS.Cells[startStroka, 7].ColumnWidth = 12;
     
            Opr.RangeMerge(ApExcel, range, false, true, 10, 45);

            int counter = 0;
            decimal ActivePloshad = db.Adres.Where(x => x.Id == O.AdresId).Select(x => x.ActivePloshad).First();
            decimal summa = 0;

            DateTime DateStart = new DateTime(1, 1, 1);
            var AOW = new List<ActiveOsmotrWork>();
             var Elements = new List<int>();

            try
            {
                AOW = db.ActiveOsmotrWorks.Where(x => x.OsmotrId == O.Id && !x.Gotovo && x.OsmotrWork.OtchetId == 0&&(x.DateZaplanirovana==DateStart||x.DateZaplanirovana==null)).OrderBy(x => x.OsmotrWork.DOMPartId).Include(x => x.OsmotrWork).Include(x => x.OsmotrWork.Izmerenie).ToList();//.Include(x=>x.OsmotrWork.DOMPart)
                Elements = AOW.Select(x => x.ElementId).Distinct().ToList();
            }
            catch
            {

            }


            var ORK = new List<OsmotrRecommendWork>();

            try
            {
                ORK = db.OsmotrRecommendWorks.Where(x => x.OsmotrId == O.Id && !x.Gotovo).OrderBy(x => x.Name).Include(x => x.Izmerenie).Include(x => x.DOMPart).ToList();

            }
            catch
            {

            }
            if (ActivePloshad == 0) { ActivePloshad = 1; }






            for (int i = 0; i < DP.Count; i++)//Elements.Count();
            {
                List<ActiveOsmotrWork> AOW2 = AOW.Where(x => x.OsmotrWork.DOMPartId == DP[i].Id).ToList();//.Where(x => x.ElementId == Elements[i]
                if (AOW2.Count == 0&& ORK.Where(x => x.DOMPartId == DP[i].Id).Count() == 0)
                {
                    continue;
                }
                    startStroka++;


              // int idd = AOW2[0].OsmotrWork.DOMPartId;
                string DomPart = DP[i].Name;// db.DOMParts.Where(x => x.Id == idd).Select(x => x.Name).First();
                WS.Cells[startStroka, 1] = DomPart;
                range = WS.get_Range("A" + startStroka, "G" + startStroka);
                //range.Merge();
                Opr.RangeMerge(ApExcel, range, true, true, 13, 20);

                foreach (ActiveOsmotrWork A in AOW2)
                {
                    decimal stavka = 1.1m;
                    if (A.TotalCost >= 50000)
                    {
                        stavka = 1.05m;
                    }
                    if (A.TotalCost >= 100000)
                    {
                        stavka = 1.03m;
                    }
                    counter++;
                    startStroka++;
                    WS.Cells[startStroka, 1] = counter;
                    WS.Cells[startStroka, 2] = A.OsmotrWork.Name;//+ "("+A.OsmotrWork.DOMPart.Name+")";
                    if (A.OsmotrWork.Name.Length > 44)
                    {
                        range = WS.get_Range("A" + startStroka, "G" + startStroka);
                        range.RowHeight = 29;//высота строки
                        range.WrapText = true;

                    }
                    decimal TC = Math.Round(A.TotalCost * stavka, 2);
                    WS.Cells[startStroka, 3] = A.OsmotrWork.Izmerenie.Name;
                    WS.Cells[startStroka, 4] = A.Number;
                    WS.Cells[startStroka, 5] = TC;
                    //   WS.Cells[startStroka, 6] =Math.Round( A.TotalCost/10,2);
                    WS.Cells[startStroka, 6] = Math.Round(((TC) / 12) / ActivePloshad, 2);
               
                    summa += TC;
                }

                foreach (var ork in ORK.Where(x=>x.DOMPartId == DP[i].Id).ToList())
                {
                    if (ork.Kommisia < 0)
                    {
                        int stavka = 10;
                        if (ork.Cost >= 50000)
                        {
                            stavka = 5;
                        }
                        if (ork.Cost >= 100000)
                        {
                            stavka = 3;
                        }
                        ork.Kommisia = stavka;
                    }
                    decimal FinKom = 1 + Convert.ToDecimal(ork.Kommisia) * 0.01m;//преобразуем коммисию из процентов в коэффициент 1.05 
                    startStroka++;
                    counter++;
                    decimal TC = Math.Round(ork.Cost * FinKom, 2);
                    WS.Cells[startStroka, 1] = counter;
                    WS.Cells[startStroka, 2] = ork.Name;
                    if (ork.Name.Length > 44)
                    {
                        range = WS.get_Range("A" + startStroka, "G" + startStroka);
                        range.RowHeight = 29;//высота строки
                        range.WrapText = true;

                    }
                    WS.Cells[startStroka, 3] = ork.Izmerenie.Name;
                    WS.Cells[startStroka, 4] = ork.Number;
                    WS.Cells[startStroka, 5] = TC;
                    //  WS.Cells[startStroka, 6] = Math.Round(ORK[i].Cost / 10, 2);
                    WS.Cells[startStroka, 6] = Math.Round(((TC) / 12) / ActivePloshad, 2);

                    summa += TC;

                }


            }
            //startStroka++;
            //WS.Cells[startStroka, 1] = "";
            //range = WS.get_Range("A" + startStroka, "G" + startStroka);
            ////range.Merge();
            //Opr.RangeMerge(ApExcel, range, true, true, 13, 20);
            ////заполняем дополнительные работы
            //for (int i = 0; i < ORK.Count; i++)
            //{
            //    if (ORK[i].Kommisia < 0)
            //    {
            //        int stavka = 10;
            //        if (ORK[i].Cost >= 50000)
            //        {
            //            stavka = 5;
            //        }
            //        if (ORK[i].Cost >= 100000)
            //        {
            //            stavka = 3;
            //        }
            //        ORK[i].Kommisia = stavka;
            //    }
            //    decimal FinKom = 1 + Convert.ToDecimal(ORK[i].Kommisia) * 0.01m;//преобразуем коммисию из процентов в коэффициент 1.05 
            //    startStroka++;
            //    counter++;
            //    decimal TC = Math.Round(ORK[i].Cost * FinKom, 2);
            //    WS.Cells[startStroka, 1] = counter;
            //    WS.Cells[startStroka, 2] = ORK[i].Name;
            //    if (ORK[i].Name.Length > 44)
            //    {
            //        range = WS.get_Range("A" + startStroka, "G" + startStroka);
            //        range.RowHeight = 29;//высота строки
            //        range.WrapText = true;

            //    }
            //    WS.Cells[startStroka, 3] = ORK[i].Izmerenie.Name;
            //    WS.Cells[startStroka, 4] = ORK[i].Number;
            //    WS.Cells[startStroka, 5] = TC;
            //    //  WS.Cells[startStroka, 6] = Math.Round(ORK[i].Cost / 10, 2);
            //    WS.Cells[startStroka, 6] = Math.Round(((TC) / 12) / ActivePloshad, 2);
               
            //    summa += TC;

            //}
            startStroka++;

            WS.Cells[startStroka, 2] = "Итого";


            WS.Cells[startStroka, 5] = summa;
            //   WS.Cells[startStroka, 6] = Math.Round(summa / 10, 2);
            string IngOEGF = "";
            string IngPTO = "";
            string DolgnostPTO = "";
            string DolgnostOEGF = "";
            string DolgnostDirektor = "";
            try
            {
                IngOEGF = G.IngenerOEGF;
            }
            catch
            {

            }
            try { IngPTO = G.IngenerPTO; } catch { }
            DolgnostPTO = G.IngenerPTODolgnost;
            DolgnostOEGF = G.IngenerOEGFDolgnost;
            DolgnostDirektor = G.DirectorDolgnost;
            WS.Cells[startStroka, 6] = Math.Round((summa / 12) / ActivePloshad, 2);
            range = WS.get_Range("A" + startStroka, "G" + startStroka);
            range.Font.Bold = true;
            range = WS.get_Range("A5", "G" + startStroka);
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            startStroka++;
            WS.Cells[startStroka, 1] = "***  - в случае отсутствия срока выполнения, работы выполняются в течение срока действия размера платы (тарифного года).";
            range = WS.get_Range("A" + startStroka, "G" + startStroka);
            Opr.RangeMerge(ApExcel, range, true, true, 11, 20);

            startStroka += 2;
            WS.Cells[startStroka, 1] = ZamOEGF.Dolgnost+"                             ____________________"+ZamOEGF.Name;
            range = WS.get_Range("A" + startStroka, "G" + startStroka);
            Opr.RangeMerge(ApExcel, range, true, false, 13, 20);
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
            startStroka++;
            WS.Cells[startStroka, 1] = MainEngineer.Dolgnost+"                                    ______________________"+MainEngineer.Name;
            range = WS.get_Range("A" + startStroka, "G" + startStroka);
            Opr.RangeMerge(ApExcel, range, true, false, 13, 20);
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
            startStroka++;
            WS.Cells[startStroka, 1] = DolgnostOEGF+"                              ____________________" + IngOEGF;
            range = WS.get_Range("A" + startStroka, "G" + startStroka);
            Opr.RangeMerge(ApExcel, range, true, false, 13, 20);
            range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            startStroka++;
            WS.Cells[startStroka, 1] = DolgnostPTO+ "                                      _____________________" + IngPTO;
            range = WS.get_Range("A" + startStroka, "G" + startStroka);
            Opr.RangeMerge(ApExcel, range, true, false, 13, 20);
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
            startStroka++;
            WS.Cells[startStroka, 1] = DolgnostDirektor +"                      ______________________" + G.DirectorIP;
            range = WS.get_Range("A" + startStroka, "G" + startStroka);
            Opr.RangeMerge(ApExcel, range, true, false, 13, 20);
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;

            //Проверяем есть ли работы с назначеной датой, если да то выводим доп лист план работы иначе не выводим листа
          
            List<ActiveOsmotrWork> AOWD = new List<ActiveOsmotrWork>();
            try
            {
                AOWD = db.ActiveOsmotrWorks.Include(x => x.OsmotrWork).Include(x=>x.OsmotrWork.Izmerenie).Where(x => x.DateZaplanirovana > DateStart&&x.OsmotrId == O.Id).ToList();
            }
            catch
            {

            }
            if (AOWD.Count == 0)
            {
                goto END;
            }

            //Добавляем лист планы работ
            ApExcel.Worksheets.Add(Type.Missing);//Добавляем лист
            WS = WbExcel.Sheets[1];
            WS.Name = "3.План работ";

            from = 1;
            startStroka = 1;
            WS.Cells[startStroka, 1] = "План";
            range = WS.get_Range("A" + startStroka, "E" + startStroka);
            range.Merge();
            range.Font.Size = 12;
            range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            startStroka++;
            WS.Cells[startStroka, 1] = "работ по устранению недостатков выявленных в результате весеннего осмотра от " + O.Date.ToString("dd.MM.yy");
            range = WS.get_Range("A" + startStroka, "E" + startStroka);
            range.Merge();
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 8;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            startStroka++;
            WS.Cells[startStroka, 1] = "по адресу " + Adres;
            range = WS.get_Range("A" + startStroka, "E" + startStroka);
            range.Merge();
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 8;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            startStroka++;
            WS.Cells[startStroka, 1] = "№";
            WS.Cells[startStroka, 2] = "Наименование работы";
            WS.Cells[startStroka, 3] = "Количество";
            WS.Cells[startStroka, 4] = "Измерение";
            WS.Cells[startStroka, 5] = "Срок";
            range = WS.get_Range("A" + startStroka, "E" + startStroka);
            range.Font.Size = 8;
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            int z = 0;
            foreach (var aowd  in AOWD)
            {
                z++;
                startStroka++;
                WS.Cells[startStroka, 1] = z;
                WS.Cells[startStroka, 2] = aowd.OsmotrWork.Name;
                WS.Cells[startStroka, 3] = aowd.Number;
                WS.Cells[startStroka, 4] = aowd.OsmotrWork.Izmerenie.Name;
                WS.Cells[startStroka, 5] = aowd.DateZaplanirovana.Value.ToString("dd.MM.yyyy");
                range = WS.get_Range("A" + startStroka, "E" + startStroka);
                range.ColumnWidth = 10;
                range.Font.Size = 8;
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            }
            startStroka++;
            WS.Cells[startStroka, 1] = "_______________(" + G.DirectorDolgnost + " " + G.DirectorIP + ")";// Начальник " + eu;
            range = WS.get_Range("A" + startStroka, "E" + startStroka);
            range.Font.Size = 8;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.Merge(Type.Missing);

            range = WS.get_Range("B" + startStroka, "B" + startStroka);
            range.ColumnWidth = 45;










            END:

            // Сохранение файла Excel.
            try
            {
                if (File.Exists(path)) { File.Delete(path); }
                WbExcel.SaveCopyAs(path);//сохраняем в папку
            }
            catch
            {

            }
            //WbExcel.PrintOutEx(1, 1, 1, true, null, null, null, null, null);//печать сразу после сохранения
            ApExcel.Visible = true;//невидимо
            ApExcel.ScreenUpdating = true;//и не обновляемо
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

        public static void GISGKHOtchet(List<MKDYearOtchet> OO, string path, string filename)
        {
           
                Excel.Application ApExcel = new Excel.Application();
                Excel.Workbooks WB = null;
                WB = ApExcel.Workbooks;
                Excel.Workbook WbExcel = WB.Add(Missing.Value);
                Excel.Worksheet WS = WbExcel.Sheets[1];
                Excel.Range range;//рэндж
                ApExcel.Visible = false;//невидимо
                ApExcel.ScreenUpdating = false;//и не обновляемо
                ApExcel.StandardFont = "TimesNewRoman";
                WS.Name = "ФГБУ Академия комфорта";


                int from = 1;

                int StartStroka = 0;
                int EndStroka = 0;
                // string month = Opr.MonthToNorm(Opr.MonthOpred(O.Date.Month));
                string year = DateTime.Now.AddYears(-1).Year.ToString();
                int usluga = 1;
                int start = 2;
                int nachisleno = 3;
                int oplacheno = 4;
                int sobstvennie = 5;
                int end = 6;






                from++;
                WS.Cells[from, 1] = " Выполненные работы";
                range = WS.get_Range("A" + from, "C" + from);
                range.Merge(Type.Missing);
                range.Font.Bold = true;
                range.Font.Size = 18;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.RowHeight = 20;//высота строки
                range.WrapText = true;
                range.Font.Name = "TimesNewRoman";

                StartStroka = from;

                from++;
                WS.Cells[from, 1] = "АДРЕС";
                WS.Cells[from, 2] = "НАЧИСЛЕНО ПО СОДЕРЖАНИЮ";
                WS.Cells[from, 3] = "ВЫПОЛНЕННЫЕ РАБОТЫ ПО СОДЕРЖАНИЮ";
                foreach (var O in OO)
                {



                    from++;
                    try
                    {
                        WS.Cells[from, 1] = O.Adres;
                        WS.Cells[from, 2] = O.ORCSoderganieCHANGE;
                        WS.Cells[from, 3] = O.Soderganie;
                    }
                    catch
                    {

                    }

                }
                EndStroka = from;


                //Рисуем границы
                range = WS.get_Range("A" + StartStroka, "C" + EndStroka);
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                // Сохранение файла Excel.
                try
                {
                    string fileName = path + filename + ".xlsx";
                    if (File.Exists(fileName)) { File.Delete(fileName); }

                    WbExcel.SaveCopyAs(fileName);//сохраняем в папку

                    fileName = path + filename + ".pdf";
                    if (File.Exists(fileName)) { File.Delete(fileName); }
                    WbExcel.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, fileName);
                    // WbExcel.SaveCopyAs(fileName);
                    //   WbExcel.SaveAs(fileName, Excel.XlFixedFormatType.xlTypePDF);



                }
                catch (Exception e)
                {

                }



                //WbExcel.PrintOutEx(1, 1, 1, true, null, null, null, null, null);//печать сразу после сохранения
                ApExcel.Visible = true;//невидимо
                ApExcel.ScreenUpdating = true;//и не обновляемо
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
                string f = path + filename + ".pdf";


                CloseProcess();
      
        }

        public static void MKDOtchet(MKDYearOtchet O, string path, string filename, int Y = 0)
        {
            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = WB.Add(Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            Excel.Range range;//рэндж
            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо
            ApExcel.StandardFont = "TimesNewRoman";
            WS.Name = "ФГБУ Академия комфорта";
          

            int from = 1;

            int StartStroka = 0;
            int EndStroka = 0;
            // string month = Opr.MonthToNorm(Opr.MonthOpred(O.Date.Month));
            string year = Y.ToString();//DateTime.Now.AddYears(-1).Year.ToString();
            int usluga = 1;
            int start = 2;
            int nachisleno = 3;
            int oplacheno = 4;
            int sobstvennie = 5;
            int end = 6;


       
            WS.Cells[from, 1] = " ФГБУ 'Академия комфорта'";
            
            range = WS.get_Range("A" + from, "F" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 18;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.ColumnWidth = 12;
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = " Отчет по выполненным работам за "+ year+" год";
            range = WS.get_Range("A" + from, "F" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 14;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = " Адрес: " + O.Adres ;
            range = WS.get_Range("A" + from, "F" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 14;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 40;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            StartStroka = from;
            WS.Cells[from, 1] = " Движение денежных средств по многоквартирному дому ";
            range = WS.get_Range("A" + from, "F" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 12;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";
            range.Font.Bold = true;

            from++;
            WS.Cells[from, usluga] = "Услуга";
            WS.Cells[from, start] = "Баланс на начало";
            WS.Cells[from, nachisleno] = "Начислено";
            WS.Cells[from, oplacheno] = "Оплачено";
            WS.Cells[from, sobstvennie] = "Выполненные работы";
            WS.Cells[from, end] = "Баланс на конец";
            range = WS.get_Range("A" + from, "F" + from);
           
            range.Font.Bold = false;
            range.Font.Size = 8;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";


            
            from++;
            int BigTableStart = from;
            WS.Cells[from, usluga] = " Аренда ";
            WS.Cells[from, start] = O.OstatkiArendaSTART + O.ArendaOld;
            WS.Cells[from, nachisleno] = O.OstatkiArendaNachisleno;
            WS.Cells[from, oplacheno] = O.OstatkiArendaOplacheno;
            WS.Cells[from, sobstvennie] = O.Arenda;// O.OstatkiArendaEND;
            WS.Cells[from, end] = O.OstatkiArendaSTART + O.ArendaOld + O.OstatkiArendaOplacheno - O.Arenda;

            from++;
            WS.Cells[from, usluga] = " Дополнительный текущий ремонт ";
            WS.Cells[from, start] = O.OstatkiDopTekRemSTART + O.DopTekRemOld;
            WS.Cells[from, nachisleno] = O.ORCDopTekRemCHANGE;
            WS.Cells[from, oplacheno] = O.ORCDopTekRemPAY;
            WS.Cells[from, sobstvennie] = O.DopTekRem;
            WS.Cells[from, end] = O.OstatkiDopTekRemSTART + O.DopTekRemOld + O.ORCDopTekRemPAY - O.DopTekRem;

            from++;
            WS.Cells[from, usluga] = " Непредвиденный/ Неотложный ремонт ";
            WS.Cells[from, start] = O.OstatkiNepredRemSTART + O.NeotlogniOld;
            WS.Cells[from, nachisleno] = O.ORCNepredRemontCHANGE;
            WS.Cells[from, oplacheno] = O.ORCNepredRemontPAY;
            WS.Cells[from, sobstvennie] = O.NepredRemont;
            WS.Cells[from, end] = O.OstatkiNepredRemSTART + O.NeotlogniOld + O.ORCNepredRemontPAY - O.NepredRemont;

            from++;
            WS.Cells[from, usluga] = " Текущий ремонт (содержание)";
            WS.Cells[from, start] = O.OstatkiTekRemSTART + O.TekRemOld;
            WS.Cells[from, nachisleno] = O.ORCTekRemCHANGE;
            WS.Cells[from, oplacheno] = O.ORCTekRemPAY;
            WS.Cells[from, sobstvennie] = O.TEKREM;
            WS.Cells[from, end] = O.OstatkiTekRemSTART + O.TekRemOld + O.ORCTekRemPAY - O.TEKREM;

            from++;
            WS.Cells[from, usluga] = " Содержание";
            WS.Cells[from, start] = O.OstatkiSoderganieSTART + O.SoderganieOld;
            WS.Cells[from, nachisleno] = O.ORCSoderganieCHANGE;
            WS.Cells[from, oplacheno] = O.ORCSoderganiePAY;
            WS.Cells[from, sobstvennie] = O.Soderganie;
            WS.Cells[from, end] = O.OstatkiSoderganieSTART + O.SoderganieOld - O.Soderganie + O.ORCSoderganiePAY;


            //Уменьшаем шрифт в строках верхней таблицы
            range = WS.get_Range("A" + BigTableStart, "F" + from);
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Font.Size = 8;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            //Делаем 1 строку шире
            range = WS.get_Range("A" + BigTableStart, "A" + from);
            range.ColumnWidth = 15;

            from++;
     

            from++;
            WS.Cells[from, 1] = " Выполненные работы";
            range = WS.get_Range("A" + from, "F" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = true;
            range.Font.Size = 18;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 20;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

     

            foreach (var S in O.MKDStatys)
            {
                decimal Summa = O.CompletedWorks.Where(x => x.WorkTip.Equals(S.Name)).Sum(x => x.WorkSumma);
                if (Summa==0)
                {
                    continue;
                }
                string Nazvanie = S.MKDName;
                //if (S.Contains("Работы по текущему ремонту общего имущества"))
                //{
                //   Nazvanie = "Периодические работы согласно утверждённого тарифа";
                //}
                //if (S.Contains("Ремонтные работы за счет статьи Аренда"))
                //{
                //    Nazvanie = "Расходы по статье Аренда";
                //}
                 
                //if (S.Contains("ТЕКУЩИЙ РЕМОНТ"))
                //{
                //    Nazvanie = "ТЕКУЩИЙ РЕМОНТ(содержание)";
                //}
               


                from++;
                WS.Cells[from, 1] = Nazvanie;
                range = WS.get_Range("A" + from, "F" + from);
                range.Merge(Type.Missing);
                range.Font.Bold = true;
                range.Font.Size = 10;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.RowHeight = 30;//высота строки
                range.WrapText = true;
                range.Font.Name = "TimesNewRoman";

                from++;
                WS.Cells[from, usluga] = "Наименование работы";
                WS.Cells[from,oplacheno] = "Ед. изм. ";
                WS.Cells[from, sobstvennie] = "Объём работ";
                WS.Cells[from, end] = "Факт. затр.";



                foreach (var w in O.CompletedWorks
.GroupBy(c => new
{
    c.WorkName,
    c.WorkTip

})
.Select(cl => new MKDCompleteWork
{
    WorkName = cl.First().WorkName,
    WorkSumma = cl.Sum(x => x.WorkSumma),
    WorkTip = cl.First().WorkTip
}).Where(x => x.WorkTip.Equals(S.Name) && x.WorkSumma != 0).ToList())
                {
                    from++;
                    WS.Cells[from, usluga] = w.WorkName;
                    WS.Cells[from, oplacheno] = "руб.";
                    WS.Cells[from, sobstvennie] = w.WorkSumma;
                    WS.Cells[from, end] = w.WorkSumma;

                    //Объединяем ячейки чтоб название влазило
                    range = WS.get_Range("A" + from, "C" + from);
                    range.Merge(Type.Missing);
                    range.Font.Bold = true;
                    range.WrapText = true;
                    range.Font.Size = 8;
                    if (w.WorkName.Length > 40)
                    {
                        range.RowHeight = 20;//высота строки
                    }
                }

             
                from++;
                WS.Cells[from, 1] = " Итог по разделу:" ;
                range = WS.get_Range("A" + from, "E" + from);
                range.Merge(Type.Missing);
                range.Font.Bold = false;
                range.Font.Size = 13;
                
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.RowHeight = 15;//высота строки
                range.WrapText = true;
                range.Font.Name = "TimesNewRoman";

                WS.Cells[from, end] = Summa;
            }
            EndStroka = from;



            //Подписи
            from++;
            from++;
            WS.Cells[from, usluga] = "Директор Кисс В.В.";
            WS.Cells[from, 4] = "__________________";
            from++;
            from++;
            WS.Cells[from, usluga] = "Заместитель директора по ЭЖФ Топчиева Т.П.";
            WS.Cells[from, 4] = "__________________";
            from++;
            from++;
            WS.Cells[from, usluga] = "Начальник ПЭО Зимина Е.Е.";
            WS.Cells[from, 4] = "__________________";
            from++;
            WS.Cells[from, 4] = "подписано ЭЦП";
            


            //Рисуем границы
            range = WS.get_Range("A" + StartStroka, "F" + EndStroka);
            range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            // Сохранение файла Excel.
            try
            {
                string fileName = path + filename + ".xlsx";
                if (File.Exists(fileName)) { File.Delete(fileName); }
            
                WbExcel.SaveCopyAs(fileName);//сохраняем в папку

                fileName = path + filename + ".pdf";
                if (File.Exists(fileName)) { File.Delete(fileName); }
                WbExcel.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, fileName);
               // WbExcel.SaveCopyAs(fileName);
             //   WbExcel.SaveAs(fileName, Excel.XlFixedFormatType.xlTypePDF);
               
               
             
            }
            catch (Exception e)
            {

            }





            //WbExcel.PrintOutEx(1, 1, 1, true, null, null, null, null, null);//печать сразу после сохранения
            ApExcel.Visible = true;//невидимо
            ApExcel.ScreenUpdating = true;//и не обновляемо
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
           string f = path + filename + ".pdf";
            //  bool flag = SaveAsPdf(f);

            //  ApExcel(Load)
            //  WbExcel.SaveAs(f, Excel.XlFixedFormatType.xlTypePDF);
           // Application excel = new Application();
          //  Workbook wb = excel.Workbooks.Open(path);


            CloseProcess();
        }

       



        public static string sostOpred (int i)
        {
            string sostoyanie = "";
            switch (i)
            {
                case 1:
                    sostoyanie = "аварийное тех. сост.";
                    break;
                case 2:
                    sostoyanie = "неудовлетворит. сост.";
                    break;
                case 3:
                    sostoyanie = "ограничено-раб. сост.";
                    break;
                case 4:
                    sostoyanie = "работоспособное сост.";
                    break;
                case 5:
                    sostoyanie = "нормативное сост.";
                    break;
                default:
                    sostoyanie = "ограничено-раб. сост.";
                    break;
            }
            return sostoyanie;
        }

        public static void SformirovatPasportTeplo( Adres Adres, List<TechElement> TechElements, string Path, string Filename)
        {


            Excel.Application ApExcel = new Excel.Application();
            Excel.Workbooks WB = null;
            WB = ApExcel.Workbooks;
            Excel.Workbook WbExcel = WB.Add(Missing.Value);
            Excel.Worksheet WS = WbExcel.Sheets[1];
            WS.Name = "2. Отопительный период ";
            Excel.Range range;//рэндж

            int from = 1;

            int year = DateTime.Now.Year;
            int nextyear = year++;
            int periodichnost = 2;
            int izmerenie = 3;
            int stoimost = 4;
            int cena = 5;
            int naimenovanie = 1;

            WS.Cells[from, 1] = "ПАСПОРТ";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "Готовности к отопительному периоду "+year+ " - "+ nextyear;
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 50;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            range.Columns.ColumnWidth = 10;
            range = WS.get_Range("A" + from, "A" + from);
            range.Columns.ColumnWidth = 2;
            range = WS.get_Range("B" + from, "B" + from);
            range.Columns.ColumnWidth = 45;

            from++;
            WS.Cells[from, 2] = "Выдан: ФГБУ «Академия комфорта»";

            WS.Cells[from, 2].Font.Bold = true;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 25;
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "В отношении следующих объектов, по которым проводилась проверка готовности к отопительному периоду:";

            WS.Cells[from, 2].Font.Bold = true;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 25;
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);


            from++;
            WS.Cells[from, 2] = "Жилой дом № " + Adres.Dom + " ул. "+ Adres.Ulica ;

            from++;
            WS.Cells[from, 2] = "Основание выдачи паспорта готовности к отопительному периоду:";

            WS.Cells[from, 2].Font.Bold = true;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 25;
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "Акт проверки готовности к отопительному периоду от            №        ";

            WS.Cells[from, 2].Font.Bold = true;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 25;
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 4] = "Д.В.Зайков";
            

            WS.Cells[from, 4].Font.Bold = true;
            WS.Cells[from, 4].Font.Size = 13;
            WS.Cells[from, 4].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 4].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 4].RowHeight = 25;
            WS.Cells[from, 4].WrapText = true;
            WS.Cells[from, 4].Font.Name = "TimesNewRoman";
            range = WS.get_Range("D" + from, "E" + from);
            range.Merge(Type.Missing);


            ApExcel.Worksheets.Add(Type.Missing);//Добавляем лист
            WS = WbExcel.Sheets[1];
            WS.Name = "1. Зимний сезон";

            ApExcel.Visible = false;//невидимо
            ApExcel.ScreenUpdating = false;//и не обновляемо
            ApExcel.StandardFont = "TimesNewRoman";

             from = 1;

             year = DateTime.Now.Year;
             periodichnost = 2;
             izmerenie = 3;
             stoimost = 4;
             cena = 5;
             naimenovanie = 1;


            WS.Cells[from, 1] = "ПАСПОРТ";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "Готовности объекта жилищно-коммунального назначения\r\n\r\nк работе в зимних условиях";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 13;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 50;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            range.Columns.ColumnWidth = 10;
            range = WS.get_Range("A" + from, "A" + from);
            range.Columns.ColumnWidth = 2;
            range = WS.get_Range("B" + from, "B" + from);
            range.Columns.ColumnWidth = 45;

            from++;
            WS.Cells[from, 2] = "город Новосибирск";

            WS.Cells[from, 2].Font.Bold = true;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 25;
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            WS.Cells[from, 5] = "район Советский";

            WS.Cells[from, 5].Font.Bold = true;
            WS.Cells[from, 5].Font.Size = 10;
            WS.Cells[from, 5].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            WS.Cells[from, 5].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 5].RowHeight = 25;
            WS.Cells[from, 5].WrapText = true;
            WS.Cells[from, 5].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "адрес " + Adres.Adress;

            WS.Cells[from, 2].Font.Bold = true;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "принадлежность объекта" + "ФГБУ 'Академия комфорта'";

            WS.Cells[from, 2].Font.Bold = true;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 5] = "15.08."+year;

            WS.Cells[from, 5].Font.Bold = true;
            WS.Cells[from, 5].Font.Size = 10;
            WS.Cells[from, 5].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 5].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 5].RowHeight = 15;
            WS.Cells[from, 5].WrapText = true;
            WS.Cells[from, 5].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "I. ОБЩИЕ СВЕДЕНИЯ";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 25;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "1. Назначение объекта - Жилое";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            var GodPostroiki = TechElements.Where(x => x.Name.Equals("Год постройки")).First(); 
            from++;
            WS.Cells[from, 2] = "2. Год постройки "+ GodPostroiki.Val;

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "3. Характеристики объекта:";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);


            var Iznos = TechElements.Where(x=>x.Name.Equals("Износ")).First();
            var Etagnost = TechElements.Where(x=>x.Name.Equals("Этажей")).First();
            var Podiezdov = TechElements.Where(x => x.Name.Equals("Подъездов")).First();
            var Obshaya = TechElements.Where(x => x.Name.Equals("Общая площадь дома")).First();
            var Gilaya = TechElements.Where(x => x.Name.Equals("Жилая площадь")).First();
            var Negilaya = TechElements.Where(x => x.Name.Equals("Нежилая площадь квартир")).First();
            var Podval = TechElements.Where(x => x.Name.Equals("Площадь подвала")).First();
            var Cokol = TechElements.Where(x => x.Name.Equals("Площадь подвала")).First();
            var Kvartir = TechElements.Where(x => x.Name.Equals("Количество квартир")).First();

            from++;
            WS.Cells[from, 2] = "Износ в % - " + Iznos.Val + " Этажность - "+Etagnost.Val + "Подъездов - "+ Podiezdov.Val;

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "Общая площадь объекта - " + Obshaya.Val + " м2, в.т.ч. ";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "Жилая - " + Gilaya.Val + " м2";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "Не жилая - " + Negilaya.Val + " м2";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "Площадь подвала - " + Podval.Val + " м2";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "Площадь цокольного этажа - " + Cokol.Val + " м2";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "Количество квартир - " + Kvartir.Val + " м2";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "4. Характеристика инженерного оборудования - Централизованное";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "5. Источники:";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "теплоснабжения ТС-1 / ТС-2";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "газоснабжения -";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "твердого и жидкого топлива -";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "энергоснабжения - Трансформаторная подстанция";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "Системы АПЗ и дымоудаления -";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("B" + from, "C" + from);
            range.Merge(Type.Missing);

            int lastyear = year - 1;

            from++;
            WS.Cells[from, 1] = "II. РЕЗУЛЬТАТЫ ЭКСПЛУАТАЦИИ ОБЪЕКТА В ЗИМНИХ\r\n\r\nУСЛОВИЯХ ПРОШЕДШЕГО "+lastyear+"-"+year+" гг.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 25;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "№ п/п";
            WS.Cells[from, 2] = "Основные виды неисправностей (аварий) конструктивных элементов и инженерного оборудования";
            WS.Cells[from, 3] = "Дата";
            WS.Cells[from, 4] = "Причина возникновения неисправностей (аварий)";
            WS.Cells[from, 5] = "Отметка о выполненных работах по ликвидации неисправностей (аварий) в текущем "+year+" г.";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 120;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "-";
            WS.Cells[from, 2] = "-";
            WS.Cells[from, 3] = "-";
            WS.Cells[from, 4] = "-";
            WS.Cells[from, 5] = "-";

            from++;

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "III. ОБЪЕМЫ ВЫПОЛНЕННЫХ РАБОТ ПО ПОДГОТОВКЕ\r\n\r\nОБЪЕКТА К ЭКСПЛУАТАЦИИ В ЗИМНИХ УСЛОВИЯХ " + year + "-" + nextyear + " гг.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 25;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "№ п/п";
            WS.Cells[from, 2] = "Виды выполненных работ по конструкциям здания и технологическому и инженерному оборудованию";
            WS.Cells[from, 3] = "Единицы измерения";
            WS.Cells[from, 4] = "Всего по плану подготовки к зиме";
            WS.Cells[from, 5] = "Выполнено при подготовке к зиме";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 50;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "1";
            WS.Cells[from, 2] = "2";
            WS.Cells[from, 3] = "3";
            WS.Cells[from, 4] = "4";
            WS.Cells[from, 5] = "5";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "1";
            WS.Cells[from, 2] = "Объём работ";
            WS.Cells[from, 3] = "-";
            WS.Cells[from, 4] = "-";
            WS.Cells[from, 5] = "-";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft; ;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "2";
            WS.Cells[from, 2] = "Ремонт кровли";
            WS.Cells[from, 3] = "кв.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft; ;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "3";
            WS.Cells[from, 2] = "Ремонт чердачных помещений, в том числе:";
            WS.Cells[from, 3] = "шт.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- утепление (засыпка) чердачного перекрытия";
            WS.Cells[from, 3] = "шт.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- изоляция трубопроводов, вентиляционных коробов и камер, расширительных баков";
            WS.Cells[from, 3] = "шт.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "4";
            WS.Cells[from, 2] = "Ремонт фасадов, в том числе:";
            WS.Cells[from, 3] = "кв.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- ремонт и покраска";
            WS.Cells[from, 3] = "кв.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- герметизация швов";
            WS.Cells[from, 3] = "кв.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- ремонт водосточных труб";
            WS.Cells[from, 3] = "кв.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- утепление оконных проемов";
            WS.Cells[from, 3] = "кв.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- утепление дверных проемов";
            WS.Cells[from, 3] = "кв.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "5";
            WS.Cells[from, 2] = "Ремонт подвальных помещений, в том числе:";
            WS.Cells[from, 3] = "п.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- изоляция трубопроводов";
            WS.Cells[from, 3] = "п.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- ремонт дренажных и водоотводящих устройств";
            WS.Cells[from, 3] = "п.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "6";
            WS.Cells[from, 2] = "Ремонт покрытий дворовых территорий, в том числе:";
            WS.Cells[from, 3] = "кв.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- отмосток";
            WS.Cells[from, 3] = "кв.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- приямков";
            WS.Cells[from, 3] = "кв.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "7";
            WS.Cells[from, 2] = "Ремонт инженерного оборудования, в том числе:";
            WS.Cells[from, 3] = "";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "1.Центрального отопления:";
            WS.Cells[from, 3] = "";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- радиаторов";
            WS.Cells[from, 3] = "шт.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- трубопроводов";
            WS.Cells[from, 3] = "п.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- запорной арматуры";
            WS.Cells[from, 3] = "шт.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- промывка и опрессовка";
            WS.Cells[from, 3] = "мкд";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "2.Горячего водоснабжения:";
            WS.Cells[from, 3] = "";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- трубопроводов";
            WS.Cells[from, 3] = "п.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- запорной арматуры";
            WS.Cells[from, 3] = "шт.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "4.Водопровода:";
            WS.Cells[from, 3] = "";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- ремонт и замена арматуры";
            WS.Cells[from, 3] = "шт.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- ремонт и изоляция труб";
            WS.Cells[from, 3] = "п.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "5.Канализации:";
            WS.Cells[from, 3] = "";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- ремонт трубопроводов";
            WS.Cells[from, 3] = "п.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "6.Электрооборудования:";
            WS.Cells[from, 3] = "";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- электропроводки";
            WS.Cells[from, 3] = "п.м.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- вводных устройств";
            WS.Cells[from, 3] = "мкд";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- электрощитовых";
            WS.Cells[from, 3] = "шт.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "";
            WS.Cells[from, 2] = "- электродвигателей";
            WS.Cells[from, 3] = "шт.";
            WS.Cells[from, 4] = "0";
            WS.Cells[from, 5] = "0";

            range = WS.get_Range("A" + from, "E" + from);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.RowHeight = 10;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "Инструментом и инвентарем для зимней уборки территорий обеспечен.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            WS.Cells[from, 1].Font.Bold = false;
            WS.Cells[from, 1].Font.Size = 10;
            WS.Cells[from, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 1].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 1].RowHeight = 15;//высота строки
            WS.Cells[from, 1].WrapText = true;
            WS.Cells[from, 1].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 1] = "IV. РЕЗУЛЬТАТЫ ПРОВЕРКИ ГОТОВНОСТИ К ЗИМЕ " + year + "-" + nextyear + " гг.";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 25;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "Комиссия в составе: ";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "Председателя " + "И.О. директора Шпедт О.А.";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 25;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "Члены комиссии:";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "Собственники жилых помещений кв.:";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "1.";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "2.";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "3.";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "4.";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "Представителя специализированных организаций:";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "1.";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 15;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";

            from++;
            from++;
            WS.Cells[from, 2] = "1. Работы по профилактике и ремонту внутри дворовых систем выполнены согласно плану.";
            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 20;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "2. Количество отопительных приборов и поверхности нагрева соответствуют проекту.";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 20;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);

            from++;
            WS.Cells[from, 2] = "3. Состояние утепления отапливаемых помещений (чердаки, лестничные клетки, подвалы), внутренней разводки – удовлетворительные.";

            WS.Cells[from, 2].Font.Bold = false;
            WS.Cells[from, 2].Font.Size = 10;
            WS.Cells[from, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            WS.Cells[from, 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            WS.Cells[from, 2].RowHeight = 20;//высота строки
            WS.Cells[from, 2].WrapText = true;
            WS.Cells[from, 2].Font.Name = "TimesNewRoman";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing); 

            from++;
            from++;

            WS.Cells[from, 1] = "Вывод:_________________Объект к эксплуатации готов__________________";
            range = WS.get_Range("A" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 65;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "Председатель комиссии: _________________________";
            range = WS.get_Range("A" + from, "B" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            WS.Cells[from, 4] = "Шпедт О.А.";
            range = WS.get_Range("D" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;

            from++;
            WS.Cells[from, 2] = "Члены комиссии: _________________________";
            range = WS.get_Range("A" + from, "B" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            WS.Cells[from, 4] = "_______________";
            range = WS.get_Range("D" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "_________________________";
            range = WS.get_Range("A" + from, "B" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            WS.Cells[from, 4] = "_______________";
            range = WS.get_Range("D" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = "_________________________";
            range = WS.get_Range("A" + from, "B" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            WS.Cells[from, 4] = "_______________";
            range = WS.get_Range("D" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            from++;
            WS.Cells[from, 2] = " _________________________";
            range = WS.get_Range("A" + from, "B" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

            WS.Cells[from, 4] = "_______________";
            range = WS.get_Range("D" + from, "E" + from);
            range.Merge(Type.Missing);
            range.Font.Bold = false;
            range.Font.Size = 10;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.RowHeight = 15;//высота строки
            range.WrapText = true;
            range.Font.Name = "TimesNewRoman";

        


            // Сохранение файла Excel.
            try
            {
                if (File.Exists(Path)) { File.Delete(Path); }
                WbExcel.SaveCopyAs(Path + Filename + ".xlsx");//сохраняем в папку
            }
            catch (Exception e)
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


        public static void SFORMIROVATAKT(List<CompleteWork> CompleteWorks, List<VipolnennieUslugi> VipolnennieUslugi, string Month, string GEU, string Year, string Ulica,string Dom, string Nachalnik, string Prikaz, string patch, string Summa, string EU, bool GG = true)
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

            string GEUEU = "ЭУ-";
            if (!GG)
            {
                GEUEU = "ЖЭУ-";
            }

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
            if (M > 12) { M = 1; year = (Convert.ToInt32(year)+1).ToString(); }
            string Mon = Opr.MonthToNorm(Opr.MonthOpred(M));
            WS.Cells[from, 3] = "22 " + Mon + " " + year;
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
                "с одной стороны, и Федеральное государственное бюджетное учреждение 'Академия комфорта' (ФГБУ 'Академия комфорта')" +
"именуемое в дальнейшем “Исполнитель”,  в лице начальника "+GEUEU + EU + " " + Nachalnik + ", действующего на основании доверенности №" + Prikaz;
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
                        WS.Cells[startStroka, naimenovanie].RowHeight = 35;
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
            WS.Cells[startStroka, 1] = "Исполнитель в лице начальника "+GEUEU + EU + " " + Nachalnik +"    ____________________";
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
            WS.Cells[startStroka, 1] = "Исполнитель в лице начальника "+GEUEU + EU + " " + Nachalnik +"    ____________________";
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

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!333333333333333333333333333333333333333!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Делаем 3 лист только если есть выполненные работы
            string Addres = Ulica + Dom;
            Addres = Addres.Replace(" ", "");
            int Y = Convert.ToInt16(Year);
            Osmotr O = new Osmotr();
            try
            {
               O = db.Osmotrs.Where(x => x.Adres.Adress.Equals(Addres) && x.Date.Year == Y).Include(x=>x.Adres).First();
            }
            catch
            {

            }
            List<ActiveOsmotrWork> AOW = new List<ActiveOsmotrWork>();
            List<int> Elements = new List<int>();// && x.DateVipolneniya.Month == monthInt  &&x.DateVipolneniya.Month == monthInt
            int monthInt = M+1;
            try
            {
                AOW = db.ActiveOsmotrWorks.Where(x => x.OsmotrId == O.Id && x.Gotovo && x.OsmotrWork.OtchetId == 0 && x.DateVipolneniya.Month == monthInt).OrderBy(x => x.ElementId).Include(x => x.OsmotrWork).Include(x => x.OsmotrWork.Izmerenie).ToList();
                Elements = AOW.Select(x => x.ElementId).Distinct().ToList();
            }
            catch
            {

            }
            List<OsmotrRecommendWork> ORK = new List<OsmotrRecommendWork>();
            try
            {
                ORK = db.OsmotrRecommendWorks.Where(x => x.OsmotrId == O.Id && x.Gotovo && x.DateVipolneniya.Month == monthInt).OrderBy(x => x.Name).Include(x => x.Izmerenie).Include(x => x.DOMPart).Include(x => x.Stati).ToList();

            }
            catch
            {

            }
            if (AOW != null && ORK != null && AOW.Count + ORK.Count > 0)
            {

                ApExcel.Worksheets.Add(Type.Missing);//Добавляем лист
                WS = WbExcel.Sheets[1];
                WS.Name = "4. Выполненные работы";

                startStroka = 1;
                WS.Cells[startStroka, 6] = " Утверждаю:";
                range = WS.get_Range("F" + startStroka, "G" + startStroka);
                //  Opr.RangeMerge(ApExcel, range, true, true, 10, 15);
                range.Merge();
                range.Font.Size = 10;
                range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                startStroka++;
                WS.Cells[startStroka, 6] = " Директор ФГБУ 'Академия комфорта'";
                range = WS.get_Range("F" + startStroka, "G" + startStroka);
                //  Opr.RangeMerge(ApExcel, range, true, true, 13, 15);
                range.Merge();
                range.Font.Size = 10;
                range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                startStroka++;
                WS.Cells[startStroka, 6] = " ____________________В.В.Кисс";
                range = WS.get_Range("F" + startStroka, "G" + startStroka);
                //  Opr.RangeMerge(ApExcel, range, true, true, 13, 15);
                range.Merge();
                range.Font.Size = 10;
                range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                startStroka++;
                WS.Cells[startStroka, 6] = " '___'________________" + (O.Date.Year) + "г.";
                range = WS.get_Range("F" + startStroka, "G" + startStroka);
                //  Opr.RangeMerge(ApExcel, range, true, true, 13, 15);
                range.Merge();
                range.Font.Size = 10;
                range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                startStroka++;

                // range = WS.get_Range("A" + 1, "H" + startStroka);
                // range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;

                WS.Cells[startStroka, 1] = "4. Отчёт по выполненным работам по дополнительному текущему ремонту на "+ month +" "+  (O.Date.Year + 1).ToString() + " по адресу " + O.Adres.Ulica + " " + O.Adres.Dom;
                range = WS.get_Range("A" + startStroka, "G" + startStroka);
                Opr.RangeMerge(ApExcel, range, true, true, 13, 50);


                startStroka++;

                range = WS.get_Range("A" + startStroka, "G" + startStroka);
                WS.Cells[startStroka, 1] = "№ п/п"; WS.Cells[startStroka, 1].ColumnWidth = 5;
                WS.Cells[startStroka, 2] = "Виды работ"; WS.Cells[startStroka, 2].ColumnWidth = 50;
                WS.Cells[startStroka, 3] = "Ед. изм."; WS.Cells[startStroka, 3].ColumnWidth = 6.25;
                WS.Cells[startStroka, 4] = "Объёмы работ"; WS.Cells[startStroka, 3].ColumnWidth = 8.5;
                WS.Cells[startStroka, 5] = "Стоимость работ руб."; WS.Cells[startStroka, 5].ColumnWidth = 22.5;
                //   WS.Cells[startStroka, 6] = "Вознаграждение УК за выполнение работ по доп. текущему ремонту"; WS.Cells[startStroka, 6].ColumnWidth = 32;
                WS.Cells[startStroka, 6] = "Статья финансирования"; WS.Cells[startStroka, 6].ColumnWidth = 15;
                //  WS.Cells[startStroka, 7] = "Дата выполнения"; WS.Cells[startStroka, 7].ColumnWidth = 12;
                Opr.RangeMerge(ApExcel, range, false, true, 10, 45);

                int count = 0;
                decimal ActiveP = db.Adres.Where(x => x.Id == O.AdresId).Select(x => x.ActivePloshad).First();
                decimal summ = 0;








                if (ActiveP == 0) { ActiveP = 1; }






                for (int i = 0; i < Elements.Count; i++)
                {
                    startStroka++;


                    List<ActiveOsmotrWork> AOW2 = AOW.Where(x => x.ElementId == Elements[i]).ToList();
                    int idd = AOW2[0].OsmotrWork.DOMPartId;
                    string DomPart = db.DOMParts.Where(x => x.Id == idd).Select(x => x.Name).First();
                    WS.Cells[startStroka, 1] = DomPart;
                    range = WS.get_Range("A" + startStroka, "G" + startStroka);
                    //range.Merge();
                    Opr.RangeMerge(ApExcel, range, true, true, 13, 20);

                    foreach (ActiveOsmotrWork A in AOW2)
                    {
                        decimal stavka = 1.1m;
                        if (A.TotalCost >= 50000)
                        {
                            stavka = 1.05m;
                        }
                        if (A.TotalCost >= 100000)
                        {
                            stavka = 1.03m;
                        }
                        count++;
                        startStroka++;
                        WS.Cells[startStroka, 1] = count;
                        WS.Cells[startStroka, 2] = A.OsmotrWork.Name;
                        if (A.OsmotrWork.Name.Length > 45)
                        {
                            range = WS.get_Range("A" + startStroka, "G" + startStroka);
                            range.RowHeight = 29;//высота строки
                            range.WrapText = true;

                        }
                        decimal TC = Math.Round(A.FinalCost * stavka, 2);
                        WS.Cells[startStroka, 3] = A.OsmotrWork.Izmerenie.Name;
                        WS.Cells[startStroka, 4] = A.Number;
                        WS.Cells[startStroka, 5] = TC;
                        //   WS.Cells[startStroka, 6] =Math.Round( A.TotalCost/10,2);
                        WS.Cells[startStroka, 6] = "Доп.тек.рем.";
                        //   WS.Cells[startStroka, 7] = A.DateVipolneniya.ToString("dd.MM.yyyy");
                        summ += TC;
                    }

                }
                startStroka++;
                WS.Cells[startStroka, 1] = "";
                range = WS.get_Range("A" + startStroka, "G" + startStroka);
                //range.Merge();
                Opr.RangeMerge(ApExcel, range, true, true, 13, 20);
                //заполняем дополнительные работы
                for (int i = 0; i < ORK.Count; i++)
                {
                    if (ORK[i].Kommisia < 0)
                    {
                        int stavka = 10;

                        if (ORK[i].Cost >= 50000)
                        {
                            stavka = 5;
                        }
                        if (ORK[i].Cost >= 100000)
                        {
                            stavka = 3;
                        }
                        ORK[i].Kommisia = stavka;
                    }
                    decimal KomStavka = 1 + Convert.ToDecimal(ORK[i].Kommisia) * 0.01m;
                    startStroka++;
                    count++;
                    decimal TC = Math.Round(ORK[i].FinalCost * KomStavka, 2);
                    WS.Cells[startStroka, 1] = count;
                    WS.Cells[startStroka, 2] = ORK[i].Name;
                    WS.Cells[startStroka, 3] = ORK[i].Izmerenie.Name;
                    WS.Cells[startStroka, 4] = ORK[i].Number;
                    WS.Cells[startStroka, 5] = TC;
                    //  WS.Cells[startStroka, 6] = Math.Round(ORK[i].Cost / 10, 2);
                    WS.Cells[startStroka, 6] = ORK[i].Stati.Name;
                    //  WS.Cells[startStroka, 7] = ORK[i].DateVipolneniya.ToString("dd.MM.yyyy");
                    summ += TC;

                }
                startStroka++;

                WS.Cells[startStroka, 2] = "Итого";


                WS.Cells[startStroka, 5] = summ;
                //   WS.Cells[startStroka, 6] = Math.Round(summa / 10, 2);
                String OEGF = "";
                String PTO = "";
                int GK = Convert.ToInt32(GKH);
                GEU G = db.GEUs.Where(x => x.GEUN == GK).First();
                try
                {
                    OEGF = G.IngenerOEGF;
                }
                catch
                {

                }
                try
                {
                    PTO = G.IngenerPTO;
                } catch { }
                //   WS.Cells[startStroka, 6] = Math.Round((summa / 12) / ActivePloshad, 2);
                range = WS.get_Range("A" + startStroka, "G" + startStroka);
                range.Font.Bold = true;
                range = WS.get_Range("A5", "G" + startStroka);
                range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                startStroka++;
                WS.Cells[startStroka, 1] = "***  - в случае отсутствия срока выполнения, работы выполняются в течение срока действия размера платы (тарифного года).";
                range = WS.get_Range("A" + startStroka, "G" + startStroka);
                Opr.RangeMerge(ApExcel, range, true, true, 11, 20);

                startStroka += 2;
                WS.Cells[startStroka, 1] = "Заместитель директора по эксплуатации жилого фонда___________________________Т.П. Топчиева";
                range = WS.get_Range("A" + startStroka, "G" + startStroka);
                Opr.RangeMerge(ApExcel, range, true, false, 13, 20);
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
                startStroka++;
                WS.Cells[startStroka, 1] = "Начальник ОЭЖФ                                    ___________________________С.Ю. Конкина";
                range = WS.get_Range("A" + startStroka, "G" + startStroka);
                Opr.RangeMerge(ApExcel, range, true, false, 13, 20);
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
                startStroka++;
                
                WS.Cells[startStroka, 1] = "Ведущий инженер ОЭЖФ                              ___________________________" + OEGF;
                range = WS.get_Range("A" + startStroka, "G" + startStroka);
                Opr.RangeMerge(ApExcel, range, true, false, 13, 20);
                range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                startStroka++;
                WS.Cells[startStroka, 1] = "Инженер ПТО                                       ___________________________" + PTO;
                range = WS.get_Range("A" + startStroka, "G" + startStroka);
                Opr.RangeMerge(ApExcel, range, true, false, 13, 20);
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                range.Borders.LineStyle = Excel.XlLineStyle.xlLineStyleNone;
                
            }


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


        public static void SFORMIROVATAKTYEAR(List<CompleteWork> CompleteWorks, List<VipolnennieUslugi> VipolnennieUslugi, string GEU, string Year, string Ulica, string Dom, string Nachalnik, string Prikaz, string patch, string Summa, string EU)
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
          //  string month = Opr.MonthToNorm(Month);
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



            if (Ad.Contains("МОЛОДЕЖИ") || Ad.Contains("ЛЕОНАРДО")) { Ad = "БУЛЬВАР " + Ad; }
            if (Ad.Contains("МОРСКОЙ") || Ad.Contains("СТРОИТЕЛЕЙ")) { Ad = Ad + " ПРОСПЕКТ"; }
            if (Ad.Contains("ДЕТСКИЙ") || Ad.Contains("ВЕСЕННИЙ") || Ad.Contains("ЦВЕТНОЙ")) { Ad = Ad + " ПРОЕЗД"; }
            Ad = "ул. " + Ad;
            string Res = " д. " + Dom.Replace(" ", "");
            string Adres = Ad + Res;






            from++;
            WS.Cells[from, 1] = "приемки оказанных услуг и(или) выполненных работ по содержанию и текущему ремонту общего имущества в многоквартирном доме по адресу " + Adres + " за  " + year + " год.";
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
           // int M = Opr.MonthObratno(Month);
            //M++;
           // if (M > 12) { M = 1; year = (Convert.ToInt32(year) + 1).ToString(); }
          //  string Mon = Opr.MonthToNorm(Opr.MonthOpred(M));
            WS.Cells[from, 3] = "за год " + year;
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
                "с одной стороны, и Федеральное государственное бюджетное учреждение 'Академия комфорта' (ФГБУ 'Академия комфорта')" +
"именуемое в дальнейшем “Исполнитель”,  в лице начальника ЭУ-" + EU + " " + Nachalnik + ", действующего на основании доверенности №" + Prikaz;
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
            string OldName = VipolnennieUslugi[0].Usluga.Name;
            string OldPeriod = VipolnennieUslugi[0].Usluga.Periodichnost.PeriodichnostName;
            decimal OldStoimost = VipolnennieUslugi[0].StoimostNaM2;
            decimal SummaYear = 0;
            int fromstroka = startStroka+1;
            int tostroka = startStroka;
            int cou = 0;
            foreach (VipolnennieUslugi U in VipolnennieUslugi)
            {
                cou++;
               
                if (OldName.Equals(U.Usluga.Name) == false)
                {
                    if (OldStoimost+SummaYear != 0)
                    {
                        tostroka = startStroka;
                       
                      
                        range = WS.get_Range("A" + fromstroka, "E" + tostroka);
                        range.EntireRow.Hidden = true;
                        startStroka++;
                        WS.Cells[startStroka, naimenovanie] = OldName;
                        if (U.Usluga.Name.Replace(" ", "").Length > 50 || U.Usluga.Periodichnost.PeriodichnostName.Replace(" ", "").Length > 20)
                        {
                            WS.Cells[startStroka, naimenovanie].RowHeight = 35;
                            WS.Cells[startStroka, naimenovanie].WrapText = true;
                            WS.Cells[startStroka, periodichnost].WrapText = true;
                        }
                        WS.Cells[startStroka, periodichnost] = OldPeriod; 
                        WS.Cells[startStroka, izmerenie] = "кв.м.";
                        if ((U.Usluga.Name.Contains("ДЕРАТИЗАЦИЯ")) && (Convert.ToDouble(U.StoimostNaM2) < 0.01)) { WS.Cells[startStroka, stoimost] = 0.01; }
                        else { WS.Cells[startStroka, stoimost] = OldStoimost; }
                        WS.Cells[startStroka, cena] = Convert.ToInt32(SummaYear);
                       
                    }
                    OldPeriod = U.Usluga.Periodichnost.PeriodichnostName;
                    OldStoimost = U.StoimostNaM2;
                    OldName = U.Usluga.Name;
                    SummaYear = 0;
                    fromstroka = startStroka+1;
                    range = WS.get_Range("A" + startStroka, "E" + startStroka);
                    range.Font.Size = 8;
                    range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;


                }
                SummaYear += U.StoimostNaMonth;

                if (U.StoimostNaM2 + U.StoimostNaMonth != 0)
                {

                     startStroka++; 
                   
                        WS.Cells[startStroka, naimenovanie] = U.Usluga.Name;
                        if (U.Usluga.Name.Replace(" ", "").Length > 50 || U.Usluga.Periodichnost.PeriodichnostName.Replace(" ", "").Length > 20)
                        {
                            WS.Cells[startStroka, naimenovanie].RowHeight = 35;
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
                if ( VipolnennieUslugi.Count == cou)
                {
                    if ( SummaYear != 0)
                    {
                        startStroka++;
                        tostroka = startStroka - 1;
                        range = WS.get_Range("A" + fromstroka, "E" + tostroka);
                        range.EntireRow.Hidden = true;
                        WS.Cells[startStroka, naimenovanie] = U.Usluga.Name;
                        if (U.Usluga.Name.Replace(" ", "").Length > 50 || U.Usluga.Periodichnost.PeriodichnostName.Replace(" ", "").Length > 20)
                        {
                            WS.Cells[startStroka, naimenovanie].RowHeight = 35;
                            WS.Cells[startStroka, naimenovanie].WrapText = true;
                            WS.Cells[startStroka, periodichnost].WrapText = true;
                        }
                        WS.Cells[startStroka, periodichnost] = OldPeriod;
                        WS.Cells[startStroka, izmerenie] = "кв.м.";
                        if ((U.Usluga.Name.Contains("ДЕРАТИЗАЦИЯ")) && (Convert.ToDouble(U.StoimostNaM2) < 0.01)) { WS.Cells[startStroka, stoimost] = 0.01; }
                        else { WS.Cells[startStroka, stoimost] = OldStoimost; }
                        WS.Cells[startStroka, cena] = Convert.ToInt32(SummaYear);
                        range = WS.get_Range("A" + startStroka, "E" + startStroka);
                        range.Font.Size = 8;
                        range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    }
                }
            }



            startStroka+=2;
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
            WS.Cells[startStroka, 1] = "2. Всего за " + year + " год выполнено работ (оказано услуг) на общую сумму " + Summa + " рублей.";
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
           // month = Month;
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
            WS.Cells[2, 1] = "приемки оказанных услуг и(или) выполненных работ по содержанию и текущему ремонту общего имущества в многоквартирном доме по адресу " + Adres + " за " + year + " год.";
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
                    if (Homes[i].Replace(" ", "").Equals(CW.WorkAdress.Replace(" ", "")) == false)//если дома не содержат такого адреса то добавим его 
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
                startStroka = from;
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
                                if (CW.WorkName.Replace(" ", "").Length > 40)
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