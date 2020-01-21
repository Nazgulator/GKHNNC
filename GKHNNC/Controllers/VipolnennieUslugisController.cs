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
using System.Net;
using System.IO;
using System.Data;


using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using GKHNNC.Models;
using Microsoft.AspNet.Identity;
using System.Web.Helpers;
using Opredelenie;

namespace GKHNNC.Controllers
{
    public class VipolnennieUslugisController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: VipolnennieUslugis
        [Authorize]
        public ActionResult Index()
        {
            var vipolnennieUslugis = db.VipolnennieUslugis.Include(v => v.Adres).Include(v => v.Usluga);
            List<string> GGEU = new List<string>();
            foreach(GEU G in db.GEUs)
            {
                GGEU.Add(G.Name);
            }
            ViewBag.GEU = GGEU;
            //создаем список годов
            ViewBag.Year = new string[DateTime.Now.Year - 2018 + 1];
            int counter = 0;
            for (int i = DateTime.Now.Year; i >= 2018; i--)
            {
                ViewBag.Year[counter] = i.ToString();
                counter++;
            }
            //формируем список домов
            List<SelectListItem> Ad = new List<SelectListItem>();
            
            foreach (Adres A in db.Adres)
            {
                if (A.GEU!=null&&A.GEU.Equals(GGEU[0]))
                {
                    SelectListItem AA = new SelectListItem();
                    AA.Value = A.Adress;
                    AA.Text = A.Adress;
                    Ad.Add(AA);
                }
            }

            SelectList SL = new SelectList(Ad, "Value", "Text");
            ViewBag.Adres = SL;




            return View(vipolnennieUslugis.ToList());
        }
       // [HttpPost]
        public ActionResult IndexPoDomamPartial (string Year)
        {
            string GEU = "";
            string Month = "";
            if (Year == null)
            {
                Year = DateTime.Now.Year.ToString();
                Month = DateTime.Now.Month.ToString();
                GEU = "";
                if (HttpContext.Request.Cookies["Month"]!=null)
                {
                    Month = HttpContext.Request.Cookies["Month"].Value;
                }
            }
            else
            {
                string[] YM = Year.Split(';');
                Month = YM[1];
                GEU = YM[2];
                Year = YM[0];
                HttpContext.Response.Cookies["Month"].Value = Month;
                HttpContext.Response.Cookies["Month"].Expires = DateTime.Now.AddDays(1);
            }
            int M = 0;
            
            Obratno(Month, out M);
            int Y = Convert.ToInt16(Year);
            Month = M.ToString();
            List<VipolnennieUslugi> vipolnennieUslugis = new List<VipolnennieUslugi>();
            if (M < 13)
            {
               vipolnennieUslugis = db.VipolnennieUslugis.Where(x => x.Date.Year == Y && x.Date.Month == M).Include(v => v.Adres).Include(v => v.Usluga).ToList();
            }
            else
            {
                vipolnennieUslugis = db.VipolnennieUslugis.Where(x => x.Date.Year == Y).Include(v => v.Adres).Include(v => v.Usluga).ToList();
            }
            if (GEU != "") { vipolnennieUslugis = vipolnennieUslugis.Where(z => z.Adres.GEU.Equals(GEU)).ToList(); }
            List<VipolnennieUslugi> vu = new List<VipolnennieUslugi>();
            List<int> NU = new List<int>();
            List<string> NUString = new List<string>();
            string adr = "";
            int nu = 0;
            string nustring = "";

            List<List<string>> Mass = new List<List<string>>();
            List<Adres> MasAdr = vipolnennieUslugis.Select(r => r.Adres).Distinct().ToList();
            List<string> MasUslugiNames = vipolnennieUslugis.Select(r => r.Usluga.Name).Distinct().ToList();
            List<string> Summ = new List<string>();
            decimal SummaRub = 0;
            foreach(Adres A in MasAdr)
            {
                decimal summarub = 0;
                List<string> s = vipolnennieUslugis.Where(u => u.Adres.Adress == A.Adress).GroupBy(x=>x.Usluga.Name).Select(x=>x.Key+ "= "+x.Sum(y=>y.StoimostNaMonth)).ToList();
                // = vipolnennieUslugis.Where(u=>u.Adres.Adress==A.Adress).OrderBy(x=>x.Usluga.Name).Select(t => t.Usluga.Name+"= "+t.StoimostNaMonth.ToString()).ToList();
                summarub = vipolnennieUslugis.Where(u => u.Adres.Adress == A.Adress).Sum(x => x.StoimostNaMonth);
                Summ.Add(summarub.ToString());
                SummaRub += summarub;
                int col = vipolnennieUslugis.Where(u => u.Adres.Adress == A.Adress).Count();
                s.Insert(0, col.ToString());
                Mass.Add(s);
            }
            ViewBag.Mass = Mass;
            ViewBag.Summ = Summ;
            ViewBag.SummaRub = SummaRub;
            foreach (VipolnennieUslugi v in vipolnennieUslugis)
            {
                if (v.Adres.Adress.Equals(adr) == false)
                {
                    NU.Add(nu);
                    NUString.Add(nustring);
                    if (v.StoimostNaM2 + v.StoimostNaMonth != 0)
                    {
                        nustring = v.Usluga.Name + ";";
                        nu = 1;
                    }
                    else
                    {
                        nustring = "";
                        nu = 0;
                    }
                }
                bool go = false;
                
                VipolnennieUslugi VU = new VipolnennieUslugi();
                VU = v;

                foreach (VipolnennieUslugi vv in vu)
                {
                    if (vv.Adres.Equals(v.Adres) == true)//если такой адрес уже есть в выборке
                    {if (v.StoimostNaM2 + v.StoimostNaMonth != 0)
                        {
                            nustring += v.Usluga.Name + ";";
                            
                            nu++;
                        }
                        go = true;
                    }
                }

                if (!go)
                {
                    adr = VU.Adres.Adress;
                    vu.Add(VU);
                }
            }
            NU.Add(nu);
            NUString.Add(nustring);
            ViewBag.NUString = NUString;//названия услуг через ;
            ViewBag.NumUslug = NU;//количество услуг
            
            return View(vu.ToList());
        }

