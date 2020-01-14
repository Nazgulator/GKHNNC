using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class ActiveDefect
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Элемент")]
        public Element Element { get; set; }
        [Display(Name = "Элемент ИД")]
        public int? ElementId { get; set; }

        [Display(Name = "Дефект")]
        public Defect Defect { get; set; }
        [Display(Name = "Дефект ИД")]
        public int? DefectId { get; set; }

        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }
        [Display(Name = "Адрес ИД")]
        public int? AdresId { get; set; }

        [Display(Name = "Осмотр")]
        public Osmotr Osmotr { get; set; }
        [Display(Name = "Осмотр ИД")]
        public int? OsmotrId { get; set; }

        [Display(Name = "Состояние")]
        public int Sostoyanie { get; set; }

        [Display(Name = "Количество")]
        public int Number { get; set; }

        [Display(Name = "Описание")]
        public string Opisanie { get; set; }

        [Display(Name = "Фото 1")]
        public string Photo1 { get; set; }
        [Display(Name = "Фото 2")]
        public string Photo2 { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
    }
}