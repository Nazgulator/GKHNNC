using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class SVN
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "АдресID")]
        public int? AdresId { get; set; }
        public Adres Adres { get; set; }
        [Display(Name = "Контрагент")]
        public string Agent { get; set; }
        [Display(Name = "СервисID")]
        public int? ServiceId { get; set; }
        [Display(Name = "Сервис")]
        public TableService Service { get; set; }
        [Display(Name = "Факт")]
        public decimal Fact { get; set; }
        [Display(Name = "План")]
        public decimal Plan { get; set; }
        [Display(Name = "Макет")]
        public decimal Maket { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

    }
}