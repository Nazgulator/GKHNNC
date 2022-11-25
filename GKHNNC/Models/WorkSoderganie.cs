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
        [Display(Name = "Ремонт?")]
        public bool Remont { get; set; }
        [Display(Name = "Стоимость материалов")]
        public decimal CostMterials { get; set; }
        [Display(Name = "Стоимость работы без материалов")]
        public decimal CostWrok { get; set; }
        [Display(Name = "% хорошее состояние")]
        public decimal ProcGood { get; set; }
        [Display(Name = "% плохое состояние")]
        public decimal ProcBad { get; set; }
        [Display(Name = "Общая стоимость материалов и работы")]
        public decimal Cost;
        [Display(Name = "Формула расчета")]
        public string Comment;
        [Display(Name = "Физическая величина")]
        public decimal Fiz;
        //public string Agent { get; set; }
        //public DateTime Date { get; set; }


    }
}