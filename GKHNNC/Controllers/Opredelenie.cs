using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Collections;
using System.Net;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using System.Threading;


namespace Opredelenie
{
    class Opr
    {
        public static List<string> MonthZabit()
        {
            List<string> Month = new List<string>();
            for (int i = 1; i < 13; i++)
            {
                Month.Add( MonthOpred(i));
            }
            return Month;
        }

        public static string OpredelenieBukvi(int i)
        {
            string bukva = "A";
            switch (i)
            {
                case 1: bukva = "A"; break;
                case 2: bukva = "B"; break;
                case 3: bukva = "C"; break;
                case 4: bukva = "D"; break;
                case 5: bukva = "E"; break;
                case 6: bukva = "F"; break;
                case 7: bukva = "G"; break;
                case 8: bukva = "H"; break;
                case 9: bukva = "I"; break;
                case 10: bukva = "J"; break;
                case 11: bukva = "K"; break;
                case 12: bukva = "L"; break;
                case 13: bukva = "M"; break;
                case 14: bukva = "N"; break;
                case 15: bukva = "O"; break;
                case 16: bukva = "P"; break;
                case 17: bukva = "Q"; break;
                case 18: bukva = "R"; break;
                case 19: bukva = "S"; break;
                case 20: bukva = "T"; break;
                case 21: bukva = "U"; break;
                case 22: bukva = "V"; break;
                case 23: bukva = "W"; break;
                case 24: bukva = "X"; break;
                case 25: bukva = "Y"; break;
                case 26: bukva = "Z"; break;
                case 27: bukva = "AA"; break;
                case 28: bukva = "AB"; break;
                case 29: bukva = "AC"; break;
                case 30: bukva = "AD"; break;
                case 31: bukva = "AE"; break;
                case 32: bukva = "AF"; break;
                case 33: bukva = "AG"; break;
                case 34: bukva = "AH"; break;
                case 35: bukva = "AI"; break;
                case 36: bukva = "AJ"; break;
                case 37: bukva = "AK"; break;
                case 38: bukva = "AL"; break;
                case 39: bukva = "AM"; break;
                case 40: bukva = "AN"; break;
                case 41: bukva = "AO"; break;
                case 42: bukva = "AP"; break;
                case 43: bukva = "AQ"; break;
                case 44: bukva = "AR"; break;
                case 45: bukva = "AS"; break;
                case 46: bukva = "AT"; break;
                case 47: bukva = "AU"; break;
                case 48: bukva = "AV"; break;
                case 49: bukva = "AW"; break;
                case 50: bukva = "AX"; break;
                case 51: bukva = "AY"; break;
                case 52: bukva = "AZ"; break;
                case 53: bukva = "BA"; break;
                case 54: bukva = "BB"; break;
                case 55: bukva = "BC"; break;
                case 56: bukva = "BD"; break;
                case 57: bukva = "BE"; break;
                case 58: bukva = "BF"; break;
                case 59: bukva = "BG"; break;
                case 60: bukva = "BH"; break;
                case 61: bukva = "BI"; break;
                case 62: bukva = "BJ"; break;
                case 63: bukva = "BK"; break;
                case 64: bukva = "BL"; break;
                case 65: bukva = "BM"; break;
                case 66: bukva = "BN"; break;
                case 67: bukva = "BO"; break;
                case 68: bukva = "BP"; break;
                case 69: bukva = "BQ"; break;
                case 70: bukva = "BR"; break;
                case 71: bukva = "BS"; break;
                case 72: bukva = "BT"; break;
                case 73: bukva = "BU"; break;
                case 74: bukva = "BV"; break;
                case 75: bukva = "BW"; break;
                case 76: bukva = "BX"; break;
                case 77: bukva = "BY"; break;
                case 78: bukva = "BZ"; break;
                case 79: bukva = "CA"; break;
                case 80: bukva = "CB"; break;
                case 81: bukva = "CC"; break;
                case 82: bukva = "CD"; break;
                case 83: bukva = "CE"; break;
                case 84: bukva = "CF"; break;
                case 85: bukva = "CG"; break;
                case 86: bukva = "CH"; break;
                case 87: bukva = "CI"; break;
                case 88: bukva = "CJ"; break;
                case 89: bukva = "CK"; break;
                case 90: bukva = "CL"; break;
                case 91: bukva = "CM"; break;
                case 92: bukva = "CN"; break;
                case 93: bukva = "CO"; break;
                case 94: bukva = "CP"; break;
                case 95: bukva = "CQ"; break;
                case 96: bukva = "CR"; break;
                case 97: bukva = "CS"; break;
                case 98: bukva = "CT"; break;
                case 99: bukva = "CU"; break;
                case 100: bukva = "CV"; break;
                case 101: bukva = "CW"; break;
                case 102: bukva = "CX"; break;
                case 103: bukva = "CY"; break;
                case 104: bukva = "CZ"; break;
                case 105: bukva = "DA"; break;
                case 106: bukva = "DB"; break;
                case 107: bukva = "DC"; break;
                case 108: bukva = "DD"; break;
                case 109: bukva = "DE"; break;
                case 110: bukva = "DF"; break;

            }
            return bukva;
        }

