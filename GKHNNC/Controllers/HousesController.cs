using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GKHNNC.DAL;
using GKHNNC.Models;
using Opredelenie;

namespace GKHNNC.Controllers
{
    public class HousesController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Houses
        public ActionResult Index()
        {
            List<House> H = new List<House>();
            List<House> Y = new List<House>();

            List<Adres> houses = db.Adres.OrderBy(x=>x.Adress).ToList();
            DateTime Date = Opr.MonthMinus(1, DateTime.Now);//берем прошлый месяц
            
            List<string> Primechanie = new List<string>();
           // List<Arendator> Arendators = db.Arendators.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month).ToList();//Берем всех арендаторов за текущий месяц
            //List<UEV> Uevs = db.UEVs.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month).ToList();
            //List<OPU> Opus = db.OPUs.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month).ToList();
            int progress = 0;
            double pro100 = 0;
            int procount = 0;
            pro100 = houses.Count;
            foreach (Adres a in houses)
            {
                
                House ho = new House();
              // List<Arendator> TekArend = Arendators.Where(d => d.AdresId == a.Id).ToList();//арендаторы в данном доме для ускорения поиска
               // List<UEV> TekUevs = Uevs.Where(d => d.AdresId == a.Id).ToList();//выставлено в УЭВ применим позже
               // List<OPU> TekOpus = Opus.Where(d => d.AdresId == a.Id).ToList();//Фактические затраты воды по ОПУ андрей Исх
                ho.AdresId = a.Id;
                ho.Adres = a.Adress;
               // ho.Ploshad = a.Ploshad;//общая площадь
               // ho.Teplota = TekOpus.Sum(e => e.OtopGkal);//TekUevs.Sum(e => e.OtEnergyGkal);//Сумма теплоты 
               // ho.Teplota12 = 0;
               // ho.HotWater = TekOpus.Sum(e => e.GWM3);//Сумма Горводы
               // ho.ColdWater = TekOpus.Sum(e => e.HWM3);//Сумма Холводы
               // ho.PloshadArendators = TekArend.Sum(e => e.Ploshad);//Сумма площадей арендаторов
               // ho.TeplotaArendators = TekArend.Sum(e => e.Teplota);//Сумма теплоты арендаторов
               // ho.Teplota12Arendators = TekArend.Sum(e => e.Teplota12);//Сумма теплоты 1/12 арендаторов
                //ho.ColdWaterArendators = TekArend.Sum(e => e.ColdWater);//Сумма Холодной воды арендаторов
               // ho.HotWaterArendators = TekArend.Sum(e => e.HotWater);//Сумма Горячей воды арендаторов
                ho.Date = Date;
                try
                {
                    DateTime Dat = DateTime.Now;
                    int O = db.Osmotrs.Where(x => x.Date.Year == Dat.Year && x.Date.Month == Dat.Month && x.AdresId == a.Id).OrderByDescending(x => x.Date).Select(x=>x.Id).First();
                    ho.OsmotrEst = true;
                }
                catch { ho.OsmotrEst = false; }
                try
                {
                    int D = db.DOMCWs.Where(x => x.AdresId == a.Id).OrderByDescending(x => x.Date).Select(x=>x.Id).First();
                    ho.GISGKH = true;
                }catch
                {
                    ho.GISGKH = false;
                }
                if (ho.GISGKH==true)
                {
                    H.Add(ho);
                }
                else
                {
                    Y.Add(ho);
                }
                procount++;
                progress = Convert.ToInt16( procount / pro100 * 100);
                ProgressHub.SendMessage("Загружаем данные домов, подождите немножко...", progress);
                if (procount > pro100) { procount = Convert.ToInt32(pro100); }
            }
            H.AddRange(Y);
            return View(H);
        }

