using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class Build
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Адрес")]
        public string Address { get; set; }
        [Display(Name = "ИД улицы")]
        public string StreetId { get; set; }
        [Display(Name = "Номер дома")]
        public int Number { get; set; }
        [Display(Name = "Буква")]
        public string Litera { get; set; }
        [Display(Name = "Тип дома ")]
        public string BuildTypeId { get; set; }
        [Display(Name = "Количество этажей")]
        public decimal CountFlats { get; set; }
        [Display(Name = "Общая площадь")]
        public decimal STotalArea { get; set; }
        [Display(Name = "Износ")]
        public int Iznos { get; set; }
        [Display(Name = "Год износа")]
        public int IznosYear { get; set; }
        
       

    }
}