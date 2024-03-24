using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using GKHNNC.DAL;
using GKHNNC.Models;
using ImageResizer;

namespace GKHNNC.Controllers
{
    public class OsmotrsController : Controller
    {
        private WorkContext db = new WorkContext();
        
        // GET: Osmotrs
        public ActionResult Index()
        {
            var osmotrs = db.Osmotrs.Where(x=>x.Sostoyanie==0).Include(o => o.Adres);
   
            return View(osmotrs.ToList());
        }

        public JsonResult MKDMASSExportToExcel(int Y = 0)
        {
            List<string> S = db.AdresMKDs.Select(x => x.ASU).ToList();
            if (Y == 0)
            {
                Y = DateTime.Now.Year;
            }
            foreach (string s in S )
            {
                MKDExportToExcel(s,Y);
               
            }
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        public FileResult MKDExportGisOtchet(int Year)
        {
            List<string> S = db.AdresMKDs.Select(x => x.ASU).ToList();
            List<MKDYearOtchet> OO = new List<MKDYearOtchet>();
            foreach (string s in S)
            {
               // MKDExportToExcel(s);
                MKDYearOtchet O = SformirovatMKDOtchet(s,Year);
                OO.Add(O);
            }


            DateTime Date = DateTime.Now.AddYears(-1);

            if (Directory.Exists(Server.MapPath("~/Files")) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files"));
            }
            if (Directory.Exists(Server.MapPath("~/Files/MKD" + Date.Year)) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files/MKD" + Date.Year));
            }
            // получаем имя файла
            string fileName = "GISGKH_"+Date.Year;
            var path = Server.MapPath("~/Files/MKD" + Date.Year + "/");

            //экспорт в эксель акта осмотра отправляем все активные элементы и сам осмотр
            ExcelExportDomVipolnennieUslugi.GISGKHOtchet(OO, path, fileName);

            string EndPath = path + fileName + ".xlsx";
            // Путь к файлу

            // Тип файла - content-type
            string file_type = "application/xlsx";
            // Имя файла - необязательно

            return File(EndPath, file_type, fileName + ".xlsx");

        }



        public FileResult MKDExportToExcel(string Adres, int Year)
        {
            MKDYearOtchet O = new MKDYearOtchet();
            string A = Adres.Replace(" ", "");
            AdresaMKDs AdresMKD = db.AdresMKDs.Where(x => x.ASU.Replace(" ", "").Equals(A)).First();
            O = SformirovatMKDOtchet(AdresMKD.ASU, Year);
            

            DateTime Date = DateTime.Now.AddYears(-1);
           
            if (Directory.Exists(Server.MapPath("~/Files")) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files"));
            }
            if (Directory.Exists(Server.MapPath("~/Files/MKD"+Year)) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files/MKD" + Year));
            }
            // получаем имя файла
            string fileName = O.Adres.Replace("/","_")+"_"+Year;
            var path = Server.MapPath("~/Files/MKD" + Date.Year+"/");

            //экспорт в эксель акта осмотра отправляем все активные элементы и сам осмотр
            ExcelExportDomVipolnennieUslugi.MKDOtchet(O, path, fileName, Year);

            string EndPath = path + fileName + ".xlsx";
            // Путь к файлу

            // Тип файла - content-type
            string file_type = "application/xlsx";
            // Имя файла - необязательно

            return File(EndPath, file_type, fileName+ ".xlsx");

        }


 


        public JsonResult MKDFixResults(int Y)
        {
            
            try
            {
                //чистим прошлые записи
                var Results = db.MKDYearResults.Where(x=>x.PeriodYear==Y).ToList();

                db.MKDYearResults.RemoveRange(Results);
                db.SaveChanges();

            }
            catch
            {

            }


            List<MKDYearOtchet> O = new List<MKDYearOtchet>();
            List<AdresaMKDs> AdresMKD = db.AdresMKDs.ToList();
            foreach (var mkd in AdresMKD)
            {
                MKDYearOtchet Otch = SformirovatMKDOtchet(mkd.ASU, Y);
                O.Add(Otch);
                try
                {
                    MKDYearResult YR = new MKDYearResult();
                    YR.AdresMKD = Otch.Adres;
                    YR.AdresFGBU = Otch.Adres;
                    YR.AdresId = Otch.AdresId;
                    YR.PeriodYear = Y;
                    YR.Statya = "Аренда";
                    YR.BallStart = Otch.OstatkiArendaSTART;
                    YR.BallEnd = Otch.ArendaRaschet; 
                    YR.Nachisleno = Otch.OstatkiArendaNachisleno;
                    YR.Oplacheno = Otch.OstatkiArendaOplacheno;
                    YR.CompleteWorks = Otch.OstatkiArendaEND;
                    try
                    {
                        db.MKDYearResults.Add(YR);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }

                    YR = new MKDYearResult();
                    YR.AdresMKD = Otch.Adres;
                    YR.AdresFGBU = Otch.Adres;
                    YR.AdresId = Otch.AdresId;
                    YR.PeriodYear = Y;
                    YR.Statya = "Дополнительный текущий ремонт";
                    YR.BallStart = Otch.OstatkiDopTekRemSTART;
                    YR.BallEnd = Otch.DopTekRemRaschet ;
                    YR.Nachisleno = Otch.ORCDopTekRemCHANGE;
                    YR.Oplacheno = Otch.ORCDopTekRemPAY;
                    YR.CompleteWorks = Otch.OstatkiDopTekRemEND;
                    try
                    {
                        db.MKDYearResults.Add(YR);
                        db.SaveChanges();
                    }
                    catch
                    {

                    }

                    YR = new MKDYearResult();
                    YR.AdresMKD = Otch.Adres;
                    YR.AdresFGBU = Otch.Adres;
                    YR.PeriodYear = Y;
                    YR.AdresId = Otch.AdresId;
                    YR.Statya = "Непредвиденный/Неотложный ремонт";
                    YR.BallStart = Otch.OstatkiNepredRemSTART;
                    YR.BallEnd = Otch.NepredRaschet;
                    YR.Nachisleno = Otch.ORCNepredRemontCHANGE;
                    YR.Oplacheno = Otch.ORCNepredRemontPAY;
                    YR.CompleteWorks = Otch.OstatkiNepredRemEND;
                    try
                    {
                        db.MKDYearResults.Add(YR);
                        db.SaveChanges();
                    }
                    catch
                    {

                    }

                    YR = new MKDYearResult();
                    YR.AdresMKD = Otch.Adres;
                    YR.AdresFGBU = Otch.Adres;
                    YR.PeriodYear = Y;
                    YR.AdresId = Otch.AdresId;
                    YR.Statya = "Текущий ремонт (содержание)";
                    YR.BallStart = 0;
                    YR.BallEnd = Otch.TekRemRaschet;  
                    YR.Nachisleno = Otch.ORCTekRemCHANGE;
                    YR.Oplacheno = Otch.ORCTekRemPAY;
                    YR.CompleteWorks = Otch.OstatkiTekRemEND;
                    try
                    {
                        db.MKDYearResults.Add(YR);
                        db.SaveChanges();
                    }
                    catch
                    {

                    }

                    YR = new MKDYearResult();
                    YR.AdresMKD = Otch.Adres;
                    YR.AdresFGBU = Otch.Adres;
                    YR.PeriodYear = Y;
                    YR.AdresId = Otch.AdresId;
                    YR.Statya = "Содержание";
                    YR.BallStart = Otch.OstatkiSoderganieSTART;
                    YR.BallEnd = Otch.SoderganieRaschet;
                    YR.Nachisleno = Otch.ORCSoderganieCHANGE;
                    YR.Oplacheno = Otch.ORCSoderganiePAY;
                    YR.CompleteWorks = Otch.Soderganie; 
                    try
                    {
                        db.MKDYearResults.Add(YR);
                        db.SaveChanges();
                    }
                    catch
                    {

                    }

                    
                }
                catch
                {

                }
            }
            return Json("Ok");

        }

        //модуль экспорта осмотра в эксель
        public FileResult ExportToExcel(int id=0)
        {
           
                DateTime Date = DateTime.Now;
                Osmotr O = new Osmotr();
                List<ActiveElement> AE = new List<ActiveElement>();
            List<ActiveOsmotrWork> AOW = new List<ActiveOsmotrWork>();
            int geu = 1;
                List<DOMPart> DP = db.DOMParts.ToList();
                HttpCookie cookieReq = Request.Cookies["Osmotr"];
                int AdresId = 0;

                // Проверить, удалось ли обнаружить cookie-набор с таким именем.
                // Это хорошая мера предосторожности, потому что         
                // пользователь мог отключить поддержку cookie-наборов,         
                // в случае чего cookie-набор не существует        
                DateTime DateCook;
            if (id == 0)
            {
                if (cookieReq != null)
                {
                    Date = Convert.ToDateTime(cookieReq["Date"]);
                    O.Id = Convert.ToInt32(cookieReq["OsmotrId"]);
                    try
                    {
                        O = db.Osmotrs.Where(x => x.Id == O.Id).Include(x => x.Adres).First();
                        AE = db.ActiveElements.Where(x => x.OsmotrId == O.Id).Include(x => x.Element).Include(x => x.Material).Include(x => x.Izmerenie).OrderBy(x => x.Element.ElementTypeId).ToList();
                        AOW = db.ActiveOsmotrWorks.Where(x => x.OsmotrId == O.Id && x.OsmotrWork.OtchetId == 0).Include(x => x.OsmotrWork).ToList();
                      
                    }
                    catch { }

                }
            }
            else
            {
                try
                {
                   O= db.Osmotrs.Where(x => x.Id == id).Include(x => x.Adres).First();
                    AE = db.ActiveElements.Where(x => x.OsmotrId == O.Id).Include(x => x.Element).Include(x => x.Material).Include(x => x.Izmerenie).OrderBy(x => x.Element.ElementTypeId).ThenBy(x=>x.ElementId).ToList();
                    AOW = db.ActiveOsmotrWorks.Where(x => x.OsmotrId == O.Id&&x.OsmotrWork.OtchetId==0).Include(x => x.OsmotrWork).ToList();

                }
                catch
                {

                }
            }
            //ищем номер жэу
            try
            {
                geu = Convert.ToInt16(db.Adres.Where(x => x.Id == O.AdresId).Select(x => x.GEU).First().Replace(" ", "").Replace("ЖЭУ-", ""));
            }
            catch
            {
                geu = 0;
            }
            if (Directory.Exists(Server.MapPath("~/Files")) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files"));
            }
            if (Directory.Exists(Server.MapPath("~/Files/" + O.Id.ToString())) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files/" + O.Id.ToString()));
            }
                    // получаем имя файла
                    string fileName = O.Id.ToString()+".xlsx";
                    var path = Server.MapPath("~/Files/" + O.Id.ToString() + "/" + fileName);

            //экспорт в эксель акта осмотра отправляем все активные элементы и сам осмотр
            ExcelExportDomVipolnennieUslugi.ActOsmotra(AE,O,DP,geu,path);


            // Путь к файлу
          
            // Тип файла - content-type
            string file_type = "application/xlsx";
            // Имя файла - необязательно
           
            return File(path, file_type, fileName);
         
        }
        public decimal RaschetWS( Osmotr O, WorkSoderganie W)
        {
            decimal result = 0; // Площадь дома
            string formula = "";//детальное описание процесса
            int Zvezda = 5;
            TechElement TE = new TechElement();
            ActiveElement E = new ActiveElement();
            if (W.Code == 1)
            {
                TE = O.TE.Where(x => x.Name == "Общая площадь дома").First();
                result =TE.Val;
                formula += "Площадь дома =" + result;

            }
            if (W.Code == 2) // Площадь кровли
            {
               TE = O.TE.Where(x => x.Name == "Площадь кровли").First();
                result = TE.Val;
                formula += "Площадь кровли =" + result;
            }
             if (W.Code == 3) //Воронки
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1134&& x.Est).ToList();
               // E = O.Elements.Where(x => x.ElementId == 1134).First();
              //  result = E.Kolichestvo;
              //  Zvezda = E.Sostoyanie;
                result = LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 4) //Количество квартир
            {
                TE = O.TE.Where(x => x.Name == "Количество квартир").First();
                result = TE.Val;
                formula += "Количество квартир =" + result;
            }
            if (W.Code == 5) //Площадь подвала
            {
                TE = O.TE.Where(x => x.Name == "Площадь подвала").First();
                result =TE.Val*2;
                formula += "Площадь подвала и чердака ="+result;
            }
            if (W.Code == 6) //Длинна трубопровода канализации
            {
               List<ActiveElement> LE = O.Elements.Where(x => (x.ElementId == 1185 || x.ElementId == 1414) && x.Est).ToList();
                result = LE.Sum(x=>x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 7) //Длинна трубопровода отопления до 50
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1164 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 50) //Длинна трубопровода отопления 50-100
            {
                List<ActiveElement> LE = O.Elements.Where(x => (x.ElementId == 1418 || x.ElementId == 1164) && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 8) //Фикс
            {
                result = W.Val;
            }
            if (W.Code == 9) //Стояки
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1402 && x.Est).ToList();
                result =  LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 10) //Лестничные площадки
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1410 && x.Est).ToList();
                result =LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 11) //ВРУ
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1160 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 12) //Двери входные и тамбурные
            {
                List<ActiveElement> LE = O.Elements.Where(x => (x.ElementId == 1146 || x.ElementId == 1409) && x.Est).ToList();
                result =  LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 13) //Ограждения и перила
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1149 && x.Est).ToList();
                result =  LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 14) //Подоконники
            {
                 List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1411 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 15) //Почтовые ящики
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1151 && x.Est).ToList();
                result =LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 16) //Металлические решетки
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1413 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 17) //Радиаторы
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1169 && x.Est).ToList();
                result =  LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 18) //Бордюры
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1217 && x.Est).ToList();
                result =  LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 19) //Контейнерная площадка
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1406 && x.Est).ToList();
                result =  LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 20) //Малые формы
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1407 && x.Est).ToList();
                result =  LE.Sum(x => x.Kolichestvo);
              
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 21) //Уличные ограждения
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1133 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
               
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 22) //количество подъездов в МКД, на 1 подъезд - 1м2
            {
                TE = O.TE.Where(x => x.Name == "Подъездов").First();
                result = TE.Val;
               
                    
                    formula +="Количество подъездов в доме ="+result;
                
            }
            if (W.Code == 23) //от длины водосточных труб, 5% (состояние не важно)
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1205 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
              
                if (LE.Count > 0)
                {
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 24) //от количества воронок, 5% (состояние не важно)
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1134 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
                result = Math.Ceiling(result);
              
            
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                }
            }
            if (W.Code == 25) //от количества вентялиционных каналов и шахт, 5%(состояние не важно)*1 м2
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1206 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
                result = Math.Ceiling(result);

                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                }
            }
            if (W.Code == 26) //от количества дверей = (подвал + чердак + мусорокамеры) * 5 % (состояние не важно)
            {
                result = KolichestvoDverei(O);
               
                    Zvezda = 5;
                
            }
            if (W.Code == 27) //количество входных дверей*10% (состояние не важно)
            {
                
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1146 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo2);
              
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo2 + " ").Aggregate((c, n) => c + " " + n);
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                }
            }
            if (W.Code == 28) //количетво оконных блоков*5% (состояние не важно)

            {
               
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1147 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo2);
            
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo2 + " ").Aggregate((c, n) => c + " " + n);
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                }
            }
            if (W.Code == 29) //от количества дверей=(тамбур+подвал+чердак+мусорокамеры)/10 дверей*2 п.м. (состояние не важно)
            {

                result = KolichestvoDverei(O);
                result = Math.Round((result/10) * 2, 2);
               
            }
            if (W.Code == 30) //от количества дверей(ДЕРЕВЯННЫЕ)=(тамбур+подъездные)*5% (состояние не важно)
            {
                List<ActiveElement> LE = O.Elements.Where(x => (x.ElementId == 1409 || x.ElementId == 1146)&&(x.MaterialId==22||x.MaterialId==2082) && x.Est).ToList();
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo2 + " ").Aggregate((c, n) => c + " " + n);
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                }
                result = LE.Sum(x => x.Kolichestvo2);

            }
            if (W.Code == 31) //от этажности и количества подъездов. До 6 этажей - 10п.м. на 1 подъезд, 6 этажей и выше - 20п.м. на 1 подъезд при наличии швов в доме
            {

                decimal Podezdov = 0;
                decimal Etagei = 0;
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1125 && x.Est).ToList();
                if (LE.Count > 0 && LE[0].Est)
                {
                    Etagei = O.TE.Where(x => x.Name == "Этажей").Select(x => x.Val).First();
                    Podezdov = O.TE.Where(x => x.Name == "Подъездов").Select(x => x.Val).First();
                    formula += "Этажей =" + Etagei + "Подъездов =" + Podezdov;
                    if (Etagei < 6)
                    {
                        result = Math.Round(Podezdov * 10, 2);
                        formula += " =10%";

                    }
                    else
                    {
                        result = Math.Round(Podezdov * 20, 2);
                        formula += " =20%";
                    }
                }

            }
            if (W.Code == 32) // от общей длины труб до 50мм = (отопление + ГВС + ХВС) *%%, 5 % -хорошее состояние, 10 % -удов
            {

                List<ActiveElement> LE = O.Elements.Where(x => (x.ElementId == 1164 || x.ElementId == 1180 || x.ElementId == 1181) && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
               
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c,n)=>c+" "+n);
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                }

            }
            if (W.Code == 33) // от общей длины труб до 50мм = (отопление + ГВС + ХВС) *%%, 5 % -хорошее состояние, 10 % -удов
            {

                List<ActiveElement> LE = O.Elements.Where(x => (x.ElementId == 1415 || x.ElementId == 1416 || x.ElementId == 1418) && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
              
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c,n)=>c+" "+n);
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                }

            }
            if (W.Code == 34) //от количества радиаторов*5(секций)*%%, 5%-хорошее состояние, 10% - удов
            {

                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1169 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo2)*5;
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo2 + " X5").Aggregate((c,n)=>c+" "+n);
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                }

            }
            if (W.Code == 35) //одна ревизия на дом, сумма фиксированная - 1600
            {

               
                result = 1;

            }
            if (W.Code == 36) //от общего кол-ва кранов более 50ММ=(отопление+ГВС+ХВС)*%%, 5%-хорошее состояние, 10% - удов

            {
                List<ActiveElement> LE = O.Elements.Where(x =>  (x.ElementId == 1423 || x.ElementId == 1425) && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c,n)=>c+" "+n);
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                }

            }
            if (W.Code == 37) //от общего кол-ва кранов до 50 мм=(отопление+ГВС+ХВС)*%%, 5%-хорошее состояние, 10% - удов


            {
                List<ActiveElement> LE = O.Elements.Where(x => (x.ElementId == 1420 || x.ElementId == 1424) && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c,n)=>c+" "+n);
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                }

            }
            if (W.Code == 38) //от общего кол-ва кранов до 50мм=(отопление+ГВС+ХВС)*0,8(80% от общего количества)*%%, 5%-хорошее состояние, 10% - удов
            {
                List<ActiveElement> LE = O.Elements.Where(x => (x.ElementId == 1420 || x.ElementId == 1424) && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo) * 0.8m;
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c,n)=>c+" "+n);
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                }

            }
            if (W.Code == 39) //от общей длины труб 100мм=(отопление+ГВС+ХВС)*длинну окружности (П*D)*10% ( не зависит от состояния)
            {
                List<ActiveElement> LE = O.Elements.Where(x => (x.ElementId == 1415 || x.ElementId == 1416 || x.ElementId == 1418) && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo) * 3.14m * 0.1m;
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c,n)=>c+" "+n);
                    Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                }

            }
            if (W.Code == 40) //Смена насоса 1 шт на 1 дом
            {
                result =1;
            }
            if (W.Code == 41) //от количества этажных щитков*1шт(автомат)
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1161 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo) /2;
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c,n)=>c+" "+n);
                }
            }
            if (W.Code == 42) // от длины магистрального провода *%%, 5 % -хорошее состояние, 10 % -удов

            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1162 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c,n)=>c+" "+n);
                }
            }
            if (W.Code == 44) //  Сумма (подъездных+подвальные)*%%, 2%-хорошее состояние, 5% - удов


            {
                List<ActiveElement> LE = O.Elements.Where(x => (x.ElementId == 1157 || x.ElementId == 1159) && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo);
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c,n)=>c+" "+n);
                }
            }
            if (W.Code == 46) //от кол-ва ВРУ*2шт
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1160 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo)*2;
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c,n)=>c+" "+n);
                }
            }
            if (W.Code == 47) //от канализации 50-100 мм
            {
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1414 && x.Est).ToList();
                result = LE.Sum(x => x.Kolichestvo) ;
                //if (result > 50) { result = 50; }
                if (LE.Count > 0)
                {
                    formula += LE.Select(x => x.Element.Name + " =" + x.Kolichestvo + " ").Aggregate((c, n) => c + " " + n);
                }
            }
            if (W.Code == 48) //от Площади кровли с фильтром (МЕТАЛЛИЧЕСКАЯ)
            {
                TechElement Tech = O.TE.Where(x =>x.AdresId == O.AdresId&&x.Name == "Площадь кровли").First();
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1129 && x.Est).ToList();
                TypeElement T = db.TypeElements.Where(x => x.AdresId == O.AdresId && x.DOMPartId == 5).First();
                if (T != null && (T.MaterialId == 1296 || T.MaterialId == 1753 || T.MaterialId == 9))
                {
                    result = Tech.Val;
              //      result = LE.Sum(x => x.Kolichestvo);
                   
                        formula += " Площадь кровли =" + Tech.Val;
                    if (LE.Count > 0)
                    {
                        Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    }
                }
                else
                {
                    formula += " Расчет нулевой так как крыша в доме не металлическая";
                }
            }

            if (W.Code == 49) //от Площади кровли с фильтром (МЯГКАЯ)
            {
                TechElement Tech = O.TE.Where(x => x.AdresId == O.AdresId && x.Name == "Площадь кровли").First();
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1129 && x.Est).ToList();
                TypeElement T = db.TypeElements.Where(x => x.AdresId == O.AdresId && x.DOMPartId == 5).First();
                if (T!=null&&(T.MaterialId == 1555 || T.MaterialId == 1728 ))
                {
                    result = Tech.Val;
                    formula += " Площадь кровли =" + Tech.Val;
                    if (LE.Count > 0)
                    {
                        Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    }

                }
                else
                {
                    formula +=  " Расчет нулевой так как крыша в доме не мягкая";
                }
              
            }

            if (W.Code == 51) //от Площади кровли с фильтром (ШИФЕР)
            {
                TechElement Tech = O.TE.Where(x => x.AdresId == O.AdresId && x.Name == "Площадь кровли").First();
                List<ActiveElement> LE = O.Elements.Where(x => x.ElementId == 1129 && x.Est).ToList();
                TypeElement T = db.TypeElements.Where(x => x.AdresId == O.AdresId && x.DOMPartId == 5).First();
                if (T != null && T.MaterialId == 1309 )
                {
                    result = Tech.Val;
                    formula += " Площадь кровли =" + Tech.Val;
                    if (LE.Count > 0)
                    {
                        Zvezda = Convert.ToInt16(LE.Sum(x => x.Sostoyanie) / LE.Count);
                    }

                }
                else
                {
                    formula += " Расчет нулевой так как крыша в доме не из шифера";
                }

            }

            if (W.Obiem > 0&&!W.Remont)
            {
                formula += "Звезды = " + Zvezda;
                formula += " Количество элементов = " + result;
              //  formula += " Ставка = " + Stavka + "% ";
                W.Fiz = result;
                result = Math.Round(result / W.Obiem,2);
            }
            if (W.Remont&&W.Code!=29&&W.Code!=31&&W.Code!=40)
            {
                formula += "Звезды = "+Zvezda;
                decimal Stavka = 100;
                if (W.ProcGood > 0)
                {
                    if (Zvezda >= 4) { Stavka =  W.ProcGood; }
                    if (Zvezda <= 3) { Stavka =  W.ProcBad; }
                }
                formula += " Количество элементов = " + result;
                W.Fiz = result;
                var RezStav = result * Stavka;
              
                result = Math.Round((RezStav)/100,2);
                if (W.Code == 32 || W.Code == 33 || W.Code == 37 || W.Code == 42||W.Code == 47||W.Code == 49)//Округляем до 50 если длинна трубопровода после учета процентов больше
                {
                    if (result > 50)
                    {
                        result = 50;
                        formula += " Поскольку длинна более 50 м, то результат округляется до 50м! ";
                    }

                }

                if ((W.Code >=24 &&W.Code<=28) || (W.Code >= 34&&W.Code<=38)||W.Code==44||W.Code==42||W.Code==22 || W.Code == 30)
                {
                   result =  Math.Ceiling(result);
                }

                formula += " Ставка = " + Stavka + "% ";

            }
            W.Comment = formula;
            return result;
        }
        public decimal KolichestvoDverei(Osmotr O)
        {
            decimal result = 0;
            List<ActiveElement> LE = O.Elements.Where(x => (x.ElementId == 1153  || x.ElementId == 1144 || x.ElementId == 1146 || x.ElementId == 1132 || x.ElementId == 1143) && x.Izmerenie2Id==2 ).ToList();
            result = LE.Sum(x => x.Kolichestvo2);
            LE = O.Elements.Where(x => (x.ElementId == 1153 || x.ElementId == 1144 || x.ElementId == 1146  || x.ElementId == 1132 || x.ElementId == 1143) && x.IzmerenieId == 2).ToList();
            result += LE.Sum(x => x.Kolichestvo);
            return result;
        }

    public ActionResult PlanoviOtchet(int AdresId)
        {
            Osmotr O = db.Osmotrs.Where(x => x.AdresId == AdresId&&x.Sostoyanie==3).Include(x=>x.Adres).OrderByDescending(x => x.Date).First();
            O.Elements = db.ActiveElements.Where(x => x.OsmotrId == O.Id).Include(x=>x.Element).Include(x=>x.Izmerenie).Include(x=>x.Izmerenie2).ToList();
            O.DOMParts = db.DOMParts.ToList();
            O.TE = db.TechElements.Where(x => x.AdresId == AdresId).Include(x=>x.Izmerenie).ToList();
            O.WS = db.WorkSoderganies.Include(x=>x.Tip).Include(x=>x.Izmerenie).OrderBy(x=>x.TipId).ToList();
            
            foreach(WorkSoderganie W in O.WS)
            {
                string Formula = "";
                W.Val = RaschetWS(O, W);
                W.Cost = W.CostMterials + W.CostWrok;
                if (W.Periodichnost == 0) { W.Periodichnost = 1; }
            }
            return View(O);
        }
        
    


            public ActionResult SaveElement(int Id=0, string Photo1="", string Photo2="",int EdIzm=0,decimal Kolvo = 0, int Material=0)
        {
            string Data = "";
            string result = "Ошибка! Необходимо загрузить две фотографии!";
            ActiveElement A = new ActiveElement();
                try
                {
                    A = db.ActiveElements.Include(x=>x.Element).Where(x => x.Id == Id).First();
                if ((A.Photo1 == null || A.Photo2 == null) && (Photo1 == "" || Photo2 == "") && A.Element.ElementTypeId != 19 )
                {
                    Data = "Ошибка; Отсутствуют фотографии! Загрузите обе фотографии и сохраните элемент.";
                }
                else
                {
                   
                    if (Photo1 != "" && Photo2 != "")
                    {
                        result = "Данные элемента успешно сохранены!";
                        string Filename1 = Path.GetFileName(Photo1);
                        string Filename2 = Path.GetFileName(Photo2);
                        if (A.Photo1==null||A.Photo1.Equals(Filename1) == false) { A.IsOld1 = false; }
                        if (A.Photo2 == null||A.Photo2.Equals(Filename2) == false) { A.IsOld2 = false; }
                        A.Photo1 = Path.GetFileName(Photo1);
                        A.Photo2 = Path.GetFileName(Photo2);
                    }
                    else
                    {
                        if (Photo1 != "")
                        {
                            result = "Данные элемента успешно сохранены!";
                            string Filename1 = Path.GetFileName(Photo1);
                            if (A.Photo1 == null || A.Photo1.Equals(Filename1) == false) { A.IsOld1 = false; }
                            A.Photo1 = Path.GetFileName(Photo1);
                        }
                        else
                        {

                        }
                        
                        //загружаются фотки по умолчанию
                    }
                }
                   
                    A.DateIzmeneniya = DateTime.Now;
                    A.UserName = User.Identity.Name;
                    A.MaterialId = Material;
                 if (Kolvo == 0 && A.Kolichestvo > 0)
                {
                    Kolvo = A.Kolichestvo;
                }
                    A.Kolichestvo = Kolvo;
                    A.IzmerenieId = EdIzm;
                    db.Entry(A).State = EntityState.Modified;
                    db.SaveChanges();

                    string Izmerenie = db.Izmerenies.Where(x => x.Id == EdIzm).Select(x=>x.Name).First();
                    string Mat = db.Materials.Where(x => x.Id == Material).Select(x => x.Name).First();
                Data = A.Photo1 + ";" + A.Photo2 + ";" + A.Sostoyanie + ";" + Izmerenie + ";" + Mat + ";" + Kolvo.ToString()+";"+result ;

                }
                catch(Exception e) { Data = "Ошибка; сохранения изменений!"; }
            
            return Json(Data);

        }



        public ActionResult DeleteElement(int Id = 0)
        {
            string Data = "";
                try
                {
                    ActiveElement A = db.ActiveElements.Where(x => x.Id == Id).First();
                    A.Est = false;
                A.UserName = User.Identity.Name;
                A.DateIzmeneniya = DateTime.Now;
                db.Entry(A).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception e) { Data = "Ошибка; сохранения изменений!"; }
            return Json(Data);
        }
        public ActionResult ReturnElement(int Id = 0)
        {
            string Data = "";
            try
            {
                ActiveElement A = db.ActiveElements.Where(x => x.Id == Id).First();
                A.Est = true;
                A.UserName = User.Identity.Name;
                A.DateIzmeneniya = DateTime.Now;
                db.Entry(A).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception e) { Data = "Ошибка; сохранения изменений!"; }
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
                AE.DateIzmeneniya = DateTime.Now;
                AE.UserName = User.Identity.Name;
                //сохраняем его
                db.Entry(AE).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e) { }
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
        public ActionResult RefreshTextAD(int Id=0 , string text="")
        {
            if (Id!=0)
            {
                try
                {
                    ActiveDefect AD = db.ActiveDefects.Where(x => x.Id == Id).First();
                    AD.Opisanie = text;
                    db.Entry(AD).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch
                {
                    return Json("Ошибка. Невозможно сохранить текст в базу данных. Попробуйте еще раз позже.");
                }
            }

            return Json("");
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
                try
                {
                    int z =db.ActiveDefects.Where(x => x.DefectId == DefectId && x.ElementId == ElementId&&x.OsmotrId==A.OsmotrId).Count();
                    if (z > 0)
                    {
                        Data = "Ошибка;Такой дефект уже есть! Выберите другой тип дефекта.";
                        return Json(Data);
                    }
                }
                catch
                {

                }
              
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
                catch (Exception e) { Data = "Ошибка; Ошибка сохранения в базу данных. Обновите страницу и попробуйте повторить через минуту."; }
            }
            return Json(Data);//RedirectToAction("ViewActiveDefect",A);
        }
        //Нужны для обработки файла Word
        private static void ProcessCell(TableCell node, List<string> textBuilder)
        {
            textBuilder.Add(node.InnerText);
            //foreach (var text in node.Descendants<Text>())
            //{
            //    try
            //    {

            //    }
            //    catch
            //    {

            //    }
                
            //   // textBuilder.Add(text.InnerText);
            //}
        }

        private static void ProcessParagraph(Paragraph node, List<string> textBuilder)
        {
            foreach (var text in node.Descendants<Text>())
            {
                try
                {
               
                }
                catch
                {

                }
                textBuilder.Add(text.InnerText);
            }
        }

        private static void ProcessTable(Table node, List<string> textBuilder)
        {
            //int Counter = 0;
            foreach (var row in node.Descendants<TableRow>())
            {
               
              //  if (Counter < 2)
              //  {
              //      Counter++;
              //  }
             //   else
             //   {
                    foreach (var cell in row.Descendants<TableCell>())
                    {

                        ProcessCell(cell, textBuilder);

                        //foreach (var para in cell.Descendants<Paragraph>())
                        //{
                        //    ProcessParagraph(para, textBuilder);
                        //}

                    }
             //   }
               
            }
        }

        public ActionResult MKDResult()
        {
            List<MKDCompleteWork> Result = new List<MKDCompleteWork>();
            try
            {
              Result =  db.MKDCompleteWork.ToList();
            }
            catch
            {

            }
  


            return View(Result);

        }

        public ActionResult MKDYear(string Adres = "", int Y =0)
        {
            List<MKDCompleteWork> Result = new List<MKDCompleteWork>();
            AdresaMKDs AdresMKD = new AdresaMKDs();
            try
            {
                 AdresMKD = db.AdresMKDs.Where(x => x.ASU.Replace(" ", "").Equals(Adres)).First();//Ищем адрес по наименованию АСУ
                if (AdresMKD.Id > 0)
                {
                    Result = db.MKDCompleteWork.Where(x=>x.AdresMKDID== AdresMKD.Id&& x.WorkDate.Year==Y).Include(x=>x.WorkTip).OrderBy(x=>x.WorkTip).ToList();
                }
                else
                {
                    Result = db.MKDCompleteWork.Where(x => x.WorkDate.Year == Y).ToList();
                }

            }
            catch
            {

            }
            List<MKDCompleteWork> result = Result
.GroupBy(c => new
{
    c.WorkName,
    c.AdresMKDID,
    c.WorkTip

})
.Select(cl => new MKDCompleteWork
{
    AdresMKDID = cl.First().AdresMKDID,
    WorkName = cl.First().WorkName,
    Count = cl.Count(),
    WorkSumma = cl.Sum(x => x.WorkSumma),
    WorkCena = cl.First().WorkCena,
    WorkIzmerenie = cl.First().WorkIzmerenie,
    WorkTip = cl.First().WorkTip,
WorkDate = cl.First().WorkDate
}).ToList();

            List<AdresaMKDs> AM = db.AdresMKDs.ToList();
            foreach (MKDCompleteWork r in result)
            {
                r.AdresMKD = AM.Where(x => x.Id == r.AdresMKDID).First();
            }

            decimal ArendaMKD = 0;
            //Ищем аренду 
            try
            {
              MKDArenda  Arenda = db.MKDArendas.Where(x => x.ASUId == AdresMKD.Id).First();
                ArendaMKD = Arenda.Vosnagragdenie;
            
            }
            catch
            {

            }

            ViewBag.Vosnagragdenie = ArendaMKD;

            return View(result);

        }

        public JsonResult FindHouses(string term)
        {
            List<AdresaMKDs> AM = db.AdresMKDs.ToList();
          /*  List<MKDCompleteWork> MK = db.MKDCompleteWork.ToList();
            AdresaMKDs Old = new AdresaMKDs();
            foreach (MKDCompleteWork r in MK)
            {
                if (Old.Id != null && Old.Id != r.AdresMKDID)
                {
                    r.AdresMKD = AM.Where(x => x.Id == r.AdresMKDID).First();
                    Old = r.AdresMKD;
                }
            }
            */
           // Dictionary<int, string> MCW = AM.Where(x=>x.ASU.Contains(term)).ToDictionary(x => x.Id, x => x.ASU);//MK.Where(x=>x.AdresMKDID!=null).Select(x => new { x.AdresMKDID, x.AdresMKD }).Distinct().ToDictionary(x => x.AdresMKDID, x => x.AdresMKD.ASU);
         List<string> MCW = AM.Where(x => x.ASU.Contains(term)).Select(x=>x.ASU).ToList();
            //  ViewBag.MCW = MCW;
            return Json(MCW, JsonRequestBehavior.AllowGet);
        }

        public MKDYearOtchet SformirovatMKDOtchet(string Adres, int Year)
        {
            MKDYearOtchet O = new MKDYearOtchet();
            string A = Adres.Replace(" ","");
            AdresaMKDs AdresMKD = db.AdresMKDs.Where(x => x.ASU.Replace(" ", "").Equals(A)).First();//Ищем адрес по наименованию АСУ
            O.Adres = AdresMKD.ASU;
            O.AdresId = AdresMKD.Id;
            List<ORC> ORCs = new List<ORC>();
            List<MKDCompleteWork> All = new List<MKDCompleteWork>();
            List<MKDOstatki> OstatkiAll = new List<MKDOstatki>();
            List<MKDYearResult> ResultsOld = new List<MKDYearResult>();

            decimal DopTekRemOld = 0;
            decimal ArendaOld = 0;
            decimal NeotlogniOld = 0;
            decimal TekRemOld = 0;
            decimal SoderganieOld = 0;

            try
            {
                int OldYear = Year - 1;
                ResultsOld = db.MKDYearResults.Where(x => x.PeriodYear == OldYear&&x.AdresId == AdresMKD.Id ).ToList();
                

            }
            catch
            {

            }

            try {
                DopTekRemOld = ResultsOld.Where(x => x.Statya.Equals("Дополнительный текущий ремонт")).Sum(x => x.BallEnd); ;
                ArendaOld = ResultsOld.Where(x => x.Statya.Equals("Аренда")).Sum(x => x.BallEnd);
                NeotlogniOld = ResultsOld.Where(x => x.Statya.Equals("Непредвиденный/Неотложный ремонт")).Sum(x => x.BallEnd);
                TekRemOld = ResultsOld.Where(x => x.Statya.Equals("Текущий ремонт (содержание)")).Sum(x => x.BallEnd);
                SoderganieOld = ResultsOld.Where(x => x.Statya.Equals("Содержание")).Sum(x => x.BallEnd);

                O.DopTekRemOld = DopTekRemOld;
                O.ArendaOld = ArendaOld;
                O.NeotlogniOld = NeotlogniOld;
                O.TekRemOld = TekRemOld;
                O.SoderganieOld = SoderganieOld;

            } catch { }

            List<MKDArenda> Arenda = new List<MKDArenda>();
            //Суммируем данные по WORD
            try
            {
                All = db.MKDCompleteWork.Where(x => x.AdresMKDID == AdresMKD.Id&&x.WorkSumma!=0&&x.WorkDate.Year==Year).OrderBy(x=>x.WorkTip).ToList();
             
             //   O.KapRemont = All.Where(x => x.WorkTip.Contains("содержания несущих конструкций") || x.WorkTip.Contains("содержания оборудования и систем")).Sum(x => x.WorkSumma);
                O.Soderganie = All.Where(x => x.WorkTip.Contains("ТЕКУЩИЙ РЕМОНТ") ==false && x.WorkTip.Contains("Непредвиденный/неотложный ремонт") == false && x.WorkTip.Contains("Ремонтные работы за счет статьи Аренда") == false && x.WorkTip.Contains("Дополнительный текущий ремонт") == false).Sum(x => x.WorkSumma);
                O.DopTekRem = All.Where(x => x.WorkTip.Contains("Дополнительный текущий ремонт")).Sum(x => x.WorkSumma);
                O.Arenda = All.Where(x => x.WorkTip.Contains("Аренда")).Sum(x => x.WorkSumma);
                O.TEKREM = All.Where(x => x.WorkTip.Contains("ТЕКУЩИЙ РЕМОНТ")).Sum(x => x.WorkSumma);
                O.NepredRemont = All.Where(x => x.WorkTip.Contains("Непредвиденный/неотложный ремонт")).Sum(x => x.WorkSumma);
            }
            catch
            {

            }
            //Суммируем данные по ОРC
            try
            {
                ORCs = db.ORCs.Where(x => x.ADDRESS.Equals(AdresMKD.ORC) && x.PROVIDER.Contains("ФГБУ АКАД-Я КОМФ")&&x.Year==Year).ToList();//Выбираем всех ОРКов
                                                                                                                              // O.ORCKapRemontCHANGE = ORCs.Where(x => x.SERVICE.Contains("КАП.РЕМОНТ") ).Sum(x => x.CHARGE);
                O.ORCSoderganieCHANGE = ORCs.Where(x => x.SERVICE.Contains("СОДЕРЖАНИЕ ЖИЛЬЯ")).Sum(x => x.CHARGE);
                O.ORCDopTekRemCHANGE = ORCs.Where(x => x.SERVICE.Contains("ДОП.ТЕК")).Sum(x => x.CHARGE);
                O.ORCTekRemCHANGE = ORCs.Where(x => x.SERVICE.Contains("ТЕКУЩ.РЕМОНТ(СОДЕРЖАНИЕ)")).Sum(x => x.CHARGE);
                O.ORCNepredRemontCHANGE = ORCs.Where(x => x.SERVICE.Contains("НЕПРЕДВ") ).Sum(x => x.CHARGE);
                O.ORCSoderganiePAY = ORCs.Where(x => x.SERVICE.Contains("СОДЕРЖАНИЕ ЖИЛЬЯ")).Sum(x => x.PAYS);
                O.ORCDopTekRemPAY = ORCs.Where(x => x.SERVICE.Contains("ДОП.ТЕК") ).Sum(x => x.PAYS);
                O.ORCTekRemPAY = ORCs.Where(x => x.SERVICE.Contains("ТЕКУЩ.РЕМОНТ(СОДЕРЖАНИЕ)")).Sum(x => x.PAYS);
                O.ORCNepredRemontPAY = ORCs.Where(x => x.SERVICE.Contains("НЕПРЕДВ")).Sum(x => x.PAYS);

            }
            catch (Exception e)
            {

            }

            //Ищем данные по остаткам
            try
            {
                OstatkiAll = db.MKDOstatkis.Where(x => x.Adres.Equals(AdresMKD.ASU.Replace("д. ", ""))&&x.Year == Year).ToList();

                O.OstatkiDopTekRemSTART = OstatkiAll.Where(x => x.Schet.Contains("Доп. тек")).Sum(x => x.OstatokJan);// + DopTekRemOld;
                O.OstatkiDopTekRemEND = OstatkiAll.Where(x => x.Schet.Contains("Доп. тек")).Sum(x => x.Rashod);

                O.OstatkiTekRemSTART = OstatkiAll.Where(x => x.Schet.Contains("Содержание")).Sum(x => x.OstatokJan);// +TekRemOld;
                O.OstatkiTekRemEND = 0;//OstatkiAll.Where(x => x.Schet.Contains("Содержание")).Sum(x => x.Rashod);

                O.OstatkiNepredRemSTART = OstatkiAll.Where(x => x.Schet.Contains("Непредвиденный")).Sum(x => x.OstatokJan);// + NeotlogniOld;
                O.OstatkiNepredRemEND = OstatkiAll.Where(x => x.Schet.Contains("Непредвиденный")).Sum(x => x.Rashod);

                O.OstatkiSoderganieSTART = OstatkiAll.Where(x => x.Schet.Contains("Содержание")).Sum(x => x.OstatokJan);// +SoderganieOld;
                O.OstatkiSoderganieEND = OstatkiAll.Where(x => x.Schet.Contains("Содержание")).Sum(x => x.Rashod);

                O.OstatkiArendaSTART = OstatkiAll.Where(x => x.Schet.Contains("Аренда")).Sum(x => x.OstatokJan);// +ArendaOld;
                O.OstatkiArendaEND = OstatkiAll.Where(x => x.Schet.Contains("Аренда")).Sum(x => x.Rashod);


                O.DopTekRemRaschet = O.OstatkiDopTekRemSTART - O.OstatkiDopTekRemEND + O.ORCDopTekRemPAY;

                O.NepredRaschet = O.OstatkiNepredRemSTART - O.OstatkiNepredRemEND + O.ORCNepredRemontPAY;

                O.TekRemRaschet = 0 - O.OstatkiTekRemEND + O.ORCTekRemPAY;

                O.SoderganieRaschet = O.OstatkiSoderganieSTART - O.Soderganie + O.ORCSoderganiePAY;
                //  O.OstatkiArendaSTART = OstatkiAll.Where(x => x.Schet.Contains("Аренда")).Sum(x => x.OstatokJan);
                //  O.OstatkiArendaEND = OstatkiAll.Where(x => x.Schet.Contains("Аренда")).Sum(x => x.OstatokDec);
                //  O.OstatkiArendaNachisleno = OstatkiAll.Where(x => x.Schet.Contains("Аренда")).Sum(x => x.Rashod);
                //  O.OstatkiArendaOplacheno = OstatkiAll.Where(x => x.Schet.Contains("Аренда")).Sum(x => x.Dohod);
            }
            catch
            {

            }


            //Ищем аренду 
            try
            {
                Arenda = db.MKDArendas.Where(x => x.ASUId == AdresMKD.Id&&x.Year==Year).ToList();
                O.ArendaVosnagragdenie = Arenda.Sum(x => x.Vosnagragdenie);
                O.OstatkiArendaNachisleno = Arenda.Sum(x => x.Nachisleno);
                O.OstatkiArendaOplacheno = Arenda.Sum(x => x.Oplacheno);
                O.Arenda += O.ArendaVosnagragdenie;

                O.OstatkiArendaEND += O.ArendaVosnagragdenie;
                O.ArendaRaschet = O.OstatkiArendaSTART + O.OstatkiArendaOplacheno - O.OstatkiArendaEND;
            
            }
            catch
            {

            }
           //Добавляем статью Вознаграждение УО
            MKDCompleteWork CW = new MKDCompleteWork();
            CW.WorkTip = "Ремонтные работы за счет статьи Аренда";
            CW.WorkName = "Вознаграждение управляющей организации 10%";
            CW.WorkSumma = O.ArendaVosnagragdenie;
            All.Add(CW);

            //Сохраняем в отчет
            O.CompletedWorks = All.OrderBy(x => x.WorkTip).ToList();
            O.Stati = All.Select(x => x.WorkTip).Distinct().ToList();

            return O;
        }
  

        public JsonResult SaveYearResults( int Id, decimal Start, decimal End , decimal Nach, decimal Opl ,decimal Works )
        {
            try
            {
              var YR =  db.MKDYearResults.Where(x => x.Id == Id).First();
                YR.BallStart = Start;
                YR.BallEnd = End;
                YR.Nachisleno = Nach;
                YR.Oplacheno = Opl;
                YR.CompleteWorks = Works;
                db.Entry(YR).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Ok");
            }
            catch
            {
                return Json("Error");
            }

            return Json("Ok");
        }


        public ActionResult LoadTableYear(string Adres, int Y=0)
        {
            MKDYearOtchet O = new MKDYearOtchet();
            O = SformirovatMKDOtchet(Adres,Y);
            return View(O);
        }

        public ActionResult MKDYearOtchetSelect()
        {
            List<AdresaMKDs> AM = db.AdresMKDs.ToList();
            List<MKDCompleteWork> MK = db.MKDCompleteWork.ToList();
            AdresaMKDs Old = new AdresaMKDs();
            foreach (MKDCompleteWork r in MK)
            {
                if (Old.Id != null && Old.Id != r.AdresMKDID)
                {
                    r.AdresMKD = AM.Where(x => x.Id == r.AdresMKDID).First();
                    Old = r.AdresMKD;
                }
            }
            Dictionary<int, string> MCW = AM.ToDictionary(x => x.Id, x => x.ASU);//MK.Where(x=>x.AdresMKDID!=null).Select(x => new { x.AdresMKDID, x.AdresMKD }).Distinct().ToDictionary(x => x.AdresMKDID, x => x.AdresMKD.ASU);
            ViewBag.MCW = MCW;
            return View();
        }
    
   

        public ActionResult ObrabotatWord()
        {
            string errors = "";
            string StreetsFalse = "";
            int Dobavleno = 0;
            int Udaleno = 0;
            int AdresMKDId = 0;
            string[] fileEntries = Directory.GetFiles(Server.MapPath("~/Files/MKD/"));
            List<MKDCompleteWork> Result = new List<MKDCompleteWork>();
            List<string> Errors = new List<string>();
            foreach (string f in fileEntries)
            {
                try
                {
                    string File = System.IO.Path.GetFileName(f).Replace(".docx", "");//.Replace("лит_","").Replace("ул_", "").Replace("ул","");
                    DateTime D = new DateTime();

                    int Month = 1;
                    int Year = DateTime.Now.AddYears(-2).Year;
                    int MaxYear = DateTime.Now.Year;
                    bool Naideno = false;
                    for (int Y = Year; Y <= MaxYear; Y++)
                    {
                        for (int M = 1; M <= 12; M++)
                        {
                            DateTime DD = new DateTime(Y, M, 1);
                            string Date = "_"+DD.ToString("MM") + "_" + DD.ToString("yyyy");
                            if (File.IndexOf(Date) > 0)
                            {
                                //Нашли дату в файле
                                D = new DateTime(Y, M, 1);
                                string Adres = File.Remove(File.IndexOf(Date));
                                try
                                {
                                 AdresMKDId =  db.AdresMKDs.Where(x => x.FileName.Equals(Adres)).First().Id;
                                    Naideno = true;
                                    break;

                                }
                                catch (Exception e)
                                {
                                   
                                }

                            }
                        }
                    }
                    if(!Naideno)
                    {
                        Errors.Add("Не найдено соответствие файлу "+ File +" сопоставьте его имя в таблице соответствия");
                        continue;
                    }

                 //   string[] S = File.Split('_');
                 //   string TYPE = "";
                 //   if (S.Length>5)
                 //   {
                 //       TYPE = S[0].ToUpper();
                 //   }
                 //   string STREET = S[1].ToUpper();
                 //   string DOM = S[2].ToUpper();
                  //  D = new DateTime(Convert.ToInt16(S[4]), Convert.ToInt16(S[3]), 1);
                    List<MKDCompleteWork> MCW = new List<MKDCompleteWork>();
                    try
                    {
                        MCW = db.MKDCompleteWork.Where(x => x.AdresMKDID == AdresMKDId && x.WorkDate == D).ToList();
                    }
                    catch
                    {

                    }
                    if (MCW.Count>0)
                    {
                        foreach (var m in MCW)
                        {
                            db.Entry(m).State = EntityState.Deleted;
                            Udaleno++;
                        }
                        try
                        {
                            db.SaveChanges();

                        }
                        catch
                        {

                        }
                    }
                    ViewBag.Udaleno = Udaleno;

                    bool NaidenaTablica = false;
                    MCW = new List<MKDCompleteWork>();
                    using (WordprocessingDocument wDoc = WordprocessingDocument.Open(f, false))
                        {

                            var parts = wDoc.MainDocumentPart.Document.Descendants().FirstOrDefault();
                            if (parts != null)
                            {
                                foreach (var node in parts.ChildElements)
                                {
                                    if (node is Paragraph)
                                    {
                                      //  ProcessParagraph((Paragraph)node, textBuilder);
                                      //  textBuilder.AppendLine("");
                                    }

                                if (node is Table)
                                {
                                    List<string> textBuilder = new List<string>();
                                    string tip = "";
                                    ProcessTable((Table)node, textBuilder);

                                    //if (textBuilder.Count>100)
                                    //{
                                    //    foreach (var v in textBuilder)
                                    //    {
                                    //        if (v.Contains("домов, в т.ч.:"))
                                    //        {
                                    //            textBuilder.Remove(v);
                                    //            break;
                                    //        }
                                    //        textBuilder.Remove(v);
                                    //    }
                                    //}
                                    List<WordComplete> WorkTypes = new List<WordComplete>();
                                    bool Nestandart = false;
                                    if (textBuilder.Count > 2)
                                    {
                                        tip = textBuilder[0].Replace(", в т.ч.:","");
                                          textBuilder.RemoveAt(0);
                                          textBuilder.RemoveAt(0);

                                        if (textBuilder.Count > 50)

                                        {
                                            Nestandart = true;
                                            List<string> NewTextBuilder = new List<string>();
                                            
                                            //List<string> Udalenie = new List<string>();
                                            //Udalenie.Add("Работы, необходимые для надлежащего ");
                                            //Udalenie.Add("Работы и услуги по содержанию");
  
                                            //bool udalit = false;
                                            bool DobavitNext = true;
                                            
                                            for (int i=0;i < textBuilder.Count; i++)
                                            {
                                                //if (udalit)
                                                //{
                                                //    textBuilder.Remove(textBuilder[i]);
                                                //    udalit = false;

                                                //}

                                                bool Dobavit = true;
                                                if (!DobavitNext)
                                                {
                                                    Dobavit = false;
                                                    DobavitNext = true;
                                                    continue;
                                                }
                                                if (i<10&&(textBuilder[i].Contains("Наименование вида работы (услуги)")
                                                    || textBuilder[i].Contains("Периодичность / количественный показатель")
                                                    || textBuilder[i].Contains("Единица измерения работы (услуги)")
                                                    || textBuilder[i].Contains("сметная стоимость")
                                                    || textBuilder[i].Contains("Цена выполненной работы (оказанной услуги)")
                                                    ))
                                                {
                                                   // textBuilder.Remove(textBuilder[i]);
                                                    Dobavit = false;
                                                    continue;
                                                }


                                                if (textBuilder[i].Contains("Работы, необходимые для надлежащего")|| textBuilder[i].Contains("Работы и услуги по содержанию") 
                                                    ||textBuilder[i].Contains("ТЕКУЩИЙ РЕМОНТ (содержание)") ||textBuilder[i].Contains("Ремонтные работы за счет статьи") 
                                                    || textBuilder[i].Contains("Работы по текущему ремонту общего имущества"))
                                                {
                                                    WordComplete WT = new WordComplete();
                                                    WT.WorkType = textBuilder[i].Replace(", в т.ч.:", "");
                                                   
                                                    WorkTypes.Add(WT);
                                                 
                                                    //  textBuilder.Remove(textBuilder[i]);
                                                    //   textBuilder.Remove(textBuilder[i]);
                                                    Dobavit = false;
                                                    DobavitNext = false;
                                                    continue;
                                                }

                                                if (Dobavit)
                                                {
                                                    WorkTypes.Last().AllWorks.Add(textBuilder[i]);
                                                    NewTextBuilder.Add(textBuilder[i]);
                                                }
                                               // textBuilder.RemoveAt(i);//.Remove(textBuilder[i]);


                                            }
                                            textBuilder = NewTextBuilder;
                                        }

                                      if (Nestandart==false)
                                        {
                                            WordComplete WT = new WordComplete();
                                            WT.WorkType = tip;
                                            WT.AllWorks = textBuilder;
                                            WorkTypes.Add(WT);
                                        }

                                        foreach (var wt in WorkTypes)
                                        {


                                            int MaxInRow = 5;

                                        // int maxRow = textBuilder.Count / MaxInRow;
                                        int maxRow = wt.AllWorks.Count / MaxInRow;
                                        for (int i = 0; i < maxRow; i++)
                                        {
                                            try
                                            {
                                                int start = MaxInRow * i;
                                                string Work = wt.AllWorks[start];
                                                try
                                                {
                                                    if (Work.Contains("."))
                                                    {
                                                        int dot = Work.IndexOf(".");
                                                        if (dot <= 5)
                                                        {
                                                         Work =  Work.Remove(0, dot + 1);
                                                        }
                                                    }
                                                }
                                                catch
                                                {

                                                }
                                              //  string Tip = tip;//textBuilder[start+1];
                                                if (Nestandart)
                                                {
                                                  //  Tip = WorkTypes[i];
                                                }
                                                if (wt.AllWorks[start + 3].Equals(""))
                                                    {
                                                        wt.AllWorks[start + 3] = "0";
                                                    }
                                                string Izmerenie = wt.AllWorks[start+2];
                                                decimal CenaZaEdinicu = Convert.ToDecimal(wt.AllWorks[start+3].Replace(" ","").Replace(",",".").Replace(" ", "")); 
                                                decimal Summa = Convert.ToDecimal(wt.AllWorks[start+4].Replace(" ", "").Replace(",", ".").Replace(" ",""));

                                                MKDCompleteWork MKD = new MKDCompleteWork();
                                                MKD.WorkDate = D;
                                                MKD.AdresMKDID = AdresMKDId;
                                              //  MKD.House = DOM;
                                             //   MKD.Street = STREET;
                                                MKD.WorkCena = CenaZaEdinicu;
                                                MKD.WorkIzmerenie = Izmerenie;
                                                MKD.WorkName = Work;
                                                MKD.WorkSumma = Summa;
                                                MKD.WorkTip = wt.WorkType;
                                             //   MKD.Prefix = TYPE;
                                                db.MKDCompleteWork.Add(MKD);
                                                db.SaveChanges();
                                                Result.Add(MKD);
                                                NaidenaTablica = true;
                                            }
                                            catch (Exception e)
                                            {
                                              //  Errors.Add("Ошибка обработки файла " + File + e.Message);
                                            }
                                        }
                                        }

                                    }
                                }
                                }
                            }
                        }
                    if (!NaidenaTablica)
                        {
                        Errors.Add("Не найдена таблица в файле "+ File+ " видимо он корректировался вручную?");
                    }
                }
                catch (Exception exx)
                {

                }
            }
            ViewBag.Dobavleno = Result.Count;
            ViewBag.Errors = Errors;
            return View(Result);
            
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

                    var path = Server.MapPath("~/Files/" + OsmotrId.ToString() + "/" + fileName);
                    upload.InputStream.Seek(0, System.IO.SeekOrigin.Begin);

                    ImageBuilder.Current.Build(
                        new ImageJob(
                            upload.InputStream,
                            path,
                            new Instructions("maxwidth=1000&maxheight=1000"),
                            false,
                            false));




                   // upload.SaveAs(Server.MapPath("~/Files/"+OsmotrId.ToString()+"/" + fileName));
                }
            }
            return Json("файл успешно загружен!");
        }


        [HttpPost]
        public JsonResult UploadPDF()
        {
            HttpCookie cookieReq = Request.Cookies["Osmotr"];
            int OsmotrId = 0;

            // Проверить, удалось ли обнаружить cookie-набор с таким именем.
            // Это хорошая мера предосторожности, потому что         
            // пользователь мог отключить поддержку cookie-наборов,         
            // в случае чего cookie-набор не существует        
            DateTime DateCook;
            if (cookieReq != null)
            {
                OsmotrId = Convert.ToInt32(cookieReq["OsmotrId"]);
            }
            //проверяем директорию и создаем если её нет
            if (Directory.Exists(Server.MapPath("~/Files")) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files"));

            }
            if (Directory.Exists(Server.MapPath("~/Files/" + OsmotrId.ToString())) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Files/" + OsmotrId.ToString()));

            }



            string fileName = "";
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    // получаем имя файла
                     fileName = System.IO.Path.GetFileName(upload.FileName);

                    var path = Server.MapPath("~/Files/" + OsmotrId.ToString() + "/" + fileName);
                    upload.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                    upload.SaveAs(Server.MapPath("~/Files/"+OsmotrId.ToString()+"/" + fileName));
                }
            }
            return Json(fileName);
        }


      


        public JsonResult SaveOsmotr (int OsmotrId=1)
        {
            Osmotr O = db.Osmotrs.Where(x => x.Id == OsmotrId).Include(x=>x.Adres).First();
            O.Sostoyanie = 1;
            O.DateEnd = DateTime.Now;
            db.Entry(O).State = EntityState.Modified;
            db.SaveChanges();

            string data = "OK";
            return Json(data);
           
        }
        public JsonResult ForAllInGEU()
        {
            List<Osmotr> O = db.Osmotrs.Include(x=>x.Adres).Where(x=>x.Adres.GEU.Equals("ЖЭУ-5")).ToList();
            foreach (Osmotr Os in O)
            {
                Os.Sostoyanie = 0;
                db.Entry(Os).State = EntityState.Modified;
                db.SaveChanges();
            }
            
            string data = "OK";
            return Json(data);

        }
        public JsonResult DeleteOsmotr(int OsmotrId = 1)
        {
            Osmotr O = db.Osmotrs.Where(x => x.Id == OsmotrId).Include(x=>x.Adres).First();
            Session["Houses" + O.Adres.Adress] = null;
            List<ActiveElement> AE = db.ActiveElements.Where(x => x.OsmotrId == O.Id).ToList();
            foreach (ActiveElement A in AE)
                {
                try
                {
                    db.ActiveElements.Remove(A);
                    db.SaveChanges();
                }
                catch
                {

                }
            }
            db.Osmotrs.Remove(O);
            db.SaveChanges();
            
            string data = "OK";
            return Json(data);

        }
        public void OtmetitEvent (string text, string cl = "table-success")
        {
            EventLog E = new EventLog();
            E.Date = DateTime.Now;
            E.Text = text;
            E.Class = cl;

            try
            {
                db.EventLogs.Add(E);
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
        }
        public JsonResult Proverka1(int OsmotrId = 1)
        {
            Osmotr O = db.Osmotrs.Where(x => x.Id == OsmotrId).Include(x=>x.Adres).First();
            O.Sostoyanie =2 ;
            O.DateOEGF = DateTime.Now;
            db.Entry(O).State = EntityState.Modified;
            db.SaveChanges();
            OtmetitEvent("Осмотр #"+O.Id+" по адресу " + O.Adres.Adress + " передан на проверку в ОЭЖФ");
            string data = "OK";
            return Json(data);

        }
        public JsonResult NeGotovo(int WorkId)
        {
            ActiveOsmotrWork AOW = db.ActiveOsmotrWorks.Where(x => x.Id == WorkId).First();
     
            AOW.Gotovo = false;

            string data = "Готово";

            AOW.DateVipolneniya = new DateTime(2000,1,1);
            AOW.Photo = "";
            AOW.FinalNumber = 0;
            AOW.FinalCost = 0;
            AOW.User = User.Identity.Name;
            data = "НеГотово";

            try
            {
                db.Entry(AOW).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch
            {
                data = "Ошибка";
            }
            return Json(data);

        }
        public JsonResult NeGotovoORW(int WorkId)
        {
            OsmotrRecommendWork AOW = db.OsmotrRecommendWorks.Where(x => x.Id == WorkId).First();

            AOW.Gotovo = false;

            string data = "Готово";

            AOW.DateVipolneniya = new DateTime(2000, 1, 1);
            AOW.Photo = "";
            AOW.FinalNumber = 0;
            AOW.FinalCost = 0;
            AOW.User = User.Identity.Name;
            data = "НеГотово";

            try
            {
                db.Entry(AOW).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch
            {
                data = "Ошибка";
            }
            return Json(data);

        }
        public JsonResult Gotovo(int WorkId,int OsmotrId,decimal FinalNumber,decimal FinalStoimost,string Filename)
        {
            ActiveOsmotrWork AOW = db.ActiveOsmotrWorks.Where(x => x.Id == WorkId).First();
            bool Gotovo = true;
            AOW.Gotovo = true;
          
            string data = "Готово";

                AOW.DateVipolneniya = DateTime.Now;
                AOW.Photo = Filename;
                AOW.FinalNumber = FinalNumber;
                AOW.FinalCost = FinalStoimost;
            AOW.User = User.Identity.Name;
                data = "Готово";

            try
            {
                db.Entry(AOW).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch {

                data = "Ошибка";
            }
            return Json(data);

        }
        public JsonResult GotovoORW(int WorkId, int OsmotrId, decimal FinalNumber, decimal FinalStoimost, string Filename)
        {
            OsmotrRecommendWork AOW = db.OsmotrRecommendWorks.Where(x => x.Id == WorkId).First();

            bool Gotovo = true;
            AOW.Gotovo = true;

            string data = "Готово";

            AOW.DateVipolneniya = DateTime.Now;
            AOW.Photo = Filename;
            AOW.FinalNumber = FinalNumber;
            AOW.FinalCost = FinalStoimost;
            AOW.User = User.Identity.Name;
            data = "Готово";

            try
            {
                db.Entry(AOW).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch
            {

                data = "Ошибка";
            }
            return Json(data);

        }

        public JsonResult Proverka2(int OsmotrId = 1)
        {
            Osmotr O = db.Osmotrs.Where(x => x.Id == OsmotrId).Include(x=>x.Adres).First();
            O.Sostoyanie = 3;
            O.DatePTO = DateTime.Now;
            db.Entry(O).State = EntityState.Modified;
            db.SaveChanges();
            OtmetitEvent("Осмотр #" + O.Id + " по адресу " + O.Adres.Adress + " передан на проверку в ПТО");
            string data = "OK";
            return Json(data);

        }
        public JsonResult Proverka3(int OsmotrId = 1)
        {
            Osmotr O = db.Osmotrs.Where(x => x.Id == OsmotrId).Include(x=>x.Adres).First();
            O.Sostoyanie = 4;
            O.DateEnd = DateTime.Now;
            db.Entry(O).State = EntityState.Modified;
            db.SaveChanges();
            OtmetitEvent("Осмотр #" + O.Id + " по адресу " + O.Adres + " передан на голосование!","table-info");
            string data = "OK";
            return Json(data);

        }
        public JsonResult Peredelat2(int OsmotrId = 1)
        {
            Osmotr O = db.Osmotrs.Where(x => x.Id == OsmotrId).Include(x=>x.Adres).First();
            O.Sostoyanie = 1;

            db.Entry(O).State = EntityState.Modified;
            db.SaveChanges();
            OtmetitEvent("Осмотр #" + O.Id + " по адресу " + O.Adres.Adress + " вернули на проверку в ОЭЖФ","table-warning");
            string data = "OK";
            return Json(data);

        }
        public JsonResult Peredelat(int OsmotrId = 1)
        {
            Osmotr O = db.Osmotrs.Where(x => x.Id == OsmotrId).Include(x => x.Adres).First();
            O.Sostoyanie=0;

            db.Entry(O).State = EntityState.Modified;
            db.SaveChanges();
            OtmetitEvent("Осмотр #" + O.Id + " по адресу " + O.Adres.Adress + " вернули на редактирование в ЭУ", "table-warning");
            string data = "OK";
            return Json(data);

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

        public ActionResult ViewMobile(DateTime Date, int ElementId = 1, int OsmotrId = 1, int AdresId = 1, ActiveDefect A = null, int DOMPartId = 0)
        {
            //Date = DateTime.Now;
            // HttpCookie cookieReq = Request.Cookies["Osmotr"];
            // if (cookieReq != null)
            // {
            //     Date = Convert.ToDateTime(cookieReq["Date"]);
            //     OsmotrId = Convert.ToInt32(cookieReq["OsmotrId"]);
            //     AdresId = Convert.ToInt32(cookieReq["AdresId"]); 
            // }
            // else
            // {
            // }

            string Data = "";
            ActiveElement AE = new ActiveElement();
            List<BuildElement> BE = new List<BuildElement>();
            Build B = new Build();

            // if (A != null)
            //{
            //    Date = A.Date;
            //     ElementId = Convert.ToInt32(A.ElementId);
            //     AdresId = Convert.ToInt32(A.AdresId);
            //     OsmotrId = Convert.ToInt32(A.OsmotrId);

            // }
            bool addwork = false;
            if (ElementId != 1)
            {
                if (Date != null)
                {


                    try
                    {//для загрузки осмотра
                       // Adres Ad = db.Adres.Where(x => x.Id == AdresId).First();
                        if (OsmotrId == 1)
                        {
                            AE = db.ActiveElements.Where(x => x.ElementId == ElementId && x.AdresId == AdresId).Include(x => x.Element).Include(x => x.Material).Include(x => x.Izmerenie).OrderByDescending(x => x.Date).First();
                        }
                        else
                        {
                            AE = db.ActiveElements.Where(x => x.ElementId == ElementId && x.AdresId == AdresId && x.OsmotrId == OsmotrId).Include(x => x.Element).Include(x => x.Material).Include(x => x.Izmerenie).First();
                        }

                        //пробуем загрузить единицы измерения и материалы
                        // B = db.Builds.Where(x => x.Id == Ad.BuildId).First();//совмещены все кроме Морского и Шатурской 10
                        int E = db.Elements.Where(x => x.Id == ElementId).Select(x => x.ElementId).First();//ищем связку элемент ИД в Элементе. Это ссылка на элементы в справочнике Build_ELements
                        try
                        {
                            int M = db.BuildElements.Where(z => z.ElementId == E && z.BuildId == B.Id).Select(z => z.Material).First();
                            if (AE != null && AE.MaterialId != 1) { M = AE.MaterialId; }
                            AE.M = M;
                            ViewBag.M = M;
                            int EI = db.BuildElements.Where(z => z.ElementId == E && z.BuildId == B.Id).Select(z => z.EdIzm).First();
                            if (AE != null && AE.IzmerenieId != 1) { EI = AE.IzmerenieId; }
                            AE.EI = EI;
                            ViewBag.EI = EI;
                        }
                        catch
                        {//если их нету
                            if (AE.IzmerenieId != 0)
                            {
                                ViewBag.EI = AE.IzmerenieId;
                            }
                            else { ViewBag.EI = 1; }
                            if (AE.MaterialId != 0)
                            {
                                ViewBag.M = AE.MaterialId;
                            }
                            else { ViewBag.M = 1; }


                        }
                    }
                    catch (Exception e)
                    {//если осмотр новый
                     //  AE.ElementId = ElementId;
                     // AE.Element = db.Elements.Where(x => x.Id == ElementId).First();
                     //  AE.OsmotrId = OsmotrId;
                     //  AE.AdresId = AdresId;
                     //  AE.Date = Date;
                     //  AE.Sostoyanie = 10;


                        //  db.ActiveElements.Add(AE);
                        //  db.SaveChanges();
                    }

                    //для загруженных и новых
                    try
                    {
                        Defect D = new Defect();
                        D.ElementId = 0;
                        D.Def = "Отсутствует";
                        AE.Defects = new List<Defect>();
                        AE.Defects.Add(D);
                        AE.Defects.AddRange(db.Defects.Where(x => x.ElementId == ElementId).ToList());

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
                    List<ActiveOsmotrWork> AOW = new List<ActiveOsmotrWork>();
                    try
                    {
                        AOW = db.ActiveOsmotrWorks.Where(x => x.ElementId == ElementId && x.OsmotrId == OsmotrId).Include(x => x.OsmotrWork).Include(x => x.OsmotrWork.Izmerenie).ToList();
                        AE.ActiveOsmotrWorks = AOW;
                    }
                    catch
                    {

                    }
                    List<OsmotrWork> OW = new List<OsmotrWork>();

                    try
                    {
                        if (DOMPartId != 0)
                        {

                            OW = db.OsmotrWorks.Where(x => x.DOMPartId == DOMPartId).ToList();
                            SelectList SL = new SelectList(OW, "Id", "Name");
                            AE.OsmotrWorks = SL;
                            addwork = true;
                        }
                    }
                    catch
                    {
                        addwork = true;
                    }
                }
            }
            else
            {
                return null;
            }
            ViewBag.AW = addwork;
            ViewBag.Materials = new SelectList(db.Materials, "Id", "Name");
            ViewBag.Izmerenies = new SelectList(db.Izmerenies, "Id", "Name");
            return PartialView("ViewActiveDefect", AE);
        }

        //служит для отображения списка активных дефектов и возможных дефектов модели
        public ActionResult ViewActiveDefect(DateTime Date,int ElementId = 1,int OsmotrId=1,int AdresId =1,ActiveDefect A=null,int DOMPartId=0)
        {
            //Date = DateTime.Now;
            
           
           // HttpCookie cookieReq = Request.Cookies["Osmotr"];
           // if (cookieReq != null)
           // {
           //     Date = Convert.ToDateTime(cookieReq["Date"]);
           //     OsmotrId = Convert.ToInt32(cookieReq["OsmotrId"]);
           //     AdresId = Convert.ToInt32(cookieReq["AdresId"]); 
           // }
           // else
           // {

           // }
            string Data = "";
            ActiveElement AE = new ActiveElement();
            List<BuildElement> BE = new List<BuildElement>();
            Build B = new Build();

            // if (A != null)
            //{
            //    Date = A.Date;
            //     ElementId = Convert.ToInt32(A.ElementId);
            //     AdresId = Convert.ToInt32(A.AdresId);
            //     OsmotrId = Convert.ToInt32(A.OsmotrId);

            // }
            bool addwork = false;
            if (ElementId != 1)
            {
                if (Date != null)
                {


                    try
                    {//для загрузки осмотра
                       // Adres Ad = db.Adres.Where(x => x.Id == AdresId).First();
                        if (OsmotrId == 1)
                        {
                            AE = db.ActiveElements.Where(x => x.ElementId == ElementId && x.AdresId == AdresId).Include(x => x.Element).Include(x => x.Material).Include(x => x.Izmerenie).OrderByDescending(x => x.Date).First();
                        }
                        else
                        {
                            AE = db.ActiveElements.Where(x => x.ElementId == ElementId && x.AdresId == AdresId&&x.OsmotrId==OsmotrId).Include(x => x.Element).Include(x => x.Material).Include(x => x.Izmerenie).First();
                        }
                       
                            //пробуем загрузить единицы измерения и материалы
                           // B = db.Builds.Where(x => x.Id == Ad.BuildId).First();//совмещены все кроме Морского и Шатурской 10
                            int E = db.Elements.Where(x => x.Id == ElementId).Select(x => x.ElementId).First();//ищем связку элемент ИД в Элементе. Это ссылка на элементы в справочнике Build_ELements
                        try
                        {
                            int M = db.BuildElements.Where(z => z.ElementId == E && z.AdresId == AdresId).Select(z => z.Material).First();
                            if (AE != null && AE.MaterialId != 1) { M = AE.MaterialId; }
                            AE.M = M;
                            ViewBag.M = M;
                            int EI = db.BuildElements.Where(z => z.ElementId == E && z.AdresId == AdresId).Select(z => z.EdIzm).First();
                            if (AE != null && AE.IzmerenieId != 1) { EI = AE.IzmerenieId; }
                            AE.EI = EI;
                            ViewBag.EI = EI;
                        }
                        catch
                        {//если их нету
                            if (AE.IzmerenieId != 0)
                            {
                                ViewBag.EI = AE.IzmerenieId;
                            }
                            else { ViewBag.EI = 1; }
                            if (AE.MaterialId != 0)
                            {
                                ViewBag.M = AE.MaterialId;
                            }
                            else { ViewBag.M = 1; }
                          
                            
                        }
                    }
                    catch (Exception e)
                    {//если осмотр новый
                      //  AE.ElementId = ElementId;
                       // AE.Element = db.Elements.Where(x => x.Id == ElementId).First();
                      //  AE.OsmotrId = OsmotrId;
                      //  AE.AdresId = AdresId;
                      //  AE.Date = Date;
                      //  AE.Sostoyanie = 10;
                        
                      
                      //  db.ActiveElements.Add(AE);
                      //  db.SaveChanges();
                    }

                    //для загруженных и новых
                    try
                    {
                        Defect D = new Defect();
                        D.ElementId = 0;
                        D.Def = "Отсутствует";
                        AE.Defects = new List<Defect>();
                        AE.Defects.Add(D);
                        AE.Defects.AddRange(db.Defects.Where(x => x.ElementId == ElementId).ToList());
                        
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
                    List<ActiveOsmotrWork> AOW = new List<ActiveOsmotrWork>();
                    try
                    {
                        AOW = db.ActiveOsmotrWorks.Where(x => x.ElementId == ElementId && x.OsmotrId == OsmotrId).Include(x=>x.OsmotrWork).Include(x=>x.OsmotrWork.Izmerenie).ToList();
                        AE.ActiveOsmotrWorks = AOW;
                    }
                    catch
                    {

                    }
                    List<OsmotrWork> OW = new List<OsmotrWork>();
                 
                    try
                    {
                        if (DOMPartId != 0)
                        {

                            OW = db.OsmotrWorks.Where(x => x.DOMPartId == DOMPartId).ToList();
                            SelectList SL = new SelectList(OW, "Id", "Name");
                            AE.OsmotrWorks = SL;
                           addwork = true;
                        }
                    }
                    catch
                    {
                        addwork = true;
                    }
                }
            }
            else
            {
                return null;
            }
            DateTime ND = AE.Date.Date;
            List<string> Dates = new List<string>();
            Dates.Add("Не запланирована");
            for (int i =0;i<13;i++)
            {
                 ND = ND.AddMonths(1);
                Dates.Add(ND.ToString("dd.MM.yyyy"));
            }

            ViewBag.Month = Dates;
            ViewBag.AW = addwork;
            ViewBag.Materials = new SelectList(db.Materials, "Id", "Name");
            ViewBag.Izmerenies = new SelectList(db.Izmerenies, "Id", "Name");
            return PartialView ("ViewActiveDefect",AE);
        }

        [HttpPost]
        public JsonResult RefreshCost(int OsmotrId)
        {
            List<ActiveOsmotrWork> AOW = new List<ActiveOsmotrWork>();
            AOW = db.ActiveOsmotrWorks.Where(x => x.OsmotrId == OsmotrId).ToList();
            foreach (ActiveOsmotrWork A in AOW)
            {
                OsmotrWork OW = new OsmotrWork();
                try
                {
                    OW = db.OsmotrWorks.Where(x => x.Id == A.OsmotrWorkId).First();
                    A.TotalCost = OW.Cost * A.Number;
                    db.Entry(A).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch { }
            }
          
          
            return Json("Ok");
        }

        [HttpPost]
        public ActionResult RefreshAllCost()
        {
            int count = 0;
            DateTime StartYear = new DateTime(DateTime.Now.Year, 1, 1);
            List<Osmotr> O = db.Osmotrs.Where(x => x.Date >= StartYear).ToList();
            foreach (Osmotr os in O)
            {
                List<ActiveOsmotrWork> AOW = new List<ActiveOsmotrWork>();
                AOW = db.ActiveOsmotrWorks.Where(x => x.OsmotrId == os.Id).ToList();
                foreach (ActiveOsmotrWork A in AOW)
                {
                    OsmotrWork OW = new OsmotrWork();
                    try
                    {
                        OW = db.OsmotrWorks.Where(x => x.Id == A.OsmotrWorkId).First();
                        decimal c = A.TotalCost;

                        A.TotalCost = OW.Cost * A.Number;
                        if (c != A.TotalCost)
                        { 
                        db.Entry(A).State = EntityState.Modified;
                        db.SaveChanges();
                        count++;
                        };

                    }
                    catch { }
                }
            }


            return Json(count);
        }

        [HttpPost]
        public JsonResult AddWork(int OsmotrWork, int Number, int OsmotrId, int ElementId, string Month)
        {

          
            ActiveOsmotrWork AOW = new ActiveOsmotrWork();
            AOW.OsmotrWorkId = OsmotrWork;
            AOW.OsmotrWork = db.OsmotrWorks.Find(AOW.OsmotrWorkId);
            AOW.Number = Number;
            int Y = 0;
            int M = 0;
            int day = 1;
            DateTime NewDate = new DateTime();
            if (Month!="")
            {
                try
                {
                    string[] S = Month.Split('-');
                    Y = Convert.ToInt16(S[0]);
                    M = Convert.ToInt16(S[1]);
                    day= Convert.ToInt16(S[2]);
                    NewDate = new DateTime(Y, M, day);
                }
                catch
                {

                }
               
            }
            OsmotrWork OW = new OsmotrWork();
            try
            {
                OW = db.OsmotrWorks.Where(x => x.Id == OsmotrWork).Include(x=>x.Izmerenie).First();
            }
            catch { }
            AOW.TotalCost = OW.Cost * Number;
            AOW.OsmotrId = OsmotrId;
            DOMPart DP = db.DOMParts.Where(x => x.Id == AOW.OsmotrWork.DOMPartId).First();
            Element E = db.Elements.Where(x => x.ElementTypeId == DP.Id).First();
            int EID = E.Id;
            AOW.ElementId = EID;
            string El2 = db.Elements.Find(AOW.ElementId).Name;
            try
            {
                //изменение количества и цены если совпал тип работы
                ActiveOsmotrWork A2 = db.ActiveOsmotrWorks.Where(x => x.ElementId == EID && x.OsmotrId == OsmotrId&&x.OsmotrWorkId==OsmotrWork).First();
                A2.TotalCost = AOW.TotalCost;
                A2.OsmotrId = AOW.OsmotrId;
                A2.ElementId = AOW.ElementId;
                string El = db.Elements.Find(A2.ElementId).Name;
                A2.OsmotrWorkId = AOW.OsmotrWorkId;
                A2.Number = AOW.Number;
                A2.User = User.Identity.Name;
                A2.Photo = "";
                if (Month.Equals("Не запланирована") == false)
                {
                    A2.Zaplanirovana = true;
                    A2.DateZaplanirovana = NewDate;
                }
                
                db.Entry(A2).State = EntityState.Modified;
                db.SaveChanges();
                string DZ = "";
                if (A2.DateZaplanirovana != null)
                {
                    DZ = A2.DateZaplanirovana.ToString();
                }
                else
                {
                    DZ = "";
                }
                string D = El+";"+OW.Name + ";" + OW.Izmerenie.Name + ";" + Number.ToString() + ";" + A2.TotalCost.ToString() + ";" + A2.Id+";Modify;"+DZ;
                return Json(D);
            }
            catch(Exception e)
            {

            }
            
           
            try
            {
                AOW.Photo = "";
                AOW.User = User.Identity.Name;
                AOW.StatiId = 1;
                AOW.KontragentId = 1;
                AOW.Kommisia = -1;
                if (Month.Equals("Не запланирована")== false)
                {
                    AOW.Zaplanirovana = true;
                    AOW.DateZaplanirovana = NewDate;
                }
                db.ActiveOsmotrWorks.Add(AOW);

                db.SaveChanges();
            }
            catch (Exception Ex)
            {

            }



             string DZZ = "";
            if (AOW.DateZaplanirovana != null)
            {
                DZZ = AOW.DateZaplanirovana.ToString();
            }
            else
            {
                DZZ = "";
            }

            string Data = El2 + ";" + OW.Name + ";" + OW.Izmerenie.Name + ";" + Number.ToString() + ";" + AOW.TotalCost.ToString()+";"+AOW.Id+ ";Add;" + DZZ;
            return Json(Data);
        }


        [HttpPost]
        public JsonResult AddRecommendWork(int OsmotrId, decimal Number, int IzmerenieId, int PartId, decimal Cost, string Name, int WorkId =0)
        {
            if (WorkId == 0)
            {
                OsmotrRecommendWork ROW = new OsmotrRecommendWork();
                ROW.OsmotrId = OsmotrId;
                ROW.Number = Number;
                ROW.Cost = Cost;
                ROW.IzmerenieId = IzmerenieId;
                ROW.Izmerenie = db.Izmerenies.Find(IzmerenieId);
                ROW.DOMPartId = PartId;
                ROW.DOMPart = db.DOMParts.Find(PartId);
                ROW.Name = Name;
                ROW.Number = Number;
                ROW.Photo = "Нет";
                ROW.StatiId = 1;
                ROW.Kommisia = -1;
                ROW.User = User.Identity.Name;
                try
                {
                    db.OsmotrRecommendWorks.Add(ROW);
                    db.SaveChanges();
                   
                }
                catch (Exception e)
                {

                }
                string Data = ROW.DOMPart.Name + ";" + ROW.Name + ";" + ROW.Izmerenie.Name + ";" + Number.ToString() + ";" + ROW.Cost.ToString() + ";" + ROW.Id + ";Add";
                return Json(Data);
            }
            else
            {
                //изменение количества и цены если совпал тип работы
                OsmotrRecommendWork A2 = db.OsmotrRecommendWorks.Where(x => x.OsmotrId == OsmotrId && x.Name.Equals(Name)).First();
                A2.Cost = Cost;
                A2.DOMPartId = PartId;
                A2.DOMPart = db.DOMParts.Find(PartId);
                A2.IzmerenieId = IzmerenieId;
                A2.Izmerenie = db.Izmerenies.Find(IzmerenieId);
                A2.Number = Number;
                A2.StatiId = 1;
                db.Entry(A2).State = EntityState.Modified;
                db.SaveChanges();
                string D = A2.DOMPart.Name + ";" + A2.Name + ";" + A2.Izmerenie.Name + ";" + Number.ToString() + ";" + A2.Cost.ToString() + ";" + A2.Id + ";Modify";
                return Json(D);
            }
     


          
           
        }

        public ActionResult PlanoviStatiView()
        {
            PlanoviStatiView PSW = new PlanoviStatiView();
            PSW.ORW = new List<OsmotrRecommendWork>();
            PSW.AOW = new List<ActiveOsmotrWork>();
            DateTime D = DateTime.Now;

            List<OsmotrRecommendWork> ORW = db.OsmotrRecommendWorks.Where(x => x.Gotovo &&x.DateVipolneniya.Year>=D.Year).OrderBy(x => x.OsmotrId).ThenBy(x => x.DOMPartId).Include(x => x.DOMPart).Include(x => x.Izmerenie).Include(x => x.Stati).ToList();
            List<ActiveOsmotrWork> AOW = db.ActiveOsmotrWorks.Where(x => x.Gotovo && x.DateVipolneniya.Year >= D.Year).OrderBy(x => x.OsmotrId).ThenBy(x=>x.ElementId).Include(x =>x.OsmotrWork).Include(x=>x.OsmotrWork.Izmerenie).Include(x=>x.OsmotrWork.DOMPart).Include(x => x.Stati).ToList();
            List<int> AID = AOW.Select(x => x.OsmotrId).Distinct().ToList();
            List<int> OID = ORW.Select(x => x.OsmotrId).Distinct().ToList();
            foreach (int I in OID)
            {
               Osmotr O = db.Osmotrs.Where(x => x.Id == I).Include(x => x.Adres).First(); //.Include(x => x.ORW.Select(y => y.Izmerenie)).Include(x => x.ORW.Select(y => y.DOMPart))
                List<OsmotrRecommendWork> RW = ORW.Where(x => x.OsmotrId == I).ToList();
                foreach(OsmotrRecommendWork R in RW)
                {
                    R.Osmotr = O;
                }
                PSW.ORW.AddRange(RW);
            }
            foreach (int I in AID)
            {
                Osmotr O = db.Osmotrs.Where(x => x.Id == I).Include(x => x.Adres).First(); //.Include(x => x.ORW.Select(y => y.Izmerenie)).Include(x => x.ORW.Select(y => y.DOMPart))
                List<ActiveOsmotrWork> RW = AOW.Where(x => x.OsmotrId == I).ToList();
                foreach (ActiveOsmotrWork R in RW)
                {
                    R.Osmotr = O;
                }
                PSW.AOW.AddRange(RW);
            }
            PSW.Statis = db.Statis.ToList();

     
         

            return View(PSW);
        }
        [HttpPost]
        public JsonResult ChangeStati (int id, int stati)
        {
            OsmotrRecommendWork ORW = db.OsmotrRecommendWorks.Where(x => x.Id == id).First();
            ORW.StatiId = stati;
            try
            {
                db.Entry(ORW).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                return Json("Ошибка");
            }
            return Json("Ок");
        }

        [HttpPost]
        public JsonResult ChangeStatiAOW(int id, int stati)
        {
           var ORW = db.ActiveOsmotrWorks.Where(x => x.Id == id).First();
            ORW.StatiId = stati;
            try
            {
                db.Entry(ORW).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                return Json("Ошибка");
            }
            return Json("Ок");
        }


        [HttpPost]
        public JsonResult ChangeKomm(int id, int komm)
        {
            OsmotrRecommendWork ORW = db.OsmotrRecommendWorks.Where(x => x.Id == id).First();
            ORW.Kommisia = komm;
            try
            {
                db.Entry(ORW).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                return Json("Ошибка");
            }
            return Json("Ок");
        }

        [HttpPost]
        public JsonResult ChangeKommAOW(int id, int komm)
        {
            var ORW = db.ActiveOsmotrWorks.Where(x => x.Id == id).First();
            ORW.Kommisia = komm;
            try
            {
                db.Entry(ORW).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                return Json("Ошибка");
            }
            return Json("Ок");
        }

        [HttpPost]
        public JsonResult EditRecommendWork(int ORW, decimal Number,  int PartId, decimal Cost, int IzmerenieId, string Name)
        {
            OsmotrRecommendWork ROW = db.OsmotrRecommendWorks.Where(x => x.Id == ORW).First();
            ROW.OsmotrId = ORW;
            ROW.Number = Number;
            ROW.Cost = Cost;
            ROW.IzmerenieId = IzmerenieId;
            ROW.Izmerenie = db.Izmerenies.Find(IzmerenieId);
            ROW.DOMPartId = PartId;
            ROW.DOMPart = db.DOMParts.Find(PartId);
            ROW.Name = Name;
            ROW.Number = Number;
            ROW.Photo = "Нет";
          


            try
            {
                db.OsmotrRecommendWorks.Add(ROW);
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
            string Data = ROW.DOMPart.Name + ";" + ROW.Name + ";" + ROW.Izmerenie.Name + ";" + Number.ToString() + ";" + ROW.Cost.ToString() + ";" + ROW.Id + ";Add";
            return Json(Data);
        }


        [HttpPost]
        public JsonResult RemoveWork(int id)
        {
            try
            {
               ActiveOsmotrWork AOW = db.ActiveOsmotrWorks.Where(x => x.Id == id).First();
                db.ActiveOsmotrWorks.Remove(AOW);
                db.SaveChanges();
            }
            catch
            {
                return Json("Ошибка");
            }
            return Json("OK");
        }
        [HttpPost]
        public JsonResult RemoveWorkORW(int id)
        {
            try
            {
                OsmotrRecommendWork ORW = db.OsmotrRecommendWorks.Where(x => x.Id == id).First();
                db.OsmotrRecommendWorks.Remove(ORW);
                db.SaveChanges();
            }
            catch
            {
                return Json("Ошибка");
            }
            return Json("OK");
        }
        [HttpPost]
        public JsonResult RemoveRecommendWork(int id)
        {
            try
            {
                OsmotrRecommendWork ROW = db.OsmotrRecommendWorks.Where(x => x.Id == id).First();
                db.OsmotrRecommendWorks.Remove(ROW);
                db.SaveChanges();
            }
            catch
            {
                return Json("Ошибка");
            }
            return Json("OK");
        }

        //служит для отображения списка активных дефектов и возможных дефектов модели
        public ActionResult ViewActiveDefectReadOnly(DateTime Date, int ElementId = 1, int OsmotrId = 1, int AdresId = 1, ActiveDefect A = null)
        {
          
           // HttpCookie cookieReq = Request.Cookies["Osmotr"];
          //  if (cookieReq != null)
           // {
           //     Date = Convert.ToDateTime(cookieReq["Date"]);
          //      OsmotrId = Convert.ToInt32(cookieReq["OsmotrId"]);
          //      AdresId = Convert.ToInt32(cookieReq["AdresId"]);
          //  }
          //  else
          //  {

          //  }
            string Data = "";
            ActiveElement AE = new ActiveElement();
            
            List<BuildElement> BE = new List<BuildElement>();
            Build B = new Build();

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
                     //   AE = db.ActiveElements.Where(x => x.ElementId == ElementId && x.AdresId == AdresId&&x.OsmotrId==OsmotrId).Include(x => x.Element).Include(x=>x.Izmerenie).Include(x=>x.Material).First();

                        Adres Ad = db.Adres.Where(x => x.Id == AdresId).First();
                        AE = db.ActiveElements.Where(x => x.OsmotrId == OsmotrId&& x.ElementId == ElementId).Include(x => x.Element).Include(x => x.Material).Include(x => x.Izmerenie).OrderByDescending(x => x.Date).First();
                       
                        //    B = db.Builds.Where(x => x.Id == Ad.BuildId).First();//совмещены все кроме Морского и Шатурской 10 и тесла
                         //   int E = db.Elements.Where(x => x.Id == ElementId).Select(x => x.ElementId).First();//ищем связку элемент ИД в Элементе. Это ссылка на элементы в справочнике Build_ELements
                        AE.M = AE.MaterialId;
                        AE.EI = AE.IzmerenieId;

                        /*
                        try
                        { 
                            BuildElement BuildE = db.BuildElements.Where(z => z.ElementId == E && z.BuildId == B.Id).First();
                            AE.M = BuildE.Material;
                            AE.EI = BuildE.EdIzm;
                        }
                        catch
                        {
                            AE.M = AE.MaterialId;
                            AE.EI = AE.IzmerenieId;
                        }
                        */

                    }
                    catch
                    {//если осмотр новый
                        AE.ElementId = ElementId;
                        AE.Element = db.Elements.Where(x => x.Id == ElementId).First();
                        AE.OsmotrId = OsmotrId;
                        AE.AdresId = AdresId;
                        AE.Date = Date;
                        AE.Sostoyanie = 5;


                        //db.ActiveElements.Add(AE);
                       // db.SaveChanges();
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
                    List<ActiveOsmotrWork> AOW = new List<ActiveOsmotrWork>();
                    try
                    {
                        AOW = db.ActiveOsmotrWorks.Where(x => x.ElementId == ElementId && x.OsmotrId == OsmotrId).Include(x => x.OsmotrWork).Include(x => x.OsmotrWork.Izmerenie).ToList();
                        AE.ActiveOsmotrWorks = AOW;
                    }
                    catch
                    {

                    }


                }
            }
            else
            {
                return null;
            }
            return PartialView("ViewActiveDefectReadOnly", AE);
        }

        public ActionResult OsmotrsTech()
        {
           DateTime D = DateTime.Now;
           List<Osmotr> O = db.Osmotrs.Where(x=>x.Date.Year==D.Year).Include(x =>x.Adres).OrderBy(x=>x.Adres.Adress).ToList();
            return View(O);
        }


        public JsonResult ChangeAEValue(int Id,decimal Val )
        {
            DateTime D = DateTime.Now;
            try
            {
                ActiveElement AE = db.ActiveElements.Where(x => x.Id == Id).First();
                AE.Kolichestvo = Val;
                AE.DateIzmeneniya = D;
                db.Entry(AE).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return Json ("");
        }
        public JsonResult ChangeAEValue2(int Id, decimal Val)
        {
            DateTime D = DateTime.Now;
            try
            {
                ActiveElement AE = db.ActiveElements.Where(x => x.Id == Id).First();
                AE.Kolichestvo2 = Val;
                AE.DateIzmeneniya = D;
                db.Entry(AE).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return Json("");
        }

        public JsonResult ChangeTypeElement(int Id, int Val)
        {
            DateTime D = DateTime.Now;
            try
            {
                TypeElement TE = db.TypeElements.Where(x => x.Id == Id).First();
                TE.ConstructiveTypeId = Val;
                TE.Date = D;
                db.Entry(TE).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return Json("");
        }

        public JsonResult ChangeMaterialElement(int Id, int Val)
        {
            DateTime D = DateTime.Now;
            try
            {
                TypeElement TE = db.TypeElements.Where(x => x.Id == Id).First();
                TE.MaterialId = Val;
                TE.Date = D;
                db.Entry(TE).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return Json("");
        }

        public JsonResult ChangeMaterial(int Id, int Val)
        {
            DateTime D = DateTime.Now;
            try
            {
               ActiveElement AE = db.ActiveElements.Where(x => x.Id == Id).First();
                AE.MaterialId = Val;
                AE.Date = D;
                db.Entry(AE).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return Json("");
        }

        public JsonResult ChangeTEValue(int Id, decimal Val)
        {
            DateTime D = DateTime.Now;
            try
            {
                TechElement TE = db.TechElements.Where(x => x.Id == Id).First();
                TE.Val = Val;
                TE.Date = D;
                db.Entry(TE).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return Json("");
        }

        public ActionResult OsmotrsTechById(int Id)
        {
            DateTime D = DateTime.Now;
           Osmotr O = db.Osmotrs.Where(x => x.Id == Id).Include(x => x.Adres).First();
            try
            {
                O.DOMParts = db.DOMParts.OrderBy(x=>x.Id).ToList();
                O.Elements = db.ActiveElements.Where(x => x.OsmotrId == Id).Include(x=>x.Element).Include(x=>x.Izmerenie).Include(x => x.Izmerenie2).Include(x=>x.Material).OrderBy(x=>x.Element.ElementTypeId).ThenBy(x=>x.Element.Name).ToList();
                O.TE = db.TechElements.Where(x => x.AdresId == O.AdresId).Include(x=>x.Izmerenie).ToList();
                O.TypeE = db.TypeElements.Where(x => x.AdresId == O.AdresId).Include(x => x.ConstructiveType).Include(x => x.DOMPart).Include(x => x.Material).ToList();
                O.Materials = db.Materials.ToList();
            }
            catch (Exception e)
            {

            }
            if (O.TypeE!=null)
            {
                foreach (TypeElement t in O.TypeE)
                {
                    t.CT = new SelectList(db.ConstructiveTypes.Where(x=>x.DOMPartId==t.DOMPartId).ToList(), "Id", "Name");
                }
            }
            ViewBag.ConstructiveTypes = new SelectList(db.ConstructiveTypes, "Id", "Name");
            ViewBag.DOMParts = new SelectList(db.DOMParts, "Id", "Name");
            ViewBag.Materials = new SelectList(db.Materials, "Id", "Name");
            ViewBag.Error = "";
            return View(O);
        }



        public static string Alert (string clas, string value)
        {
            return "<div class='" + clas + "' role='alert'>'" + value + "'</div>";
        }
        [HttpPost]
        public JsonResult MaterialToMaterial(string Material = "", string ToMaterial = "")
        {
            Material Mat = new Material();
            Material ToMat = new Material();
            string clas = "alert alert-success";
            string value = "";
            //string Alert = "<div class='"+clas+"' role='alert'>'"+value+"'</div>";
            int M = Convert.ToInt32(Material);
            int TOM = Convert.ToInt32(ToMaterial);
            List<string> Errors = new List<string>();
             if (Material != "" && ToMaterial != "")
            {
                bool errors = false;
                try
                {
                    Mat = db.Materials.Where(x => x.Id == M).First();
                }
                catch (Exception e)
                {
                    clas = "alert alert-danger";
                    value = "Нет такого материала" + Mat.Id;
                    Errors.Add(Alert(clas, value));
                    errors = true;
                }
                try
                {
                    ToMat = db.Materials.Where(x => x.Id == TOM).First();
                }
                catch (Exception e)
                {
                    errors = true;
                    clas = "alert alert-danger";
                    value = "Нельзя превратить в выбранный материал" + ToMat.Id;
                    Errors.Add(Alert(clas,value));
                }
                if (errors == false)
                {
                    clas = "alert alert-success";
                    try
                    {
                        List<BuildElement> BE = db.BuildElements.Where(x => x.Material == Mat.Id).ToList();
                        foreach (BuildElement B in BE)
                        {
                            try
                            {
                                B.Material = ToMat.Id;
                                db.Entry(B).State = EntityState.Modified;
                                db.SaveChanges();
                                clas = "alert alert-primary";
                                value = "Ссылка на материал в Билд Элементе " + B.Id.ToString() + " успешно изменена!";
                                Errors.Add(Alert(clas, value));

                            }
                            catch
                            {
                                clas = "alert alert-danger";
                                value = "Нельзя превратить материал " + B.Material;
                                Errors.Add(Alert(clas, value));
                            }
                        }
                        try
                        {
                            Material RM = db.Materials.Where(x => x.Id == Mat.Id).First();
                            db.Materials.Remove(RM);
                            db.SaveChanges();
                            clas = "alert alert-success";
                            value = "Материал " + RM.Id.ToString() + " успешно удалён!";
                            Errors.Add(Alert(clas, value));

                        }
                        catch
                        {
                            clas = "alert alert-danger";
                            value = "Нельзя удалить материал " + Mat.Id;
                            Errors.Add(Alert(clas, value));
                        }

                    }
                    catch
                    {
                       
                    }
                }
            }
           
            return Json(Errors);
        }

 

        public ActionResult MaterialEdit (string S = "")
        {
            S =S.ToUpper();
            List<Material> Materials = db.Materials.ToList();
            List<Material> ToMaterials = db.Materials.ToList();
            List<List<BuildElement>> BE = new List<List<BuildElement>>();
            if (S != "") {
                Materials = Materials.Where(x => x.Name.ToUpper().Contains(S)).ToList();
                ToMaterials = Materials.Where(x => x.Name.ToUpper().Contains(S)).ToList();
                foreach (Material M in Materials)
                {

                    List<BuildElement>  BEM = db.BuildElements.Where(x => x.Material == M.Id).ToList();
                    BE.Add(BEM);
                        
                        }
                    }
            ViewBag.ToMaterials = ToMaterials;
            ViewBag.BuildElements = BE;
            return View(Materials);
        }
        // GET: Osmotrs/Create

            public JsonResult SaveVivods(string Vivod, int id)
        {
            string Result = "";
            try
            {
               var O = db.Osmotrs.Where(x => x.Id == id).First();
                O.Vivods = Vivod;
                db.Entry(O).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
            return Json(Result);
        }

        public ActionResult Create(DateTime date,int id = 0,bool NewOsmotr=false)
        {
            Start:
            bool LoadOsmotr = false;
            string error = "";

            //Проверяем можно ли создавать новые осмотры
            if (NewOsmotr)
            {
              NewOsmotr = db.CanCreateOsmotrs.OrderByDescending(x => x.Id).Select(x=>x.Sozdanie).First();
            }
            //Если можно то проверяем есть ли осмотр по этому дому за этот год
            if (NewOsmotr)
            {
                NewOsmotr = false;
                int TO = 0;

                try
                {
                    //Ищем осмотры по Году и адресу
                    int DY = date.Year;
                 TO = db.Osmotrs.Where(x => x.Date.Year == DY && x.AdresId== id).Count();
                }
                catch
                {

                }
                if (TO==0)
                {
                    //Если количество найденных осмотров по году и адресу = 0 то разрешаем создать новый осмотр
                    NewOsmotr = true;
                }
                else
                {
                    //Иначе отправляем на главную страницу
                   return RedirectToAction("Index");
                }
            }

            List<Element> Elements = GetElements();// db.Elements.ToList();
            List<FundamentMaterial> FM = GetFundaments();
            ViewBag.FundamentMaterials = new SelectList(FM, "Id", "Material");
            List<FundamentType> FT = GetFundamentTypes();
            ViewBag.FundamentTypes = new SelectList(FT, "Id", "Type");
            List<DOMPart> Parts = GetDOMParts();


         //   List<DOMPart> DOMParts = new List<DOMPart>();
        //    DOMParts = GetDOMParts();
            var ALLParts = new SelectList(Parts, "Id", "Name");
            ViewBag.ALLParts = ALLParts;

            // ViewBag.DOMParts = Parts;

            if (date == null)
            {
                date = DateTime.Now;
                
            }

          
            Osmotr Result = new Osmotr();

            //ищем по базе осмотры, если есть за текущий месяц на данном доме то продолжаем заполнять его.

           
            // Если осмотр не новый то грузим старый иначе создаем новый
            if (!NewOsmotr)
            {
                try
                {

                    Result = db.Osmotrs.Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month && x.AdresId == id && x.Sostoyanie == 0).OrderByDescending(x => x.Date).Include(x => x.Adres).Include(x => x.DOMCW).Include(x => x.DOMElectro).Include(x => x.DOMFasad).Include(x => x.DOMFundament).Include(x => x.DOMHW).Include(x => x.DOMOtoplenie).Include(x => x.DOMRoof).Include(x => x.DOMRoom).Include(x => x.DOMVodootvod).First();
                    LoadOsmotr = true;//Удалось загрузить осмотр используем уже имеющиеся данные

                    Result.Elements = GetActiveElements(Result.Id);//db.ActiveElements.Where(x => x.OsmotrId == Result.Id).ToList();//берем все активные элементы и кидаем в список
                    Result.DOMParts = Parts;

                    try
                    {
                        Result.ORW = db.OsmotrRecommendWorks.Where(x => x.OsmotrId == Result.Id).Include(x=>x.Izmerenie).Include(x=>x.DOMPart).ToList();
                    }
                    catch
                    {

                    }


                    List<ActiveDefect> AD = GetActiveDefects(Result.Id);
                    if (Result.Elements.Count > 0)
                    {
                        foreach (ActiveElement A in Result.Elements)
                        {
                            A.ActiveDefects = AD.Where(x=> x.ElementId == A.ElementId).ToList();
                            A.Defects = db.Defects.Where(x => x.ElementId == A.ElementId).ToList();
                            A.Element = Elements.Where(x => x.Id == A.ElementId).First();
                        }
                    }
                    else
                    {

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
                catch (Exception ex)
                {
                    if (NewOsmotr)
                    {
                        NewOsmotr = db.CanCreateOsmotrs.OrderByDescending(x => x.Id).Select(x => x.Sozdanie).First();
                    }
                    if (NewOsmotr)
                    {
                        if (id != 0)
                        {
                            Result.AdresId = id;
                            Result.Adres = db.Adres.Where(x => x.Id == id).First();
                            Result.Date = date;

                            try
                            {
                                Result.ORW = db.OsmotrRecommendWorks.Where(x => x.OsmotrId == Result.Id).Include(x => x.Izmerenie).Include(x => x.DOMPart).ToList();
                            }
                            catch
                            {

                            }

                            Build B = new Build();//сашкины данные
                            List<BuildElement> BE = new List<BuildElement>();
                            List<Build> Builds = new List<Build>();
                            int test = 0;
                            try
                            {//пробуем грузануть данные по дому

                                Builds = db.Builds.ToList();
                                foreach (Build b in Builds)
                                {
                                    test++;
                                    try
                                    {
                                        b.Address = b.Address.ToUpper().Replace("Д.", "").Replace(",", "").Replace(" ", "").Replace("-", "").Replace("БУЛЬВ.", "").Replace("ПРОСП.", "");
                                    }
                                    catch { }
                                }


                                Result.DOMParts = new List<DOMPart>();
                                Result.DOMParts = Parts;
                                //Пробуем грузануть Сашкины поэлементные данные
                                try
                                {
                                    B = Builds.Where(x => x.Address.Equals(Result.Adres.Adress)).First();
                                    BE = db.BuildElements.Where(x => x.BuildId == B.Id).ToList();

                                }
                                catch (Exception e)
                                {

                                }



                                Result.BE = BE;
                            }
                            catch (Exception e)
                            {//если данных нет, значит проблема с загрузкой данных с ГИСЖКХ. Проверьте данные. 
                                Console.WriteLine(test);
                            }
                            if (BE == null || BE.Count == 0)
                            {
                                error += "Нет данных дома из ГИСЖКХ! Проверьте данные или заполните с нуля. Созданы нулевые данные.";
                                Result.DOMCW = new DOMCW(); ;
                                Result.DOMHW = new DOMHW();
                                Result.DOMElectro = new DOMElectro();
                                Result.DOMFasad = new DOMFasad();
                                Result.DOMFundament = new DOMFundament();

                                Result.DOMOtoplenie = new DOMOtoplenie();
                                Result.DOMRoof = new DOMRoof();
                                Result.DOMRoom = new DOMRoom();
                                Result.DOMVodootvod = new DOMVodootvod();
                            }

                            Result.Sostoyanie = 0;
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

                                        AE.Date = date;
                                        AE.OsmotrId = Result.Id;


                                    }
                                    catch (Exception e2)
                                    {
                                        AE.ElementId = E.Id;
                                        AE.Element = db.Elements.Where(x => x.Id == E.Id).First();
                                        AE.OsmotrId = Result.Id;
                                        AE.AdresId = id;
                                        AE.Date = date;
                                        AE.Sostoyanie = 5;
                                        AE.DateIzmeneniya = date;
                                        AE.UserName = User.Identity.Name;
                                        AE.Kolichestvo = 0;
                                        AE.IzmerenieId = 1;
                                        AE.MaterialId = 1;
                                        AE.Izmerenie = db.Izmerenies.Where(x => x.Id == AE.IzmerenieId).First();
                                        AE.Material = db.Materials.Where(x => x.Id == AE.MaterialId).First();
                                        AE.Est = true;
                                        try
                                        {
                                            AE.Defects = db.Defects.Where(x => x.ElementId == E.Id).ToList();
                                            AE.Defects = AE.Defects.OrderBy(x => x.Def).ToList();
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
                            error += " Не определен ИД дома!!! Не можем создать осмотр. ИД дома =" + id.ToString() + " Дата=" + date.ToString();
                            return RedirectToAction("Error", error);
                        }
                    }
                }
            }
            else
            {
                //Создаем новый осмотр.
                if (NewOsmotr)
                {
                    Osmotr LastO = new Osmotr();
                    Osmotr NewO = new Osmotr();

                    try
                    {
                        //Ищем старый осмотр и берем из него данные
                        //пробуем загрузить данные предыдущих осмотров
                        try
                        {
                            LastO = db.Osmotrs.Where(x => x.AdresId == id).OrderByDescending(x => x.Id).First();

                            LastO.Adres = db.Adres.Where(x => x.Id == id).First();
                            LastO.BE = new List<BuildElement>();
                            LastO.Elements = new List<ActiveElement>();
                            LastO.Defects = new List<ActiveDefect>();
                            LastO.DOMParts = new List<DOMPart>();
                        }
                        catch { LastO = null; }
                        //теперь у нас могут быть данные предыдущего осмотра
                        // DateTime D = DateTime.Now;
                        DateTime D = new DateTime(date.Year, 5, 15);
                        //сохраняем новый осмотр если дата прошлого отличается хотя бы на  или прошлого осмотра нет
                        if (LastO == null ||LastO.Date.Month != D.Date.Month || LastO.Date.Year != D.Date.Year)
                        {
                            int last = 0;
                            
                            int newo =0;
                            try
                            {
                                NewO = new Osmotr();
                                NewO.AdresId = id;
                                NewO.Adres = db.Adres.Where(x => x.Id == id).First();
                                NewO.Date = date;
                                NewO.DateEnd = date;
                                NewO.DateOEGF = date;
                                NewO.DatePTO = date;
                                NewO.Opisanie = "Первый осмотр";
                               
                                db.Osmotrs.Add(NewO);
                                db.SaveChanges();
                                newo = NewO.Id;
                                OtmetitEvent(User.Identity.Name+" создал новый осмотр #" + NewO.Id + " по адресу " + NewO.Adres.Adress, "table-info");
                               // System.IO.File.Copy(HostingEnvironment.MapPath(LastPath + AE.Photo1), HostingEnvironment.MapPath(NewPath + AE.Photo1), true);
                                if (Directory.Exists(HostingEnvironment.MapPath("/Files/" + newo)) ==false)
                                {
                                    try
                                    {
                                        Directory.CreateDirectory(HostingEnvironment.MapPath("/Files/" + newo));
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    
                                }
                                NewO.AOW = new List<ActiveOsmotrWork>();
                                NewO.ORW = new List<OsmotrRecommendWork>();
                                NewO.Elements = new List<ActiveElement>();
                                NewO.Defects = new List<ActiveDefect>();

                                NewO.DOMParts = Parts;

                                //добавляем куки с осмотром
                                HttpCookie cookie = new HttpCookie("Osmotr");
                                cookie["Date"] = NewO.Date.ToString();
                                cookie["OsmotrId"] = NewO.Id.ToString();
                                cookie["AdresId"] = id.ToString();
                                // Добавить куки в ответ
                                Response.Cookies.Add(cookie);
                            }
                            catch (Exception ec) { }
                            //теперь у нас есть ID нового осмотра

                            // работаем со старым осмотром
                            if (LastO != null)
                            {
                                try
                                {
                                    //пробуем найти активные работы старого осмотра и которые не выполнены
                                    List<ActiveOsmotrWork> LastAOW = db.ActiveOsmotrWorks.Where(x => x.OsmotrId == LastO.Id && !x.Gotovo).ToList();

                                    //Сохраняем их под новыми ID и делаем ссылку на новый осмотр
                                    foreach (ActiveOsmotrWork AOW in LastAOW)
                                    {
                                        AOW.OsmotrId = NewO.Id;
                                        db.ActiveOsmotrWorks.Add(AOW);
                                        db.SaveChanges();
                                        NewO.AOW.Add(AOW);
                                    }

                                }
                                catch { }


                                try
                                {
                                    // пробуем найти рекомендуемые работы предыдущего осмотра которые еще не выполнены
                                    List<OsmotrRecommendWork> LastORW = db.OsmotrRecommendWorks.Where(x => x.OsmotrId == LastO.Id && !x.Gotovo).Include(x=>x.Izmerenie).Include(x=>x.DOMPart).ToList();

                                    //Сохраняем их под новыми ID и делаем ссылку на новый осмотр
                                    foreach (OsmotrRecommendWork ORW in LastORW)
                                    {
                                        ORW.OsmotrId = NewO.Id;
                                        db.OsmotrRecommendWorks.Add(ORW);
                                        db.SaveChanges();

                                    }

                                }
                                catch { }

                                //Создаем элементы 
                                CreateElements(Elements, NewO, LastO);


                                //если нашелся осмотр то отлично
                                /*  try { LastO.DOMCW = db.DOMCWs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMCW = new DOMCW(); LastO.DOMCW.AdresId = id; LastO.DOMCW.Date = DateTime.Now; }
                                  try { LastO.DOMHW = db.DOMHWs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMHW = new DOMHW(); LastO.DOMHW.AdresId = id; LastO.DOMHW.Date = DateTime.Now; }
                                  try { LastO.DOMElectro = db.DOMElectroes.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMElectro = new DOMElectro(); LastO.DOMElectro.AdresId = id; LastO.DOMElectro.Date = DateTime.Now; }
                                  try { LastO.DOMFasad = db.DOMFasads.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMFasad = new DOMFasad(); LastO.DOMFasad.AdresId = id; LastO.DOMFasad.Date = DateTime.Now;LastO.DOMFasad.Sostoyanie = 1; }
                                  try { LastO.DOMFundament = db.DOMFundaments.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).Include(x => x.Material).Include(x => x.Type).First(); } catch { LastO.DOMFundament = new DOMFundament(); LastO.DOMFundament.Date = DateTime.Now; LastO.DOMFundament.Sostoyanie = 1; LastO.DOMFundament.MaterialId = 1;LastO.DOMFundament.AdresId = id; }
                                  try { LastO.DOMOtoplenie = db.DOMOtoplenies.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMOtoplenie = new DOMOtoplenie();  LastO.DOMOtoplenie.AdresId = id; LastO.DOMOtoplenie.Date = DateTime.Now; }
                                  try { LastO.DOMRoof = db.DOMRoofs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMRoof = new DOMRoof(); LastO.DOMRoof.AdresId = id; LastO.DOMRoof.Date = DateTime.Now; }
                                  try { LastO.DOMRoom = db.DOMRooms.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMRoom = new DOMRoom(); LastO.DOMRoom.AdresId = id; LastO.DOMRoom.Date = DateTime.Now; }
                                  try { LastO.DOMVodootvod = db.DOMVodootvods.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMVodootvod = new DOMVodootvod(); LastO.DOMVodootvod.AdresId = id; LastO.DOMVodootvod.Date = DateTime.Now; }
                                */

                            }
                            else
                            {
                                //сделать для каждого элемента создание активного элемента
                                List<Element> El = new List<Element>();
                               
                                    El = db.Elements.ToList();
                                    foreach (Element E in El)
                                    {
                                    try
                                    {
                                        ActiveElement AE = new ActiveElement();
                                        AE.OsmotrId = NewO.Id;
                                        AE.ElementId = E.Id;

                                        AE.IzmerenieId = 1;
                                        AE.Izmerenie = db.Izmerenies.Where(x => x.Id == 1).First();
                                        AE.Date = DateTime.Now;
                                        AE.DateIzmeneniya = DateTime.Now;
                                        AE.Kolichestvo = 0;
                                        AE.MaterialId = 1;
                                        AE.Material = db.Materials.Where(x => x.Id == 1).First();
                                        AE.Photo1 = "";
                                        AE.Photo2 = "";
                                        AE.Sostoyanie = 5;
                                        AE.Est = true;
                                        AE.AdresId = NewO.AdresId.Value;
                                        AE.Adres = NewO.Adres;
                                        AE.UserName = User.Identity.Name;
                                        db.ActiveElements.Add(AE);
                                        db.SaveChanges();
                                        NewO.Elements.Add(AE);
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                
                                }
                               
                                
                            }

                            Result = NewO;//отправляем осмотр в результат
                        }

                    }
                    catch (Exception e3)
                    {//если нет осмотра то возвращаем пустой осмотр с созданными элементами
                        LastO = new Osmotr();
                        LastO.Date = DateTime.Now;
                        NewO.AdresId = id;
                        CreateElements(Elements, NewO, LastO);
                        Result = NewO;
                    }
                    //Нужно обновить сессию осмотров
                    //  DateTime FromDate = DateTime.Now.AddYears(-1);
                    //  DateTime ToDate = DateTime.Now;
                    //DateTime FromDate = Convert.ToDateTime("");
                    Session["Houses" + NewO.Adres.Adress] = null;
                    NewOsmotr = false;
                    goto Start;
                }
            }
            ViewBag.Month = Opredelenie.Opr.MonthToNorm(Opredelenie.Opr.MonthOpred(Result.Date.Month));
            ViewBag.Materials = new SelectList(db.Materials, "Id", "Material");
            ViewBag.Izmerenies = new SelectList(db.Izmerenies, "Id", "Name");
            ViewBag.Error = error;
            return View(Result);
        }



        public ActionResult OsmotrMobile(DateTime date, int id = 0)
        {
            bool NewOsmotr = false;
            bool LoadOsmotr = false;
            string error = "";



            List<Element> Elements = GetElements();// db.Elements.ToList();
            List<FundamentMaterial> FM = GetFundaments();
            ViewBag.FundamentMaterials = new SelectList(FM, "Id", "Material");
            List<FundamentType> FT = GetFundamentTypes();
            ViewBag.FundamentTypes = new SelectList(FT, "Id", "Type");
            List<DOMPart> Parts = GetDOMParts();
            // ViewBag.DOMParts = Parts;

            if (date == null)
            {
                date = DateTime.Now;
            }

            Osmotr Result = new Osmotr();

            //ищем по базе осмотры, если есть за текущий месяц на данном доме то продолжаем заполнять его.


            // Если осмотр не новый то грузим старый иначе создаем новый
            if (!NewOsmotr)
            {
                try
                {

                    Result = db.Osmotrs.Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month && x.AdresId == id && x.Sostoyanie == 0).OrderByDescending(x => x.Date).Include(x => x.Adres).Include(x => x.DOMCW).Include(x => x.DOMElectro).Include(x => x.DOMFasad).Include(x => x.DOMFundament).Include(x => x.DOMHW).Include(x => x.DOMOtoplenie).Include(x => x.DOMRoof).Include(x => x.DOMRoom).Include(x => x.DOMVodootvod).First();
                    LoadOsmotr = true;//Удалось загрузить осмотр используем уже имеющиеся данные

                    Result.Elements = GetActiveElements(Result.Id);//db.ActiveElements.Where(x => x.OsmotrId == Result.Id).ToList();//берем все активные элементы и кидаем в список
                    Result.DOMParts = Parts;

                    List<ActiveDefect> AD = GetActiveDefects(Result.Id);
                    if (Result.Elements.Count > 0)
                    {
                        foreach (ActiveElement A in Result.Elements)
                        {
                            A.ActiveDefects = AD.Where(x => x.ElementId == A.ElementId).ToList();
                            A.Defects = db.Defects.Where(x => x.ElementId == A.ElementId).ToList();
                            A.Element = Elements.Where(x => x.Id == A.ElementId).First();
                        }
                    }
                    else
                    {

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
                catch (Exception ex)
                {

                    if (id != 0)
                    {
                        Result.AdresId = id;
                        Result.Adres = db.Adres.Where(x => x.Id == id).First();
                        Result.Date = date;


                        Build B = new Build();//сашкины данные
                        List<BuildElement> BE = new List<BuildElement>();
                        List<Build> Builds = new List<Build>();
                        int test = 0;
                        try
                        {//пробуем грузануть данные по дому

                            Builds = db.Builds.ToList();
                            foreach (Build b in Builds)
                            {
                                test++;
                                try
                                {
                                    b.Address = b.Address.ToUpper().Replace("Д.", "").Replace(",", "").Replace(" ", "").Replace("-", "").Replace("БУЛЬВ.", "").Replace("ПРОСП.", "");
                                }
                                catch { }
                            }
                            //отключены 11.11.2020 нужно перенести в отдельную таблицу
                            //  Result.DOMCW = db.DOMCWs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                            // Result.DOMHW = db.DOMHWs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                            //  Result.DOMElectro = db.DOMElectroes.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                            //  Result.DOMFasad = db.DOMFasads.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                            //  Result.DOMFundament = db.DOMFundaments.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).Include(x => x.Material).Include(x => x.Type).First();

                            //  Result.DOMOtoplenie = db.DOMOtoplenies.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                            //   Result.DOMRoof = db.DOMRoofs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                            //   Result.DOMRoom = db.DOMRooms.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                            //   Result.DOMVodootvod = db.DOMVodootvods.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();

                            Result.DOMParts = new List<DOMPart>();
                            Result.DOMParts = Parts;
                            //Пробуем грузануть Сашкины поэлементные данные
                            try
                            {
                                B = Builds.Where(x => x.Address.Equals(Result.Adres.Adress)).First();
                                BE = db.BuildElements.Where(x => x.BuildId == B.Id).ToList();

                            }
                            catch (Exception e)
                            {

                            }

                            // BE=db.BuildElements.ToList();
                            //Тут сохранение материалов

                            /*
                              foreach (BuildElement El in BE)
                              {
                                  El.Count = El.Count.Replace(" ", "").Replace("-","").Replace(".","").ToLower();
                                 if (El.Count == "") { El.Count = "0"; }
                                  try
                                  {
                                         decimal Z = Convert.ToDecimal(El.Count);
                                      El.Count = Z.ToString();
                                      db.Entry(El).State = EntityState.Modified;
                                      db.SaveChanges();
                                     continue;

                                  }catch (Exception e)
                                  {
                                     try
                                     {
                                         El.Count = "0";
                                         if (El.Material!=null&&El.Material < 1) {
                                             El.Material = 1;
                                         }
                                         if (El.Material == 50) { El.Material = 1; }
                                         db.Entry(El).State = EntityState.Modified;
                                         db.SaveChanges();
                                     }
                                     catch
                                     {

                                     }
                                 }

                              }
                             */


                            /*
                             foreach (BuildElement El in BE)
                             {
                                 El.EdIzm = El.EdIzm.Replace(" ", "").Replace(",", "").Replace("-","").Replace(".","").ToLower();
                                 if (El.EdIzm.Length>0&&El.EdIzm[0].Equals('/'))
                                 {
                                     El.EdIzm = El.EdIzm.Remove(0, 1);
                                 }
                                 if (El.EdIzm.Equals(""))
                                 {
                                     El.EdIzm = "нет";
                                 }
                                 if (El.EdIzm.Equals("мпог")|| El.EdIzm.Equals("мп") || El.EdIzm.Equals("пм") || El.EdIzm.Equals("погм"))
                                 {
                                     El.EdIzm = "пог.м";
                                 }
                                 try
                                 {
                                     int Z = Convert.ToInt32(El.EdIzm);
                                         continue;

                                 }catch
                                 {

                                 }
                                 Izmerenie M = null;
                                 try
                                 {
                                    M = db.Izmerenies.Where(x => x.Name.Equals(El.EdIzm)).First();
                                     try
                                     {
                                         El.EdIzm = M.Id.ToString();
                                         db.Entry(El).State = EntityState.Modified;
                                         db.SaveChanges();
                                     }
                                     catch
                                     {
                                     }
                                 } catch
                                 {

                                 }
                                 if (M == null)
                                 {
                                     try
                                     {
                                         M = new Izmerenie();
                                         M.Name = El.EdIzm.Replace(" ", "");
                                         db.Izmerenies.Add(M);
                                         db.SaveChanges();
                                         El.EdIzm = M.Id.ToString();
                                         db.Entry(El).State = EntityState.Modified;
                                         db.SaveChanges();
                                     }
                                     catch
                                     {

                                     }
                                 }


                             }
                             */


                            /*   //Совмещаем адреса и билдингс
                             *   List<Adres> Ad = db.Adres.ToList();
                               foreach (Adres A in Ad)
                               {
                                   if (A.BuildId != 0) { continue; }
                                   try
                                   {
                                       B = db.Builds.Where(x => x.Address.Replace("д.", "").Replace(",", "").Replace(" ", "").Replace("БУЛЬВ.", "").ToUpper().Replace("ПРОЕЗД","").Replace("ПРОСПЕКТ", "").Replace("БУЛЬВ", "").Equals(A.Adress.Replace(" ",""))).First();
                                       A.BuildId = B.Id;
                                       db.Entry(A).State = EntityState.Modified;
                                       db.SaveChanges();
                                   }
                                   catch (Exception e)
                                   {

                                   }
                               }
                               */

                            Result.BE = BE;
                        }
                        catch (Exception e)
                        {//если данных нет, значит проблема с загрузкой данных с ГИСЖКХ. Проверьте данные. 
                            Console.WriteLine(test);
                        }
                        if (BE == null || BE.Count == 0)
                        {
                            error += "Нет данных дома из ГИСЖКХ! Проверьте данные или заполните с нуля. Созданы нулевые данные.";
                            Result.DOMCW = new DOMCW(); ;
                            Result.DOMHW = new DOMHW();
                            Result.DOMElectro = new DOMElectro();
                            Result.DOMFasad = new DOMFasad();
                            Result.DOMFundament = new DOMFundament();

                            Result.DOMOtoplenie = new DOMOtoplenie();
                            Result.DOMRoof = new DOMRoof();
                            Result.DOMRoom = new DOMRoom();
                            Result.DOMVodootvod = new DOMVodootvod();
                        }

                        Result.Sostoyanie = 0;
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

                                    AE.Date = date;
                                    AE.OsmotrId = Result.Id;


                                }
                                catch (Exception e2)
                                {
                                    AE.ElementId = E.Id;
                                    AE.Element = db.Elements.Where(x => x.Id == E.Id).First();
                                    AE.OsmotrId = Result.Id;
                                    AE.AdresId = id;
                                    AE.Date = date;
                                    AE.Sostoyanie = 5;
                                    AE.DateIzmeneniya = date;
                                    AE.UserName = User.Identity.Name;
                                    AE.Kolichestvo = 0;
                                    AE.IzmerenieId = 1;
                                    AE.MaterialId = 1;
                                    AE.Izmerenie = db.Izmerenies.Where(x => x.Id == AE.IzmerenieId).First();
                                    AE.Material = db.Materials.Where(x => x.Id == AE.MaterialId).First();
                                    AE.Est = true;
                                    try
                                    {
                                        AE.Defects = db.Defects.Where(x => x.ElementId == E.Id).ToList();
                                        AE.Defects = AE.Defects.OrderBy(x => x.Def).ToList();
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
                        error += " Не определен ИД дома!!! Не можем создать осмотр. ИД дома =" + id.ToString() + " Дата=" + date.ToString();
                        return RedirectToAction("Error", error);
                    }

                }
            }
            else
            {
                //Создаем новый осмотр.

                Osmotr LastO = new Osmotr();
                Osmotr NewO = new Osmotr();

                try
                {
                    //Ищем старый осмотр и берем из него данные
                    //пробуем загрузить данные предыдущих осмотров
                    try
                    {
                        LastO = db.Osmotrs.Where(x => x.AdresId == id).OrderByDescending(x => x.Id).First();

                        LastO.Adres = db.Adres.Where(x => x.Id == id).First();
                        LastO.BE = new List<BuildElement>();
                        LastO.Elements = new List<ActiveElement>();
                        LastO.Defects = new List<ActiveDefect>();
                        LastO.DOMParts = new List<DOMPart>();
                    }
                    catch { LastO = null; }
                    //теперь у нас могут быть данные предыдущего осмотра
                    DateTime D = DateTime.Now;
                    //сохраняем новый осмотр если дата прошлого отличается хотя бы на месяц
                    if (LastO.Date.Month != D.Date.Month || LastO.Date.Year != D.Date.Year)
                    {
                        try
                        {
                            NewO = new Osmotr();
                            NewO.AdresId = id;

                            NewO.Date = date;
                            NewO.DateEnd = date;
                            NewO.DateOEGF = date;
                            NewO.DatePTO = date;
                            NewO.Opisanie = "Повторный осмотр";
                            db.Osmotrs.Add(NewO);
                            db.SaveChanges();

                            NewO.AOW = new List<ActiveOsmotrWork>();
                            NewO.ORW = new List<OsmotrRecommendWork>();
                            NewO.Elements = new List<ActiveElement>();
                            NewO.Defects = new List<ActiveDefect>();

                            NewO.DOMParts = Parts;

                            //добавляем куки с осмотром
                            HttpCookie cookie = new HttpCookie("Osmotr");
                            cookie["Date"] = LastO.Date.ToString();
                            cookie["OsmotrId"] = LastO.Id.ToString();
                            cookie["AdresId"] = id.ToString();
                            // Добавить куки в ответ
                            Response.Cookies.Add(cookie);
                        }
                        catch (Exception ec) { }
                        //теперь у нас есть ID нового осмотра

                        // работаем со старым осмотром
                        if (LastO != null)
                        {
                            try
                            {
                                //пробуем найти активные работы старого осмотра и которые не выполнены
                                List<ActiveOsmotrWork> LastAOW = db.ActiveOsmotrWorks.Where(x => x.OsmotrId == LastO.Id && !x.Gotovo).ToList();

                                //Сохраняем их под новыми ID и делаем ссылку на новый осмотр
                                foreach (ActiveOsmotrWork AOW in LastAOW)
                                {
                                    AOW.OsmotrId = NewO.Id;
                                    db.ActiveOsmotrWorks.Add(AOW);
                                    db.SaveChanges();
                                    NewO.AOW.Add(AOW);
                                }

                            }
                            catch { }


                            try
                            {
                                // пробуем найти рекомендуемые работы предыдущего осмотра которые еще не выполнены
                                List<OsmotrRecommendWork> LastORW = db.OsmotrRecommendWorks.Where(x => x.OsmotrId == LastO.Id && !x.Gotovo).ToList();

                                //Сохраняем их под новыми ID и делаем ссылку на новый осмотр
                                foreach (OsmotrRecommendWork ORW in LastORW)
                                {
                                    ORW.OsmotrId = NewO.Id;
                                    db.OsmotrRecommendWorks.Add(ORW);
                                    db.SaveChanges();

                                }

                            }
                            catch { }

                            //Создаем элементы 
                            CreateElements(Elements, NewO, LastO);


                            //если нашелся осмотр то отлично
                            /*  try { LastO.DOMCW = db.DOMCWs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMCW = new DOMCW(); LastO.DOMCW.AdresId = id; LastO.DOMCW.Date = DateTime.Now; }
                              try { LastO.DOMHW = db.DOMHWs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMHW = new DOMHW(); LastO.DOMHW.AdresId = id; LastO.DOMHW.Date = DateTime.Now; }
                              try { LastO.DOMElectro = db.DOMElectroes.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMElectro = new DOMElectro(); LastO.DOMElectro.AdresId = id; LastO.DOMElectro.Date = DateTime.Now; }
                              try { LastO.DOMFasad = db.DOMFasads.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMFasad = new DOMFasad(); LastO.DOMFasad.AdresId = id; LastO.DOMFasad.Date = DateTime.Now;LastO.DOMFasad.Sostoyanie = 1; }
                              try { LastO.DOMFundament = db.DOMFundaments.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).Include(x => x.Material).Include(x => x.Type).First(); } catch { LastO.DOMFundament = new DOMFundament(); LastO.DOMFundament.Date = DateTime.Now; LastO.DOMFundament.Sostoyanie = 1; LastO.DOMFundament.MaterialId = 1;LastO.DOMFundament.AdresId = id; }
                              try { LastO.DOMOtoplenie = db.DOMOtoplenies.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMOtoplenie = new DOMOtoplenie();  LastO.DOMOtoplenie.AdresId = id; LastO.DOMOtoplenie.Date = DateTime.Now; }
                              try { LastO.DOMRoof = db.DOMRoofs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMRoof = new DOMRoof(); LastO.DOMRoof.AdresId = id; LastO.DOMRoof.Date = DateTime.Now; }
                              try { LastO.DOMRoom = db.DOMRooms.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMRoom = new DOMRoom(); LastO.DOMRoom.AdresId = id; LastO.DOMRoom.Date = DateTime.Now; }
                              try { LastO.DOMVodootvod = db.DOMVodootvods.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First(); } catch { LastO.DOMVodootvod = new DOMVodootvod(); LastO.DOMVodootvod.AdresId = id; LastO.DOMVodootvod.Date = DateTime.Now; }
                            */

                        }

                        Result = NewO;//отправляем осмотр в результат
                    }

                }
                catch (Exception e3)
                {//если нет осмотра то возвращаем пустой осмотр с созданными элементами
                    CreateElements(Elements, NewO, LastO);
                    Result = NewO;
                }
                //Нужно обновить сессию осмотров
                //  DateTime FromDate = DateTime.Now.AddYears(-1);
                //  DateTime ToDate = DateTime.Now;
                //DateTime FromDate = Convert.ToDateTime("");
                Session["Houses" + NewO.Adres.Adress] = null;
            }
            ViewBag.Month = Opredelenie.Opr.MonthToNorm(Opredelenie.Opr.MonthOpred(Result.Date.Month));
            ViewBag.Materials = new SelectList(db.Materials, "Id", "Material");
            ViewBag.Izmerenies = new SelectList(db.Izmerenies, "Id", "Name");
            ViewBag.Error = error;
            return View(Result);
        }


        public void CreateElements (List<Element> Elements, Osmotr NewO, Osmotr LastO)
        {
            if (Elements.Count==0)
            {
                Elements = db.Elements.ToList();
            }
           
            foreach (Element E in Elements)
            {
                //Теперь копируем информацию поэлементно в новый осмотр

                ActiveElement AE = new ActiveElement();

                try
                {
                    //Загружаем активные элементы прошлого осмотра
                    AE = db.ActiveElements.Where(x => x.ElementId == E.Id && x.AdresId == NewO.AdresId&& x.OsmotrId == LastO.Id).OrderByDescending(x => x.Date).First();
                    //Если элемент есть то сохраняем с новой датой и новым ID осмотра
                    AE.Date = NewO.Date;
                    AE.OsmotrId = NewO.Id;
                    AE.IsOld1 = true;
                    AE.IsOld2 = true;
                    db.ActiveElements.Add(AE);
                    db.SaveChanges();
                    NewO.Elements.Add(AE);
                    string LastPath = "/Files/" + LastO.Id + "/";
                    string NewPath = "/Files/" + NewO.Id + "/";
                    //Копируем фотографии
                    try
                    {
                        System.IO.File.Copy(HostingEnvironment.MapPath(LastPath + AE.Photo1), HostingEnvironment.MapPath(NewPath + AE.Photo1),true);
                        System.IO.File.Copy(HostingEnvironment.MapPath(LastPath + AE.Photo2), HostingEnvironment.MapPath(NewPath + AE.Photo2),true);
                    }
                    catch (Exception e)
                    {

                    }


                    try
                    {
                        List<ActiveDefect> ADN = new List<ActiveDefect>();
                        ADN = db.ActiveDefects.Where(x => x.ElementId == E.Id && x.AdresId == NewO.AdresId && x.Date == LastO.Date).OrderByDescending(x => x.Date).Include(x => x.Defect).ToList();
                        foreach (ActiveDefect ad in ADN)
                        {
                            //Сохраняем каждый дефект из предыдущего осмотра в новый

                            ad.Date = NewO.Date;
                            ad.OsmotrId = NewO.Id;
                            db.ActiveDefects.Add(ad);
                            db.SaveChanges();
                            NewO.Defects.Add(ad);
                            AE.Defects = db.Defects.Where(x => x.ElementId == E.Id).ToList();
                            AE.Defects = AE.Defects.OrderBy(x => x.Def).ToList();
                        }
                        AE.ActiveDefects = ADN;

                    }
                    catch (Exception e)
                    {
                        AE.ActiveDefects = new List<ActiveDefect>();
                    }




                }
                catch
                {
                    //Создаем новые активные элементы если в предыдущем осмотре их нет (на случай ошибки или если добавятся новые элементы дома)
                    AE.ElementId = E.Id;
                    AE.Element = db.Elements.Where(x => x.Id == E.Id).First();
                    AE.OsmotrId = NewO.Id;
                    AE.AdresId = (int) NewO.AdresId;
                    AE.Date = NewO.Date;
                    AE.Sostoyanie = 5;
                    AE.DateIzmeneniya = NewO.Date;
                    AE.UserName = User.Identity.Name;
                    AE.Kolichestvo = 0;
                    AE.IzmerenieId = 1;
                    AE.MaterialId = 1;
                    AE.Izmerenie = db.Izmerenies.Where(x => x.Id == AE.IzmerenieId).First();
                    AE.Material = db.Materials.Where(x => x.Id == AE.MaterialId).First();
                    AE.Est = true;
                    try
                    {
                        db.ActiveElements.Add(AE);
                        db.SaveChanges();
                        NewO.Elements.Add(AE);
                    }
                    catch { }
                    try
                    {
                        AE.Defects = db.Defects.Where(x => x.ElementId == E.Id).ToList();
                        AE.Defects = AE.Defects.OrderBy(x => x.Def).ToList();
                     
                    }
                    catch (Exception e)
                    {
                        AE.Defects = new List<Defect>();
                    }

                    AE.ActiveDefects = new List<ActiveDefect>();

                }

            }
        }
        public List<Element> GetElements()
        {
            List<Element> Elements = new List<Element>();
            if (Session["Elements"] == null)
            {
                //Получаем все элемнты
                Elements = db.Elements.ToList();

                //Сохраняем в сессию чтобы все было свеженькое
                Session["Elements"] = Elements;
            }
            else
            {//Загружаем из сессии
                Elements = (List<Element>)Session["Elements"];
            }
            return Elements;


        }

        public List<FundamentMaterial> GetFundaments()
        {
            List<FundamentMaterial> FM = new List<FundamentMaterial>();
            if (Session["FundamentMaterials"] == null)
            {
                //Получаем все элемнты
                FM = db.FundamentMaterials.ToList();

                //Сохраняем в сессию чтобы все было свеженькое
                Session["FundamentMaterials"] = FM;
            }
            else
            {//Загружаем из сессии
                FM = (List<FundamentMaterial>)Session["FundamentMaterials"];

            }
            return FM;
        }
        public List<FundamentType> GetFundamentTypes()
        {
            List<FundamentType> FT = new List<FundamentType>();
            if (Session["FundamentTypes"] == null)
            {
                //Получаем все элемнты
                FT = db.FundamentTypes.ToList();

                //Сохраняем в сессию чтобы все было свеженькое
                Session["FundamentTypes"] = FT;
            }
            else
            {//Загружаем из сессии
                FT = (List<FundamentType>)Session["FundamentTypes"];

            }
            return FT;
        }
        public List<DOMPart> GetDOMParts()
        {
            List<DOMPart> DOMParts = new List<DOMPart>();
            if (Session["DOMParts"] == null)
            {
                //Получаем все элемнты
                DOMParts = db.DOMParts.ToList();

                //Сохраняем в сессию чтобы все было свеженькое
                Session["DOMParts"] = DOMParts;
            }
            else
            {//Загружаем из сессии
                DOMParts = (List<DOMPart>)Session["DOMParts"];

            }
            return DOMParts;
        }
        public List<OsmotrWork> GetOsmotrWorks()
        {
            List<OsmotrWork> OW = new List<OsmotrWork>();
            if (Session["OsmotrWorks"] == null)
            {
                //Получаем все элемнты
                OW = db.OsmotrWorks.ToList();

                //Сохраняем в сессию чтобы все было свеженькое
                Session["OsmotrWorks"] = OW;
            }
            else
            {//Загружаем из сессии
                OW = (List<OsmotrWork>)Session["OsmotrWorks"];

            }
            return OW;
        }

        public List<Izmerenie> GetIzmerenies()
        {
            List<Izmerenie> Izm = new List<Izmerenie>();
            if (Session["Izmerenies"] == null)
            {
                //Получаем все элемнты
                Izm = db.Izmerenies.ToList();

                //Сохраняем в сессию чтобы все было свеженькое
                Session["Izmerenies"] = Izm;
            }
            else
            {//Загружаем из сессии
                Izm = (List<Izmerenie>)Session["Izmerenies"];
            }

            return Izm;
        }
        public List<Adres> GetAdresa()
        {
            List<Adres> Adresa = new List<Adres>();
            if (Session["Adresa"] == null)
            {
                //Получаем все элемнты
                Adresa = db.Adres.ToList();

                //Сохраняем в сессию чтобы все было свеженькое
                Session["Adresa"] = Adresa;
            }
            else
            {//Загружаем из сессии
                Adresa = (List<Adres>)Session["Adresa"];
            }
            return Adresa;
        }

        public List<ActiveElement>GetActiveElements(int OsmotrId)
        {
            List<ActiveElement> dbAE = new List<ActiveElement>();
            if (Session["ActiveElements" + OsmotrId] == null)
            {
                try
                {
                    dbAE = db.ActiveElements.Where(x => x.OsmotrId == OsmotrId).OrderBy(x=>x.ElementId).ThenBy(x=>x.Id).ToList();
                    Session["ActiveElements" + OsmotrId] = dbAE;
                }
                catch
                {

                }
            }
            else
            {//Загружаем из сессии
                dbAE = (List<ActiveElement>)Session["ActiveElements" + OsmotrId];
            }
            return dbAE;
        }

     public List<ActiveDefect> GetActiveDefects(int OsmotrId)
        {
            List<ActiveDefect> dbAD = new List<ActiveDefect>();
            if (Session["ActiveDefects" + OsmotrId] == null)
            {
                try
                {
                    dbAD = db.ActiveDefects.Where(x => x.OsmotrId == OsmotrId).OrderByDescending(x => x.Date).Include(x => x.Defect).ToList();
                    Session["ActiveDefects" + OsmotrId] = dbAD;
                }
                catch
                {

                }
            }
            else
            {//Загружаем из сессии
                dbAD = (List<ActiveDefect>)Session["ActiveDefects" + OsmotrId];
            }
            return dbAD;
        }

        public List<Osmotr> GetOsmotrs(bool Obnovit)
        {
            List<Osmotr> Osmotrs = new List<Osmotr>();
            if (Obnovit == false && Session["Osmotrs"] != null)
            {
                try
                {
                    Osmotrs = (List<Osmotr>)Session["Osmotrs"];
                }
                catch
                {

                }
            }

            if (Osmotrs.Count == 0)
            {
                try
                {
                    Osmotrs = db.Osmotrs.Include(x=>x.Adres).Include(x => x.ORW).Include(x => x.AOW).ToList();
                    Session["Osmotrs"] = Osmotrs;
                }
                catch (Exception e)
                {

                }
            }
            return Osmotrs;
        }

        public ActionResult Info(DateTime date, int id = 0)
        {
            bool LoadOsmotr = true;
            string error = "";
            List<Element> Elements = new List<Element>();
            Elements = GetElements();

            List<FundamentMaterial> FM = new List<FundamentMaterial>();
            FM = GetFundaments();
            ViewBag.FundamentMaterials = new SelectList(FM, "Id", "Material");

            List<FundamentType> FT = new List<FundamentType>();
            FT = GetFundamentTypes();
            ViewBag.FundamentTypes = new SelectList(FT, "Id", "Type");

            List<DOMPart> DOMParts = new List<DOMPart>();
            DOMParts = GetDOMParts();
            var ALLParts = new SelectList(DOMParts, "Id", "Name");
            ViewBag.ALLParts = ALLParts;

            List<string> Parts = new List<string>();
            if (Session["Parts"] == null)
            {
                //Получаем все элемнты
                Parts = DOMParts.OrderBy(y => y.Id).Select(x => x.Name).ToList();

                //Сохраняем в сессию чтобы все было свеженькое
                Session["Parts"] = Parts;
            }
            else
            {//Загружаем из сессии
                Parts = (List<string>)Session["Parts"];
            }
            ViewBag.DOMParts = Parts;

            List<OsmotrWork> OW = new List<OsmotrWork>();
            OW = GetOsmotrWorks();
            var ow = new SelectList(OW, "Id", "Name");
            ViewBag.OW = ow;

            List<Izmerenie> Izm = new List<Izmerenie>();
            Izm = GetIzmerenies();
            ViewBag.Izmerenies = new SelectList(Izm, "Id", "Name");
       
            if (date == null)
            {
                  date = DateTime.Now;
            }


            List<Adres> Adresa = new List<Adres>();
            Adresa = GetAdresa();
            Osmotr Result = new Osmotr();
            //ищем по базе осмотры, если есть за текущий месяц на данном доме то продолжаем заполнять его.

          
            if (id != 0)
            {
                Result.AdresId = id;

                //  Result.Date = date;
                try
                {//пробуем грузануть данные по дому

                    Osmotr O = new Osmotr();
                    //Не грузим данные осмотра из сессии а каждый раз ищем в БД
                 //   if (Session["Osmotr"+id+"Y"+ date.Year+"M"+ date.Month] == null)
                 //  {

                        //Получаем все элемнты
                        try
                        {
                        //Include(x => x.DOMCW).Include(x => x.DOMHW).Include(x => x.DOMElectro).Include(x => x.DOMFasad).Include(x => x.DOMFundament).Include(x => x.DOMOtoplenie).Include(x => x.DOMRoof).Include(x => x.DOMRoom).Include(x => x.DOMVodootvod)
                        O = GetOsmotrs(false).Where(x => x.AdresId == id && x.Date.Year == date.Year && x.Date.Month == date.Month).OrderByDescending(x => x.Date).First();
                       // O.Adres = Adresa.Where(x => x.Id == id).First();
                        }
                        catch
                        {

                        }

                        //Сохраняем в сессию чтобы все было свеженькое
                        Session["Osmotr" + id + "Y" + date.Year + "M" + date.Month] = O;
                 //   }
                  //  else
                  //  {
                     //   O = (Osmotr)Session["Osmotr" + id + "Y" + date.Year + "M" + date.Month];
                  //  }

                    Result = O;
                    Result.Adres = O.Adres;



                    //добавляем куки с осмотром
                    HttpCookie cookie = new HttpCookie("Osmotr");
                    cookie["Date"] = date.ToString();
                    cookie["OsmotrId"] = Result.Id.ToString();
                    cookie["AdresId"] = Result.AdresId.ToString();

                    HttpCookie cookieR = Request.Cookies["Osmotr"];
                    if (cookieR != null)
                    {
                        Response.Cookies.Set(cookie);
                       
                    }
                    else
                    {
                        // Добавить куки в ответ
                        Response.Cookies.Add(cookie);
                    }
                    

                    //Result.DOMCW = db.DOMCWs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                    // Result.DOMHW = db.DOMHWs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                    // Result.DOMElectro = db.DOMElectroes.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                    // Result.DOMFasad = db.DOMFasads.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                    // Result.DOMFundament = db.DOMFundaments.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).Include(x => x.Material).Include(x => x.Type).First();

                    // Result.DOMOtoplenie = db.DOMOtoplenies.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                    // Result.DOMRoof = db.DOMRoofs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                    //Result.DOMRoom = db.DOMRooms.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                    // Result.DOMVodootvod = db.DOMVodootvods.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
                }
                catch
                {//если данных нет, значит проблема с загрузкой данных с ГИСЖКХ. Проверьте данные. 
                   
                    error += " На данном доме небыло осмотров со времён царя:) Нужно срочно проверить существование дома или создать новый осмотр. ИД дома =" + id.ToString() ;
                    return RedirectToAction("Error", error);
                }
                try
                {
                    Result.ORW = db.OsmotrRecommendWorks.Include(x=>x.DOMPart).Include(x=>x.Izmerenie).Where(x => x.OsmotrId == Result.Id).ToList();
                }
                catch
                {

                }
               

                // Result.Sostoyanie = 0;
                Result.Elements = new List<ActiveElement>();
                

             

                try
                {//поскольку дефекты фиксируются осмотрами то у всех должна быть одна дата даже на разные элементы
                    DateTime D = date;
                    // try
                    // {
                    //     db.ActiveDefects.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).Select(x => x.Date).First();
                    // }
                    //catch { }

                    List<ActiveElement> dbAE = new List<ActiveElement>();
                    dbAE =GetActiveElements(Result.Id);


                    List<ActiveDefect> dbAD = new List<ActiveDefect>();
                    dbAD = GetActiveDefects(Result.Id);

                    foreach (Element E in Elements)
                    {
                        //ищем самый новый по дате и если такого нет то создаем пустой

                        ActiveElement AE = new ActiveElement();

                        try
                        {
                            AE = new ActiveElement();
                            if (Session["ActiveElement"+Result.Id+"E"+E.Id] == null)
                            {
                                //Получаем все элемнты
                                AE = dbAE.Where(x => x.ElementId == E.Id).First();
                                try
                                {
                                    AE.ActiveDefects =dbAD.Where(x => x.ElementId == E.Id).ToList();
                                    AE.Element = E;
                                }
                                catch
                                {
                                    AE.ActiveDefects = new List<ActiveDefect>();
                                }
                                //Сохраняем в сессию чтобы все было свеженькое
                                Session["ActiveElement" + Result.Id + "E" + E.Id] = AE;
                            }
                            else
                            {//Загружаем из сессии
                               AE = (ActiveElement)Session["ActiveElement" + Result.Id + "E" + E.Id];
                            }

                            //AE = db.ActiveElements.Where(x => x.OsmotrId == Result.Id && x.ElementId == E.Id ).OrderByDescending(x => x.Date).First();
                           
                            }
                        catch (Exception e)
                        {
                            AE.ElementId = E.Id;
                            AE.Element = db.Elements.Where(x => x.Id == E.Id).First();
                            AE.OsmotrId = Result.Id;
                            AE.AdresId = id;
                            AE.Date = date;
                            AE.Sostoyanie = 10;

                        }
                     
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
                error += " Не определен ИД дома!!! Не можем создать осмотр. ИД дома =" + id.ToString() + " Дата=" + date.ToString();
                return RedirectToAction("Error", error);
            }
            List<ActiveOsmotrWork> AOW = new List<ActiveOsmotrWork>();
            try
            {
                AOW = db.ActiveOsmotrWorks.Where(x => x.OsmotrId == Result.Id).Include(x=>x.OsmotrWork.DOMPart).Include(x=>x.OsmotrWork.Izmerenie).ToList();
            }
            catch (Exception e) { }
            Result.AOW = AOW;
            ViewBag.Month = Opredelenie.Opr.MonthToNorm(Opredelenie.Opr.MonthOpred(Result.Date.Month));
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
      
        public JsonResult SavePrimechanie(int id=0, string text="")
        {
            string Data = "Ошибка";
            if (id!=0&&text.Equals("")==false)
            {
                Osmotr O = null;
                try
                {
                    O = db.Osmotrs.Where(x => x.Id == id).First();
                    O.Opisanie = text;
                    db.Entry(O).State = EntityState.Modified;
                    db.SaveChanges();
                    Data = "Ok";
                }
                catch
                {

                }
            }
            return Json(Data);

        }

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
