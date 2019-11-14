using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using GKHNNC.Controllers;

namespace GKHNNC.Models
{
    public class DOMVodootvod
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Адрес ИД")]
        public int? AdresId { get; set; }
    [Display(Name = "Адрес")]
        public Adres Adres { get; set; }
        [Display(Name = "Год ремонта")]
    public int Remont { get; set; }
        [Display(Name = "Износ")]
        public int Iznos { get; set; }
        [Display(Name = "Материал водоотведения ИД")]
        public int? MaterialId{ get; set; }
    [Display(Name = "Материал водоотведения")]
        public Material Material { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }


        // public System.Data.Entity.DbSet<DOMOtoplenie> DB { get; set; }
    }
}