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
    public class AutoScansSverka
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Display(Name = "Километраж")]
        public decimal KM { get; set; }

        [Display(Name = "Время в движении")]
        public DateTime TimeInMove { get; set; }

        [Display(Name = "Моточасы")]
        public DateTime MotoHours { get; set; }

        [Display(Name = "Максимальная скорость")]
        public decimal MaxSpeed { get; set; }

        [Display(Name = "Количество поездок")]
        public decimal Poesdki { get; set; }

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

        [Display(Name = "АвтоID")]
        public int AvtoId { get; set; }

        [Display(Name = "Дата снятия")]
        public DateTime DateSnyatia { get; set; }


    }
}