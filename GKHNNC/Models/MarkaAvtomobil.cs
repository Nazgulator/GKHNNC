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
    public class MarkaAvtomobil
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Марка автомобиля")]
        public string Name { get; set; }

        [Display(Name = "Летняя норма л/100км")]
        public decimal SNorm { get; set; }

        [Display(Name = "Зимняя норма л/100км")]
        public decimal WNorm { get; set; }

        [Display(Name = "Вид топлива")]
        public string Toplivo { get; set; }

        [Display(Name = "Считать по километражу?")]
        public bool KmMoto { get; set; }

        [Display(Name = "Норма на ездку? (нет - прицеп)")]
        public bool EzdkaPricep { get; set; }

        [Display(Name = "Норма на ездку")]
        public decimal NormaEzdka { get; set; }
    }
}