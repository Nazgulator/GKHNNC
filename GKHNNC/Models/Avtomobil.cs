using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using GKHNNC.Models;

namespace GKHNNC.Models
{
    public class Avtomobil
    {
        
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Марка автомобиля ИД")]
        public int? MarkaId { get; set; }
        //для подгрузки через айди
        [Display(Name = "Марка автомобиля")]
        public MarkaAvtomobil Marka { get; set; }

        [Display(Name = "Тип автомобиля ИД")]
        public int? TypeId { get; set; }

        //для подгрузки через айди
        [Display(Name = "Тип автомобиля")]
        public TypeAvto Type { get; set; }

        [Display(Name = "Гос. Номер")]
        public string Number { get; set; }

        [Display(Name = "Год выпуска")]
        [Range(1700, 10000, ErrorMessage = "Недопустимый год")]
        public int Date { get; set; }

        [Display(Name = "Гаражный номер")]
        public int? Garage { get; set; }

        [Display(Name = "Глонасс")]
        public bool? Glonass { get; set; }

        



    }
}