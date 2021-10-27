using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class OsmotrRecommendWork
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Осмотр ИД")]
        public int OsmotrId { get; set; }
        [Display(Name = "Осмотр ИД")]
        public Osmotr Osmotr;
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Измерение ID")]
        public int IzmerenieId { get; set; }
        [Display(Name = "Измерение")]
        public Izmerenie Izmerenie { get; set; }
        [Display(Name = "Стоимость")]
        public decimal Cost { get; set; }
        [Display(Name = "Количество")]
        public decimal Number { get; set; }
        [Display(Name = "Итоговая Стоимость")]
        public decimal FinalCost { get; set; }
        [Display(Name = "Итоговое Количество")]
        public decimal FinalNumber { get; set; }
        [Display(Name = "Часть дома")]
        public DOMPart DOMPart { get; set; }
        [Display(Name = "Часть дома ид")]
        public int DOMPartId { get; set; }
        [Display(Name = "Смета?")]
        public bool Smeta { get; set; }
        [Display(Name = "Готово?")]
        public bool Gotovo { get; set; }
        [Display(Name = "Дата выполнения")]
        public DateTime DateVipolneniya { get; set; }
        [Display(Name = "Фотография акта выполненных работ")]
        public string Photo { get; set; }
        [Display(Name = "Пользователь")]
        public string User { get; set; }
        [Display(Name = "СтатьяID")]
        public int StatiId { get; set; }
        [Display(Name = "Статья")]
        public Stati Stati { get; set; }
        [Display(Name = "Коммисия")]
        public int Kommisia { get; set; }

    }
}