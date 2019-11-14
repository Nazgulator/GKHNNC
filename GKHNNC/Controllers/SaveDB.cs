using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using GKHNNC.Models;
using GKHNNC.DAL;

namespace GKHNNC.Controllers
{
    public abstract class SaveDB
    {
        private WorkContext db = new WorkContext();
        [Display(Name = "Адрес ИД")]
        public int? AdresId;
        [Display(Name = "Адрес")]
        public Adres Adres;
        [Display(Name = "Год ремонта")]
        public int Remont;
        [Display(Name = "Износ")]
        public int Iznos;
        [Display(Name = "Материал водоотведения ИД")]
        public int? MaterialId;
        [Display(Name = "Материал водоотведения")]
        public Material Material;
        public System.Data.Entity.DbSet<Object> DB { get; set; }

    }
}