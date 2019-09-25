using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class House
    {
      
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "АдресId")]
        public int? AdresId { get; set; }
        [Display(Name = "Адрес")]
        public string Adres { get; set; }
        [Display(Name = "Арендаторы")]
        public Arendator[] Arendators { get; set; }
        [Display(Name = "Площадь")]
        public decimal Ploshad { get; set; }
        [Display(Name = "Площадь арендаторов")]
        public decimal PloshadArendators { get; set; }
        [Display(Name = "Теплота")]
        public decimal Teplota { get; set; }
        [Display(Name = "Теплота Арендаторов")]
        public decimal TeplotaArendators { get; set; }
        [Display(Name = "Теплота 1/12")]
        public decimal Teplota12 { get; set; }
        [Display(Name = "Теплота 1/12 Арендаторов")]
        public decimal Teplota12Arendators { get; set; }
        [Display(Name = "Горячая вода")]
        public decimal HotWater { get; set; }
        [Display(Name = "Горячая вода Арендаторов")]
        public decimal HotWaterArendators { get; set; }
        [Display(Name = "Холодная вода")]
        public decimal ColdWater { get; set; }
        [Display(Name = "Холодная вода Арендаторов")]
        public decimal ColdWaterArendators { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM/yyyy}", ApplyFormatInEditMode = true)]

        //теперь типы крыши и фундамента

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


        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
    }
}