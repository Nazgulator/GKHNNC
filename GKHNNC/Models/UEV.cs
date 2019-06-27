using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class UEV
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "АдресID")]
        public int? AdresId { get; set; }
        public Adres Adres { get; set; }
        [Display(Name = "Имя как в ведомости")]
        public string Name { get; set; }
        [Display(Name = "Код UEV")]
        public int KodUEV { get; set; }
        [Display(Name = "Прибор учета")]
        public int Pribor { get; set; }
        [Display(Name = "Отопление Руб")]
        public decimal OtEnergyRub { get; set; }
        [Display(Name = "Отопление Гкал")]
        public decimal OtEnergyGkal { get; set; }
        [Display(Name = "ГВ Руб")]
        public decimal HwEnergyRub { get; set; }
        [Display(Name = "ГВ Гкал")]
        public decimal HwEnergyGkal { get; set; }
        [Display(Name = "Теплоноситель Руб")]
        public decimal HwVodaRub { get; set; }
        [Display(Name = "Теплоноситель Гкал")]
        public decimal HwVodaM3 { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

    }
}