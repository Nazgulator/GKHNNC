using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class WorkSoderganie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Измерение")]
        public int IzmerenieId { get; set; }
        [Display(Name = "Измерение")]
        public Izmerenie Izmerenie { get; set; }
        [Display(Name = "Норма")]
        public decimal Norma { get; set; }
        [Display(Name = "Объём")]
        public decimal Obiem { get; set; }
        [Display(Name = "От чего зависит")]
        public string Opisanie { get; set; }
        [Display(Name = "Код для раcчета")]
        public int Code { get; set; }
        [Display(Name = "Значение")]
        public decimal Val { get; set; }
        [Display(Name = "Тип работ")]
        public int TipId { get; set; }
        [Display(Name = "Измерение")]
        public Tip Tip { get; set; }
        [Display(Name = "Периодичность")]
        public int Periodichnost { get; set; }
        //public string Agent { get; set; }
        //public DateTime Date { get; set; }


    }
}