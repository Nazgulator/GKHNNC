using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class DOMRoof
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "АдресId")]
        public int? AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }

        [Display(Name = "Вид крыши")]
        public int? VidId { get; set; }
        [Display(Name = "Вид крыши")]
        public RoofVid Vid { get; set; }
        [Display(Name = "Тип крыши")]
        public int? TypeId { get; set; }
        [Display(Name = "Тип крыши")]
        public RoofType Type { get; set; }
        [Display(Name = "Форма крыши")]
        public int? FormId { get; set; }
        [Display(Name = "Форма крыши")]
        public RoofForm Form { get; set; }
        [Display(Name = "Утепление крыши")]
        public int? UteplenieId { get; set; }
        [Display(Name = "Утепление крыши")]
        public RoofUteplenie Uteplenie { get; set; }

        [Display(Name = "Год последнего кап.ремонта кровли")]
        public int YearKrovlya { get; set; }
        [Display(Name = "Год последнего кап. ремонта несущей части")]
        public int Year { get; set; }
        [Display(Name = "Площадь крыши")]
        public decimal Ploshad { get; set; }
        [Display(Name = "Износ кровли")]
        public decimal IznosKrovlya { get; set; }
        [Display(Name = "Износ несущей части")]
        public decimal Iznos { get; set; }

        [Display(Name = "Дата последнего изменения")]
        public DateTime Date { get; set; }
    }
}