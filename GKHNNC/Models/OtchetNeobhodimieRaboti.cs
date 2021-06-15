using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class OtchetNeobhodimieRaboti
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Адрес")]
        public Adres Adres { get; set; }
        [Display(Name = "Осмотр")]
        public Osmotr Osmotr { get; set; }
        [Display(Name = "Активные работы осмотра")]
        public List<ActiveOsmotrWork> AOW { get; set; }
        [Display(Name = "Активные элементы осмотра")]
        public List<ActiveElement> AE { get; set; }

       

    }
}