﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class ActiveOsmotrWork
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Идентификатор Работы осмотра")]
        public int OsmotrWorkId { get; set; }
        [Display(Name = "Осмотр Ворк")]
        public OsmotrWork OsmotrWork { get; set; }
        [Display(Name = "Стоимость")]
        public decimal TotalCost { get; set; }
        [Display(Name = "Количество")]
        public decimal Number { get; set; }
        [Display(Name = "Итоговая Стоимость")]
        public decimal FinalCost { get; set; }
        [Display(Name = "Итоговое Количество")]
        public decimal FinalNumber { get; set; }
        [Display(Name = "Активный элемент")]
        public int ElementId { get; set; }
        [Display(Name = "Осмотр ид")]
        public int OsmotrId { get; set; }
        [Display(Name = "Готово?")]
        public bool Gotovo { get; set; }
        [Display(Name = "Дата выполнения")]
        public DateTime DateVipolneniya { get; set; }
        [Display(Name = "Дата запланирована")]
        public DateTime? DateZaplanirovana { get; set; }
        [Display(Name = "Запланирована")]
        public bool Zaplanirovana { get; set; }
        [Display(Name = "Фотография акта выполненных работ")]
        public string Photo { get; set; }
        [Display(Name = "Пользователь")]
        public string User { get; set; }
        [Display(Name = "статья")]
        public int StatiId { get; set; }
        [Display(Name = "статья")]
        public Stati Stati { get; set; }
        [Display(Name = "Контрагент")]
        public int KontragentId { get; set; }
        [Display(Name = "Коммисия")]
        public int Kommisia { get; set; }
        [Display(Name = "осмотр")]
        public Osmotr Osmotr;


        // public virtual ICollection<Sopostavlenie> Sopostavlenies { get; set; }
    }
}