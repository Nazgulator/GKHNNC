using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class Arendator
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "АдресId")]
        public int? AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }
        [Display(Name = "Имя")]
        public string Name { get; set; }
        [Display(Name = "Площадь")]
        public decimal Ploshad { get; set; }
        [Display(Name = "Теплота")]
        public decimal Teplota { get; set; }
        [Display(Name = "Теплота 1/12")]
        public decimal Teplota12 { get; set; }
        [Display(Name = "Горячая вода")]
        public decimal HotWater { get; set; }
        [Display(Name = "Холодная вода")]
        public decimal ColdWater { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
    }
}