        public ActionResult IndexPoDomamPartial0()
        {
            List<string> GGEU = new List<string>();
            foreach (GEU G in db.GEUs)
            {
                GGEU.Add(G.Name);
            }
            ViewBag.GEU = GGEU;
            //создаем список годов
            ViewBag.Year = new string[DateTime.Now.Year - 2018 + 1];
            int counter = 0;
            for (int i = DateTime.Now.Year; i >= 2018; i--)
            {
                ViewBag.Year[counter] = i.ToString();
                counter++;
            }
            //формируем список домов в выборку ЖЭУ
            /*
            List<SelectListItem> Ad = new List<SelectListItem>();

            foreach (Adres A in db.Adres)
            {
                if (A.GEU != null && A.GEU.Equals(GGEU[0]))
                {
                    SelectListItem AA = new SelectListItem();
                    string ADR = A.Adress;
                    int ind = 0;
                    if (ADR.Contains("БОРОВАЯ ПАРТИЯ")|| ADR.Contains("МУСЫ ДЖАЛИЛЯ") || ADR.Contains("ГЕРОЕВ ТРУДА") || ADR.Contains("АКАДЕМИКА ТРОФИМУКА"))
                    {
                        ind = ADR.IndexOf(' ');
                        string ADR2 = ADR.Remove(0, ind);
                        ind = ADR2.IndexOf(' ');
                    }
                    else
                    {
                        ind = ADR.IndexOf(' ');
                        
                    }
                    string ADR3 = ADR.Remove(0, ind);
                    int ind2 = ADR3.IndexOf(' ');
                    ADR.Remove(ind2);
                    ADR =ADR.Remove(ind)+","+ADR.Remove(0,ind+1);
                    AA.Value = ADR;
                    AA.Text = ADR;
                    Ad.Add(AA);
                }
            }

            SelectList SL = new SelectList(Ad, "Value", "Text");
            
    */
            // ViewBag.Adres = SL;
            //Делаем список месяцев из них первый тот что в куки записан
            List<SelectListItem> Month = new List<SelectListItem>();
            for (int i = 1; i < 13; i++)
            {
                string mon = "";
                mon = Opr.MonthOpred(i);
                SelectListItem SLI = new SelectListItem();
                SLI.Text = mon;
                SLI.Value = mon;//i.ToString();
                Month.Add(SLI);

            }
            SelectListItem M = new SelectListItem();
            //если в куки что-то есть
            if (HttpContext.Request.Cookies["Month"] != null)
            {
                M.Text = HttpContext.Request.Cookies["Month"].Value;
                M.Value = HttpContext.Request.Cookies["Month"].Value;//Opr.MonthObratno(M.Text).ToString();
                //Month.RemoveAt(Opr.MonthObratno(M.Text) - 1);
                Month.Insert(0, M);
            }

            M = new SelectListItem();
            M.Text = "Год";
            M.Value = "Год";
            Month.Add(M);
            ViewBag.Month = Month;
            


            ViewBag.Adres = AdresaForGeu(GGEU[0]);
            return View();
        }
        public List<string> AdresaForGeu(string geu)
        {
            List<string> Ad = db.Adres.Where(x=>x.GEU.Equals(geu)).Select(y=>y.Adress).ToList();

           /* foreach (Adres A in db.Adres)
            {
                if (A.GEU != null && A.GEU.Equals(geu))
                {
                    string AA = "";
                    string ADR = A.Adress;
                    int ind = 0;

                  //убрали пустой хвост
                    ADR = ADR.Replace(" ", "-");
                  /*  if (ADR.Contains("БОРОВАЯ ПАРТИЯ") || ADR.Contains("МУСЫ ДЖАЛИЛЯ") || ADR.Contains("ГЕРОЕВ ТРУДА") || ADR.Contains("АКАДЕМИКА ТРОФИМУКА") || ADR.Contains("ЗЕЛЕНАЯ ГОРКА"))
                    {
                        ind = ADR.IndexOf(' ');
                        string ADR2 = ADR.Remove(0, ind);
                        ind = ADR2.IndexOf(' ');
                    }
                    else
                    {
                        ind = ADR.IndexOf(' ');

                    }
                    string ADR3 = ADR.Remove(0, ind);
                    int ind2 = ADR3.IndexOf(' ');
                    ADR.Remove(ind2);
                    ADR = ADR.Remove(ind) + "," + ADR.Remove(0, ind + 1);
                    AA = ADR;
                    
                    
                    Ad.Add(ADR);
                }
            }*/

            
           return (Ad);
        }

