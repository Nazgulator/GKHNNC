using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class WorkStandart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "ГруппаId")]
        public int TipId { get; set; }
        [Display(Name = "Группа")]
        public Tip Tip { get; set; }
        [Display(Name = "ИзмерениеID")]
        public int IzmerenieId { get; set; }
        [Display(Name = "Измерение")]
        public Izmerenie Izmerenie { get; set; }
        [Display(Name = "Стоимость")]
        public decimal Cost { get; set; }
        //public string Agent { get; set; }
        //public DateTime Date { get; set; }

    }
}