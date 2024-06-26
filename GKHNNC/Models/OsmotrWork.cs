﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class OsmotrWork
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Измерение ID")]
        public int IzmerenieId { get; set; }
        [Display(Name = "Измерение")]
        public Izmerenie Izmerenie { get; set; }
        [Display(Name = "Стоимость")]
        public decimal Cost { get; set; }
        [Display(Name = "Часть дома")]
        public DOMPart DOMPart { get; set; }
        [Display(Name = "Часть дома ид")]
        public int DOMPartId { get; set; }
        [Display(Name = "Отчет ИД")]
        public int OtchetId { get; set; }
        [Display(Name = "Архивная?")]
        public bool Archive { get; set; }


        // public virtual ICollection<Sopostavlenie> Sopostavlenies { get; set; }
    }
}