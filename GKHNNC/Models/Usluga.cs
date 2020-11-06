using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class Usluga
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Периодичность")]
        public int? PeriodichnostId { get; set; }
        //для подгрузки через айди
        public Periodichnost Periodichnost { get; set; }
        [Display(Name = "Порядок")]
        public int Poryadok { get; set; }

        [Display(Name = "Наименование услуги")]
        public string Name { get; set; }

        public ICollection<VipolnennieUslugi> VipolnennieUslugis { get; set; }
        public Usluga()
        {
            VipolnennieUslugis = new List<VipolnennieUslugi>();
        }


    }
}