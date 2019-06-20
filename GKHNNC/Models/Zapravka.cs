using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC
{
    public class Zapravka
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Номер карты")]
        public string CardNumber { get; set; }

        [Display(Name = "Номер автомобиля")]
        public string AvtoNumber { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Display(Name = "Объём")]
        public int Liters { get; set; }

        [Display(Name = "Сумма")]
        public int Summa { get; set; }

    }
}