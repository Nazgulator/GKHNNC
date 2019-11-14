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
using Opredelenie;
using System.Collections;
using Microsoft.AspNet.SignalR;

namespace GKHNNC.Controllers
{
    public class DOMFundamentsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: DOMFundaments
        public ActionResult Index()
        {
            var dOMFundaments = db.DOMFundaments.Include(d => d.Adres).Include(d => d.Material).Include(d => d.Type);
            return View(dOMFundaments.ToList());
        }

        // GET: DOMFundaments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMFundament dOMFundament = db.DOMFundaments.Find(id);
            if (dOMFundament == null)
            {
                return HttpNotFound();
            }
            return View(dOMFundament);
        }

        // GET: DOMFundaments/Create
        public ActionResult Create()
        {
            ViewBag.AdresId = new SelectList(db.Adres.OrderBy(x=>x.Adress), "Id", "Adress");
            ViewBag.MaterialId = new SelectList(db.FundamentMaterials, "Id", "Material");
            ViewBag.TypeId = new SelectList(db.FundamentTypes, "Id", "Type");
            return View();
        }

        // POST: DOMFundaments/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Ploshad,MaterialId,TypeId,AdresId,Date")] DOMFundament dOMFundament)
        {
            if (ModelState.IsValid)
            {
                db.DOMFundaments.Add(dOMFundament);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMFundament.AdresId);
            ViewBag.MaterialId = new SelectList(db.FundamentMaterials, "Id", "Material", dOMFundament.MaterialId);
            ViewBag.TypeId = new SelectList(db.FundamentTypes, "Id", "Type", dOMFundament.TypeId);
            return View(dOMFundament);
        }

        // GET: DOMFundaments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMFundament dOMFundament = db.DOMFundaments.Find(id);
            if (dOMFundament == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMFundament.AdresId);
            ViewBag.MaterialId = new SelectList(db.FundamentMaterials, "Id", "Material", dOMFundament.MaterialId);
            ViewBag.TypeId = new SelectList(db.FundamentTypes, "Id", "Type", dOMFundament.TypeId);
            return View(dOMFundament);
        }

        [HttpGet]
        public ActionResult Upload()
        {

            return View();
        }
        public ActionResult NotUpload()
        {
            return View();
        }
        public void otklik (int max,ref int tek,string message)
        {
           tek++;
            int progress = Convert.ToInt16(tek / max * 100);
            if (tek > max) { tek = Convert.ToInt32(max); }
            ProgressHub.SendMessage(message, progress);
        }
        public string zachistkaAdresa (string s)
        {
            //ищем первые три запятые и вырезаем строку ищем улицу
            int zap = 0;
            for (int i = s.Length - 1; i > 0; i--)
            {
                if (s[i].Equals(','))
                {
                    zap++;
                    if (zap == 2)
                    {
                        s = s.Remove(0, i).Replace("пр-кт", "").Replace("Бульвар", "").Replace("проезд", "").Replace("ул", "").Replace("д.", "").Replace("б-р", "").Replace(",", "").Replace(" ", "").ToUpper();
                        break;
                    }
                }
            }
            return s;
        }
        public int poiskAdresa(string s)
        {
            int ind = 0;
            //ищем первые три запятые и вырезаем строку ищем улицу
          
            return ind;
        }

        public int PoiskMateriala (string s)
        {
            string SS = s.ToLower().Replace(" ", "").Replace("-","").Replace(".","");
            Material Mat = new Material();
            int id = 1;
        
            try
            {
                Mat = db.Materials.Where(x => x.Name.ToLower().Replace(" ", "").Equals(SS)).First();
                id = Mat.Id;
            } catch
            {

            }
           
            return id;
        }
      
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, DateTime Date)
        {
            int progress = 0;
            double pro100 = 0;
            int procount = 0;
            Date = new DateTime(Date.Year, Date.Month, 1);
            if (upload != null)
            {
                HttpCookie cookie = new HttpCookie("My localhost cookie");

                //найдем старые данные за этот месяц и заменим их не щадя
                List<DOMFundament> dbFundaments = db.DOMFundaments.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList();
               
                int tek = 0;
                foreach (DOMFundament S in dbFundaments)
                {
                    try
                    {
                        db.DOMFundaments.Remove(S);
                        db.SaveChanges();
                        otklik(dbFundaments.Count, ref tek, "удаляем старые данные фундамента...");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                List<DOMRoof> dbRoofs = db.DOMRoofs.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList();
                
                tek = 0;
                foreach (DOMRoof S in dbRoofs)
                {
                    try
                    {
                        db.DOMRoofs.Remove(S);
                        db.SaveChanges();
                        otklik(dbRoofs.Count, ref tek, "удаляем старые данные крыш...");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                List<DOMFasad> dbFasads = db.DOMFasads.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList();
                tek = 0;
                foreach (DOMFasad S in dbFasads)
                {
                    try
                    {
                        db.DOMFasads.Remove(S);
                        db.SaveChanges();
                        otklik(dbRoofs.Count, ref tek, "удаляем старые данные фасадов...");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                List<DOMRoom> dbRooms = db.DOMRooms.Where(x => x.Date.Year == Date.Year && x.Date.Month == Date.Month).ToList();
                tek = 0;
                foreach (DOMRoom S in dbRooms)
                {
                    try
                    {
                        db.DOMRooms.Remove(S);
                        db.SaveChanges();
                        otklik(dbRoofs.Count, ref tek, "удаляем старые данные внутридомовых элементов...");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }


                // Установить значения в нем
                cookie["Download"] = "0";
                // Добавить куки в ответ
                Response.Cookies.Add(cookie);




                //call this method inside your working action
                ProgressHub.SendMessage("Инициализация и подготовка...", 0);

                // получаем имя файла
                string fileName = Path.GetFileName(upload.FileName);
                // сохраняем файл в папку Files в проекте
                if (Directory.Exists(Server.MapPath("~/Files/")) == false)
                {
                    Directory.CreateDirectory(Server.MapPath("~/Files/"));

                }
                
                upload.SaveAs(Server.MapPath("~/Files/" + fileName));
                //обрабатываем файл после загрузки


                                                  //0адрес        1площадь_отмостки     2материал_фундамента    3тип_фундамента        4Кап_Ремонт_кровли        5Утеплитель          6ФормаКрыши      7КапРемонтНесущейЧасти    8ВидНесущейЧасти         9 ТипКровли       10 Износ фасада     11 Год ремонта фасада  12 Материал отделки фасада  13 Тип фасада      14 Утепление фасада  15 Балконы, лоджии 16 Количество балконов 17 Количество лоджий 18Тип внутренних стен 19Перекрытия                 20 Окна               21 Двери
                string[] Names = new string[] { "HOME_ADDRESS", "MKDSPECIFIED_14581", "MKDSPECIFIED_15016_1", "MKDSPECIFIED_13516_1","MKDSPECIFIED_20083","MKDSPECIFIED_15246_1","MKDSPECIFIED_12179_1","MKDSPECIFIED_20078","MKDSPECIFIED_20152_1","MKDSPECIFIED_12185_1","MKDSPECIFIED_20072","MKDSPECIFIED_20073", "MKDSPECIFIED_13049_1","MKDSPECIFIED_14549_1","MKDSPECIFIED_14011_1","MKDSPECIFIED_20120","MKDSPECIFIED_11556","MKDSPECIFIED_13056","MKDSPECIFIED_16590_1","MKDSPECIFIED_15087_1","MKDSPECIFIED_13059_1","MKDSPECIFIED_12139_1"};
                string Error = "";
                List<List<string>> excel = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName), Names, out Error, "КонстЭлемОЖФ");
                if (excel.Count < 1)
                {
                    //если нифига не загрузилось то 
                    ViewBag.Error = Error;
                    ViewBag.Names = Names;
                    Console.WriteLine("Пустой массив значит файл не загрузился!(он уже удалился)");
                    return View("NotUpload");
                }
                else
                {
                    pro100 = excel.Count;
                    DOMFundament Fundament = new DOMFundament();
                    DOMRoof Roof = new DOMRoof();
                    DOMFasad Fasad = new DOMFasad();
                    DOMRoom Room = new DOMRoom();
                    List<Adres> Adresa = db.Adres.ToList();// грузим все адреса из БД
                    List<FundamentMaterial> FundamentMat = db.FundamentMaterials.ToList();
                    List<FundamentType> FundamentType = db.FundamentTypes.ToList();
                    List<RoofType> RT = db.RoofTypes.ToList();
                    List<RoofForm> RF = db.RoofForms.ToList();
                    List<RoofVid> RV = db.RoofVids.ToList();
                    List<RoofUteplenie> RU = db.RoofUteplenies.ToList();
                    List<string> save = new List<string>();
                    List<string> errors = new List<string>();
                    List<string> saveR = new List<string>();
                    List<string> errorsR = new List<string>();
                    List<string> saveRoom = new List<string>();
                    List<string> errorsRoom = new List<string>();
                    List<string> saveF = new List<string>();
                    List<string> errorsF = new List<string>();


                    //для каждой строки в экселе
                    foreach (List<string> L in excel)
                    {

                        string adr = zachistkaAdresa(L[0]);
                        //сверяем улицу 
                        bool go = false;
                        foreach (Adres A in Adresa)
                        {
                            if (A.Adress.Equals(adr))
                            {
                                //если улица совпала то сохраняем айдишник
                                Fundament.AdresId = A.Id;
                                Roof.AdresId = A.Id;
                                Fasad.AdresId = A.Id;
                                Room.AdresId = A.Id;
                                go = true;
                                break;
                            }
                        }
                        //если нашли адрес то сохраняем все остальные данные
                      
                        if (go)
                        {
                            Fundament.Date = Date;
                            //ищем материал 
                            Fundament.MaterialId = 1;//если не найдет
                            Fundament.Ploshad = Convert.ToDecimal(L[1]);

                            foreach (FundamentMaterial FM in FundamentMat)
                            {
                                if (FM.Material.Replace(" ", "").Equals(L[2].Replace(" ", "")))
                                {
                                    Fundament.MaterialId = FM.Id;
                                    break;
                                }
                            }
                            //ищем тип фундамента
                            Fundament.TypeId = 1;//если не найдет
                            foreach (FundamentType FT in FundamentType)
                            {
                                if (FT.Type.Replace(" ", "").Equals(L[3].Replace(" ", "")))
                                {
                                    Fundament.TypeId = FT.Id;
                                    break;
                                }
                            }
                            
                            //сохраняем фундамент
                            if (Fundament.TypeId != 1 && Fundament.MaterialId != 1)
                            {
                                try
                                {
                                    db.DOMFundaments.Add(Fundament);
                                    db.SaveChanges();
                                    save.Add(L[0]);
                                }
                                catch (Exception e)
                                {
                                    
                                    errors.Add(L[0]+ "(ошибка сохранения)");
                                }
                            }
                            else
                            {
                               
                                errors.Add(L[0]+ "(нулевые данные)");
                            }


                            //теперь ищем крыши
                            Roof.Date = Date;
                          

                            Roof.Ploshad = 0;
                            Roof.YearKrovlya = Convert.ToInt16(L[4]);
                            Roof.Year = Convert.ToInt16(L[7]);
                            //Тип крыши
                            Roof.TypeId = 1;//если не найдет
                            foreach (RoofType R in RT)
                            {
                                if (R.Type.Replace(" ", "").Equals(L[9].Replace(" ", "")))
                                {
                                    Roof.TypeId = R.Id;
                                    break;
                                }
                            }
                            //Вид крыши
                            Roof.VidId = 1;//если не найдет
                            foreach (RoofVid R in RV)
                            {
                                if (R.Vid.Replace(" ", "").Equals(L[8].Replace(" ", "")))
                                {
                                    Roof.VidId = R.Id;
                                    break;
                                }
                            }
                            //Форма крыши
                            Roof.FormId = 1;//если не найдет
                            foreach (RoofForm R in RF)
                            {
                                if (R.Form.Replace(" ", "").Equals(L[6].Replace(" ", "")))
                                {
                                    Roof.FormId = R.Id;
                                    break;
                                }
                            }
                            //Утепление крыши
                            Roof.UteplenieId = 1;//если не найдет
                            foreach (RoofUteplenie R in RU)
                            {
                                if (R.Uteplenie.Replace(" ", "").Equals(L[5].Replace(" ", "")))
                                {
                                    Roof.UteplenieId = R.Id;
                                    break;
                                }
                            }

                            //если данные по крышам не нулевые то сохраняем
                            if (Roof.UteplenieId + Roof.TypeId + Roof.VidId + Roof.FormId >4)
                            {
                                try
                                {
                                    db.DOMRoofs.Add(Roof);
                                    db.SaveChanges();
                                    saveR.Add(L[0]);
                                }
                                catch (Exception e )
                                {
                                    db.DOMRoofs.Remove(Roof);
                                    errorsR.Add(L[0] + "(ошибка сохранения)");
                                }
                            }
                            else
                            {
                                errorsR.Add(L[0] + "(нулевые данные)");
                            }
                            // Теперь ищем фасады 10 Износ фасада     11 Год ремонта фасада  12 Материал отделки фасада  13 Тип фасада      14 Утепление фасада
                            Fasad.Date = Date;
                            Fasad.Iznos = Convert.ToInt16(L[10]);
                            Fasad.Year = Convert.ToInt16(L[11]);
                            Fasad.MaterialId =1;
                            try
                            {
                                foreach (FasadMaterial F in db.FasadMaterials)
                                {
                                    if (F.Material.Replace(" ", "").ToUpper().Equals(L[12].Replace(" ", "").ToUpper()))
                                    {
                                        Fasad.MaterialId = F.Id;
                                        break;
                                    }
                                }
                            }
                            catch { }
                            Fasad.UteplenieId = 1;
                            try
                            {
                                foreach (FasadUteplenie F in db.FasadUteplenies)
                                {
                                    if (F.Uteplenie.Replace(" ", "").ToUpper().Equals(L[14].Replace(" ", "").ToUpper()))
                                    {
                                        Fasad.UteplenieId = F.Id;
                                        break;
                                    }
                                }
                            }
                            catch { }
                            Fasad.TypeId = 1;
                            try
                            {
                                foreach (FasadType F in db.FasadTypes)
                                {
                                    if (F.Type.Replace(" ", "").ToUpper().Equals(L[13].Replace(" ", "").ToUpper()))
                                    {
                                        Fasad.TypeId = F.Id;
                                        break;
                                    }
                                }
                            }
                            catch { }
                            if (Fasad.UteplenieId + Fasad.TypeId + Fasad.MaterialId > 3)
                            {
                                try
                                {
                                    db.DOMFasads.Add(Fasad);
                                    db.SaveChanges();
                                    saveF.Add(L[0]);
                                }
                                catch (Exception e)
                                {
                                    db.DOMFasads.Remove(Fasad);
                                    errorsF.Add(L[0] + "(ошибка сохранения)");
                                }
                            }
                            else
                            {
                                errorsF.Add(L[0] + "(нулевые данные)");
                            }

                            //Теперь внутренности помещений 16 Количество балконов 17 Количество лоджий 18Тип внутренних стен 19Перекрытия 20 Окна 21 Двери
                            Room.Date = Date;
                            Room.Lodgi = Convert.ToInt16(L[17]);
                            Room.Balkon = Convert.ToInt16(L[16]);
                            Room.TypeId = 1;
                            try
                            {
                                foreach (RoomType F in db.RoomTypes)
                                {
                                    if (F.Type.Replace(" ", "").ToUpper().Equals(L[18].Replace(" ", "").ToUpper()))
                                    {
                                        Room.TypeId = F.Id;
                                        break;
                                    }
                                }
                            }
                            catch { }
                            Room.WindowId = 1;
                            try
                            {
                                foreach (RoomWindow F in db.RoomWindows)
                                {
                                    if (F.Window.Replace(" ", "").ToUpper().Equals(L[20].Replace(" ", "").ToUpper()))
                                    {
                                        Room.WindowId = F.Id;
                                        break;
                                    }
                                }
                            }
                            catch { }
                            Room.OverlapId = 1;
                            try
                            {
                                foreach (RoomOverlap F in db.RoomOverlaps)
                                {
                                    if (F.Overlap.Replace(" ", "").ToUpper().Equals(L[19].Replace(" ", "").ToUpper()))
                                    {
                                        Room.TypeId = F.Id;
                                        break;
                                    }
                                }
                            }
                            catch { }
                            Room.DoorId = 1;
                            try
                            {
                                foreach (RoomDoor F in db.RoomDoors)
                                {
                                    if (F.Door.Replace(" ", "").ToUpper().Equals(L[21].Replace(" ", "").ToUpper()))
                                    {
                                        Room.DoorId = F.Id;
                                        break;
                                    }
                                }
                            }
                            catch { }
                            if (Room.DoorId + Room.TypeId + Room.OverlapId+Room.WindowId > 4)
                            {
                                try
                                {
                                    db.DOMRooms.Add(Room);
                                    db.SaveChanges();
                                    saveRoom.Add(L[0]);
                                }
                                catch (Exception e)
                                {
                                    db.DOMRooms.Remove(Room);
                                    errorsRoom.Add(L[0] + "(ошибка сохранения)");
                                }
                            }
                            else
                            {
                                errorsRoom.Add(L[0] + "(нулевые данные)");
                            }


                        }
                        else
                        {
                            errors.Add(L[0]);
                        }

                        procount++;
                        progress = Convert.ToInt16(50 + procount / pro100 * 50);
                        ProgressHub.SendMessage("Обрабатываем файл ГИС ЖКХ...", progress);
                        if (procount > pro100) { procount = Convert.ToInt32(pro100); }

                    }
                   
                    ViewBag.Save = save;
                    ViewBag.Errors = errors;
                    ViewBag.SaveR = saveR;
                    ViewBag.ErrorsR = errorsR;
                    ViewBag.SaveRoom = saveRoom;
                    ViewBag.ErrorsRoom = errorsRoom;
                    ViewBag.SaveF = saveF;
                    ViewBag.ErrorsF = errorsF;


                    ViewBag.date = Date;
                    ViewBag.file = fileName;
                    
                    //Грузим 2 часть файла!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    upload.SaveAs(Server.MapPath("~/Files/" + fileName));
                    //обрабатываем файл после загрузки


                                             //0адрес          1Износ ГВ         2материал ХВ              3Износ ХВ            4Материал ГВ             5Материал стояков    6КоличествоЭлектровводов  7МатериалВодоотведения  8Год кап. рем. водоотвед.  9Кап. рем. электро.  10Кап. рем. ХВ.   11Кап.рем.ГВ    12Износ отопл. 13Количество вводов отопления. 14.Материал отопления1 15.Материал отопления2  16.Материал труб отопления 17.Износ электро 18.Износ водоотвод
                    Names = new string[] { "HOME_ADDRESS", "MKDSPECIFIED_20114", "MKDSPECIFIED_14778_1", "MKDSPECIFIED_20107", "MKDSPECIFIED_12023_1", "MKDSPECIFIED_12060_1", "MKDSPECIFIED_12545", "MKDSPECIFIED_13412_1", "MKDSPECIFIED_20143", "MKDSPECIFIED_20122", "MKDSPECIFIED_20105", "MKDSPECIFIED_20112", "MKDSPECIFIED_20096", "MKDSPECIFIED_12035", "MKDSPECIFIED_14721_1", "MKDSPECIFIED_14721_2", "MKDSPECIFIED_15055_1", "MKDSPECIFIED_21834", "MKDSPECIFIED_21833" };
                    Error = "";
                    excel = ExcelSVNUpload.IMPORT(Server.MapPath("~/Files/" + fileName), Names, out Error, "ВнутрСистемыОЖФ");
                    if (excel.Count < 1)
                    {
                        //если нифига не загрузилось то 
                        ViewBag.Error = Error;
                        ViewBag.Names = Names;
                        Console.WriteLine("Пустой массив значит файл не загрузился!(он уже удалился)");
                        return View("NotUpload");
                    }
                    else
                    {
                        DOMOtoplenie Otoplenie = new DOMOtoplenie();
                        DOMVodootvod Vodootvod = new DOMVodootvod();
                        DOMHW HW = new DOMHW();
                        DOMCW CW = new DOMCW();
                        DOMElectro Electro = new DOMElectro();

                        pro100 = excel.Count;
                        Material Material = new Material();

                        Adresa = db.Adres.ToList();// грузим все адреса из БД
                                                   // List<Material> Materials = db.Materials.ToList();

                        //для каждой строки в экселе
                        foreach (List<string> L in excel)
                        {

                            string adr = zachistkaAdresa(L[0]);
                            //сверяем улицу 
                         
                            foreach (Adres A in Adresa)
                            {
                                if (A.Adress.Equals(adr))
                                {
                                  
                                    //если улица совпала то сохраняем айдишник
                                    bool s = false;
                                    try
                                    {
                                        Otoplenie = db.DOMOtoplenies.Where(x => x.AdresId == A.Id&& x.Date.Year == Date.Year && x.Date.Month == Date.Month).First();
                                        s = true;
                                    

                                    }
                                    catch { }
                                    Otoplenie.AdresId = A.Id;
                                    Otoplenie.IznosOtop = Convert.ToInt32(L[12]);
                                    Otoplenie.VvodsOtop = Convert.ToInt32(L[13]);
                                    Otoplenie.MaterialOtop1Id = PoiskMateriala(L[14].ToString());
                                    Otoplenie.MaterialOtop2Id = PoiskMateriala(L[15].ToString());
                                    Otoplenie.MaterialOtopTrubId = PoiskMateriala(L[16].ToString());
                                    Otoplenie.MaterialTeploId = PoiskMateriala(L[5].ToString());
                                    Otoplenie.Date = Date;
                                    if (s)
                                    {
                                        db.Entry(Otoplenie).State = EntityState.Modified;
                                        db.SaveChanges();
                                    } else
                                    {
                                        db.DOMOtoplenies.Add(Otoplenie);
                                        db.SaveChanges();
                                    }
                                   

                                    s = false;
                                    try
                                    {
                                        Vodootvod = db.DOMVodootvods.Where(x => x.AdresId == A.Id && x.Date.Year == Date.Year && x.Date.Month == Date.Month).First();
                                        s = true;
                                    }
                                    catch { }

                                    Vodootvod.AdresId = A.Id;
                                    Vodootvod.Iznos = Convert.ToInt32(L[18]);
                                    Vodootvod.MaterialId = PoiskMateriala(L[7].ToString());
                                    Vodootvod.Remont = Convert.ToInt32(L[8]);
                                    Vodootvod.Date = Date;
                                    if (s)
                                    {
                                        db.Entry(Vodootvod).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        db.DOMVodootvods.Add(Vodootvod);
                                        db.SaveChanges();
                                    }

                                    s = false;
                                    try
                                    {
                                        HW = db.DOMHWs.Where(x => x.AdresId == A.Id && x.Date.Year == Date.Year && x.Date.Month == Date.Month).First();
                                        s = true;
                                    }
                                    catch { }


                                    HW.AdresId = A.Id;
                                    HW.IznosHW = Convert.ToInt32(L[1]);
                                    HW.MaterialHWId = PoiskMateriala(L[4].ToString());
                                    HW.RemontHW = Convert.ToInt32(L[11]);
                                    HW.Date = Date;
                                    if (s)
                                    {
                                        db.Entry(HW).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        db.DOMHWs.Add(HW);
                                        db.SaveChanges();
                                    }

                                    s = false;
                                    try
                                    {
                                        CW = db.DOMCWs.Where(x => x.AdresId == A.Id && x.Date.Year == Date.Year && x.Date.Month == Date.Month).First();
                                        s = true;
                                    }
                                    catch { }

                                    CW.AdresId = A.Id;
                                    CW.IznosCW = Convert.ToInt32(L[3]);
                                    CW.MaterialCWId = PoiskMateriala(L[2].ToString());
                                    CW.RemontCW = Convert.ToInt32(L[10]);
                                    CW.Date = Date;
                                    if (s)
                                    {
                                        db.Entry(CW).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        db.DOMCWs.Add(CW);
                                        db.SaveChanges();
                                    }


                                    s = false;
                                    try
                                    {
                                        Electro = db.DOMElectroes.Where(x => x.AdresId == A.Id && x.Date.Year == Date.Year && x.Date.Month == Date.Month).First();
                                        s = true;
                                    }
                                    catch { }
                                    Electro.AdresId = A.Id;
                                    Electro.Electrovvods = Convert.ToInt32(L[6]);
                                    Electro.IznosElectro = Convert.ToInt32(L[17]);
                                    Electro.RemontElectro = Convert.ToInt32(L[9]);
                                    Electro.Date = Date;
                                    

                                    if (s)
                                    {
                                        db.Entry(Electro).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        db.DOMElectroes.Add(Electro);
                                        db.SaveChanges();
                                    }
                                    break;
                                }
                            }
                            //если нашли адрес то сохраняем все остальные данные

                        }
                        return View("UploadComplete");
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult UploadComplete()
        {

            return View();
        }



        // POST: DOMFundaments/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Ploshad,MaterialId,TypeId,AdresId,Date")] DOMFundament dOMFundament)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dOMFundament).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresId = new SelectList(db.Adres, "Id", "Adress", dOMFundament.AdresId);
            ViewBag.MaterialId = new SelectList(db.FundamentMaterials, "Id", "Material", dOMFundament.MaterialId);
            ViewBag.TypeId = new SelectList(db.FundamentTypes, "Id", "Type", dOMFundament.TypeId);
            return View(dOMFundament);
        }

        // GET: DOMFundaments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOMFundament dOMFundament = db.DOMFundaments.Find(id);
            if (dOMFundament == null)
            {
                return HttpNotFound();
            }
            return View(dOMFundament);
        }

        // POST: DOMFundaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DOMFundament dOMFundament = db.DOMFundaments.Find(id);
            db.DOMFundaments.Remove(dOMFundament);
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
