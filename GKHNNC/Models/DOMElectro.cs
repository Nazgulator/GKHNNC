using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using GKHNNC.Controllers;

namespace GKHNNC.Models
{
    public class DOMElectro 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Адрес ИД")]
        public int? AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }
        [Display(Name = "Количество электровводов")]
        public int Electrovvods { get; set; }
        [Display(Name = "Год ремонта Электросети")]
        public int RemontElectro { get; set; }
        [Display(Name = "Износ энергосети")]
        public int IznosElectro { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
        [Display(Name = "Состояние")]
        public int Sostoyanie { get; set; }
    }
}