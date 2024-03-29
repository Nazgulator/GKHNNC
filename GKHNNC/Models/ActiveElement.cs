﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Web.Mvc;

namespace GKHNNC.Models
{
    public class ActiveElement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Элемент ИД")]
        public int ElementId { get; set; }
        [Display(Name = "Элемент")]
        public Element Element { get; set; }
        [Display(Name = "Адрес ИД")]

        public int AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }
        [Display(Name = "Осмотр ИД")]
        public int OsmotrId { get; set; }
        [Display(Name = "Осмотр")]
        public Osmotr Osmotr { get; set; }
        [Display(Name = "Состояние")]
        public int Sostoyanie { get; set; }
        [Display(Name = "Фото1")]
        public string Photo1 { get; set; }
        [Display(Name = "Фото2")]
        public string Photo2 { get; set; }
        [Display(Name = "СтароеЛиФото1")]
        public bool IsOld1 { get; set; }
        [Display(Name = "СтароеЛиФото2")]
        public bool IsOld2 { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
        [Display(Name = "Дата изменения")]
        public DateTime DateIzmeneniya { get; set; }
        [Display(Name = "Пользователь")]
        public string UserName { get; set; }
        [Display(Name = "ИзмерениеИД")]
        public int IzmerenieId { get; set; }
        [Display(Name = "МатериалИД")]
        public int MaterialId { get; set; }
        [Display(Name = "Количество")]
        public decimal Kolichestvo { get; set; }
        [Display(Name = "Присутствует ли элемент")]
        public bool Est { get; set; }

        [Display(Name = "ИзмерениеИД")]
        public int Izmerenie2Id { get; set; }
       
        [Display(Name = "Количество")]
        public decimal Kolichestvo2 { get; set; }


        [Display(Name = "Измерения")]
        public Izmerenie Izmerenie2 { get; set; }
        [Display(Name = "Измерения")]
        public Izmerenie Izmerenie { get; set; }
        [Display(Name = "Материалы")]
        public Material Material { get; set; }

        //сюда заливаем список дефектов для view и только
        [Display(Name = "АктивныеОсмотрВорк")]
        public List<ActiveOsmotrWork> ActiveOsmotrWorks;
        [Display(Name = "ОсмотрВорк")]
        public SelectList OsmotrWorks;
        //[Display(Name = "нужно ли показывать работы")]
        //public bool AddWork  ;
        [Display(Name = "Возможные дефекты")]
        public List<Defect> Defects;
        [Display(Name = "Активные дефекты")]
        public List<ActiveDefect> ActiveDefects;
      
        [Display(Name = "Выбранный материал")]
        public int M=0;
        [Display(Name = "Выбранная ед. изм.")]
        public int EI=0;

        [Display(Name = "Часть дома")]
        public DOMPart DomPart;



    }
}