using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class DOMView
    {
        
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }

        [Display(Name = "Фундамент")]
        public DOMFundament Fundament { get; set; }
        [Display(Name = "Тип фасада")]
        public FasadType Type { get; set; }
        [Display(Name = "Утепление фасада")]
        public int? UteplenieId { get; set; }
        public FasadUteplenie Uteplenie { get; set; }

        [Display(Name = "Дата последнего изменения")]
        public DateTime Date { get; set; }
    }
}