using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class Adres
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Адрес")]
        public string Adress { get; set; }
        [Display(Name = "Улица")]
        public string Ulica { get; set; }
        [Display(Name = "Дом")]
        public string Dom { get; set; }
        [Display(Name = "ЖЭУ")]
        public string GEU { get; set; }
        [Display(Name = "Код УЭВ")]
        public int UEV { get; set; }
        [Display(Name = "Код ОБСД")]
        public int OBSD { get; set; }
        //для подгрузки через ID
        public ICollection<VipolnennieUslugi> VipolnennieUslugis { get; set; }
        public Adres()
        {
            VipolnennieUslugis = new List<VipolnennieUslugi>();
        }

    }
}