using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class DOM
    {
        
        [Display(Name = "Адрес")]
        public string Adres { get; set; }


        [Display(Name = "Вид крыши")]
        public string RoofVid { get; set; }

        [Display(Name = "Тип крыши")]
        public string RoofType { get; set; }

        [Display(Name = "Форма крыши")]
        public string RoofForm { get; set; }

        [Display(Name = "Утепление крыши")]
        public string RoofUteplenie { get; set; }
        [Display(Name = "Год последнего кап.ремонта кровли")]
        public int RoofYearKrovlya { get; set; }
        [Display(Name = "Год последнего кап. ремонта несущей части")]
        public int RoofYear { get; set; }
        [Display(Name = "Площадь крыши")]
        public decimal RoofPloshad { get; set; }
        [Display(Name = "Износ кровли")]
        public decimal RoofIznosKrovlya { get; set; }
        [Display(Name = "Износ несущей части")]
        public decimal RoofIznos { get; set; }
        [Display(Name = "Дата последнего изменения")]
        public DateTime RoofDate { get; set; }

        [Display(Name = "Площадь отмостки")]
        public decimal FundamentPloshad { get; set; }
        [Display(Name = "Материал фундамента")]
        public string FundamentMaterial { get; set; }
        [Display(Name = "Тип фундамента")]
        public string FundamentType { get; set; }
       
        [Display(Name = "Дата последнего изменения фундамента")]
        public DateTime FundamentDate { get; set; }

    }
}