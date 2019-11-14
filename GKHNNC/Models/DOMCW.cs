using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using GKHNNC.Controllers;

namespace GKHNNC.Models
{
    public class DOMCW
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Адрес ИД")]
        public int? AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }
        [Display(Name = "Износ ХВ")]
        public int IznosCW { get; set; }
        [Display(Name = "МатериалХВ ИД")]
        public int? MaterialCWId { get; set; }
        [Display(Name = "МатериалXВ ")]
        public Material MaterialCW { get; set; }
        [Display(Name = "Год ремонта ХВ")]
        public int RemontCW { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }


    }
}