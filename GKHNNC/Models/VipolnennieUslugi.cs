using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class VipolnennieUslugi
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Дата")]
        [DisplayFormat(DataFormatString = "{0:yyyy'/'MM}")]
        public DateTime Date { get; set; }
        [Display(Name = "Адрес")]
        public int? AdresId { get; set; }
        //для подгрузки через айди
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }

        [Display(Name = "Услуга")]
        public int? UslugaId { get; set; }
        //для подгрузки через айди
        [Display(Name = "Услуга")]
        public Usluga Usluga { get; set; }

        [Display(Name = "Разрешение печати")]
        public bool ForPrint { get; set; }

        [Display(Name = "Стоимость на кв.метр")]
        public decimal StoimostNaM2 { get; set; }
        [Display(Name = "Стоимость в месяц")]
        public decimal StoimostNaMonth { get; set; }

    }
}