        // GET: Houses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adres adr = db.Adres.Find(id);
            if (adr == null)
            {
                return HttpNotFound();
            }
            Adres Adr = db.Adres.Where(c => c.Id == id).Single();
            List<DateTime> SelectDate = new List<DateTime>();//массив для выбора минимальной из максимальных дат по которой все и будем считать
            try { SelectDate.Add(db.CompleteWorks.Where(d => d.WorkDate == db.CompleteWorks.Max(x => x.WorkDate)).Select(c => c.WorkDate).First()); } catch { }//Выбираем макс дату из комплит воркс
            try { SelectDate.Add(db.Arendators.Where(c => c.Date == db.Arendators.Max(x => x.Date) && c.AdresId == id).Select(d => d.Date).First()); }catch{ }
            try { SelectDate.Add(db.UEVs.Where(c => c.AdresId == id && c.Date == db.UEVs.Max(x => x.Date)).Select(d => d.Date).First()); } catch { }
            try { SelectDate.Add(db.OPUs.Where(c => c.Date == db.OPUs.Max(x => x.Date) && c.AdresId == id).Select(d => d.Date).First()); } catch { }
            try { SelectDate.Add(db.VipolnennieUslugis.Where(c => c.Date == db.VipolnennieUslugis.Max(x => x.Date) && c.AdresId == id).Select(d => d.Date).First()); } catch { }
            
            DateTime MaxDate = SelectDate.Where(c => c.Date == SelectDate.Min(x => x.Date)).First();

