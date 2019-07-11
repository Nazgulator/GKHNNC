using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class Tarif
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Тариф горячая вода")]
        public decimal HotWater { get; set; }

        [Display(Name = "Тариф холодная вода")]
        public decimal ColdWater { get; set; }

        [Display(Name = "Тариф отопление Гкал")]
        public decimal OtoplenieEnergy { get; set; }

        [Display(Name = "Тариф теплота в кубе")]
        public decimal TeplotaVKube { get; set; }

        [Display(Name = "Дата выполнения")]
        public DateTime Date { get; set; }
    }
}