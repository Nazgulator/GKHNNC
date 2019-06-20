using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GKHNNC.Models;

namespace GKHNNC.Models
{
    public class Ezdka
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Архивный номер")]
        public int ArhNumb { get; set; }

        [Display(Name = "ID автомобиля")]
        public int? AvtoId { get; set; }

        [Display(Name = "Автомобиль")]
        public Avtomobil Avto { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Display(Name = "Пробег")]
        public int Probeg { get; set; }

        [Display(Name = "Количество ездок")]
        public int Ezdki { get; set; }

        [Display(Name = "Время работы")]
        public int Time { get; set; }

        [Display(Name = "Водитель")]
        public string Voditel { get; set; }

        [Display(Name = "Прицеп")]
        public bool Pricep { get; set; }



    }
}