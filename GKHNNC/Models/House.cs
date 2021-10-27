using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using GKHNNC.Models;

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

        [Display(Name = "Загружен из гисжкх")]
        public bool GISGKH { get; set; }

        [Display(Name = "Осмотр создан")]
        public bool OsmotrEst { get; set; }
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
        [Display(Name = "Фундамент")]
        public DOMFundament Fundament { get; set; }
        [Display(Name = "Крыша")]
        public DOMRoof Roof { get; set; }
        [Display(Name = "Фасад")]
        public DOMFasad Fasad { get; set; }
        [Display(Name = "Внутренние помещения")]
        public DOMRoom Room { get; set; }
        [Display(Name = "Отопление")]
        public DOMOtoplenie Otoplenie { get; set; }
        [Display(Name = "Горячая вода")]
        public DOMHW HotW { get; set; }
        [Display(Name = "Холодная вода")]
        public DOMCW ColdW { get; set; }
        [Display(Name = "Электросеть")]
        public DOMElectro Electro { get; set; }
        [Display(Name = "Электросеть")]
        public DOMVodootvod Vodootvod { get; set; }

        [Display(Name = "Сколько осмотров")]
        public int NumberOsmotrs { get; set; }
        [Display(Name = "Сколько выполненных работ")]
        public int NumberWorks { get; set; }

        [Display(Name = "Данные дома")]
        public Adres AdresAll { get; set; }


        [Display(Name = "Осмотры")]
        public List<Osmotr> Osmotrs;
        [Display(Name = "События")]
        public List<EventLog> Events;



    }
}