            DateTime Date = new DateTime(MaxDate.Year,MaxDate.Month,1);//берем минимально максимальную дату //DateTime.Now.Year, DateTime.Now.Month - 1, 1);//берем прошлый месяц
            List<Arendator> Arendators = db.Arendators.Where(c => c.Date.Year == Date.Year&&c.Date.Month == Date.Month&& c.AdresId==id).ToList();//Берем выбранный дом и ищем в нем арендаторов
            List<UEV> Uevs = db.UEVs.Where(c => c.AdresId == id&& c.Date.Year == Date.Year && c.Date.Month == Date.Month).ToList();
            List<OPU> Opus = db.OPUs.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month && c.AdresId == id).ToList();
            DOMOtoplenie Otoplenie = null;
            try
            {
                Otoplenie = db.DOMOtoplenies.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(x=>x.MaterialOtop1).Include(x=>x.MaterialOtop2).Include(x=>x.MaterialOtopTrub).Include(x=>x.MaterialTeplo).First();
            }
            catch (Exception e) { }
            DOMCW ColdW = null;
            try
            {
                ColdW = db.DOMCWs.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(x => x.MaterialCW).First();
            }
            catch (Exception e) { }
            DOMHW HotW = null;
            try
            {
                HotW = db.DOMHWs.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(x => x.MaterialHW).First();
            }
            catch (Exception e) { }
            DOMElectro Electro = null;
            try
            {
               Electro = db.DOMElectroes.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).First();
            }
            catch (Exception e) { }
            DOMVodootvod Vodootvod = null;
            try
            {
                Vodootvod = db.DOMVodootvods.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(x => x.Material).First();
            }
            catch (Exception e) { }
            DOMFundament Fundament = new DOMFundament();
            try
            {
                Fundament = db.DOMFundaments.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(c => c.Material).Include(c => c.Type).First();
            } catch
            {
              //  Fundament.Material.Material = "Не определен";
              // Fundament.Type.Type = "Не определен";
            }
            DOMRoof Roof = new DOMRoof();
            try
            {
                Roof = db.DOMRoofs.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(c => c.Form).Include(c => c.Vid).Include(c => c.Type).Include(c => c.Uteplenie).First();
            }
            catch
            {
               // Roof.Form.Form = "Не определена";
               // Roof.Uteplenie.Uteplenie = "Не определен";
               // Roof.Vid.Vid= "Не определен";
               // Roof.Type.Type = "Не определен";
            }
            List<string> Works = new List<string>();//ищем работы
            List<CompleteWork> CW = new List<CompleteWork>();
            try
            {
               CW= db.CompleteWorks.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month && c.WorkAdress.Replace(" ", "").Equals(Adr.Adress)).ToList();//.Select(d => d.WorkName + " " + d.WorkNumber.ToString())
            }
            catch { }
            foreach (CompleteWork CCW in CW)
            {
                Works.Add(CCW.WorkName + " " + CCW.WorkNumber.ToString() + " " + CCW.WorkIzmerenie);
            }
            List<string> Uslugis = new List<string>();//ищем услуги
            List<string> UslugisCost = new List<string>();//ищем услуги
            try
            {
                Uslugis = db.VipolnennieUslugis.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month && c.AdresId == id).Include(e => e.Usluga).Select(d => d.Usluga.Name).ToList();
                UslugisCost = db.VipolnennieUslugis.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month && c.AdresId == id).Include(e => e.Usluga).Select(d => d.StoimostNaMonth.ToString() + " руб.").ToList();
            }
            catch { }
           
                House ho = new House();
            ho.AdresId = id;
            ho.Adres = adr.Ulica +" "+ adr.Dom;
            ho.Ploshad = adr.Ploshad;//пока не знаем общую площадь
                ho.Teplota = Opus.Sum(e => e.OtopGkal);//Сумма теплоты
                ho.Teplota12 = adr.Teplota12;//пока не знаем общую площадь
                ho.HotWater = Opus.Sum(e => e.GWM3);//сумма ГВ
                ho.ColdWater = Opus.Sum(e => e.HWM3);//сумма ХВ
                ho.PloshadArendators = Arendators.Sum(e => e.Ploshad);//Сумма площадей арендаторов
                ho.TeplotaArendators = Arendators.Sum(e => e.Teplota);//Сумма теплоты арендаторов
                ho.Teplota12Arendators = Arendators.Sum(e => e.Teplota12);//Сумма теплоты 1/12 арендаторов
                ho.ColdWaterArendators = Arendators.Sum(e => e.ColdWater);//Сумма Холодной воды арендаторов
                ho.HotWaterArendators = Arendators.Sum(e => e.HotWater);//Сумма Горячей воды арендаторов
                ho.Date =  Date;//берем макс дату (Она единственная для всех)
            ho.Otoplenie = Otoplenie;
            ho.HotW = HotW;
            ho.ColdW = ColdW;
            ho.Electro = Electro;
            ho.Vodootvod = Vodootvod;
            //пишем все данные по конструктивным элементам

            if (Fundament.Type != null)
            {
                ho.FundamentPloshad = Fundament.Ploshad;
                ho.FundamentType = Fundament.Type.Type;
                ho.FundamentMaterial = Fundament.Material.Material;
                ho.FundamentDate = Fundament.Date;
                string ico = "";
                switch (Fundament.TypeId)
                {
                    case 1: ico = "1N"; break;
                    case 2: ico = "5"; break;
                    case 3: ico = "2"; break;
                    case 4: ico = "3"; break;
                    case 5: ico = "4"; break;
                    case 6: ico = "1"; break;
                }
                string ico2 = "";
                switch (Fundament.MaterialId)
                {
                    case 1: ico2 = ""; break;
                    case 2: ico2 = "B"; break;
                    case 3: ico2 = "BB"; break;
                    case 4: ico2 = ""; break;
                    case 5: ico2 = "K"; break;
                    case 6: ico2 = "D"; break;
                    case 7: ico2 = "SGB"; break;
                    case 8: ico2 = ""; break;
                    case 9: ico2 = "S"; break;
                  
                }


                ViewBag.FundamentIco = ico + ico2 + ".png";
                ViewBag.FundamentText = Fundament.Type.Type.Replace(" ", "_") + "_"+Fundament.Material.Material.Replace(" ","_");
            }
            if (Roof.Type != null)
            {
                ho.RoofType = Roof.Type.Type;
                ho.RoofVid = Roof.Vid.Vid;
                ho.RoofUteplenie = Roof.Uteplenie.Uteplenie;
                ho.RoofForm = Roof.Form.Form;
                ho.RoofDate = Roof.Date;
                ho.RoofYear = Roof.Year;
                ho.RoofYearKrovlya = Roof.YearKrovlya;
                string ico = "";
                switch (Roof.TypeId)
                {
                    case 1: ico = "R"; break;
                    case 2: ico = "G"; break;
                    case 3: ico = "V"; break;
                    case 4: ico = "M"; break;
                    case 5: ico = "M"; break;
                    case 6: ico = "M"; break;
                    case 7: ico = "S"; break;
                    case 8: ico = "R"; break;
                    case 9: ico = "H"; break;
                }
                string ico2 = "";
                switch (Roof.FormId)
                {
                    case 1: ico2 = "N"; break;
                    case 2: ico2 = "O"; break;
                    case 3: ico2 = "O"; break;
                    case 4: ico2 = "O"; break;
                    case 5: ico2 = "O"; break;
                    case 6: ico2 = "P"; break;

                }
                string ico3 = "";
                switch (Roof.VidId)
                {
                    case 1: ico3 = "N"; break;
                    case 2: ico3 = "S"; break;
                    case 3: ico3 = "B"; break;
                    case 4: ico3 = "B"; break;
                    case 5: ico3 = "B"; break;
                    case 6: ico3 = "B"; ico = "R"; break;
                    case 7: ico3 = "S"; break;

                }
                string ico4 = "";
                switch (Roof.UteplenieId)
                {
                    case 1: ico4 = "N"; break;
                    case 2: ico4 = "K"; break;
                    case 3: ico4 = "P"; break;
                    case 4: ico4 = "V"; break;
                    case 5: ico4 = "B"; break;

                }
                ViewBag.RoofIco = ico + ico2 +ico3+ico4+ ".png";
                ViewBag.RoofText = Roof.Type.Type.Replace(" ", "_") + "_" + Roof.Form.Form.Replace(" ", "_") + "_" + Roof.Vid.Vid.Replace(" ", "_") + "_" + Roof.Uteplenie.Uteplenie.Replace(" ", "_");

            }



            ViewBag.Date = Opr.MonthOpred(Date.Month) + " " + Date.Year.ToString();
            ViewBag.Arendators = Arendators.Select(x => x.Name).ToList();
            ViewBag.Works = Works;
            ViewBag.Uslugis = Uslugis;
            ViewBag.UslugisCost = UslugisCost;
            
            string prim = "";
            try
            {
               prim = Opus.Select(c => c.Primech).First();
            }
            catch { }
            ViewBag.Primechanie = prim;

            

            return View(ho);
        }

        public ActionResult Info(int? id,string DateZapros)
        {
            
           
            if (id == null)
            {
                // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                id = 13;
            }
            else
            {

            }
            Adres adr = db.Adres.Find(id);
            if (adr == null)
            {
                return HttpNotFound();
            }
            Adres Adr = db.Adres.Where(c => c.Id == id).Single();
            List<DateTime> SelectDate = new List<DateTime>();//массив для выбора минимальной из максимальных дат по которой все и будем считать
            try { SelectDate.Add(db.CompleteWorks.Where(d => d.WorkDate == db.CompleteWorks.Max(x => x.WorkDate)).Select(c => c.WorkDate).First()); } catch { }//Выбираем макс дату из комплит воркс
            try { SelectDate.Add(db.Arendators.Where(c => c.Date == db.Arendators.Max(x => x.Date) && c.AdresId == id).Select(d => d.Date).First()); } catch { }
            try { SelectDate.Add(db.UEVs.Where(c => c.AdresId == id && c.Date == db.UEVs.Max(x => x.Date)).Select(d => d.Date).First()); } catch { }
            try { SelectDate.Add(db.OPUs.Where(c => c.Date == db.OPUs.Max(x => x.Date) && c.AdresId == id).Select(d => d.Date).First()); } catch { }
            try { SelectDate.Add(db.VipolnennieUslugis.Where(c => c.Date == db.VipolnennieUslugis.Max(x => x.Date) && c.AdresId == id).Select(d => d.Date).First()); } catch { }

            DateTime MaxDate = SelectDate.Where(c => c.Date == SelectDate.Min(x => x.Date)).First();
            if (DateZapros!=null)//если запрошена дата то ищем все за эту дату
            {
                string[] s = DateZapros.Split('.');
                
                MaxDate = new DateTime(Convert.ToInt16(s[2]), Convert.ToInt16(s[1]),1);
            }
            DateTime Date = new DateTime(MaxDate.Year, MaxDate.Month, 1);//берем минимально максимальную дату //DateTime.Now.Year, DateTime.Now.Month - 1, 1);//берем прошлый месяц
            List<Arendator> Arendators = null;
            try { Arendators=db.Arendators.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month && c.AdresId == id).ToList(); } catch { }//Берем выбранный дом и ищем в нем арендаторов
            List<UEV> Uevs = null;
            try { Uevs = db.UEVs.Where(c => c.AdresId == id && c.Date.Year == Date.Year && c.Date.Month == Date.Month).ToList(); } catch { }
            List<OPU> Opus = null;
            try { Opus = db.OPUs.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month && c.AdresId == id).ToList(); } catch { }
            DOMFasad Fasad = null;
            try
            {
                Fasad = db.DOMFasads.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(x => x.Material).Include(x => x.Type).Include(x => x.Uteplenie).First();
            }
            catch (Exception e) { }
            DOMRoom Room = null;
            try
            {
                Room = db.DOMRooms.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(x => x.Overlap).Include(x => x.Type).Include(x => x.Window).First();
            }
            catch (Exception e) { }
            DOMFundament Fundament = null;
            try
            {
                Fundament = db.DOMFundaments.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(x => x.Material).Include(x => x.Type).First();
            }
            catch (Exception e) { }
            DOMRoof Roof = null;
            try
            {
                Roof = db.DOMRoofs.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(x => x.Uteplenie).Include(x => x.Type).Include(x => x.Vid).First();
            }
            catch (Exception e) { }

            DOMOtoplenie Otoplenie = null;
            try
            {
                Otoplenie = db.DOMOtoplenies.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(x => x.MaterialOtop1).Include(x => x.MaterialOtop2).Include(x => x.MaterialOtopTrub).Include(x => x.MaterialTeplo).First();
            }
            catch (Exception e) { }

            DOMCW ColdW = null;
            try
            {
                ColdW = db.DOMCWs.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(x => x.MaterialCW).First();
            }
            catch (Exception e) { }
            DOMHW HotW = null;
            try
            {
                HotW = db.DOMHWs.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(x => x.MaterialHW).First();
            }
            catch (Exception e) { }
            DOMElectro Electro = null;
            try
            {
                Electro = db.DOMElectroes.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).First();
            }
            catch (Exception e) { }
            DOMVodootvod Vodootvod = null;
            try
            {
                Vodootvod = db.DOMVodootvods.Where(c => c.AdresId == id).OrderByDescending(c => c.Date).Include(x => x.Material).First();
            }
            catch (Exception e) { }
           
            List<string> Works = new List<string>();//ищем работы
            List<CompleteWork> CW = new List<CompleteWork>();
            try
            {
            
                DateTime D = db.CompleteWorks.Where(d => d.WorkDate.Year == Date.Year&&d.WorkDate.Month==Date.Month).Select(c => c.WorkDate).First();
            
                //ищем максимальные данные по дате и их используем
                var WWW= db.CompleteWorks.Where(c => c.WorkDate.Year == D.Year && c.WorkDate.Month == D.Month && c.WorkAdress.Replace(" ", "").Equals(Adr.Adress)).Select(x=>new {WN = x.WorkName.ToString() ,WNum =  x.WorkNumber.ToString() ,WI = x.WorkIzmerenie.ToString() }).ToList();
                foreach (var V in WWW)
                {
                    Works.Add(V.WN+";"+V.WNum+";"+V.WI);
                }
                ViewBag.DateWorks = D;
                // if (D < Date)
                //   {
                //       var CCC = db.CompleteWorks.Where(x => x.WorkDate.Year == D.Year && x.WorkDate.Month == D.Month && x.WorkAdress.Replace(" ", "").Equals(Adr.Adress)).ToList();//.Select(d => d.WorkName + " " + d.WorkNumber.ToString())
                //       ViewBag.DateWorks = D;
                //   }
                //   else
                //  {
                //      var CCC = db.CompleteWorks.Where(x => x.WorkDate.Year == Date.Year && x.WorkDate.Month == Date.Month && x.WorkAdress.Replace(" ", "").Equals(Adr.Adress)).Include(x=>x.WorkWork).ToList();
                //      ViewBag.DateWorks = Date;
                //  }

            }
            catch (Exception e) {  }
            // foreach (CompleteWork CCW in CW)
            // {
            //     Works.Add(CCW.WorkName + ";" + CCW.WorkNumber.ToString() + ";" + CCW.WorkIzmerenie);
            // }
            List<VipolnennieUslugi> VipUs = new List<VipolnennieUslugi>();
            List<string> Uslugis = new List<string>();//ищем услуги
            List<string> UslugisCost = new List<string>();//ищем услуги
            try
            {
                Uslugis = db.VipolnennieUslugis.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month && c.AdresId == id).Include(e => e.Usluga).Select(d => d.Usluga.Name).ToList();
                UslugisCost = db.VipolnennieUslugis.Where(c => c.Date.Year == Date.Year && c.Date.Month == Date.Month && c.AdresId == id).Include(e => e.Usluga).Select(d => d.StoimostNaMonth.ToString() + " руб.").ToList();

            }
            catch { }

            House ho = new House();
            ho.AdresId = id;
            ho.Adres = adr.Ulica + " " + adr.Dom;
            ho.Ploshad = adr.Ploshad;//пока не знаем общую площадь
            ho.Teplota = Opus.Sum(e => e.OtopGkal);//Сумма теплоты
            ho.Teplota12 = adr.Teplota12;//пока не знаем общую площадь
            ho.HotWater = Opus.Sum(e => e.GWM3);//сумма ГВ
            ho.ColdWater = Opus.Sum(e => e.HWM3);//сумма ХВ
            ho.PloshadArendators = Arendators.Sum(e => e.Ploshad);//Сумма площадей арендаторов
            ho.TeplotaArendators = Arendators.Sum(e => e.Teplota);//Сумма теплоты арендаторов
            ho.Teplota12Arendators = Arendators.Sum(e => e.Teplota12);//Сумма теплоты 1/12 арендаторов
            ho.ColdWaterArendators = Arendators.Sum(e => e.ColdWater);//Сумма Холодной воды арендаторов
            ho.HotWaterArendators = Arendators.Sum(e => e.HotWater);//Сумма Горячей воды арендаторов
            ho.Date = Date;//берем макс дату (Она единственная для всех)
            ho.Otoplenie = Otoplenie;
            ho.HotW = HotW;
            ho.ColdW = ColdW;
            ho.Electro = Electro;
            ho.Vodootvod = Vodootvod;
            ho.Roof = Roof;
            ho.Fundament = Fundament;
            ho.Room = Room;
            ho.Fasad = Fasad;
            
            //пишем все данные по конструктивным элементам

            if (Fundament.Type != null)
            {
                ho.FundamentPloshad = Fundament.Ploshad;
                ho.FundamentType = Fundament.Type.Type;
                ho.FundamentMaterial = Fundament.Material.Material;
                ho.FundamentDate = Fundament.Date;
                string ico = "";
                switch (Fundament.TypeId)
                {
                    case 1: ico = "1N"; break;
                    case 2: ico = "5"; break;
                    case 3: ico = "2"; break;
                    case 4: ico = "3"; break;
                    case 5: ico = "4"; break;
                    case 6: ico = "1"; break;
                }
                string ico2 = "";
                switch (Fundament.MaterialId)
                {
                    case 1: ico2 = ""; break;
                    case 2: ico2 = "B"; break;
                    case 3: ico2 = "BB"; break;
                    case 4: ico2 = ""; break;
                    case 5: ico2 = "K"; break;
                    case 6: ico2 = "D"; break;
                    case 7: ico2 = "SGB"; break;
                    case 8: ico2 = ""; break;
                    case 9: ico2 = "S"; break;

                }


                ViewBag.FundamentIco = ico + ico2 + ".png";
                ViewBag.FundamentText = Fundament.Type.Type.Replace(" ", "_") + "_" + Fundament.Material.Material.Replace(" ", "_");
            }
         



            ViewBag.Date = Opr.MonthOpred(Date.Month) + " " + Date.Year.ToString();
            ViewBag.Arendators = Arendators.Select(x => x.Name).ToList();
            ViewBag.Works = Works;
            ViewBag.Uslugis = Uslugis;
            ViewBag.UslugisCost = UslugisCost;
            List<string> Years = Opr.YearZabit();
            Years.Insert(0, Date.Year.ToString());
            List<SelectListItem> Months = Opr.IMonthZabit();
            SelectListItem SLI = new SelectListItem();
            SLI.Value = Date.Month.ToString();
            SLI.Text = Opr.MonthOpred(Date.Month);
            Months.Insert(0, SLI);
            Years.Insert(0, Date.Year.ToString());
            ViewBag.Months = Months;
            ViewBag.Years = Years;
            ViewBag.MaxDate = MaxDate;
            string prim = "";
            try
            {
                prim = Opus.Select(c => c.Primech).First();
            }
            catch { }
            ViewBag.Primechanie = prim;



            return View(ho);
        }


        // POST: Houses/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdresId,Ploshad,PloshadArendators,Teplota,TeplotaArendators,Teplota12,Teplota12Arendators,HotWater,HotWaterArendators,ColdWater,ColdWaterArendators,Date")] House house)
        {
            if (ModelState.IsValid)
            {
                db.Entry(house).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", house.AdresId);
            return View(house);
        }

        // GET: Houses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = db.Houses.Find(id);
            if (house == null)
            {
                return HttpNotFound();
            }
            return View(house);
        }

        // POST: Houses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            House house = db.Houses.Find(id);
            db.Houses.Remove(house);
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
