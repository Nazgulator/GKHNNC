using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class ViewVoda
    {
        [Display(Name = "Адрес")]
        public string Adres { get; set; }


        [Display(Name = "Факт.")]
        public decimal Fact { get; set; }
        [Display(Name = "План")]
        public decimal Plan { get; set; }
        [Display(Name = "ИПУ")]
        public decimal IPU{ get; set; }
        [Display(Name = "УЭВ")]
        public decimal Uev { get; set; }
        [Display(Name = "УЭВ-Факт")]
        public decimal RaznPlan { get; set; }
        [Display(Name = "УЭВ-План")]
        public decimal RaznFact { get; set; }
        [Display(Name = "ПУ")]
        public bool PU { get; set; }
        [Display(Name = "Доп")]
        public string Primech { get; set; }
        [Display(Name = "Объём Факт")]
        public decimal VFact { get; set; }
        [Display(Name = "Объём УЭВ")]
        public decimal GVUEVM3 { get; set; }

        [Display(Name = "Средняя за отопительный сезон (объём)")]
        public decimal Srednyaya { get; set; }
        [Display(Name = "Объём за прошлый аналогичный период")]
        public decimal LastYear { get; set; }

        [Display(Name = "Средняя за отопительный сезон (рубли)")]
        public decimal SrednyayaRub { get; set; }
        [Display(Name = "Рубли за прошлый аналогичный период")]
        public decimal LastYearRub { get; set; }

        [Display(Name = "Рубли за прошлый аналогичный период")]
        public decimal GVObshee { get; set; }


    }
}