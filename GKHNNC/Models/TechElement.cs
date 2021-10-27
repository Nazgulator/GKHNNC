using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Web.Mvc;

namespace GKHNNC.Models
{
    public class TechElement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "ИзмерениеИД")]
        public int IzmerenieId { get; set; }
        [Display(Name = "Измерение")]
        public Izmerenie Izmerenie { get; set; }

        [Display(Name = "АдресID")]
        public int AdresId { get; set; }
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }

        [Display(Name = "Дата изменения")]
        public DateTime Date { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Пользователь")]
        public string UserName { get; set; }
       
        [Display(Name = "Количество")]
        public decimal Val { get; set; }
      



    }
}