using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class OPU
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "АдресID")]
        public int? AdresId { get; set; }
        public Adres Adres { get; set; }
        [Display(Name = "Отопление Гкал")]
        public decimal OtopGkal { get; set; }
        [Display(Name = "ГВ м3")]
        public decimal GWM3 { get; set; }
        [Display(Name = "ХВ м3")]
        public decimal HWM3 { get; set; }
        [Display(Name = "Отопление руб")]
        public decimal OtopRub { get; set; }
        [Display(Name = "ГВ руб")]
        public decimal GWRub { get; set; }
        [Display(Name = "ХВ руб")]
        public decimal HWRub { get; set; }
        [Display(Name = "Примечание")]
        public string Primech { get; set; }//ремонт поверка или звездочка

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
    }
}