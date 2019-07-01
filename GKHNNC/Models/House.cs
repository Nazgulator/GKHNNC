using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class House
    {
      
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "АдресId")]
        public int? AdresId { get; set; }
        [Display(Name = "Адрес")]
        public string Adres { get; set; }
        [Display(Name = "Арендаторы")]
        public Arendator[] Arendators { get; set; }
        [Display(Name = "Площадь")]
        public decimal Ploshad { get; set; }
        [Display(Name = "Площадь арендаторов")]
        public decimal PloshadArendators { get; set; }
        [Display(Name = "Теплота")]
        public decimal Teplota { get; set; }
        [Display(Name = "Теплота Арендаторов")]
        public decimal TeplotaArendators { get; set; }
        [Display(Name = "Теплота 1/12")]
        public decimal Teplota12 { get; set; }
        [Display(Name = "Теплота 1/12 Арендаторов")]
        public decimal Teplota12Arendators { get; set; }
        [Display(Name = "Горячая вода")]
        public decimal HotWater { get; set; }
        [Display(Name = "Горячая вода Арендаторов")]
        public decimal HotWaterArendators { get; set; }
        [Display(Name = "Холодная вода")]
        public decimal ColdWater { get; set; }
        [Display(Name = "Холодная вода Арендаторов")]
        public decimal ColdWaterArendators { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM/yyyy}", ApplyFormatInEditMode = true)]
        
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
    }
}