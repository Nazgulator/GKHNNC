using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class ASControl
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "АвтоID")]
        public int? AvtoId { get; set; }

        [Display(Name = "Автомобиль")]
        public Avtomobil Avto { get; set; }

        [Display(Name = "Водитель")]
        public int? VoditelId { get; set; }

        [Display(Name = "Водитель")]
        public Voditel Voditel { get; set; }


        [Display(Name = "ЗаказчикИД")]
        public int? ZakazchikId { get; set; }

        [Display(Name = "Заказчик")]
        public Zakazchik Zakazchik { get; set; }

        [Display(Name = "Прицеп")]
        public bool Pricep { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Display(Name = "Дата Завершения")]
        public DateTime DateClose { get; set; }

        [Display(Name = "Выехал?")]
        public bool Go { get; set; }

        [Display(Name = "Примечание")]
        public string Primech { get; set; }

        [Display(Name = "Километраж Автоскан")]
        public decimal KMAS { get; set; }

        [Display(Name = "Километраж водитель")]
        public decimal KM { get; set; }

        [Display(Name = "Потрачено по ДУТ")]
        public decimal DUT { get; set; }

        [Display(Name = "Начальный уровень")]
        public decimal Start { get; set; }

        [Display(Name = "Конечный уровень")]
        public decimal End { get; set; }

        [Display(Name = "Заправлено")]
        public decimal Zapravleno { get; set; }

        [Display(Name = "Слито")]
        public decimal Sliv { get; set; }

        [Display(Name = "Потеря связи")]
        public bool Warning { get; set; }

        [Display(Name = "Места")]
        public List<string> Mesta { get; set; }

        [Display(Name = "Потери связи")]
        public List<string> NoSvaz { get; set; }

        [Display(Name = "Подтверждено")]
        public bool Podtvergdeno { get; set; }




    }
}