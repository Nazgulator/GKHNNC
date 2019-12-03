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

namespace GKHNNC.Controllers
{
    public class OsmotrsController : Controller
    {
        private WorkContext db = new WorkContext();

        // GET: Osmotrs
        public ActionResult Index()
        {
            var osmotrs = db.Osmotrs.Include(o => o.Adres).Include(o => o.DOMCW).Include(o => o.DOMElectro).Include(o => o.DOMFasad).Include(o => o.DOMFundament).Include(o => o.DOMHW).Include(o => o.DOMOtoplenie).Include(o => o.DOMRoof).Include(o => o.DOMRoom).Include(o => o.DOMVodootvod);
            return View(osmotrs.ToList());
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
        public ActionResult AddActiveDefect (DateTime date , int AdresId=13,int ElementId=1,int DefectId = 1)
        {
            string Data = "";
            ActiveDefect A = new ActiveDefect();
            A.AdresId = AdresId;
            A.Date = date;
            A.DefectId = DefectId;
            A.ElementId = ElementId;
            try
            {
                db.ActiveDefects.Add(A);
                db.SaveChanges();
                Data = "Дефект успешно добавлен";
            }
            catch { Data = "Ошибка сохранения в БД"; }
            return Json (Data);
        }
        //служит для отображения списка активных дефектов и возможных дефектов модели
        public ActionResult ViewActiveDefect( DateTime Date,int ElementId = 1, int AdresId = 1,int OsmotrId=1)
        {
            string Data = "";
            ActiveElement AE = new ActiveElement();

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
                    AE.Defects = db.Defects.Where(x => x.ElementId == ElementId).ToList();
                }
                catch (Exception e)
                {
                    AE.Defects = new List<Defect>();
                }
                try
                {

                    AE.ActiveDefects = db.ActiveDefects.Where(x => x.ElementId == ElementId && x.AdresId == AdresId && x.Date == Date).OrderByDescending(x => x.Date).Include(x => x.Defect).ToList();
                }
                catch (Exception e)
                {
                    AE.ActiveDefects = new List<ActiveDefect>();
                }
            }
            db.ActiveElements.Add(AE);
            db.SaveChanges();
            
            return View (AE);
        }

        // GET: Osmotrs/Create
        public ActionResult Create(DateTime date,int id = 13)
        {
            Osmotr Result = new Osmotr();
            Result.AdresId = id;
            Result.Adres = db.Adres.Where(x => x.Id == id).First();
            Result.Date = date;
            Result.DOMCW = db.DOMCWs.Where(x => x.AdresId == id).OrderByDescending(x=>x.Date).First();
            Result.DOMHW = db.DOMHWs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
            Result.DOMElectro = db.DOMElectroes.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
            Result.DOMFasad = db.DOMFasads.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
            Result.DOMFundament = db.DOMFundaments.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).Include(x=>x.Material).Include(x=>x.Type).First();
            ViewBag.FundamentMaterials = new SelectList(db.FundamentMaterials, "Id", "Material");
            ViewBag.FundamentTypes = new SelectList(db.FundamentTypes, "Id", "Type");
            Result.DOMOtoplenie = db.DOMOtoplenies.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
            Result.DOMRoof = db.DOMRoofs.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
            Result.DOMRoom = db.DOMRooms.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
            Result.DOMVodootvod = db.DOMVodootvods.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).First();
            Result.Sostoyanie = 10;
            Result.Elements = new List<ActiveElement>();
            List<Element> Elements = db.Elements.ToList();
            //сохраняем осмотр
            try
            {
                db.Osmotrs.Add(Result);
                db.SaveChanges();
            }
            catch (Exception e) { ViewBag.Id = 0; }
            try
            {//поскольку дефекты фиксируются осмотрами то у всех должна быть одна дата даже на разные элементы
                DateTime D = date;
                try
                {
                    db.ActiveDefects.Where(x => x.AdresId == id).OrderByDescending(x => x.Date).Select(x => x.Date).First();
                }
                catch { }
                foreach (Element E in Elements)
                {
                    //ищем самый новый по дате и если такого нет то создаем пустой
                    
                    ActiveElement AE = new ActiveElement();

                    try
                    {
                        AE = db.ActiveElements.Where(x => x.ElementId == E.Id&&x.AdresId==id).Include(x=>x.Element).Include(x=>x.Defects).OrderByDescending(x => x.Date).First();
                    }
                    catch
                    {
                        AE.ElementId = E.Id;
                        AE.Element = db.Elements.Where(x => x.Id == E.Id).First();
                        AE.OsmotrId = Result.Id;
                        AE.AdresId = id;
                        AE.Date = date;
                        AE.Sostoyanie = 10;
                        
                        try
                        {
                            AE.Defects = db.Defects.Where(x => x.ElementId == E.Id).ToList();
                        }
                        catch (Exception e)
                        {
                            AE.Defects = new List<Defect>();
                        }
                        try
                        {
                            
                            AE.ActiveDefects = db.ActiveDefects.Where(x => x.ElementId == E.Id&&x.AdresId==id&&x.Date==D).OrderByDescending(x=>x.Date).Include(x=>x.Defect).ToList();
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
            catch (Exception e){ }
          
            try
            {
                Result.Defects = db.ActiveDefects.Where(x => x.AdresId == id).ToList();
            }
            catch
            {

            }
           

            return View(Result);
        }

        // POST: Osmotrs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
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
