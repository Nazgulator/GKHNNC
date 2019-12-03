using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using GKHNNC.Controllers;

namespace GKHNNC.Models
{
    public class DOMHW

    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Адрес ИД")]
        public int? AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }
        [Display(Name = "Износ ГВ")]
        public int IznosHW { get; set; }
        [Display(Name = "МатериалГВ ИД")]
        public int? MaterialHWId { get; set; }
        [Display(Name = "МатериалГВ ")]
        public Material MaterialHW { get; set; }
        [Display(Name = "Год ремонта ГВ")]
        public int RemontHW { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
        [Display(Name = "Состояние")]
        public int Sostoyanie { get; set; }


    }
}