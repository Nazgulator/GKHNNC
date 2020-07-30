using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class OsmotrRecommendWork
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Осмотр ИД")]
        public int OsmotrId { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Измерение ID")]
        public int IzmerenieId { get; set; }
        [Display(Name = "Измерение")]
        public Izmerenie Izmerenie { get; set; }
        [Display(Name = "Стоимость")]
        public decimal Cost { get; set; }
        [Display(Name = "Количество")]
        public decimal Number { get; set; }
        [Display(Name = "Часть дома")]
        public DOMPart DOMPart { get; set; }
        [Display(Name = "Часть дома ид")]
        public int DOMPartId { get; set; }
        [Display(Name = "Смета?")]
        public bool Smeta { get; set; }


        
    }
}