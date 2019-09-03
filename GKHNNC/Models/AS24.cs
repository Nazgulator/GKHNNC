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
    public class AS24
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "АвтоID")]
        public int AvtoId { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Display(Name = "Километраж Автоскан")]
        public decimal KM { get; set; }

        [Display(Name = "Потрачено по ДУТ")]
        public decimal DUT { get; set; }

        [Display(Name = "Начальный уровень")]
        public decimal Start { get; set; }

        [Display(Name = "Конечный уровень")]
        public decimal End { get; set; }

        [Display(Name = "Заправлено")]
        public decimal Zapravleno { get; set; }

        [Display(Name = "Хронология")]
        public string Mesta { get; set; }

        [Display(Name = "Потери связи")]
        public string NoSvaz { get; set; }
    }
}