        public static string MonthOpred(int mes)
        {
            string messtr = "Январь";
            switch (mes)
            {
                case 1:
                    messtr = "Январь";
                    break;
                case 2:
                    messtr = "Февраль";
                    break;
                case 3:
                    messtr = "Март";
                    break;
                case 4:
                    messtr = "Апрель";
                    break;
                case 5:
                    messtr = "Май";
                    break;
                case 6:
                    messtr = "Июнь";
                    break;
                case 7:
                    messtr = "Июль";
                    break;
                case 8:
                    messtr = "Август";
                    break;
                case 9:
                    messtr = "Сентябрь";
                    break;
                case 10:
                    messtr = "Октябрь";
                    break;
                case 11:
                    messtr = "Ноябрь";
                    break;
                case 12:
                    messtr = "Декабрь";
                    break;

            }
            return messtr;
        }

        public static int MonthObratno(string mes)
        {
            int mesint = 1;
            switch (mes)
            {
                case "Январь":
                    mesint = 1;
                    break;
                case "Февраль":
                    mesint = 2;
                    break;
                case "Март":
                    mesint = 3;
                    break;
                case "Апрель":
                    mesint = 4;
                    break;
                case "Май":
                    mesint = 5;
                    break;
                case "Июнь":
                    mesint = 6;
                    break;
                case "Июль":
                    mesint = 7;
                    break;
                case "Август":
                    mesint = 8;
                    break;
                case "Сентябрь":
                    mesint = 9;
                    break;
                case "Октябрь":
                    mesint = 10;
                    break;
                case "Ноябрь":
                    mesint = 11;
                    break;
                case "Декабрь":
                    mesint = 12;
                    break;

            }
            return mesint;
        }

        public static string MonthToNorm (string mes)
        {
            string s = mes.Replace(" ", "");
            s = s.Replace("ь", "").Replace("й","");
           if (s[s.Length-1].Equals('т')) { s += "а"; }
           else
            {
                s += "я";
            }



            return s;
        }

        public static void ChistkaStroki(string fs, out string fss)
        {
            fss = "0";
            if (fs != "" && fs != null)
            {
                fs = fs.Replace(".", ",");

                fs = fs.Replace(" ", "");
                if (fs.Length == 1) { fs = fs.Replace("-", ""); }

            }
            else
            {
                fs = "0";

            }

            if (fs == "")
            {
                fs = "0";
            }
            fss = fs;
        }


        public static void ExportPasta(string distFile, string localFile)
        {
            if (File.Exists(distFile))//Если файл есть на сервере
            {
                File.Delete(distFile);//удаляем

            }

            if (File.Exists(localFile))//если файл есть на клиенте
            {

                try
                {
                    File.Copy(localFile, distFile);//копируем его на сервер
                }
                catch
                {
                    //MessageBox.Show("Ошибка копирования файла. Сервер недоступен...");
                }
            }
        }

        public static void CopyPasta(string distFile, string localFile)
        {
            if (File.Exists(localFile))//и файл есть на клиенте
            {
                File.Delete(localFile);
                // MessageBox.Show("Файл удален");

            }
            else
            {
                //  MessageBox.Show("Файла "+localFile.ToString()+" нету.");
            }

            if (File.Exists(distFile))//если файл есть на сервере
            {
                try
                {
                    File.Copy(distFile, localFile);
                    //  MessageBox.Show(localFile.ToString() + "Файл скопирован");
                }
                catch
                {
                    // MessageBox.Show(localFile.ToString()+ "Ошибка копирования");
                }

            }
        }
    }
}
