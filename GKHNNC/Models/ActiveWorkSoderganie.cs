using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class ActiveWorkSoderganie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Адрес дома")]
        public int AdresId { get; set; }
        [Display(Name = "Адрес дома")]
        public Adres Adres { get; set; }
        [Display(Name = "Id работы")]
        public int WorkSoderganieId { get; set; }
        [Display(Name = "Работа")]
        public WorkSoderganie WorkSoderganie { get; set; }
     //   [Display(Name = "Объём")]
     //   public decimal Obiem { get; set; }
        [Display(Name = "Значение")]
        public decimal Val { get; set; }
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        //public string Agent { get; set; }
        //public DateTime Date { get; set; }


    }
}