        [Authorize]
        public ActionResult IndexPoDomam()
        {
          
            return View();
        }

       

        public void Obratno(string month, out int mes)
        {
            mes = 1;
            switch (month)
            {
                case "Январь":
                    mes = 1;
                    break;
                case "Февраль":
                    mes = 2;
                    break;
                case "Март":
                    mes = 3;
                    break;
                case "Апрель":
                    mes = 4;
                    break;
                case "Май":
                    mes = 5;
                    break;
                case "Июнь":
                    mes = 6;
                    break;
                case "Июль":
                    mes = 7;
                    break;
                case "Август":
                    mes = 8;
                    break;
                case "Сентябрь":
                    mes = 9;
                    break;
                case "Октябрь":
                    mes = 10;
                    break;
                case "Ноябрь":
                    mes = 11;
                    break;
                case "Декабрь":
                    mes = 12;
                    break;
                case "Год":
                    mes = 13;
                    break;

            }
        }

        public ActionResult PoiskPoGeu(string selection)
        {
            //сделать поиск домов по жэу (p => p.Agent.Replace(" ", "") == xxx);

         //   var data2 = db.Adres.Where(x => x.GEU.Contains(selection));
            //Adres[] data3 = data2.ToArray();
         //   List<string> data = new List<string>();
         //   foreach (Adres A in data2)
         //   {
         //       data.Add(A.Adress);
         //   }
            //string[] data = data2. 
            //   string[] data = new string[] { "A", "B", "C" };
            return Json(AdresaForGeu(selection));
        }


        [Authorize]
        [HttpPost]
        public ActionResult OtchetMonth(FormCollection form)
        {
            
            string Month = form["Month"].ToString();
            string GEU = form["GEU"].ToString();
            string Year = form["Year"].ToString();
            string Adres = form["Adres"].ToString();
            string Prikaz = "";
            int Y = Convert.ToInt16(Year);//год в цифру
            
            //string geu = GEU.Replace(" ", "");
            string s = GEU.Replace(" ", "");
         
            int A = 0;
            try
            {
                A = db.Adres.Where(x => x.Adress.Replace(" ", "").Equals(Adres.Replace("-", ""))).First().Id;//ищем айдишник адреса в ДБ
            }
            catch
            {
                A = 0;
            }
                GEU GeuIsBasi = db.GEUs.Where(x => x.Name.Replace(" ", "").Equals(s)).First();
            if (Request.Form["b1"] != null)
            {
                int m = 0;
                Obratno(Month, out m);
                IEnumerable<VipolnennieUslugi> vu = db.VipolnennieUslugis;
                List<VipolnennieUslugi> ww = db.VipolnennieUslugis.Where(x => x.Date.Year == Y&&x.Date.Month == m&&x.AdresId==A).Include(y=>y.Usluga.Periodichnost).Include(f => f.Usluga).ToList();


                ViewBag.patch = GEU.Replace("-", "") ;
                if (ww.Count == 0) { ViewBag.Adres = Adres; ViewBag.GEU = GEU.Replace("-",""); ViewBag.Month = Month; ViewBag.Year = Year; return View("Error"); }
                string path = Server.MapPath("~/" + "АКТ_" + GEU + ".xlsx");//Server.MapPath("~/" + "АКТ_" + GEU + ".xlsx");"C:\\inetpub\\Otchets\\" + "АКТ_" + GEU + ".xlsx";
                ViewBag.path = path;
                decimal Summa =0;
                foreach (VipolnennieUslugi V in ww)
                {
                    Summa += V.StoimostNaMonth;
                }

                ExcelExportDomVipolnennieUslugi.EXPORT(ww, Month, GEU.Replace("-",""), Year, Adres,GeuIsBasi.Director,GeuIsBasi.Doverennost,path,Summa.ToString());
               
                
                return View();
            }
            return View();
        }
        [HttpPost]
        public ActionResult ZamenaGalki(string selection)
        {
            string[] S = selection.Split(';');
            string Adres = S[0];
            bool Check = false;
            if (S.Length > 3)
            {
                Check = Convert.ToBoolean(S[3]);
            }
            int Year =Convert.ToInt16(S[1]);
            int Month = Convert.ToInt16(S[2]);

            List<VipolnennieUslugi> VU = db.VipolnennieUslugis.Include(y => y.Adres).Where(x => x.Adres.Adress.Equals(Adres)).Where(z => z.Date.Year == Year && z.Date.Month == Month).ToList();
            foreach (VipolnennieUslugi V in VU)
            {
                V.ForPrint = Check;
               
                db.SaveChanges();
            }
            return Json(selection);
        }
        public ActionResult Download(string GEU,string path)
        {
            string file = path;
            string filename = "АКТ_" + GEU + ".xlsx";
            string contentType = "application/vnd.ms-excel";
            //патч,тип файла,новое имя файла
            return File(file, contentType, filename);//отправка файла пользователю (сохранение, скачать файл)
        }

        // GET: VipolnennieUslugis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VipolnennieUslugi vipolnennieUslugi = db.VipolnennieUslugis.Find(id);
            if (vipolnennieUslugi == null)
            {
                return HttpNotFound();
            }
            return View(vipolnennieUslugi);
        }

        // GET: VipolnennieUslugis/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress");
            ViewBag.UslugaId = new SelectList(db.Usluga, "Id", "Name");
            return View();
        }

        // POST: VipolnennieUslugis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,AdresId,UslugaId,StoimostNaM2,StoimostNaMonth")] VipolnennieUslugi vipolnennieUslugi)
        {
            if (ModelState.IsValid)
            {
                db.VipolnennieUslugis.Add(vipolnennieUslugi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", vipolnennieUslugi.AdresId);
            ViewBag.UslugaId = new SelectList(db.Usluga, "Id", "Name", vipolnennieUslugi.UslugaId);
            return View(vipolnennieUslugi);
        }

        // GET: VipolnennieUslugis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VipolnennieUslugi vipolnennieUslugi = db.VipolnennieUslugis.Find(id);
            if (vipolnennieUslugi == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", vipolnennieUslugi.AdresId);
            ViewBag.UslugaId = new SelectList(db.Usluga, "Id", "Name", vipolnennieUslugi.UslugaId);
            return View(vipolnennieUslugi);
        }

        // POST: VipolnennieUslugis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,AdresId,UslugaId,StoimostNaM2,StoimostNaMonth")] VipolnennieUslugi vipolnennieUslugi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vipolnennieUslugi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", vipolnennieUslugi.AdresId);
            ViewBag.UslugaId = new SelectList(db.Usluga, "Id", "Name", vipolnennieUslugi.UslugaId);
            return View(vipolnennieUslugi);
        }

        // GET: VipolnennieUslugis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VipolnennieUslugi vipolnennieUslugi = db.VipolnennieUslugis.Where(x => x.Id == id).Include(v => v.Adres).Include(f => f.Usluga).First();//db.VipolnennieUslugis.Find(id);
            if (vipolnennieUslugi == null)
            {
                return HttpNotFound();
            }
           // vipolnennieUslugi.
           // ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", vipolnennieUslugi.AdresId);
          //  ViewBag.UslugaId = new SelectList(db.Usluga, "Id", "Name", vipolnennieUslugi.UslugaId);
            return View(vipolnennieUslugi);
        }

        // POST: VipolnennieUslugis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VipolnennieUslugi vipolnennieUslugi = db.VipolnennieUslugis.Find(id);
            db.VipolnennieUslugis.Remove(vipolnennieUslugi);
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
